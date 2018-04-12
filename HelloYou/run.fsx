#if !COMPILED

#I @"C:/Program Files/nodejs/node_modules/azure-functions-core-tools/bin/"

#r "Microsoft.Azure.Webjobs.Host.dll"
open Microsoft.Azure.WebJobs.Host

#r "System.Net.Http.Formatting.dll"
#r "System.Web.Http.dll"
#r "System.Net.Http.dll"
#r "Newtonsoft.Json.dll"

#else

#r "System.Net.Http"
#r "Newtonsoft.Json"

#endif

open System.Net
open System.Net.Http
open Newtonsoft.Json

type Name = {
    First: string
    Last: string
}

type Greeting = {
    Greeting: string
}

let Run(req: HttpRequestMessage, log: TraceWriter) =
    async {
        log.Info("Webhook was triggered!")
        let! jsonContent = req.Content.ReadAsStringAsync() |> Async.AwaitTask

        let jsonFormatter = System.Net.Http.Formatting.JsonMediaTypeFormatter()
        jsonFormatter.SerializerSettings.ContractResolver 
            <- Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()

        try
            let name = JsonConvert.DeserializeObject<Name>(jsonContent)
            return req.CreateResponse(
                HttpStatusCode.OK, 
                { Greeting = sprintf "Hello %s %s!" name.First name.Last },
                jsonFormatter)
        with _ ->
            return req.CreateResponse(HttpStatusCode.BadRequest)
    } |> Async.StartAsTask

// For more information see https://aka.ms/fsharp-console-apps

open Farmer
open Farmer.Arm.Storage
open Farmer.Builders
open Farmer.Storage
open Farmer.WebApp
open Microsoft.Extensions.Logging

let loggerFactory = LoggerFactory.Create(fun builder -> builder.SetMinimumLevel(LogLevel.Trace).AddConsole() |> ignore<_>)
let logger = loggerFactory.CreateLogger("Program")

logger.LogInformation "Generating azure resource graph"

// logger.LogDebug "Provisioning monitor"
// let monitor = monitor {
//
// }

logger.LogDebug "Provisioning storage account"
let storeAccount = storageAccount {
    name "memories"


}

logger.LogDebug "Provisioning functions brain"
let brain = functions {
    storageAccount {
        name "brain_storage"

        restrict_to_azure_services [NetworkRuleSetBypass.AzureServices]

        sku (GeneralPurpose(V1(V1Replication.LRS(StoragePerformance.Standard))))
    } |> ignore

    name "brain"
    service_plan_name "neurons"

    use_runtime Functions.FunctionsRuntime.DotNet80Isolated
    worker_process Bits64
}

let proxy = containerApp {
    name "gateway"
}

logger.LogDebug "Constructing graph"
let template = arm {
    location Location.NorthEurope

    add_resource storeAccount
    add_resource brain


    add_tag "deployed-by" "farmer"
}

logger.LogInformation "Exporting generated ARM template"
template
|> Writer.quickWrite "diva"
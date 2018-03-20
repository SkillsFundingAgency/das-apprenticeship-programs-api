open Fake

Target "Dotnet Restore" (fun _ ->
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.Providers.Api.Client" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.Apprenticeships.Api.Client" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.Apprenticeships.Api.Types" })
)

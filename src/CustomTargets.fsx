open Fake

let testDirectory = getBuildParamOrDefault "buildMode" "Debug"
let nugetOutputDirectory = getBuildParamOrDefault "nugetOutputDirectory" "bin/Release"

Target "Create WiN Nuget Packages" (fun _ ->


    if testDirectory.ToLower() = "release" then
        let nupkgFiles = !! (currentDirectory + "/**/*.nuspec")

        let packageVersion = environVarOrDefault "BUILD_BUILDNUMBER" "1.0.0.0"
        for nupkgFile in nupkgFiles do
            let fileInfo = fileSystemInfo(nupkgFile)
            let name = fileInfo.Name.Replace(fileInfo.Extension,"")

            let mutable outputPath = FileSystemHelper.DirectoryName(fileInfo.FullName)

            if FileSystemHelper.directoryExists(FileSystemHelper.DirectoryName(fileInfo.FullName) @@ nugetOutputDirectory) then
                outputPath <- FileSystemHelper.DirectoryName(fileInfo.FullName) @@ nugetOutputDirectory

            (fileInfo.FullName)
            |> NuGet (fun p ->
                {p with
                    Authors = [name]
                    Project = name
                    Summary = name
                    Description = name
                    Version = packageVersion
                    NoPackageAnalysis = true
                    OutputPath = outputPath
                    WorkingDir = FileSystemHelper.DirectoryName(fileInfo.FullName)
                    })
)

Target "Dotnet Restore" (fun _ ->
    DotNetCli.Restore(fun p ->
            { p with
                Project = ""})
)

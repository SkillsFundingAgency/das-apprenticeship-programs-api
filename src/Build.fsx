﻿#r @"tools/FAKE/tools/FakeLib.dll"
#r @"tools/FAKE/tools/Fake.IIS.dll"
#r @"tools/FAKE/tools/Microsoft.Web.Administration.dll"

open Fake

#load "DefaultTargets.fsx"
#load "CustomTargets.fsx"

"Set version number"
   ==>"Set Solution Name"
   ==>"Clean Directories"
   ==>"Build Projects"
   ==>"Building Acceptance Tests"
    //==>"Run Acceptance Tests"

"Set Solution Name"
   ==>"Update WiN Assembly Info Version Numbers"
   ==>"Clean Directories"
   ==>"Dotnet Restore"
   ==>"Build Projects"
   ==>"Run NUnit Tests"
   ==>"Run XUnit Tests"
   ==>"Run Jasmine Tests"
   ==>"Build Cloud Projects"
   ==>"Build Database project"
   ==>"Build WebJob Project"
   ==>"Publish Solution"
   ==>"Compile Views"
   ==>"Create WiN Nuget Packages"

"Set Solution Name"
    ==> "Build Database project"
    ==> "Publish Database project"

"Set version number"
    ==>"Set Solution Name"
    ==> "Zip Compiled Source"

RunTargetOrDefault  "Create WiN Nuget Packages"
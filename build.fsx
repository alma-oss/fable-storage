#load ".fake/build.fsx/intellisense.fsx"
open System
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.Tools.Git

// ========================================================================================================
// === F# / Fable Library fake build ============================================================== 2.0.0 =
// --------------------------------------------------------------------------------------------------------
// Options:
//  - no-clean   - disables clean of dirs in the first step (required on CI)
//  - no-lint    - lint will be executed, but the result is not validated
// --------------------------------------------------------------------------------------------------------
// Table of contents:
//      1. Information about project, configuration
//      2. Utilities, DotnetCore functions
//      3. FAKE targets
//      4. FAKE targets hierarchy
// ========================================================================================================

// --------------------------------------------------------------------------------------------------------
// 1. Information about the project to be used at NuGet and in AssemblyInfo files and other FAKE configuration
// --------------------------------------------------------------------------------------------------------

let project = "Lmc.Fable.Storage"
let summary = "Fable library for a working with Browser storages."

let changeLog = Some "CHANGELOG.md"
let gitCommit = Information.getCurrentSHA1(".")
let gitBranch = Information.getBranchName(".")

[<RequireQualifiedAccess>]
module ProjectSources =
    let release =
        !! "src/**/*.fsproj"

    let tests =
        !! "tests/**/*.fsproj"

    let all =
        release
        ++ "tests/**/*.fsproj"

// --------------------------------------------------------------------------------------------------------
// 2. Utilities, DotnetCore functions, etc.
// --------------------------------------------------------------------------------------------------------

[<AutoOpen>]
module private Utils =
    let tee f a =
        f a
        a

    let skipOn option action p =
        if p.Context.Arguments |> Seq.contains option
        then Trace.tracefn "Skipped ..."
        else action p

    let orFail = function
        | Error e -> raise e
        | Ok ok -> ok

    let stringToOption = function
        | null | "" -> None
        | string -> Some string

    let envVar name =
        if Environment.hasEnvironVar(name)
            then Environment.environVar(name) |> Some
            else None

    let createProcess exe arg dir =
        CreateProcess.fromRawCommandLine exe arg
        |> CreateProcess.withWorkingDirectory dir
        |> CreateProcess.ensureExitCode

    let run proc arg dir =
        proc arg dir
        |> Proc.run
        |> ignore

    [<RequireQualifiedAccess>]
    module Dotnet =
        let dotnet = createProcess "dotnet"

        let run command dir = try run dotnet command dir |> Ok with e -> Error e
        let runInRoot command = run command "."
        let runOrFail command dir = run command dir |> orFail

    [<RequireQualifiedAccess>]
    module Option =
        let mapNone f = function
            | Some v -> v
            | None -> f None

        let bindNone f = function
            | Some v -> Some v
            | None -> f None

// --------------------------------------------------------------------------------------------------------
// 3. Targets for FAKE
// --------------------------------------------------------------------------------------------------------

Target.create "Clean" <| skipOn "no-clean" (fun _ ->
    !! "./**/bin/Release"
    ++ "./**/bin/Debug"
    ++ "./**/obj"
    ++ "./**/.ionide"
    |> Shell.cleanDirs
)

Target.create "AssemblyInfo" (fun _ ->
    let getAssemblyInfoAttributes projectName =
        let now = DateTime.Now

        let gitValue fallbackEnvironmentVariableNames initialValue =
            initialValue
            |> String.replace "NoBranch" ""
            |> stringToOption
            |> Option.bindNone (fun _ -> fallbackEnvironmentVariableNames |> List.tryPick envVar)
            |> Option.defaultValue "unknown"

        let release =
            changeLog
            |> Option.bind (fun changeLog ->
                try ReleaseNotes.parse (System.IO.File.ReadAllLines changeLog |> Seq.filter ((<>) "## Unreleased")) |> Some
                with _ -> None
            )

        [
            AssemblyInfo.Title projectName
            AssemblyInfo.Product project
            AssemblyInfo.Description summary

            match release with
            | Some release ->
                AssemblyInfo.Version release.AssemblyVersion
                AssemblyInfo.FileVersion release.AssemblyVersion
            | _ ->
                AssemblyInfo.Version "1.0"
                AssemblyInfo.FileVersion "1.0"

            AssemblyInfo.InternalsVisibleTo "tests"
            AssemblyInfo.Metadata("gitbranch", gitBranch |> gitValue [ "GIT_BRANCH"; "branch" ])
            AssemblyInfo.Metadata("gitcommit", gitCommit |> gitValue [ "GIT_COMMIT"; "commit" ])
            AssemblyInfo.Metadata("buildNumber", "BUILD_NUMBER" |> envVar |> Option.defaultValue "-")
            AssemblyInfo.Metadata("createdAt", now.ToString("yyyy-MM-dd HH:mm:ss"))
        ]

    let getProjectDetails (projectPath: string) =
        let projectName = IO.Path.GetFileNameWithoutExtension(projectPath)
        (
            projectPath,
            projectName,
            IO.Path.GetDirectoryName(projectPath),
            (getAssemblyInfoAttributes projectName)
        )

    ProjectSources.all
    |> Seq.map getProjectDetails
    |> Seq.iter (fun (_, _, folderName, attributes) ->
        AssemblyInfoFile.createFSharp (folderName </> "AssemblyInfo.fs") attributes
    )
)

Target.create "Build" (fun _ ->
    ProjectSources.all
    |> Seq.iter (DotNet.build id)
)

Target.create "Lint" <| skipOn "no-lint" (fun _ ->
    let lint project =
        project
        |> sprintf "fsharplint lint %s"
        |> Dotnet.runInRoot
        |> tee (function Ok _ -> Trace.tracefn "Lint %s is OK" project | _ -> ())

    let errors =
        ProjectSources.all
        |> Seq.map lint
        |> Seq.choose (function Error e -> Some e.Message | _ -> None)
        |> Seq.toList

    match errors with
    | [] -> Trace.tracefn "Lint is OK!"
    | errors -> errors |> String.concat "\n" |> failwithf "Lint ends with errors:\n%s"
)

Target.create "Tests" (fun _ ->
    if ProjectSources.tests |> Seq.isEmpty
    then Trace.tracefn "There are no tests yet."
    else Dotnet.runOrFail "run" "tests"
)

Target.create "Release" (fun _ ->
    Dotnet.runOrFail "pack" ("src" </> project)

    Directory.ensure "release"

    !! "**/bin/**/*.nupkg"
    |> Seq.iter (Shell.moveFile "release")
)

// --------------------------------------------------------------------------------------------------------
// 4. FAKE targets hierarchy
// --------------------------------------------------------------------------------------------------------

"Clean"
    ==> "AssemblyInfo"
    ==> "Build"
    ==> "Lint"
    ==> "Tests"
    ==> "Release"

Target.runOrDefaultWithArguments "Build"

module Commands

open System
open System.Runtime.CompilerServices
open Argu
open CommandLine
open Elastic.Markdown

let exampleDocSet (results: ParseResults<DocSetGeneratorArgs>) =
    let count = match results.TryGetResult Count with | Some s -> Nullable s | None -> Nullable()
    let path = match results.TryGetResult Path with | Some s -> s | None -> null
    task { return! ExampleGenerator(count, path).Build() }
    |> Async.AwaitTask
    |> Async.RunSynchronously
    
let linkChecker (a: ParseResults<Arguments>) = raise <| Exception("boom!")
let convert (results: ParseResults<ConvertArgs>) =
    
    let path = match results.TryGetResult ConvertArgs.Path with | Some s -> s | None -> null
    let output = match results.TryGetResult ConvertArgs.Ouput with | Some s -> s | None -> null
    task { return! DocSetConverter(path, output).Build() }
    |> Async.AwaitTask
    |> Async.RunSynchronously

let BindCommands (parsed:ParseResults<Arguments>) =
    for target in (CommandLine.SubCommands parsed) do
        let createCommand = 
            match target with
            // commands
            | DocSetGenerator a -> CommandLine.Bind exampleDocSet a
            | LinkChecker -> CommandLine.Bind linkChecker parsed
            | Convert a -> CommandLine.Bind convert a
            // flags
            | Token _ -> CommandLine.Ignore
        createCommand target
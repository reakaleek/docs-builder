module Program

open System.IO
open Argu
open Bullseye
open CommandLine

[<EntryPoint>]
let main argv =
    (*
    task {
        do! Async.SwitchToThreadPool ()
        let! markdown = Reader.ParseAsync("example-docs/long-2.md");
        let html = markdown.ToHtml(Reader.DefaultPipeline)
        File.WriteAllText("short-1.html", html)
        printfn "Hello %b" markdown.IsBreakable;
        return 0
      } |> Async.AwaitTask |> Async.RunSynchronously
 
    *)
    let parser = ArgumentParser.Create<Arguments>(programName = "docs-builder")
    let parsed = 
        try
            let parsed = parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)
            Commands.BindCommands parsed
            
            Some parsed
        with e ->
            printfn $"%s{e.Message}"
            None
    
    match parsed with
    | None -> 2
    | Some parsed ->
        let command = parsed.GetSubCommand().Name
        let swallowTypes = [typeof<ExceptionExiter>]
        let shortErrorsFor = (fun e -> swallowTypes |> List.contains (e.GetType()) )
        
        task {
            do! Async.SwitchToThreadPool ()
            return! Targets.RunTargetsAndExitAsync([command], shortErrorsFor, (fun _ -> ":")) 
        } |> Async.AwaitTask |> Async.RunSynchronously
        
        0
module CommandLine

open System
open Argu
open Bullseye
open Microsoft.FSharp.Reflection

type ConvertArgs =
    | [<First>] Path of path: string
    | [<Last>] Ouput of output: string

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Path _ -> "The docset folder"
            | Ouput _ -> "The html ouput folder"
        
and DocSetGeneratorArgs =
    | Count of count: int
    | Path of path: string
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Path _ -> "Path to store results"
            | Count _ -> "The number of files to generate"
            
and Arguments =
    | [<SubCommand;CustomCommandLine("generate_doc_set")>] DocSetGenerator of ParseResults<DocSetGeneratorArgs>
    | [<SubCommand;CustomCommandLine("link_checker")>] LinkChecker
    | [<SubCommand;CliPrefix(CliPrefix.None)>] Convert of ParseResults<ConvertArgs>
    
    | [<Inherit>] Token of string 
with
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            // commands
            | DocSetGenerator _ -> "Generate an example doc set"
            | LinkChecker -> "Validates cross references"
            | Convert _ -> "Convert to html"
            
            // flags
            | Token _ -> "Token to be used to authenticate with github"

            
    member this.Name =
        match FSharpValue.GetUnionFields(this, typeof<Arguments>) with
        | case, _ -> case.Name.ToLowerInvariant()
        
type CommandLine =
    static member SubCommands (parsed:ParseResults<Arguments>) =
        let cases =
            FSharpType.GetUnionCases(typeof<Arguments>)
            |> Seq.filter(fun c -> c.GetCustomAttributes(typeof<SubCommandAttribute>).Length > 0)
        let args (case: UnionCaseInfo) =
            match case.Name.ToLowerInvariant() with
            | "convert" -> [match parsed.TryGetResult Convert with Some c -> c :> obj | _-> None]
            | "docsetgenerator" -> [match parsed.TryGetResult DocSetGenerator with Some c -> c :> obj | _-> None]
            | _ -> []
        seq {
             for c in cases do
                 printfn "%s" c.Name
                 FSharpValue.MakeUnion(c, args c |> List.toArray) :?> Arguments
        }
        
    static member Ignore (_: Arguments) = ()
        
    static member Bind action (parsed: ParseResults<'a>) (target: Arguments)  =
        Targets.Target(target.Name, Action(fun _ -> action parsed))
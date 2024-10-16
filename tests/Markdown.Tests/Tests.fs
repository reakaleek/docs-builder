module Tests

open Argon
open Elastic.Markdown.Myst
open VerifyTests
open VerifyXunit
open Xunit

VerifierSettings.AddExtraSettings(fun settings -> settings.NullValueHandling <- NullValueHandling.Include)

[<Fact>]
let ``Can parse {literalinclude}`` () =
     // language=markdown
     let markdown =
         """```{literalinclude} /_static/yaml/settings.yaml
:language: yaml
:linenos:
```"""
     
     let md = MarkdownParser()
     
     
     Verifier.Verify(markdown).ToTask() |> Async.AwaitTask
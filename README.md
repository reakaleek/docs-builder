# POC doc set builder. 


Use `./run.sh` to build and run the `./docs-builder` application without installing any dependencies locally.


## Generate Sample DocSet


```bash
$ ./run.sh generate --count 10000
```
- Defaults to 1000 pages. 
- Generates an example docset folder locally under `.artifacts/docset-source`.
- It randomly creates a somewhat nested folder structure underneath this path.
- The templates are listed under `src/Elastic.Markdown/docset-templates/`

## Convert to html

```bash
$ ./run.sh convert
```

Converts the generated sample docset from markdown files in `.artifacts/docset-source` to HTML files
under `.artifacts/docset-generated`.


# Build without docker

If you have dotnet 8 installed you can use its CLI to publish a self contained `docs-builder`
binary. 

(On my M2 Pro mac the binary is currently 13mb)


```bash
$ dotnet publish "src/Elastic.Markdown/Elastic.Markdown.csproj" -c Release -o .artifacts/publish \
    --self-contained true /p:PublishTrimmed=true /p:PublishSingleFile=true -a arm64
```

Note `-a` should be the machines CPU architecture
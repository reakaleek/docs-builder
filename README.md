# docs-builder. 

You've reached the home of the latest incarnation of the documentation tooling. 

This repository is host to:

* *`docs-builder`* command line tool to generate single doc-sets (13mb native code, no dependencies)
* *`docs-assembler`* command line tool to assemble all the doc sets. (IN PROGRESS)
* `elastic/docs-builder@main` Github Action to build and validate a repositories documentation


### Docs Builder

In the near term native code will be published by CI for the following platforms

| OS       | Architectures |
|----------|---------------|
| Windows	 | x64, Arm64    |
| Linux	   | x64, Arm64    |
| macOS    | 	x64, Arm64   |

And we'll invest time in making sure these are easily obtainable (`brew`, `winget`, `apt`)

For now you can run the tool locally through `docker run`

```bash
docker run -v "./.git:/app/.git" -v "./docs:/app/docs" -v "./.artifacts:/app/.artifacts" \
  ghcr.io/elastic/docs-builder:edge
```

This ensures `.git`/`docs` and `.artifacts` (the default output directory) are mounted.

The tool will default to incremental compilation. 
Only the changed files on subsequent runs will be compiled unless you pass `--force`
to force a new compilation.

```bash
docker run -v "./.git:/app/.git" -v "./docs:/app/docs" -v "./.artifacts:/app/.artifacts" \
  ghcr.io/elastic/docs-builder:edge --force
```

#### Live mode

Through the `serve` command you can continuously and partially compile your documentation.

```bash
docker run -v "./.git:/app/.git" -v "./docs:/app/docs" -v "./.artifacts:/app/.artifacts" \
  --expose 8080 ghcr.io/elastic/docs-builder:edge serve
```

Each page is compiled on demand as you browse http://localhost:8080 and is never cached so changes to files and 
navigation will always be reflected upon refresh.

# Run without docker

If you have dotnet 8 installed you can use its CLI to publish a self-contained `docs-builder` native code
binary. (On my M2 Pro mac the binary is currently 13mb)

```bash
$ dotnet publish "src/docs-builder/docs-builder.csproj" -c Release -o .artifacts/publish \
    --self-contained true /p:PublishTrimmed=true /p:PublishSingleFile=false /p:PublishAot=true -a arm64
```

**Note**: `-a` should be the machines CPU architecture

The resulting binary `./.artifacts/publish/docs-builder` will run on machines without .NET installed

# Performance

To test performance it's best to build the binary and run outside of docker:

For refence here's the `markitpy-doc` docset (50k markdown files) currently takes `14s` vs `several minutes` compared to 
existing surveyed tools
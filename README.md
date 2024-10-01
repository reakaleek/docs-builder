# POC doc set builder. 


Use `./run.sh` to build and run the `./docs-builder` application without installing any dependencies locally.


## Continuous serve documentation live

```bash
$ ./run.sh serve
```

- Loads the documentation source (by default [`docs/source`](docs/source))
- Serves the docs at http://localhost:8080
- Reacts to changes e.g:
  - changing toctree, removing/adding files will cause the site tree to be reconstructed (background)
  - markdown content itself is always parsed and converted

If https://github.com/elastic/markitpy-poc is cloned at `../markitpy-poc` relative to this `README.md` it is automatically 
mounted by `run.sh`

```bash
$ ./run.sh serve --path markitpy-poc/docs/source
```

Will serve the https://github.com/elastic/markitpy-poc docset at http://localhost:8080

## Convert to html

```bash
$ ./run.sh generate
```

- Converts documents (by default [`docs/source`](docs/source))
- Outputs HTML to `.artifacts/docs/html/` this path is mounted in docker so can be inspected locally

Likewise as with `serve` if `..markitpy-poc` is available its auto mounted and can be used to generate HTML.

```bash
$ ./run.sh generate --path markitpy-poc/docs/source
```

DO NOTE that the container might not be best served to test performance. 

E.g simply enumerating the files takes 3x as long as generating with locally build binary.

# Build without docker

If you have dotnet 8 installed you can use its CLI to publish a self-contained `docs-builder` native code
binary. (On my M2 Pro mac the binary is currently 13mb)

```bash
$ dotnet publish "src/docs-builder/docs-builder.csproj" -c Release -o .artifacts/publish \
    --self-contained true /p:PublishTrimmed=true /p:PublishSingleFile=false /p:PublishAot=true -a arm64
```

**Note**: `-a` should be the machines CPU architecture

The resulting binary `./.artifacts/publish/docs-builder` will run on machines without .NET installed

Long term native code will be published by CI for the following platforms

| OS       | Architectures |
|----------|---------------|
| Windows	 | x64, Arm64    |
| Linux	   | x64, Arm64    |
| macOS    | 	x64, Arm64   |


# Performance

To test performance its best to build the binary and run the code on physical hardware:

For refence here's the `markitpy-doc` generation output on my local machine where it takes `14s`


```bash
$ ./.artifacts/publish/docs-builder generate --path ../markitpy-poc/docs/source/
```
```text
:: generate :: Starting
Fetched documentation set
Resolving tree
Resolved tree
Handled 1010 files
Handled 2013 files
...TRUNCATED...
Handled 5007 files
Handled 56005 files
Handled 57010 files
:: generate :: Finished in '00:00:14.9856495
```


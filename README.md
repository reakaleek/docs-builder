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

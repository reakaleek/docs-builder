# Navigation

Two types of nav files are supported: `docset.yml` and `toc.yml`.

## `docset.yml`

Example:

```yaml
project: 'PROJECT_NAME'

external_hosts:
  - EXTERNAL_LINKS_HERE

exclude:
  - 'EXCLUDED_FILES'

toc:
  - file: index.md
  - toc: elastic-basics
  - folder: top-level-bucket-a
    children:
      - file: index.md
      - file: file-a.md
      - file: file-b.md
  - folder: top-level-bucket-b
    children:
      - file: index.md
      - folder: second-level-bucket-c
        children:
          - file: index.md
```

### `project`

The name of the project.

Example:

```yaml
project: 'APM Java agent reference'
```

### `external_hosts`

All links to external hosts must be declared in this section of `docset.yml`.

Example:

```yaml
external_hosts:
  - google.com
  - github.com
```

### `exclude`

Files to exclude from the TOC. Supports glob patterns.

The following example excludes all markdown files beginning with `_`:

```yaml
exclude:
  - '_*.md'
```

### `toc`

Defines the table of contents (navigation) for the content set. A minimal toc is:

```yaml
toc:
  - file: index.md
```

The table of contents can be created independent of the directory structure of the files it defines. You can use directories to define nesting in the TOC, but you don't have to. For example, both of the following create the same nav structure:

```yaml
  ...
  - file: subsection/index.md
    children:
      - file: subsection/page-one.md
      - file: subsection/page-two.md
```

```yaml
  ...
  - folder: subsection
    children:
      - file: index.md
      - file: page-one.md
      - file: page-two.md
```

#### Nest `toc`

The `toc` key can include nested `toc.yml` files.

The following example includes two sub-`toc.yml` files located in directories named `elastic-basics` and `solutions`:

```yml
toc:
  - file: index.md
  - toc: elastic-basics
  - toc: solutions
```

### Attributes

Example:

```yml
subs:
  attr-name:   "attr value"
  ea:   "Elastic Agent"
  es:   "Elasticsearch"
```

See [Attributes](./attributes.md) to learn more.

## `toc.yml`

As a rule, each `docset.yml` file can only be included once in the assembler. This prevents us from accidentally duplicating pages in the docs. However, there are times when you want to split content sets and include them partially in different areas of the TOC. That's what `toc.yml` files are for. These files split one documentation set into multiple “sub-TOCs,” each mapped to a different navigation node.

All configuration options that `docset.yml` supports are supported by `toc.yml`.
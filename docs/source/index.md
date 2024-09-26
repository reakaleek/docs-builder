---
title: Elastic Docs v3
---

Elastic Docs v3 is built with [markitpy](https://github.com/elastic/markitpy)—a custom wrapper for the `sphinx-build`, `sphinx-autobuild`, and `sphinx-multiversion` command-line tools that provides:

* A simple CLI for building and auto-building content
* Automatic installation of Sphinx, MyST-Parser, and other Sphinx and MyST extensions
* New functionality, like partial builds

Most importantly, markitpy allows contributors to **focus on content**, not complex Sphinx internals or documentation configuration.

:::{tip}
On the right side of every page, there is an `Edit this page` link that you can use to see the markdown source for that page.
:::

:::{admonition} My custom title with *Markdown*!
:class: tip

This is a custom title for a tip admonition.
:::

````{note}
The next info should be nested
```{warning}
Here's my warning
```
````


```javascript
const variable = "hello world";
```

## Feedback

Hate it? Love it? We want to hear it all. Say hello and leave your thoughts in [#elastic-docs-v3](https://elastic.slack.com/archives/C07APH4RCDT).

## Build the docs locally

Hosted docs are great and all, but what's the contributor experience like?
Read the [quick start guide](https://github.com/elastic/markitpy/tree/main), clone the repository, and spin up the docs locally in seconds.


```{toctree}
:caption: Elastic Docs Guide
:hidden:

elastic/index.md
markup/index.md
nested/index.md
versioning/index.md
```

ReStructuredText (RST)
======================

Sphinx uses ReStructuredText format by default. We should always use markdown,
but in case some features are not available in Myst, one can always use RST.
Just name the file with `.rst` extension.

Remember that RST and MD syntax can not be mixed in the same file.
An `.rst` file is parsed as ReStructuredText and an `.md` file is parsed as markdown. However,
you can use the `eval-rst` directive to include RST content in a markdown file,
as shown in :doc:`RST in Markdown <rst_in_markdown>`.

.. code-block:: yaml

    project:
    title: MyST Markdown
    github: https://github.com/jupyter-book/mystmd
    license:
        code: MIT
        content: CC-BY-4.0
    subject: MyST Markdown

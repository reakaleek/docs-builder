from __future__ import annotations
from docutils import nodes

from sphinx.application import Sphinx
from sphinx.util.docutils import SphinxDirective, SphinxRole, directives
from sphinx.util.typing import ExtensionMetadata

def rejoin_text(text: str, split_by: str, join_by: str) -> str:
    return join_by.join(text.lower().split(split_by))

class RejoinRole(SphinxRole):
    """A role to split by a string and rejoin by another string !"""

    def run(self) -> tuple[list[nodes.Node], list[nodes.system_message]]:
        node = nodes.inline(text=rejoin_text(self.text, ' ', '-'))
        return [node], []

class RejoinDirective(SphinxDirective):
    """A directive to split by a string and rejoin by another string !"""

    required_arguments = 1
    has_content = True
    FROM_OPTION = 'from'
    TO_OPTION = 'to'
    optional_arguments = 2
    option_spec = {
        FROM_OPTION: directives.unchanged,
        TO_OPTION: directives.unchanged
    }

    def run(self) -> list[nodes.Node]:
        label = self.arguments[0]
        text = ''.join(self.content)
        fr = self.options[self.FROM_OPTION]
        to = self.options[self.TO_OPTION]
        rejoined_text = rejoin_text(text, fr, to)
        converted_text = f"{label} : {rejoined_text}"
        paragraph_node = nodes.inline(text=converted_text)
        return [paragraph_node]

def setup(app: Sphinx) -> ExtensionMetadata:
    app.add_role('rejoin', RejoinRole())
    app.add_directive('rejoin', RejoinDirective)
    return {
        'version': '0.1',
        'parallel_read_safe': True,
        'parallel_write_safe': True,
    }
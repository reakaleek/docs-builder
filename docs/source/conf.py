# Configuration file for the Sphinx documentation builder.
#
# For the full list of built-in configuration values, see the documentation:
# https://www.sphinx-doc.org/en/master/usage/configuration.html

# -- Project information -----------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#project-information

project = 'Elastic'
copyright = '2024, Elasticsearch B.V. All Rights Reserved.'

# -- General configuration ---------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#general-configuration

# If a project defines its own extension, the path to the extension
# should be added to the path so that Sphinx can find it.
import os
import sys
sys.path.append(os.path.abspath("./_ext"))

extensions = [
    # Commenting out the myst_parser extensions, since it is already
    # included in myst_nb. Including it again will cause an error.
    #'myst_parser',
    'myst_nb',
    'sphinx_design',
    'sphinx.ext.duration',
    'sphinx_copybutton',
    'sphinx_togglebutton',
    'sphinxcontrib.mermaid',
    'sphinx_multiversion',
    'rejoin',
    'markitpy.extensions.yaml_to_md'
]

templates_path = ['_templates']
html_static_path = ['_static']

exclude_patterns = []

suppress_warnings = ["myst.strikethrough"]

myst_enable_extensions = [
    'substitution',
    'attrs_inline',
    'attrs_block',
    'tasklist',
    'strikethrough',
    'deflist'
]

myst_substitutions = {
    'project': "MarkItPy",
}

myst_title_to_header = True
# myst_heading_anchors = 0

this_dir = os.path.dirname(os.path.abspath(__file__))
sys.path.append(this_dir)
from theme_conf import *
from multiversion_conf import *

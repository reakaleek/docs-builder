# -- Options for HTML output -------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#options-for-html-output

html_theme = "shibuya"

html_theme_options = {
    "announcement": '<center>👋 Thanks for checking out our Elastic Docs v3 POC. Have feedback? Reach out <a href="https://elastic.slack.com/archives/C07APH4RCDT">here</a>. Thanks!</center>',
    "light_logo": "_static/logo-light.svg",
    "dark_logo": "_static/logo-dark.svg",
    "accent_color": "blue",
    "globaltoc_expand_depth": 1,
    "logo_target": "index.html",
    "nav_links": [
        {
            "title": "<strong>Elastic Docs v3</strong>",
            "url": "index"
        },
        {
            "title": "Start Here",
            "children": [
                {
                    "title": "Search",
                    "url": "index",
                    "summary": "Build custom applications with your data using Elasticsearch.",
                },
                {
                    "title": "Observability",
                    "url": "index",
                    "summary": "Monitor applications and systems with Elastic Observability.",
                },
                {
                    "title": "Security",
                    "url": "index",
                    "summary": "Detect, investigate, and respond to threats with Elastic Security.",
                },
            ]
        },
        {
            "title": "Deploy",
            "children": [
                {
                    "title": "Elastic Cloud",
                    "url": "index",
                    "summary": "Deploy instances of the Elastic Stack in the cloud, with the provider of your choice."
                },
                {
                    "title": "Elastic Cloud Enterprise",
                    "url": "index",
                    "summary": "Deploy Elastic Cloud on public or private clouds, virtual machines, or your own premises."
                },
                {
                    "title": "Elastic Cloud on Kubernetes",
                    "url": "index",
                    "summary": "Deploy Elastic Cloud on Kubernetes."
                },
                {
                    "title": "Self managed",
                    "url": "index",
                    "summary": "Install, configure, and run Elastic products on your own premises."
                }
            ]
        },
        {
            "title": "Reference",
            "children": [
                {
                    "title": "API Reference",
                    "url": "index",
                    "summary": "Description."
                },
                {
                    "title": "Configuration Reference",
                    "url": "index",
                    "summary": "Description."
                },
                {
                    "title": "Troubleshooting",
                    "url": "index",
                    "summary": "Description."
                },
            ]
        },
        {
            "title": "What's New",
            "url": "index"
        }
    ]
}

html_context = {
    "source_type": "github",
    "source_user": "elastic",
    "source_repo": "markitpy-samples",
    "source_version": "master",  # Optional
    "source_docs_path": "/docs/source/",  # Optional
}

html_css_files = ["custom.css"]

html_sidebars = {
  "**": [
    "sidebars/localtoc.html",
    "sidebars/edit-this-page.html",
  ]
}

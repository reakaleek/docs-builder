// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Elastic.Markdown.Myst.Directives;

/// <summary>
/// A HTML renderer for a <see cref="Role"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{CustomContainerInline}" />
public class HtmlCustomContainerInlineRenderer : HtmlObjectRenderer<Role>
{
    protected override void Write(HtmlRenderer renderer, Role obj)
    {
        renderer.Write("<span").WriteAttributes(obj).Write('>');
        renderer.WriteChildren(obj);
        renderer.Write("</span>");
    }
}

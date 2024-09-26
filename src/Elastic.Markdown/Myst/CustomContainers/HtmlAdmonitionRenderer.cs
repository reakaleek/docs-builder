// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Elastic.Markdown.Myst.CustomContainers;

/// <summary>
/// A HTML renderer for a <see cref="Admonition"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{CustomContainer}" />
public class HtmlAdmonitionRenderer : HtmlObjectRenderer<Admonition>
{
	protected override void Write(HtmlRenderer renderer, Admonition obj)
	{
		renderer.EnsureLine();

		var attributes = obj.GetAttributes();
		var arguments = obj.Arguments;
		var info = obj.Info;
		// language=html

		if (renderer.EnableHtmlForBlock)
		{
			renderer.Write($@"<div class=""admonition {info}""").WriteAttributes(obj).Write('>');
			if (!string.IsNullOrEmpty(arguments))
			{
				// language=html
				renderer.Write($@"<p class=""admonition-title"">{arguments}</p>");
			}
		}

		// We don't escape a CustomContainer
		renderer.WriteChildren(obj);
		if (renderer.EnableHtmlForBlock)
		{
			renderer.WriteLine("</div>");
		}
	}
}

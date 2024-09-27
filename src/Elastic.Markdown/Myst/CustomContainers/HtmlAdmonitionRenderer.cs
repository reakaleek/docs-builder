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
		string info;
		string title;
		switch (obj.Info)
		{
			case "{attention}":
				info = "attention";
				title = "Attention";
				break;
			case "{caution}":
				info = "caution";
				title = "Caution";
				break;
			case "{danger}":
				info = "danger";
				title = "Danger";
				break;
			case "{error}":
				info = "error";
				title = "Error";
				break;
			case "{hint}":
				info = "hint";
				title = "Hint";
				break;
			case "{important}":
				info = "important";
				title = "Important";
				break;
			case "{note}":
				info = "note";
				title = "Note";
				break;
			case "{seealso}":
				info = "seealso";
				title = "See also";
				break;
			case "{tip}":
				info = "tip";
				title = "Tip";
				break;
			case "{versionadded}":
				info = "versionadded";
				title = "Added in version";
				break;
			case "{versionchanged}":
				info = "versionchanged";
				title = "Changed in version";
				break;
			case "{versionremoved}":
				info = "versionremoved";
				title = "Removed in version";
				break;
			case "{deprecated}":
				info = "deprecated";
				title = "Deprecated since version";
				break;
			default:
				info = obj.Info ?? string.Empty;
			title = string.Empty;
				break;
		}
		if (!string.IsNullOrEmpty(arguments))
			title += $" {arguments}";

		if (info.StartsWith("version") || info == "deprecated")
		{
			// language=html
			renderer.Write(
				$@"<div class=""{info}"" <p><span class=""versionmodified {info.Replace("version", "")}"">{title}: </span>");
			renderer.WriteChildren(obj);
			renderer.WriteLine("</p></div>");
		}
		else
		{
			var inlineClasses = obj.AdmonitionData.TryGetValue("class", out var classes) ? classes : string.Empty;
			var id = obj.AdmonitionData.TryGetValue("name", out var i) ? i : string.Empty;
			// language=html
			renderer.Write($@"<div class=""admonition {info} {inlineClasses}""")
				.WriteAttributes(obj);
			if (!string.IsNullOrEmpty(id))
			{
				renderer.Write(" id=\"");
				renderer.WriteEscape(id);
				renderer.Write("\"");
			}
			renderer.Write('>');
			// language=html
			renderer.Write($@"<p class=""admonition-title"">{title}</p>");

			renderer.WriteChildren(obj);
			renderer.WriteLine("</div>");
		}

	}

}

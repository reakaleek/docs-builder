// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Elastic.Markdown.Slices.Blocks;
using Elastic.Markdown.Templating;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using RazorSlices;

namespace Elastic.Markdown.Myst.CustomContainers;

/// <summary>
/// A HTML renderer for a <see cref="Admonition"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{CustomContainer}" />
public class HtmlAdmonitionRenderer : HtmlObjectRenderer<Admonition>
{
	public enum Mode
	{
		Admonition, Code, Version, SideBar, TabSet, TabItem
	}

	private int _seenTabSets = 0;

	protected override void Write(HtmlRenderer renderer, Admonition obj)
	{
		renderer.EnsureLine();

		var attributes = obj.GetAttributes();
		var arguments = obj.Arguments;
		string info;
		string title;
		var mode = Mode.Admonition;
		switch (obj.Info)
		{
			case "{attention}":
			case "{caution}":
			case "{danger}":
			case "{error}":
			case "{hint}":
			case "{important}":
			case "{note}":
			case "{tip}":
				info = obj.Info.Trim('{', '}');
				title = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(info);
				break;
			case "{seealso}":
				info = "seealso";
				title = "See also";
				break;
			case "{versionadded}":
				info = "versionadded";
				title = "Added in version";
				mode = Mode.Version;
				break;
			case "{versionchanged}":
				info = "versionchanged";
				title = "Changed in version";
				mode = Mode.Version;
				break;
			case "{versionremoved}":
				info = "versionremoved";
				title = "Removed in version";
				mode = Mode.Version;
				break;
			case "{deprecated}":
				info = "deprecated";
				title = "Deprecated since version";
				mode = Mode.Version;
				break;
			case "{code}":
				info = "code";
				mode = Mode.Code;
				title = string.Empty;
				break;
			case "{code-block}":
				info = "code-block";
				mode = Mode.Code;
				title = string.Empty;
				break;
			case "{sidebar}":
				info = "sidebar";
				mode = Mode.SideBar;
				title = string.Empty;
				break;
			case "{tab-set}":
				info = "tabset";
				mode = Mode.TabSet;
				title = string.Empty;
				break;
			case "{tab-item}":
				info = "tabitem";
				mode = Mode.TabItem;
				title = string.Empty;
				break;
			default:
				info = obj.Info ?? string.Empty;
				if (obj.Info != null && !obj.Info.StartsWith("{"))
					mode = Mode.Code;
				title = string.Empty;
				break;
		}
		if (!string.IsNullOrEmpty(arguments))
			title += $" {arguments}";

		if (mode == Mode.Version)
		{
			// language=html
			renderer.Write(
				$@"<div class=""{info}"" <p><span class=""versionmodified {info.Replace("version", "")}"">{title}: </span>");
			renderer.WriteChildren(obj);
			renderer.WriteLine("</p></div>");
		}
		else if (mode == Mode.Admonition)
		{
			var classes = obj.AdmonitionData.GetValueOrDefault("class");
			var id = obj.AdmonitionData.GetValueOrDefault("name");

			var slice = Admon.Create(new AdmonitionHtml
			{
				Info = info, Id = id, Classes = classes, Title = title
			});
			RenderRazorSlice(slice, renderer, obj);
		}
		else if (mode == Mode.Code)
		{
			var classes = obj.AdmonitionData.GetValueOrDefault("class");
			var id = obj.AdmonitionData.GetValueOrDefault("name");
			var language = info == "code" || info == "code-block"
				? arguments ?? "unknown" : info;

			var caption = obj.AdmonitionData.GetValueOrDefault("caption");

			var slice = Code.Create(new CodeHtml
			{
				Language = language, Info = info, Id = id, Classes = classes, Caption = caption
			});
			RenderRazorSlice(slice, renderer, obj);
		}

		else if (mode == Mode.SideBar)
		{
			var classes = obj.AdmonitionData.GetValueOrDefault("class");
			var id = obj.AdmonitionData.GetValueOrDefault("name");
			var language = info == "code" || info == "code-block"
				? arguments ?? "unknown" : info;

			var caption = obj.AdmonitionData.GetValueOrDefault("caption");

			var slice = SideBar.Create(new SideBarHtml
			{
				Info = info, Id = id, Classes = classes
			});
			RenderRazorSlice(slice, renderer, obj);
		}
		else if (mode == Mode.TabSet)
		{
			var classes = obj.AdmonitionData.GetValueOrDefault("class");
			var id = obj.AdmonitionData.GetValueOrDefault("name");
			var slice = TabSet.Create(new TabSetHtml
			{
				Info = info, Id = id, Classes = classes
			});
			RenderRazorSlice(slice, renderer, obj);
			_seenTabSets++;
		}
		else if (mode == Mode.TabItem)
		{
			var classes = obj.AdmonitionData.GetValueOrDefault("class");
			var id = obj.AdmonitionData.GetValueOrDefault("name");
			var index = obj.Parent!.IndexOf(obj);
			var slice = TabItem.Create(new TabItemHtml
			{
				Info = info, Id = id, Classes = classes, Index = index, Title = arguments ?? "Tab", TabSetIndex = _seenTabSets
			});
			RenderRazorSlice(slice, renderer, obj);
		}

	}

	private static void RenderRazorSlice<T>(RazorSlice<T> slice, HtmlRenderer renderer, Admonition obj)
	{
		var html = slice.RenderAsync().GetAwaiter().GetResult();
		var blocks = html.Split("[CONTENT]", 2, StringSplitOptions.RemoveEmptyEntries);
		renderer.Write(blocks[0]);
		renderer.WriteChildren(obj);
		renderer.Write(blocks[1]);
	}
}

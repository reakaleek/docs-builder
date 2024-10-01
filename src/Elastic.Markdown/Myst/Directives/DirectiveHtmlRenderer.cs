// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Elastic.Markdown.Slices.Directives;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using RazorSlices;

namespace Elastic.Markdown.Myst.Directives;

/// <summary>
/// An HTML renderer for a <see cref="DirectiveBlock"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{CustomContainer}" />
public class DirectiveHtmlRenderer : HtmlObjectRenderer<DirectiveBlock>
{
	protected override void Write(HtmlRenderer renderer, DirectiveBlock directiveBlock)
	{
		if (directiveBlock is TocTreeBlock)
			return;

		renderer.EnsureLine();

		switch (directiveBlock.Info)
		{
			case "{attention}":
			case "{caution}":
			case "{danger}":
			case "{error}":
			case "{hint}":
			case "{important}":
			case "{note}":
			case "{tip}":
				WriteAdmonition(renderer, directiveBlock);
				break;
			case "{seealso}":
				WriteAdmonition(renderer, directiveBlock);
				break;
			case "{versionadded}":
			case "{versionchanged}":
			case "{versionremoved}":
			case "{deprecated}":
				WriteVersion(renderer, directiveBlock);
				break;
			case "{code-block}":
			case "{code}":
				WriteCode(renderer, directiveBlock);
				break;
			case "{sidebar}":
				WriteSideBar(renderer, directiveBlock);
				break;
			case "{tab-set}":
				WriteTabSet(renderer, directiveBlock);
				break;
			case "{tab-item}":
				WriteTabItem(renderer, directiveBlock);
				break;
			case "{card}":
				WriteCard(renderer, directiveBlock);
				break;
			case "{grid}":
				WriteGrid(renderer, directiveBlock);
				break;
			case "{grid-item-card}":
				WriteGridItemCard(renderer, directiveBlock);
				break;
			default:
				if (!string.IsNullOrEmpty(directiveBlock.Info) && !directiveBlock.Info.StartsWith('{'))
					WriteCode(renderer, directiveBlock);
				else if (!string.IsNullOrEmpty(directiveBlock.Info))
					WriteAdmonition(renderer, directiveBlock);
				else
					WriteChildren(renderer, directiveBlock);
				break;
		}
	}

	private void WriteChildren(HtmlRenderer renderer, DirectiveBlock directiveBlock) =>
		renderer.WriteChildren(directiveBlock);

	private void WriteCard(HtmlRenderer renderer, DirectiveBlock directiveBlock)
	{
		var title = directiveBlock.Arguments;
		var link = directiveBlock.DirectiveProperties.GetValueOrDefault("link");
		var slice = Card.Create(new CardViewModel { Title = title, Link = link });
		RenderRazorSlice(slice, renderer, directiveBlock, implicitParagraph: false);
	}

	private void WriteGrid(HtmlRenderer renderer, DirectiveBlock directiveBlock)
	{
		//todo we always assume 4 integers
		var columns = directiveBlock.Arguments?.Split(' ')
			.Select(t => int.TryParse(t, out var c) ? c : 0).ToArray() ?? [];
		// 1 1 2 3
		int xs = 1, sm = 1, md = 2, lg = 3;
		if (columns.Length >= 4)
		{
			xs = columns[0];
			sm = columns[1];
			md = columns[2];
			lg = columns[3];
		}
		var slice = Grid.Create(new GridViewModel
		{
			BreakPointLg = lg, BreakPointMd = md, BreakPointSm = sm, BreakPointXs = xs
		});
		RenderRazorSlice(slice, renderer, directiveBlock);
	}
	private void WriteGridItemCard(HtmlRenderer renderer, DirectiveBlock directiveBlock)
	{
		var title = directiveBlock.Arguments;
		var link = directiveBlock.DirectiveProperties.GetValueOrDefault("link");
		var slice = GridItemCard.Create(new GridItemCardViewModel { Title = title, Link = link });
		RenderRazorSlice(slice, renderer, directiveBlock);
	}


	private void WriteVersion(HtmlRenderer renderer, DirectiveBlock directiveBlock)
	{
		var admonition = directiveBlock.Info!.Trim('{', '}');
		var title = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(admonition);
		if (!string.IsNullOrEmpty(directiveBlock.Arguments))
			title += $" {directiveBlock.Arguments}";
		var versionClass = directiveBlock.Info!.Replace("version", "");
		var slice = Slices.Directives.Version.Create( new VersionViewModel
		{
			Directive = admonition, Title = title, VersionClass = versionClass
		});
		RenderRazorSlice(slice, renderer, directiveBlock);
	}

	private void WriteAdmonition(HtmlRenderer renderer, DirectiveBlock directiveBlock)
	{
		var classes = directiveBlock.DirectiveProperties.GetValueOrDefault("class");
		var id = directiveBlock.DirectiveProperties.GetValueOrDefault("name");

		var admonition = directiveBlock.Info?.Trim('{', '}') ?? "unknown";
		var title = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(admonition);
		if (!string.IsNullOrEmpty(directiveBlock.Arguments))
			title += $" {directiveBlock.Arguments}";

		var slice = Admonition.Create(new AdmonitionViewModel
		{
			Directive = admonition, Id = id, Classes = classes, Title = title
		});
		RenderRazorSlice(slice, renderer, directiveBlock);
	}

	private void WriteCode(HtmlRenderer renderer, DirectiveBlock directiveBlock)
	{
		var codeBlockLanguage = directiveBlock.Arguments ?? "unknown";
		var info = directiveBlock.Info;
		var language = info is "{code}" or "{code-block}" ? codeBlockLanguage : info ?? "unknown";
		var caption = directiveBlock.DirectiveProperties.GetValueOrDefault("caption");
		var id = directiveBlock.DirectiveProperties.GetValueOrDefault("name");

		var slice = Code.Create(new CodeViewModel { Id = id, Language = language, Caption = caption });
		RenderRazorSlice(slice, renderer, directiveBlock);
	}


	private void WriteSideBar(HtmlRenderer renderer, DirectiveBlock directiveBlock)
	{
		var slice = SideBar.Create(new SideBarViewModel());
		RenderRazorSlice(slice, renderer, directiveBlock);
	}

	private int _seenTabSets;
	private void WriteTabSet(HtmlRenderer renderer, DirectiveBlock directiveBlock)
	{
		var slice = TabSet.Create(new TabSetViewModel());
		RenderRazorSlice(slice, renderer, directiveBlock);
		_seenTabSets++;
	}

	private void WriteTabItem(HtmlRenderer renderer, DirectiveBlock directiveBlock)
	{
		var title = directiveBlock.Arguments ?? "Unnamed Tab";
		var index = directiveBlock.Parent!.IndexOf(directiveBlock);
		var slice = TabItem.Create(new TabItemViewModel
		{
			Index = index, Title = title, TabSetIndex = _seenTabSets
		});
		RenderRazorSlice(slice, renderer, directiveBlock);
	}

	private static void RenderRazorSlice<T>(
		RazorSlice<T> slice, HtmlRenderer renderer, DirectiveBlock obj, bool implicitParagraph = true)
	{
		var html = slice.RenderAsync().GetAwaiter().GetResult();
		var blocks = html.Split("[CONTENT]", 2, StringSplitOptions.RemoveEmptyEntries);
		renderer.Write(blocks[0]);
		renderer.WriteChildren(obj);
		renderer.Write(blocks[1]);
	}
}

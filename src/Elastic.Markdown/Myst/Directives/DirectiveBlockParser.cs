// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Syntax;
using static System.StringSplitOptions;

namespace Elastic.Markdown.Myst.Directives;

/// <summary>
/// The block parser for a <see cref="DirectiveBlock"/>.
/// </summary>
/// <seealso cref="FencedBlockParserBase{CustomContainer}" />
public class DirectiveBlockParser : FencedBlockParserBase<DirectiveBlock>
{
	/// <summary>
    /// Initializes a new instance of the <see cref="DirectiveBlockParser"/> class.
    /// </summary>
    public DirectiveBlockParser()
    {
        OpeningCharacters = [':', '`'];
        // We don't need a prefix
        InfoPrefix = null;
    }

	private Dictionary<string, string> _admonitionData = new();

	private readonly string[] _admonitions = [
		"admonition", "attention", "caution", "danger", "error", "hint", "important", "note", "tip", "seealso"
	];

	private readonly string[] _versionBlocks = [ "versionadded", "versionchanged", "versionremoved", "deprecated" ];

	private readonly string[] _codeBlocks = [ "code", "code-block", "sourcecode"];

    protected override DirectiveBlock CreateFencedBlock(BlockProcessor processor)
    {
	    _admonitionData = new Dictionary<string, string>();
	    var info = processor.Line.AsSpan();

	    if (info.EndsWith("{toctree}"))
		    return new TocTreeBlock(this, _admonitionData);

	    if (info.IndexOf("{") == -1)
		    return new CodeBlock(this, "raw", _admonitionData);

	    if (info.IndexOf("{tab-set}") > 0)
		    return new TabSetBlock(this, _admonitionData);

	    if (info.IndexOf("{tab-item}") > 0)
		    return new TabItemBlock(this, _admonitionData);

	    if (info.IndexOf("{sidebar}") > 0)
		    return new SideBarBlock(this, _admonitionData);

	    if (info.IndexOf("{card}") > 0)
		    return new CardBlock(this, _admonitionData);

	    if (info.IndexOf("{grid}") > 0)
		    return new GridBlock(this, _admonitionData);

	    if (info.IndexOf("{grid-item-card}") > 0)
		    return new GridItemCardBlock(this, _admonitionData);

	    if (info.IndexOf("{dropdown}") > 0)
		    return new DropdownBlock(this, _admonitionData);

	    if (info.IndexOf("{image}") > 0)
		    return new ImageBlock(this, _admonitionData);

	    if (info.IndexOf("{figure}") > 0)
		    return new FigureBlock(this, _admonitionData);

	    if (info.IndexOf("{figure-md}") > 0)
		    return new FigureBlock(this, _admonitionData);

	    if (info.IndexOf("{mermaid}") > 0)
		    return new MermaidBlock(this, _admonitionData);

	    foreach (var admonition in _admonitions)
	    {
		    if (info.IndexOf($"{{{admonition}}}") > 0)
			    return new AdmonitionBlock(this, admonition, _admonitionData);
	    }

	    foreach (var version in _versionBlocks)
	    {
		    if (info.IndexOf($"{{{version}}}") > 0)
			    return new VersionBlock(this, version, _admonitionData);
	    }

	    foreach (var code in _codeBlocks)
	    {
		    if (info.IndexOf($"{{{code}}}") > 0)
			    return new CodeBlock(this, code, _admonitionData);
	    }

	    return new UnknownDirectiveBlock(this, info.ToString(), _admonitionData);
    }

    public override bool Close(BlockProcessor processor, Block block)
    {
	    if (block is DirectiveBlock directiveBlock)
		    directiveBlock.FinalizeAndValidate();


	    if (block is not TocTreeBlock toc)
		    return base.Close(processor, block);

	    if (toc is not { Count: > 0 } || toc[0] is not ParagraphBlock p)
			return base.Close(processor, block);

		var text =  p.Lines.ToSlice().AsSpan().ToString();
		foreach (var line in text.Split('\n'))
		{
			var tokens = line.Split('<', '>').Where(e => !string.IsNullOrWhiteSpace(e)).ToArray();
			var fileName = tokens.Last().Trim();
			var title = string.Join(" ", tokens.Take(tokens.Length - 1)).Trim();
			toc.Links.Add(new TocTreeLink { Title = title, Link = fileName });
		}

	    return base.Close(processor, block);
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
	    var line = processor.Line.AsSpan();

	    // TODO only parse this if no content proceeds it (and not in a code fence)
	    if (line.StartsWith(":"))
	    {
		    var tokens = line.ToString().Split(':', 3, RemoveEmptyEntries | TrimEntries);
		    if (tokens.Length < 1)
				return base.TryContinue(processor, block);

		    var name = tokens[0];
		    var data = tokens.Length > 1 ? string.Join(":", tokens[1..]) : string.Empty;
		    _admonitionData[name] = data;
		    return BlockState.Continue;
	    }

	    return base.TryContinue(processor, block);
    }
}

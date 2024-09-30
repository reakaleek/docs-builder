// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
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

    protected override DirectiveBlock CreateFencedBlock(BlockProcessor processor)
    {
	    _admonitionData = new Dictionary<string, string>();
	    var info = processor.Line;
	    if (info.AsSpan().EndsWith("{toctree}"))
	    {

			return new TocTreeBlock(this, _admonitionData);
	    }
	    return new DirectiveBlock(this, _admonitionData);
    }

    public override bool Close(BlockProcessor processor, Block block)
    {
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
		    var data = tokens.Length > 1 ? tokens[1] : string.Empty;
		    _admonitionData[name] = data;
		    return BlockState.Continue;
	    }

	    return base.TryContinue(processor, block);
    }
}

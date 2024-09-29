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

    protected override DirectiveBlock CreateFencedBlock(BlockProcessor processor)
    {
	    _admonitionData = new Dictionary<string, string>();
	    return new DirectiveBlock(this, _admonitionData);
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
	    var line = processor.Line.AsSpan();

	    // TODO only parse this if no content proceeds it (and not in a code fence)
	    if (line.StartsWith(":"))
	    {
		    var tokens = line.ToString().Split(':', 3, RemoveEmptyEntries | TrimEntries);
		    var name = tokens[0];
		    var data = tokens.Length > 1 ? tokens[1] : string.Empty;
		    _admonitionData[name] = data;
	    }

	    return base.TryContinue(processor, block);
    }
}

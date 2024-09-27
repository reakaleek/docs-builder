// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Syntax;
using static System.StringSplitOptions;

namespace Elastic.Markdown.Myst.CustomContainers;

/// <summary>
/// The block parser for a <see cref="Admonition"/>.
/// </summary>
/// <seealso cref="FencedBlockParserBase{CustomContainer}" />
public class AdmonitionParser : FencedBlockParserBase<Admonition>
{
	/// <summary>
    /// Initializes a new instance of the <see cref="AdmonitionParser"/> class.
    /// </summary>
    public AdmonitionParser()
    {
        OpeningCharacters = [':', '`'];

        // We don't need a prefix
        InfoPrefix = null;
    }

	private Dictionary<string, string> _admonitionData = new();
    public IReadOnlyDictionary<string, string> AdmonitionData => _admonitionData;

    protected override Admonition CreateFencedBlock(BlockProcessor processor)
    {
	    var data = _admonitionData;
	    _admonitionData = new Dictionary<string, string>();
	    return new Admonition(this, _admonitionData);
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
		    return BlockState.ContinueDiscard;
	    }

	    return base.TryContinue(processor, block);
    }
}

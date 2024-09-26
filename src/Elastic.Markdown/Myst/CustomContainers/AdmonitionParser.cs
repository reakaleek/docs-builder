// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

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

    protected override Admonition CreateFencedBlock(BlockProcessor processor) => new(this);
}

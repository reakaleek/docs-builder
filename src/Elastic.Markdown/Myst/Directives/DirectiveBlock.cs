// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Elastic.Markdown.Myst.Directives;

/// <summary>
/// A block custom container.
/// </summary>
/// <seealso cref="ContainerBlock" />
/// <seealso cref="IFencedBlock" />
public abstract class DirectiveBlock : ContainerBlock, IFencedBlock
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DirectiveBlock"/> class.
	/// </summary>
	/// <param name="blockParser">The parser used to create this block.</param>
	/// <param name="properties"></param>
	public DirectiveBlock(DirectiveBlockParser blockParser, Dictionary<string, string> properties) : base(blockParser) =>
	    Properties = properties;

    public IReadOnlyDictionary<string, string> Properties { get; }

    /// <inheritdoc />
    public char FencedChar { get; set; }

    /// <inheritdoc />
    public int OpeningFencedCharCount { get; set; }

    /// <inheritdoc />
    public StringSlice TriviaAfterFencedChar { get; set; }

    /// <inheritdoc />
    public string? Info { get; set; }

    /// <inheritdoc />
    public StringSlice UnescapedInfo { get; set; }

    /// <inheritdoc />
    public StringSlice TriviaAfterInfo { get; set; }

    /// <inheritdoc />
    public string? Arguments { get; set; }

    /// <inheritdoc />
    public StringSlice UnescapedArguments { get; set; }

    /// <inheritdoc />
    public StringSlice TriviaAfterArguments { get; set; }

    /// <inheritdoc />
    public NewLine InfoNewLine { get; set; }

    /// <inheritdoc />
    public StringSlice TriviaBeforeClosingFence { get; set; }

    /// <inheritdoc />
    public int ClosingFencedCharCount { get; set; }

    /// <summary>
    /// Allows blocks to finalize setting properties once fully parsed
    /// </summary>
    public abstract void FinalizeAndValidate();

	protected void ParseBool(string key, Action<bool> setter)
	{
		var value = Properties.GetValueOrDefault(key);
		if (string.IsNullOrEmpty(value))
		{
			setter(Properties.ContainsKey(key));
			return;
		}

		if (bool.TryParse(value, out var result))
			setter(result);
		//todo invalidate
	}

}

using Markdig.Helpers;

namespace Elastic.Markdown.Myst.Directives;

public class TocTreeLink
{
	public required string Link { get; init; }
	public string? Title { get; set; }
}

public class TocTreeBlock(DirectiveBlockParser parser, Dictionary<string, string> properties)
	: DirectiveBlock(parser, properties)
{
	public OrderedList<TocTreeLink> Links { get; } = new();

	public override void FinalizeAndValidate()
	{
	}
}

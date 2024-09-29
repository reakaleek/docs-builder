using Elastic.Markdown.DocSet;

namespace Elastic.Markdown.Templating;

public class LayoutModel
{
	public string Title { get; set; } = "Elastic Documentation";
	public required IReadOnlyCollection<PageTocItem> PageTocItems { get; init; }
	public required DocumentationGroup Tree { get; init; }
}

public class PageTocItem
{
	public required string Heading { get; init; }
	public required string Slug { get; init; }
}

public class TreeItemModel
{
	public required int Level { get; init; }
	public required MarkdownFile? Index { get; init; }
	public required DocumentationGroup SubTree { get; init; }
}

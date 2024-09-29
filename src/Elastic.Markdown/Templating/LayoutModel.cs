namespace Elastic.Markdown.Templating;

public class LayoutModel
{
	public string Title { get; set; } = "Elastic Documentation";
	public required IReadOnlyCollection<PageTocItem> PageTocItems { get; init; }
}

public class PageTocItem
{
	public required string Heading { get; init; }
	public required string Slug { get; init; }
}

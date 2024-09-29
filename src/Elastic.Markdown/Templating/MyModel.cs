using Elastic.Markdown.DocSet;

namespace Elastic.Markdown.Templating;

public class MyModel
{
	public required string Title { get; init; }
	public required string MarkdownHtml { get; init; }
	public required DocumentationGroup Tree { get; init; }
	public required IReadOnlyCollection<PageTocItem> PageTocItems { get; init; }
}

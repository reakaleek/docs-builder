using Elastic.Markdown.DocSet;
using Markdig.Syntax;

namespace Elastic.Markdown.Templating;

public class MarkdownPageModel
{
	public required string Title { get; init; }
	public required string MarkdownHtml { get; init; }
	public required DocumentationGroup Tree { get; init; }
	public required IReadOnlyCollection<PageTocItem> PageTocItems { get; init; }
	public required MarkdownFile CurrentDocument { get; init; }
	public required string Navigation { get; init; }
}

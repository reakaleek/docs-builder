namespace Elastic.Markdown.Slices.Blocks;

public class AdmonitionHtml
{
	public required string Title { get; init; }
	public required string Info { get; init; }
	public required string? Id { get; init; }
	public required string? Classes { get; init; }
}

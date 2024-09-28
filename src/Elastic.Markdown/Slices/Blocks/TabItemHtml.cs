namespace Elastic.Markdown.Slices.Blocks;

public class TabItemHtml
{
	public required string Info { get; init; }
	public required string? Id { get; init; }
	public required string? Classes { get; init; }
	public required int Index { get; init; }
	public required int TabSetIndex { get; init; }
	public required string Title { get; init; }
}

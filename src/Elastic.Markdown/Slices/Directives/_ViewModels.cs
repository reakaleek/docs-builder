namespace Elastic.Markdown.Slices.Directives;

public class AdmonitionViewModel
{
	public required string Title { get; init; }
	public required string Directive { get; init; }
	public required string? Id { get; init; }
	public required string? Classes { get; init; }
}

public class CodeViewModel
{
	public required string? Caption { get; init; }
	public required string Language { get; init; }
	public required string? Id { get; init; }
}

public class VersionViewModel
{
	public required string Directive { get; init; }
	public required string VersionClass { get; init; }
	public required string Title { get; init; }
}

public class SideBarViewModel;
public class TabSetViewModel;

public class TabItemViewModel
{
	public required int Index { get; init; }
	public required int TabSetIndex { get; init; }
	public required string Title { get; init; }
}

public class CardViewModel
{
	public required string? Title { get; init; }
	public required string? Link { get; init; }
}

public class GridViewModel
{
	public required int BreakPointXs { get; init; }
	public required int BreakPointSm { get; init; }
	public required int BreakPointMd { get; init; }
	public required int BreakPointLg { get; init; }

}

public class GridItemCardViewModel
{
	public required string? Title { get; init; }
	public required string? Link { get; init; }
}

using System.IO.Abstractions;

namespace Elastic.Markdown;

public record BuildContext
{
	public bool Force { get; init; }
	public string? UrlPathPrefix { get; init; }

	public required IFileSystem ReadFileSystem { get; init; }
	public required IFileSystem WriteFileSystem { get; init; }
}

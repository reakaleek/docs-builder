using System.IO.Abstractions;

namespace Elastic.Markdown.IO;

public abstract class DocumentationFile(IFileInfo sourceFile, IDirectoryInfo rootPath)
{
	public IFileInfo SourceFile { get; } = sourceFile;
	public string RelativePath { get; } = Path.GetRelativePath(rootPath.FullName, sourceFile.FullName);

	public FileInfo OutputFile(IDirectoryInfo outputPath) =>
		new(Path.Combine(outputPath.FullName, RelativePath.Replace(".md", ".html")));
}

public class ImageFile(IFileInfo sourceFile, IDirectoryInfo rootPath, string mimeType = "image/png")
	: DocumentationFile(sourceFile, rootPath)
{
	public string MimeType { get; } = mimeType;
}

public class StaticFile(IFileInfo sourceFile, IDirectoryInfo rootPath)
	: DocumentationFile(sourceFile, rootPath);

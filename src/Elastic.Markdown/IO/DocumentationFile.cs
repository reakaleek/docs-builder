namespace Elastic.Markdown.IO;

public abstract class DocumentationFile(FileInfo sourceFile, DirectoryInfo sourcePath)
{
	public FileInfo SourceFile { get; } = sourceFile;
	public string RelativePath { get; } = Path.GetRelativePath(sourcePath.FullName, sourceFile.FullName);

	public FileInfo OutputFile(DirectoryInfo outputPath) =>
		new(Path.Combine(outputPath.FullName, RelativePath.Replace(".md", ".html")));
}

public class ImageFile(FileInfo sourceFile, DirectoryInfo sourcePath)
	: DocumentationFile(sourceFile, sourcePath);

public class StaticFile(FileInfo sourceFile, DirectoryInfo sourcePath)
	: DocumentationFile(sourceFile, sourcePath);

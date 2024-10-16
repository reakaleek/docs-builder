using System.IO.Abstractions;
using Elastic.Markdown.Myst;

namespace Elastic.Markdown.IO;

public class DocumentationSet
{
	public string Name { get; }
	public IDirectoryInfo SourcePath { get; }
	public IDirectoryInfo OutputPath { get; }

	private MarkdownParser MarkdownParser { get; } = new();

	public DocumentationSet(IDirectoryInfo? sourcePath, IDirectoryInfo? outputPath, IFileSystem fileSystem)
	{
		SourcePath = sourcePath ?? fileSystem.DirectoryInfo.New(Path.Combine(Paths.Root.FullName, "docs/source"));
		OutputPath = outputPath ?? fileSystem.DirectoryInfo.New(Path.Combine(Paths.Root.FullName, ".artifacts/docs/html"));
		Name = SourcePath.FullName;

		Files = fileSystem.Directory.EnumerateFiles(SourcePath.FullName, "*.*", SearchOption.AllDirectories)
			.Select(f => fileSystem.FileInfo.New(f))
			.Select<IFileInfo, DocumentationFile>(file => file.Extension switch
			{
				".svg" => new ImageFile(file, SourcePath, "image/svg+xml"),
				".png" => new ImageFile(file, SourcePath),
				".md" => new MarkdownFile(file, SourcePath, MarkdownParser),
				_ => new StaticFile(file, SourcePath)
			})
			.ToList();

		FlatMappedFiles = Files.ToDictionary(file => file.RelativePath, file => file);

		var markdownFiles = Files.OfType<MarkdownFile>()
			.Where(file => !file.RelativePath.StartsWith("_"))
			.GroupBy(file =>
			{
				var path = file.ParentFolders.Count >= 1 ? file.ParentFolders[0] : file.FileName;
				return path;
			})
			.ToDictionary(k => k.Key, v => v.ToArray());

		Tree = new DocumentationFolder(markdownFiles, 0, "");
	}

	public DocumentationFolder Tree { get; }

	public List<DocumentationFile> Files { get; }

	public Dictionary<string, DocumentationFile> FlatMappedFiles { get; }

	public void ClearOutputDirectory()
	{
		if (OutputPath.Exists)
			OutputPath.Delete(true);
		OutputPath.Create();
	}
}

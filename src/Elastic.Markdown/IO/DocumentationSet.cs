using Elastic.Markdown.Myst;

namespace Elastic.Markdown.IO;

public class DocumentationSet
{
	public string Name { get; }
	public DirectoryInfo SourcePath { get; } = new (Path.Combine(Paths.Root.FullName, "docs/source"));
	public DirectoryInfo OutputPath { get; } = new (Path.Combine(Paths.Root.FullName, ".artifacts/docs/html"));

	private MarkdownParser MarkdownParser { get; } = new();

	public DocumentationSet(DirectoryInfo? sourcePath, DirectoryInfo? outputPath)
	{
		SourcePath = sourcePath ?? SourcePath;
		OutputPath = outputPath ?? OutputPath;
		Name = SourcePath.FullName;

		Files = Directory.EnumerateFiles(SourcePath.FullName, "*.*", SearchOption.AllDirectories)
			.Select(f => new FileInfo(f))
			.Select<FileInfo, DocumentationFile>(file => file.Extension switch
			{
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

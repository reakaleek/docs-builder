namespace Elastic.Markdown.DocSet;

public class DocumentationSet
{
	public string Name { get; }
	public DirectoryInfo SourcePath { get; }
	public DirectoryInfo OutputPath { get; }

	private MarkdownConverter MarkdownConverter { get; }

	public DocumentationSet(string name, DirectoryInfo sourcePath, DirectoryInfo outputPath, MarkdownConverter markdownConverter)
	{
		Name = name;
		SourcePath = sourcePath;
		OutputPath = outputPath;
		MarkdownConverter = markdownConverter;

		Files = Directory.EnumerateFiles(SourcePath.FullName, "*.*", SearchOption.AllDirectories)
			.Select(f => new FileInfo(f))
			.Select<FileInfo, DocumentationFile>(file => file.Extension switch
			{
				".png" => new ImageFile(file, SourcePath, OutputPath),
				".md" => new MarkdownFile(file, SourcePath, OutputPath)
				{
					MarkdownConverter = MarkdownConverter
				},
				_ => new StaticFile(file, SourcePath, OutputPath)
			})
			.ToList();

		Map = Files.ToDictionary(file => file.RelativePath, file => file);
	}

	public List<DocumentationFile> Files { get; }
	public Dictionary<string, DocumentationFile> Map { get; }

	public void ClearOutputDirectory()
	{
		if (OutputPath.Exists)
			OutputPath.Delete(true);
		OutputPath.Create();
	}
}

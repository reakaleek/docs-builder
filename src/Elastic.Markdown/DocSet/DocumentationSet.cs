namespace Elastic.Markdown.DocSet;

public class DocumentationGroup
{
	public MarkdownFile? Index { get; }
	public MarkdownFile[] Files { get; }
	public DocumentationGroup[] Nested { get; }
	public int Level { get; }
	public string? FolderName { get; }

	public DocumentationGroup(Dictionary<string, MarkdownFile[]> markdownFiles, int level, string folderName)
	{
		Level = level;
		FolderName = folderName;

		var files = markdownFiles
			.Where(k => k.Key.EndsWith(".md")).SelectMany(g => g.Value)
			.Where(file => file.ParentFolders.Count == level)
			.ToArray();


		Files = files
			.Where(file => file.FileName != "index.md")
			.ToArray();

		Index = files.FirstOrDefault(f => f.FileName == "index.md");

		var newLevel = level + 1;
		var groups = new List<DocumentationGroup>();
		foreach (var kv in markdownFiles.Where(kv=> !kv.Key.EndsWith(".md")))
		{
			var folder = kv.Key;
			var folderFiles = kv.Value
				.Where(file => file.ParentFolders.Count > level)
				.Where(file => file.ParentFolders[level] == folder).ToArray();
			var mapped = folderFiles
				.GroupBy(file =>
				{
					var path = file.ParentFolders.Count > newLevel ? file.ParentFolders[newLevel] : file.FileName;
					return path;
				})
				.ToDictionary(k => k.Key, v => v.ToArray());
			var documentationGroup  = new DocumentationGroup(mapped, newLevel, folder);
			groups.Add(documentationGroup);

		}
		Nested = groups.ToArray();
	}

	public async Task Resolve(CancellationToken ctx = default)
	{
		await (Index?.ParseAsync(ctx) ?? Task.CompletedTask);
		foreach (var f in Files) await f.ParseAsync(ctx);
		foreach (var n in Nested) await n.Resolve(ctx);
	}
}

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

		FlatMappedFiles = Files.ToDictionary(file => file.RelativePath, file => file);

		var markdownFiles = Files.OfType<MarkdownFile>()
			.Where(file => !file.RelativePath.StartsWith("_"))
			.GroupBy(file =>
			{
				var path = file.ParentFolders.Count >= 1 ? file.ParentFolders[0] : file.FileName;
				return path;
			})
			.ToDictionary(k => k.Key, v => v.ToArray());

		Tree = new DocumentationGroup(markdownFiles, 0, "");
	}

	public DocumentationGroup Tree { get; }

	public List<DocumentationFile> Files { get; }
	public Dictionary<string, DocumentationFile> FlatMappedFiles { get; }

	public void ClearOutputDirectory()
	{
		if (OutputPath.Exists)
			OutputPath.Delete(true);
		OutputPath.Create();
	}
}

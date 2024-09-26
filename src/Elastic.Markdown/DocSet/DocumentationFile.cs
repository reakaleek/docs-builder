using System.Text;

namespace Elastic.Markdown.DocSet;

public class DocumentationFile
{
	public Encoding Encoding { get; }
	public FileInfo SourceFile { get; }
	public FileInfo OutputFile { get; }
	public string RelativePath { get; }

	public DocumentationFile(FileInfo sourceFile, DirectoryInfo sourcePath, DirectoryInfo outputPath)
	{
		SourceFile = sourceFile;
		RelativePath = Path.GetRelativePath(sourcePath.FullName, sourceFile.FullName);
		OutputFile  = new FileInfo(Path.Combine(outputPath.FullName, RelativePath.Replace(".md", ".html")));
		Encoding = GetEncoding();
	}

	private Encoding GetEncoding()
	{
		using var sr = new StreamReader(SourceFile.FullName, true);
		while (sr.Peek() >= 0) sr.Read();

		return sr.CurrentEncoding;
	}
}

public class DocumentationSet
{
	public string Name { get; }
	public DirectoryInfo SourcePath { get; }
	public DirectoryInfo OutputPath { get; }

	public DocumentationSet(string name, DirectoryInfo sourcePath, DirectoryInfo outputPath)
	{
		Name = name;
		SourcePath = sourcePath;
		OutputPath = outputPath;

		Files = Directory.EnumerateFiles(SourcePath.FullName, "*.*", SearchOption.AllDirectories)
			.Select(file => new DocumentationFile(new FileInfo(file), SourcePath, OutputPath))
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

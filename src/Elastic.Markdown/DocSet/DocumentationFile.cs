using System.Text;
using Elastic.Markdown.Templating;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Slugify;

namespace Elastic.Markdown.DocSet;

public abstract class DocumentationFile
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

public class ImageFile(FileInfo sourceFile, DirectoryInfo sourcePath, DirectoryInfo outputPath)
	: DocumentationFile(sourceFile, sourcePath, outputPath);

public class StaticFile(FileInfo sourceFile, DirectoryInfo sourcePath, DirectoryInfo outputPath)
	: DocumentationFile(sourceFile, sourcePath, outputPath);

public class MarkdownFile(FileInfo sourceFile, DirectoryInfo sourcePath, DirectoryInfo outputPath)
	: DocumentationFile(sourceFile, sourcePath, outputPath)
{
	private readonly SlugHelper _slugHelper = new();
	public required MarkdownConverter MarkdownConverter { get; init; }
	private YamlFrontMatterConverter YamlFrontMatterConverter { get; } = new();

	public string? Title { get; private set; }

	private MarkdownDocument? Document { get; set; }
	public List<PageTocItem> TableOfContents { get; } = new();

	public async Task<MarkdownDocument> ParseAsync(CancellationToken ctx)
	{
		Document = await MarkdownConverter.ParseAsync(SourceFile, ctx);
		if (Document.FirstOrDefault() is YamlFrontMatterBlock yaml)
		{
			var raw = string.Join(Environment.NewLine, yaml.Lines.Lines);
			var frontMatter = YamlFrontMatterConverter.Deserialize(raw);
			Title = frontMatter.Title;
		}

		var contents = Document
			.Where(block => block is HeadingBlock { Level: 2 })
			.Cast<HeadingBlock>()
			.Select(h => h.Inline?.FirstChild?.ToString())
			.Where(title => !string.IsNullOrWhiteSpace(title))
			.Select(title => new PageTocItem { Heading = title!, Slug = _slugHelper.GenerateSlug(title) })
			.ToList();
		TableOfContents.Clear();
		TableOfContents.AddRange(contents);


		return Document;
	}

	public string CreateHtml() => Document?.ToHtml(MarkdownConverter.Pipeline) ?? string.Empty;
}

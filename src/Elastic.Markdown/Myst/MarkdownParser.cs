using System.IO.Abstractions;
using Cysharp.IO;
using Elastic.Markdown.Myst.Comments;
using Elastic.Markdown.Myst.Directives;
using Elastic.Markdown.Myst.Substitution;
using Markdig;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Syntax;

namespace Elastic.Markdown.Myst;


public class MystMarkdownParserContext(MarkdownParser markdownParser, IFileInfo path, YamlFrontMatter? front) : MarkdownParserContext
{
	public MarkdownParser Parser { get; } = markdownParser;
	public IFileInfo Path { get; } = path;
	public YamlFrontMatter? FrontMatter { get; } = front;
}

public class MarkdownParser(IDirectoryInfo sourcePath, IFileSystem fileSystem)
{
	public IDirectoryInfo SourcePath { get; } = sourcePath;
	public IFileSystem FileSystem { get; } = fileSystem;

	public MarkdownPipeline Pipeline =>
		new MarkdownPipelineBuilder()
			.EnableTrackTrivia()
			.UseGenericAttributes()
			.UseEmphasisExtras(EmphasisExtraOptions.Default)
			.UseSoftlineBreakAsHardlineBreak()
			.UseSubstitution()
			.UseComments()
			.UseYamlFrontMatter()
			.UseGridTables()
			.UsePipeTables()
			.UseDirectives()
			.Build();


	// TODO only scan for yaml front matter and toc information
	public Task<MarkdownDocument> QuickParseAsync(IFileInfo path, Cancel ctx)
	{
		var context = new MystMarkdownParserContext(this, path, null);
		return ParseAsync(path, context, ctx);
	}

	public Task<MarkdownDocument> ParseAsync(IFileInfo path, YamlFrontMatter? matter, Cancel ctx)
	{
		var context = new MystMarkdownParserContext(this, path, matter);
		if (matter?.Properties is { } props)
		{
			foreach (var (key, value) in props)
				context.Properties[key] = value;
		}
		return ParseAsync(path, context, ctx);
	}

	private async Task<MarkdownDocument> ParseAsync(IFileInfo path, MarkdownParserContext context, Cancel ctx)
	{
		if (path.FileSystem is FileSystem)
		{
			//real IO optimize through UTF8 stream reader.
			await using var streamReader = new Utf8StreamReader(path.FullName, fileOpenMode: FileOpenMode.Throughput);
			var inputMarkdown = await streamReader.AsTextReader().ReadToEndAsync(ctx);
			var markdownDocument = Markdig.Markdown.Parse(inputMarkdown, Pipeline, context);
			return markdownDocument;
		}
		else
		{
			var inputMarkdown = await path.FileSystem.File.ReadAllTextAsync(path.FullName, ctx);
			var markdownDocument = Markdig.Markdown.Parse(inputMarkdown, Pipeline, context);
			return markdownDocument;
		}
	}
}

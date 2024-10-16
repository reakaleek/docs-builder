using System.IO.Abstractions;
using Cysharp.IO;
using Elastic.Markdown.Myst.Directives;
using Markdig;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Syntax;

namespace Elastic.Markdown.Myst;

public class MarkdownParser
{
	public MarkdownPipeline Pipeline =>
		new MarkdownPipelineBuilder()
			.EnableTrackTrivia()
			.UseGenericAttributes()
			.UseEmphasisExtras(EmphasisExtraOptions.Default)
			.UseSoftlineBreakAsHardlineBreak()
			.UseComments()
			.UseYamlFrontMatter()
			.UseGridTables()
			.UsePipeTables()
			.UseDirectives()
			.Build();

	public async Task<MarkdownDocument> ParseAsync(IFileInfo path, Cancel ctx)
	{
		var context = new MarkdownParserContext();
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

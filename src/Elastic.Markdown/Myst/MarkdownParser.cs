using Cysharp.IO;
using Elastic.Markdown.Myst.Directives;
using Markdig;
using Markdig.Syntax;

namespace Elastic.Markdown.Myst;

public class MarkdownParser
{
	public MarkdownPipeline Pipeline =>
		new MarkdownPipelineBuilder()
			.EnableTrackTrivia()
			.UseYamlFrontMatter()
			.UseGridTables()
			.UsePipeTables()
			.UseDirectives()
			.Build();

	public async Task<MarkdownDocument> ParseAsync(FileInfo path, CancellationToken ctx)
	{
		await using var streamReader = new Utf8StreamReader(path.FullName, fileOpenMode: FileOpenMode.Throughput);
		var inputMarkdown = await streamReader.AsTextReader().ReadToEndAsync(ctx);
		var context = new MarkdownParserContext();
		var markdownDocument = Markdig.Markdown.Parse(inputMarkdown, Pipeline, context);
		return markdownDocument;
	}
}

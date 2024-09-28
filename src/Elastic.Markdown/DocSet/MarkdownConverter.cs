using Cysharp.IO;
using Elastic.Markdown.Myst.Directives;
using Markdig;
using Markdig.Syntax;

namespace Elastic.Markdown.DocSet;

public class MarkdownConverter(MarkdownPipeline? pipeline = null)
{
	public MarkdownPipeline Pipeline { get; } =
		pipeline ??
		new MarkdownPipelineBuilder()
			.EnableTrackTrivia()
			.UseYamlFrontMatter()
			.UseGridTables()
			.UsePipeTables()
			.UseDirectives()
			//.UseGenericAttributes()
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

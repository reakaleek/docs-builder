using Cysharp.IO;
using Elastic.Markdown.Myst.CustomContainers;
using Markdig;
using Markdig.Syntax;

namespace Elastic.Markdown.DocSet;

public class MarkdownConverter(MarkdownPipeline? pipeline = null)
{
	private readonly MarkdownPipeline _defaultPipeline =
		pipeline ??
		new MarkdownPipelineBuilder()
			.UseAdmonitions()
			.EnableTrackTrivia()
			//.UseGenericAttributes()
			.Build();

	public async Task<MarkdownDocument> ParseAsync(FileInfo path, CancellationToken ctx)
	{
		await using var streamReader = new Utf8StreamReader(path.FullName, fileOpenMode: FileOpenMode.Throughput);
		var inputMarkdown = await streamReader.AsTextReader().ReadToEndAsync(ctx);
		var context = new MarkdownParserContext();
		var markdownDocument = Markdig.Markdown.Parse(inputMarkdown, _defaultPipeline, context);
		return markdownDocument;
	}

	public string CreateHtml(MarkdownDocument document) => document.ToHtml(_defaultPipeline);
}

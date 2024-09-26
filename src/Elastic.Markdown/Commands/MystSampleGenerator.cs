using Elastic.Markdown.DocSet;

namespace Elastic.Markdown.Commands;

public class MystSampleGenerator
{
	public DirectoryInfo SourcePath { get; } = new (Path.Combine(Paths.Root.FullName, "docs/source"));
	public DirectoryInfo OutputPath { get; } = new (Path.Combine(Paths.Root.FullName, ".artifacts/docs/html"));

	public DocumentationSet DocumentationSet { get; set; }

	public HtmlTemplateWriter HtmlWriter { get; } = new ();

	public MarkdownConverter MarkdownConverter { get; } = new();

	public MystSampleGenerator() =>
		DocumentationSet = new DocumentationSet("Documentation Reference", SourcePath, OutputPath);

	public async Task Build(CancellationToken ctx)
	{
		DocumentationSet.ClearOutputDirectory();

		foreach (var file in DocumentationSet.Files)
		{
			var parsed = await MarkdownConverter.ParseAsync(file.SourceFile, ctx);
			var html = MarkdownConverter.CreateHtml(parsed);
			await HtmlWriter.WriteAsync(file.OutputFile, html, ctx);
		}

		await Task.CompletedTask;
	}

}

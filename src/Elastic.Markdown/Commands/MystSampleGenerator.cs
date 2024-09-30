using Elastic.Markdown.DocSet;
using Markdig.Syntax;

namespace Elastic.Markdown.Commands;

public class MystSampleGenerator
{
	public DirectoryInfo SourcePath { get; } = new (Path.Combine(Paths.Root.FullName, "docs/source"));
	public DirectoryInfo OutputPath { get; } = new (Path.Combine(Paths.Root.FullName, ".artifacts/docs/html"));

	public DocumentationSet DocumentationSet { get; set; }

	public HtmlTemplateWriter HtmlWriter { get; } = new ();

	public MarkdownConverter MarkdownConverter { get; } = new();

	public MystSampleGenerator() =>
		DocumentationSet = new DocumentationSet("Documentation Reference", SourcePath, OutputPath, MarkdownConverter);

	public async Task Build(CancellationToken ctx)
	{
		DocumentationSet.ClearOutputDirectory();

		foreach (var file in DocumentationSet.Files)
		{
			if (file is MarkdownFile markdown)
			{
				await markdown.ParseAsync(ctx);
				await HtmlWriter.WriteAsync(DocumentationSet, file.OutputFile, markdown, ctx);
			}
			else
				File.Copy(file.SourceFile.FullName, file.OutputFile.FullName, true);
		}

		await Task.CompletedTask;
	}

	public async Task<string?> RenderLayout(MarkdownFile markdown, CancellationToken ctx)
	{
		await DocumentationSet.Tree.Resolve(markdown, ctx);
		return await HtmlWriter.RenderLayout(DocumentationSet, markdown, ctx);
	}
}

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

	public MystSampleGenerator(string? path, string? output)
	{
		SourcePath = path != null ? new DirectoryInfo(path) : SourcePath;
		OutputPath = output != null ? new DirectoryInfo(output) : OutputPath;
		DocumentationSet = new DocumentationSet(SourcePath, OutputPath, MarkdownConverter);
	}

	public async Task Build(CancellationToken ctx)
	{
		DocumentationSet.ClearOutputDirectory();

		Console.WriteLine("Resolving tree");
		await DocumentationSet.Tree.Resolve(ctx);
		Console.WriteLine("Resolved tree");

		var handledItems = 0;
		await Parallel.ForEachAsync(DocumentationSet.Files, ctx, async (file, token) =>
		{
			var item = Interlocked.Increment(ref handledItems);
			if (file is MarkdownFile markdown)
			{
				await markdown.ParseAsync(token);
				await HtmlWriter.WriteAsync(DocumentationSet, file.OutputFile, markdown, token);
			}
			else
			{
				if (file.OutputFile.Directory is { Exists: false })
					file.OutputFile.Directory.Create();
				File.Copy(file.SourceFile.FullName, file.OutputFile.FullName, true);
			}
			if (item % 1_000 == 0)
				Console.WriteLine($"Handled {handledItems} files");
		});
	}

	private string? _navigation;
	public async Task<string?> RenderLayout(MarkdownFile markdown, CancellationToken ctx)
	{
		await DocumentationSet.Tree.Resolve(ctx);
		_navigation ??= await HtmlWriter.RenderNavigation(DocumentationSet, markdown, ctx);
		return await HtmlWriter.RenderLayout(DocumentationSet, markdown, _navigation, ctx);
	}
}

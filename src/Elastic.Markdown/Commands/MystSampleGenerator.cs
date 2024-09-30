using Elastic.Markdown.DocSet;

namespace Elastic.Markdown.Commands;

public class MystSampleGenerator
{
	public DirectoryInfo SourcePath { get; } = new (Path.Combine(Paths.Root.FullName, "docs/source"));
	public DirectoryInfo OutputPath { get; } = new (Path.Combine(Paths.Root.FullName, ".artifacts/docs/html"));

	public DocumentationSet DocumentationSet { get; }

	public HtmlTemplateWriter HtmlWriter { get; }

	public MarkdownConverter MarkdownConverter { get; } = new();

	public MystSampleGenerator(string? path, string? output)
	{
		SourcePath = path != null ? new DirectoryInfo(path) : SourcePath;
		OutputPath = output != null ? new DirectoryInfo(output) : OutputPath;
		DocumentationSet = new DocumentationSet(SourcePath, OutputPath, MarkdownConverter);
		HtmlWriter = new HtmlTemplateWriter(DocumentationSet);
	}

	public async Task ResolveDirectoryTree(CancellationToken ctx) =>
		await DocumentationSet.Tree.Resolve(ctx);

	public async Task ReloadNavigation(MarkdownFile current, CancellationToken ctx) =>
		await HtmlWriter.ReloadNavigation(current, ctx);

	public async Task Build(CancellationToken ctx)
	{
		DocumentationSet.ClearOutputDirectory();

		Console.WriteLine("Resolving tree");
		await ResolveDirectoryTree(ctx);
		Console.WriteLine("Resolved tree");

		var handledItems = 0;
		await Parallel.ForEachAsync(DocumentationSet.Files, ctx, async (file, token) =>
		{
			var item = Interlocked.Increment(ref handledItems);
			if (file is MarkdownFile markdown)
			{
				await markdown.ParseAsync(token);
				await HtmlWriter.WriteAsync(file.OutputFile, markdown, token);
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

	public async Task<string?> RenderLayout(MarkdownFile markdown, CancellationToken ctx)
	{
		await DocumentationSet.Tree.Resolve(ctx);
		return await HtmlWriter.RenderLayout(markdown, ctx);
	}
}

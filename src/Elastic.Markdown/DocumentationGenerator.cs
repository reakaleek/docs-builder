using Elastic.Markdown.IO;
using Elastic.Markdown.Slices;
using Microsoft.Extensions.Logging;

namespace Elastic.Markdown;

public class DocumentationGenerator
{
	private readonly ILogger _logger;
	private HtmlWriter HtmlWriter { get; }

	public DocumentationSet DocumentationSet { get; }

	public DocumentationGenerator(DocumentationSet docSet, ILoggerFactory logger)
	{
		_logger = logger.CreateLogger(nameof(DocumentationGenerator));

		DocumentationSet = docSet;
		HtmlWriter = new HtmlWriter(DocumentationSet);
	}

	public static DocumentationGenerator Create(string? path, string? output, ILoggerFactory logger)
	{
		var sourcePath = path != null ? new DirectoryInfo(path) : null;
		var outputPath = output != null ? new DirectoryInfo(output) : null;
		var docSet = new DocumentationSet(sourcePath, outputPath);
		return new DocumentationGenerator(docSet, logger);
	}

	public async Task ResolveDirectoryTree(Cancel ctx) =>
		await DocumentationSet.Tree.Resolve(ctx);

	public async Task ReloadNavigationAsync(MarkdownFile current, Cancel ctx) =>
		await HtmlWriter.ReloadNavigation(current, ctx);

	public async Task GenerateAll(Cancel ctx)
	{
		DocumentationSet.ClearOutputDirectory();

		_logger.LogInformation("Resolving tree");
		await ResolveDirectoryTree(ctx);
		_logger.LogInformation("Resolved tree");

		var handledItems = 0;
		await Parallel.ForEachAsync(DocumentationSet.Files, ctx, async (file, token) =>
		{
			var item = Interlocked.Increment(ref handledItems);
			var outputFile = file.OutputFile(DocumentationSet.OutputPath);
			if (file is MarkdownFile markdown)
			{
				await markdown.ParseAsync(token);
				await HtmlWriter.WriteAsync(outputFile, markdown, token);
			}
			else
			{
				if (outputFile.Directory is { Exists: false })
					outputFile.Directory.Create();
				File.Copy(file.SourceFile.FullName, outputFile.FullName, true);
			}
			if (item % 1_000 == 0)
				Console.WriteLine($"Handled {handledItems} files");
		});
	}

	public async Task<string?> RenderLayout(MarkdownFile markdown, Cancel ctx)
	{
		await DocumentationSet.Tree.Resolve(ctx);
		return await HtmlWriter.RenderLayout(markdown, ctx);
	}
}

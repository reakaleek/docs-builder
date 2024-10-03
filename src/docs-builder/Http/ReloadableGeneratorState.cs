using Elastic.Markdown;
using Elastic.Markdown.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Documentation.Builder.Http;

/// <summary>Singleton behaviour enforced by registration on <see cref="IServiceCollection"/></summary>
public class ReloadableGeneratorState(DirectoryInfo? sourcePath, DirectoryInfo? outputPath, ILoggerFactory logger)
{
	private DirectoryInfo? SourcePath { get; } = sourcePath;
	private DirectoryInfo? OutputPath { get; } = outputPath;

	private DocumentationGenerator _generator = new(new DocumentationSet(sourcePath, outputPath), logger);
	public DocumentationGenerator Generator => _generator;

	public async Task ReloadAsync(Cancel ctx)
	{
		SourcePath?.Refresh();
		OutputPath?.Refresh();
		var docSet = new DocumentationSet(SourcePath, OutputPath);
		var generator = new DocumentationGenerator(docSet, logger);
		await generator.ResolveDirectoryTree(ctx);
		Interlocked.Exchange(ref _generator, generator);
	}

	public async Task ReloadNavigationAsync(MarkdownFile current, CancellationToken ctx) =>
		await Generator.ReloadNavigationAsync(current, ctx);
}

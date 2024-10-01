using Elastic.Markdown;
using Elastic.Markdown.Files;
using Microsoft.Extensions.DependencyInjection;

namespace Documentation.Builder.Http;

/// <summary>Singleton behaviour enforced by registration on <see cref="IServiceCollection"/></summary>
public class ReloadableGeneratorState(DirectoryInfo? sourcePath, DirectoryInfo? outputPath)
{
	private DirectoryInfo? SourcePath { get; } = sourcePath;
	private DirectoryInfo? OutputPath { get; } = outputPath;

	private DocumentationGenerator _generator = new(new DocumentationSet(sourcePath, outputPath));
	public DocumentationGenerator Generator => _generator;

	public async Task ReloadAsync(CancellationToken ctx)
	{
		SourcePath?.Refresh();
		OutputPath?.Refresh();
		var docSet = new DocumentationSet(SourcePath, OutputPath);
		var generator = new DocumentationGenerator(docSet);
		await generator.ResolveDirectoryTree(ctx);
		Interlocked.Exchange(ref _generator, generator);
	}

	public async Task ReloadNavigationAsync(MarkdownFile current, CancellationToken ctx) =>
		await Generator.ReloadNavigationAsync(current, ctx);
}

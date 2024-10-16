using System.IO.Abstractions;
using Elastic.Markdown;
using Elastic.Markdown.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Documentation.Builder.Http;

/// <summary>Singleton behaviour enforced by registration on <see cref="IServiceCollection"/></summary>
public class ReloadableGeneratorState(IDirectoryInfo? sourcePath, IDirectoryInfo? outputPath, ILoggerFactory logger, IFileSystem fileSystem)
{
	private IDirectoryInfo? SourcePath { get; } = sourcePath;
	private IDirectoryInfo? OutputPath { get; } = outputPath;

	private DocumentationGenerator _generator = new(new DocumentationSet(sourcePath, outputPath, fileSystem), logger, fileSystem);
	public DocumentationGenerator Generator => _generator;

	public async Task ReloadAsync(Cancel ctx)
	{
		SourcePath?.Refresh();
		OutputPath?.Refresh();
		var docSet = new DocumentationSet(SourcePath, OutputPath, fileSystem);
		var generator = new DocumentationGenerator(docSet, logger, fileSystem);
		await generator.ResolveDirectoryTree(ctx);
		Interlocked.Exchange(ref _generator, generator);
	}

	public async Task ReloadNavigationAsync(MarkdownFile current, CancellationToken ctx) =>
		await Generator.ReloadNavigationAsync(current, ctx);
}

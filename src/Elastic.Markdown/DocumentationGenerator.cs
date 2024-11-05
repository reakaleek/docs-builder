using System.IO.Abstractions;
using Elastic.Markdown.IO;
using Elastic.Markdown.Slices;
using Microsoft.Extensions.Logging;

namespace Elastic.Markdown;

public class DocumentationGenerator
{
	private readonly IFileSystem _readFileSystem;
	private readonly ILogger _logger;
	private readonly IFileSystem _writeFileSystem;
	private HtmlWriter HtmlWriter { get; }

	public DocumentationSet DocumentationSet { get; }

	public DocumentationGenerator(DocumentationSet docSet, ILoggerFactory logger, IFileSystem readFileSystem, IFileSystem? writeFileSystem = null)
	{
		_readFileSystem = readFileSystem;
		_writeFileSystem = writeFileSystem ?? readFileSystem;
		_logger = logger.CreateLogger(nameof(DocumentationGenerator));

		DocumentationSet = docSet;
		HtmlWriter = new HtmlWriter(DocumentationSet, _writeFileSystem);

		_logger.LogInformation($"Created documentation set for: {DocumentationSet.Name}");
		_logger.LogInformation($"Source directory: {docSet.SourcePath} Exists: {docSet.SourcePath.Exists}");
		_logger.LogInformation($"Output directory: {docSet.OutputPath} Exists: {docSet.OutputPath.Exists}");
	}

	public static DocumentationGenerator Create(string? path, string? output, ILoggerFactory logger, IFileSystem fileSystem, string? pathPrefix = null)
	{
		var sourcePath = path != null ? fileSystem.DirectoryInfo.New(path) : null;
		var outputPath = output != null ? fileSystem.DirectoryInfo.New(output) : null;
		var docSet = new DocumentationSet(sourcePath, outputPath, fileSystem, pathPrefix);
		return new DocumentationGenerator(docSet, logger, fileSystem);
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
			var outputFile = OutputFile(file.RelativePath);
			if (file is MarkdownFile markdown)
			{
				await markdown.ParseAsync(token);
				await HtmlWriter.WriteAsync(outputFile, markdown, token);
			}
			else
			{
				if (outputFile.Directory is { Exists: false })
					outputFile.Directory.Create();
				await CopyFileFsAware(file, outputFile, ctx);
			}
			if (item % 1_000 == 0)
				_logger.LogInformation($"Handled {handledItems} files");
		});

		IFileInfo OutputFile(string relativePath)
		{
			var outputFile = _writeFileSystem.FileInfo.New(Path.Combine(DocumentationSet.OutputPath.FullName, relativePath));
			return outputFile;
		}
	}

	private async Task CopyFileFsAware(DocumentationFile file, IFileInfo outputFile, Cancel ctx)
	{
		// fast path, normal case.
		if (_readFileSystem == _writeFileSystem)
			_readFileSystem.File.Copy(file.SourceFile.FullName, outputFile.FullName, true);
		//slower when we are mocking the write filesystem
		else
		{
			var bytes = await file.SourceFile.FileSystem.File.ReadAllBytesAsync(file.SourceFile.FullName, ctx);
			await outputFile.FileSystem.File.WriteAllBytesAsync(outputFile.FullName, bytes, ctx);
		}
	}

	public async Task<string?> RenderLayout(MarkdownFile markdown, Cancel ctx)
	{
		await DocumentationSet.Tree.Resolve(ctx);
		return await HtmlWriter.RenderLayout(markdown, ctx);
	}
}

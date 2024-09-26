using Cysharp.IO;
using Markdig;
using Markdig.Syntax;

namespace Elastic.Markdown.Commands;

public class DocSetConverter(string? sourcePath, string? outputPath)
{
	public string SourcePath { get; } = sourcePath ?? ".artifacts/docset-source";
	public string OutputPath { get; } = outputPath ?? ".artifacts/docset-generated";

	public static MarkdownPipeline DefaultPipeline =
		new MarkdownPipelineBuilder()
			.UseAlertBlocks()
			.UseAbbreviations()
			.UseAutoIdentifiers()
			.UseCitations()
			.UseCustomContainers()
			.UseDefinitionLists()
			.UseEmphasisExtras()
			.UseFigures()
			.UseFooters()
			.UseFootnotes()
			.UseMathematics()
			.UseMediaLinks()
			.UsePipeTables()
			.UseListExtras()
			.UseTaskLists()
			.UseAutoLinks()
			//.UseGridTables()
			//.UseDiagrams()
			.UseGenericAttributes() // Must be last as it is one parser that is modifying other parsers
			//.EnableTrackTrivia()
			.DisableHtml()
			.Build();

	public async Task Build(CancellationToken cancellationToken)
	{
		var dir = new DirectoryInfo(OutputPath);
		if (OutputPath.StartsWith(".artifacts") && dir.Exists)
			dir.Delete(true);
		dir.Create();

		var files = Directory.EnumerateFiles(SourcePath, "*.*", SearchOption.AllDirectories);

		/*
		foreach (var file in files)
		{
			await Task.CompletedTask;
		}

		*/
		/*
		foreach (var file in files)
		{
			var parsed = await ParseAsync(file);
			var relative = Path.GetRelativePath(SourcePath, file);
			var outputPath = new FileInfo(Path.Combine(OutputPath, relative.Replace(".md", ".html")));
			Directory.CreateDirectory(outputPath.Directory!.FullName);
			var html = parsed.ToHtml(DefaultPipeline);
			await File.WriteAllTextAsync(outputPath.FullName, html);
		}
		*/

		await Parallel.ForEachAsync(files, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, async (file, ctx) =>
		{
			var parsed = await ParseAsync(file);
			var relative = Path.GetRelativePath(SourcePath, file);
			var outputPath = new FileInfo(Path.Combine(OutputPath, relative.Replace(".md", ".html")));
			Directory.CreateDirectory(outputPath.Directory!.FullName);
			var html = parsed.ToHtml(DefaultPipeline);
			await File.WriteAllTextAsync(outputPath.FullName, html);
		});
		await Task.CompletedTask;
	}

	public static async Task<MarkdownDocument> ParseAsync(string path)
	{
		await using var streamReader = new Utf8StreamReader(path, fileOpenMode: FileOpenMode.Throughput);
		var inputMarkdown = await streamReader.AsTextReader().ReadToEndAsync();
		var context = new MarkdownParserContext();
		var markdownDocument = Markdig.Markdown.Parse(inputMarkdown, DefaultPipeline, context);
		return markdownDocument;
	}
}

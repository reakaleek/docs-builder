// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information
using System.IO.Abstractions;
using Cysharp.IO;
using Elastic.Markdown.Myst.Comments;
using Elastic.Markdown.Myst.Directives;
using Elastic.Markdown.Myst.Substitution;
using Markdig;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Syntax;

namespace Elastic.Markdown.Myst;


public class ParserContext : MarkdownParserContext
{
	public ParserContext(MarkdownParser markdownParser,
		IFileInfo path,
		YamlFrontMatter? frontMatter,
		BuildContext context)
	{
		Parser = markdownParser;
		Path = path;
		FrontMatter = frontMatter;
		Build = context;

		if (frontMatter?.Properties is { } props)
		{
			foreach (var (key, value) in props)
				Properties[key] = value;
		}
	}

	public MarkdownParser Parser { get; }
	public IFileInfo Path { get; }
	public YamlFrontMatter? FrontMatter { get; }
	public BuildContext Build { get; }
}

public class MarkdownParser(IDirectoryInfo sourcePath, BuildContext context)
{
	public IDirectoryInfo SourcePath { get; } = sourcePath;
	public BuildContext Context { get; } = context;

	public MarkdownPipeline Pipeline =>
		new MarkdownPipelineBuilder()
			.EnableTrackTrivia()
			.UseGenericAttributes()
			.UseEmphasisExtras(EmphasisExtraOptions.Default)
			.UseSoftlineBreakAsHardlineBreak()
			.UseSubstitution()
			.UseComments()
			.UseYamlFrontMatter()
			.UseGridTables()
			.UsePipeTables()
			.UseDirectives()
			.Build();


	// TODO only scan for yaml front matter and toc information
	public Task<MarkdownDocument> QuickParseAsync(IFileInfo path, Cancel ctx)
	{
		var context = new ParserContext(this, path, null, Context);
		return ParseAsync(path, context, ctx);
	}

	public Task<MarkdownDocument> ParseAsync(IFileInfo path, YamlFrontMatter? matter, Cancel ctx)
	{
		var context = new ParserContext(this, path, matter, Context);
		return ParseAsync(path, context, ctx);
	}

	private async Task<MarkdownDocument> ParseAsync(IFileInfo path, MarkdownParserContext context, Cancel ctx)
	{
		if (path.FileSystem is FileSystem)
		{
			//real IO optimize through UTF8 stream reader.
			await using var streamReader = new Utf8StreamReader(path.FullName, fileOpenMode: FileOpenMode.Throughput);
			var inputMarkdown = await streamReader.AsTextReader().ReadToEndAsync(ctx);
			var markdownDocument = Markdig.Markdown.Parse(inputMarkdown, Pipeline, context);
			return markdownDocument;
		}
		else
		{
			var inputMarkdown = await path.FileSystem.File.ReadAllTextAsync(path.FullName, ctx);
			var markdownDocument = Markdig.Markdown.Parse(inputMarkdown, Pipeline, context);
			return markdownDocument;
		}
	}
}

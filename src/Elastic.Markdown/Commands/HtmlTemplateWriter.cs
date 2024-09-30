using Elastic.Markdown.DocSet;
using Elastic.Markdown.Templating;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RazorSlices;

namespace Elastic.Markdown.Commands;

public class HtmlTemplateWriter
{
	public HtmlTemplateWriter()
	{
		var services = new ServiceCollection();
		services.AddLogging();

		ServiceProvider = services.BuildServiceProvider();
		LoggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
	}

	public ILoggerFactory LoggerFactory { get; }
	public ServiceProvider ServiceProvider { get; }

	public async Task<string> RenderNavigation(DocumentationSet documentationSet, MarkdownFile markdown, CancellationToken ctx = default)
	{
		var slice = Slices.Layout._TocTree.Create(new Navigationmodel
		{
			Tree = documentationSet.Tree,
			CurrentDocument = markdown
		});
		return await slice.RenderAsync(cancellationToken: ctx);
	}

	public async Task<string> RenderLayout(
		DocumentationSet documentationSet,
		MarkdownFile markdown,
		string navigation,
		CancellationToken ctx = default)
	{
		var html = await markdown.CreateHtmlAsync(ctx);
		await documentationSet.Tree.Resolve(ctx);
		var slice = Slices.Index.Create(new MarkdownPageModel
		{
			Title = markdown.Title ?? "[TITLE NOT SET]",
			MarkdownHtml = html,
			PageTocItems = markdown.TableOfContents,
			Tree = documentationSet.Tree,
			CurrentDocument = markdown,
			Navigation = navigation
		});
		return await slice.RenderAsync(cancellationToken: ctx);
	}

	private string? _navigation;
	public async Task WriteAsync(DocumentationSet documentationSet, FileInfo outputFile, MarkdownFile markdown,
		CancellationToken ctx = default)
	{
		if (outputFile.Directory is { Exists: false })
			outputFile.Directory.Create();

		_navigation ??= await RenderNavigation(documentationSet, markdown, ctx);
		var rendered = await RenderLayout(documentationSet, markdown, _navigation, ctx);
		await File.WriteAllTextAsync(outputFile.FullName, rendered, ctx);
	}
}

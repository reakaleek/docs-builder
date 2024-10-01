using Elastic.Markdown.Files;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RazorSlices;

namespace Elastic.Markdown.Slices;

public class HtmlWriter
{
	public HtmlWriter(DocumentationSet documentationSet)
	{
		var services = new ServiceCollection();
		services.AddLogging();

		ServiceProvider = services.BuildServiceProvider();
		LoggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
		DocumentationSet = documentationSet;
	}

	private DocumentationSet DocumentationSet { get; }
	public ILoggerFactory LoggerFactory { get; }
	public ServiceProvider ServiceProvider { get; }

	private string? _navigation;

	private async Task<string> RenderNavigation(MarkdownFile markdown, CancellationToken ctx = default)
	{
		if (_navigation is { Length: > 0 }) return _navigation;
		var slice = Layout._TocTree.Create(new NavigationViewModel
		{
			Tree = DocumentationSet.Tree,
			CurrentDocument = markdown
		});
		_navigation ??= await slice.RenderAsync(cancellationToken: ctx);
		return _navigation;
	}

	public async Task ReloadNavigation(MarkdownFile current, CancellationToken ctx)
	{
		_navigation = null;
		_ = await RenderNavigation(current, ctx);
	}

	public async Task<string> RenderLayout(MarkdownFile markdown, CancellationToken ctx = default)
	{
		var html = await markdown.CreateHtmlAsync(ctx);
		await DocumentationSet.Tree.Resolve(ctx);
		var navigationHtml = await RenderNavigation(markdown, ctx);
		var slice = Index.Create(new IndexViewModel
		{
			Title = markdown.Title ?? "[TITLE NOT SET]",
			MarkdownHtml = html,
			PageTocItems = markdown.TableOfContents,
			Tree = DocumentationSet.Tree,
			CurrentDocument = markdown,
			Navigation = navigationHtml
		});
		return await slice.RenderAsync(cancellationToken: ctx);
	}

	public async Task WriteAsync(FileInfo outputFile, MarkdownFile markdown, CancellationToken ctx = default)
	{
		if (outputFile.Directory is { Exists: false })
			outputFile.Directory.Create();

		var rendered = await RenderLayout(markdown, ctx);
		await File.WriteAllTextAsync(outputFile.FullName, rendered, ctx);
	}

}

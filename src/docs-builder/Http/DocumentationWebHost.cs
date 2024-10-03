using Elastic.Markdown.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Documentation.Builder.Http;

public class DocumentationWebHost
{
	private readonly WebApplication _webApplication;

	private readonly string _staticFilesDirectory =
		Path.Combine(Paths.Root.FullName, "docs", "source", "_static_template");

	public DocumentationWebHost(string? path, ILoggerFactory logger)
	{
		var builder = WebApplication.CreateSlimBuilder();
		var sourcePath = path != null ? new DirectoryInfo(path) : null;
		builder.Services.AddSingleton<ReloadableGeneratorState>(_ => new ReloadableGeneratorState(sourcePath, null, logger));
		builder.Services.AddHostedService<ReloadGeneratorService>();
		builder.Services.AddSingleton(logger);

		_webApplication = builder.Build();
		SetUpRoutes();
	}

	public async Task RunAsync(Cancel ctx) => await _webApplication.RunAsync(ctx);

	private void SetUpRoutes()
	{
		_webApplication.UseStaticFiles(new StaticFileOptions
		{
			FileProvider = new PhysicalFileProvider(_staticFilesDirectory),
			RequestPath = "/_static"
		});
		_webApplication.UseRouting();

		_webApplication.MapGet("/", async (ReloadableGeneratorState holder, Cancel ctx) =>
			await ServeDocumentationFile(holder, "index.md", ctx));

		_webApplication.MapGet("{**slug}", async (string slug, ReloadableGeneratorState holder, Cancel ctx) =>
			await ServeDocumentationFile(holder, slug, ctx));
	}

	private static async Task<IResult> ServeDocumentationFile(ReloadableGeneratorState holder, string slug, Cancel ctx)
	{
		var generator = holder.Generator;
		slug = slug.Replace(".html", ".md");
		if (!generator.DocumentationSet.FlatMappedFiles.TryGetValue(slug, out var documentationFile))
			return Results.NotFound();

		switch (documentationFile)
		{
			case MarkdownFile markdown:
			{
				await holder.ReloadNavigationAsync(markdown, ctx);
				await markdown.ParseAsync(ctx);
				var rendered = await generator.RenderLayout(markdown, ctx);
				return Results.Content(rendered, "text/html");
			}
			case ImageFile image:
				return Results.File(image.SourceFile.FullName, "image/png");
			default:
				return Results.NotFound();
		}
	}
}

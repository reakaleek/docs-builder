using ConsoleAppFramework;
using Elastic.Markdown;
using Elastic.Markdown.Commands;
using Elastic.Markdown.DocSet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

var app = ConsoleApp.Create();
app.UseFilter<CommandTimings>();

app.Add("example-generator", async Task (int? count = null, string? path = null, CancellationToken ctx = default) =>
{
	var generator = new ExampleGenerator(count, path);
	await generator.Build(ctx);
});

app.Add("example-converter", async Task (string? path = null, string? output = null, CancellationToken ctx = default) =>
{
	var generator = new DocSetConverter(path, output);
	await generator.Build(ctx);
});

app.Add("generate", async Task (string? path = null, string? output = null, CancellationToken ctx = default) =>
{
	var generator = new MystSampleGenerator(path, output);
	Console.WriteLine("Fetched documentation set");
	await generator.Build(ctx);
});

app.Add("serve", (string? path = null, string? output = null) =>
{
	var builder = WebApplication.CreateSlimBuilder(args);
	builder.Services.AddSingleton<LiveDocumentationHolder>(c => new LiveDocumentationHolder(path, output));
	builder.Services.AddHostedService<LiveDocumentationService>();

	var webApplication = builder.Build();

	webApplication.UseStaticFiles(new StaticFileOptions
	{
		FileProvider = new PhysicalFileProvider(Path.Combine(Paths.Root.FullName, "docs", "source", "_static_template")),
		RequestPath = "/_static"
	});
	webApplication.UseRouting();

	webApplication.MapGet("/", async (LiveDocumentationHolder holder, CancellationToken ctx) =>
	{
		var generator = holder.Generator;
		if (!generator.DocumentationSet.FlatMappedFiles.TryGetValue("index.md", out var documentationFile)
		    || documentationFile is not MarkdownFile markdown)
			return Results.NotFound();

		await holder.ReloadNavigation(markdown, ctx);

		await markdown.ParseAsync(ctx);
		var rendered = await generator.RenderLayout(markdown, ctx);
		return Results.Content(rendered, "text/html");
	});
	webApplication.MapGet("{**slug}", async (string slug, LiveDocumentationHolder holder, CancellationToken ctx) =>
	{
		var generator = holder.Generator;
		slug = slug.Replace(".html", ".md");
		if (!generator.DocumentationSet.FlatMappedFiles.TryGetValue(slug, out var documentationFile))
			return Results.NotFound();

		switch (documentationFile)
		{
			case MarkdownFile markdown:
			{
				await holder.ReloadNavigation(markdown, ctx);
				await markdown.ParseAsync(ctx);
				var rendered = await generator.RenderLayout(markdown, ctx);
				return Results.Content(rendered, "text/html");
			}
			case ImageFile image:
				return Results.File(image.SourceFile.FullName, "image/png");
			default:
				return Results.NotFound();
		}
	});

	webApplication.Run();
});
app.Run(args);

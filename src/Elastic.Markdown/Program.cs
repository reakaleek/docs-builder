using ConsoleAppFramework;
using Elastic.Markdown;
using Elastic.Markdown.Commands;
using Elastic.Markdown.DocSet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

var app = ConsoleApp.Create();
app.UseFilter<CommandTimings>();

app.Add("generate", async Task (int? count = null, string? path = null, CancellationToken ctx = default) =>
{
	var generator = new ExampleGenerator(count, path);
	await generator.Build(ctx);
});

app.Add("convert", async Task (string? path = null, string? output = null, CancellationToken ctx = default) =>
{
	var generator = new DocSetConverter(path, output);
	await generator.Build(ctx);
});

app.Add("myst", async Task (CancellationToken ctx = default) =>
{
	var generator = new MystSampleGenerator();
	await generator.Build(ctx);
});

app.Add("serve", () =>
{
	var builder = WebApplication.CreateSlimBuilder(args);

	var app = builder.Build();

	app.UseStaticFiles(new StaticFileOptions
	{
		FileProvider = new PhysicalFileProvider(Path.Combine(Paths.Root.FullName, "docs", "source", "_static_template")),
		RequestPath = "/_static",
	});
	app.UseRouting();

	app.MapGet("/", async (CancellationToken ctx) =>
	{
		var generator = new MystSampleGenerator();
		if (!generator.DocumentationSet.FlatMappedFiles.TryGetValue("index.md", out var documentationFile)
		    || documentationFile is not MarkdownFile markdown)
			return Results.NotFound();

		await markdown.ParseAsync(ctx);
		var rendered = await generator.RenderLayout(markdown, ctx);
		return Results.Content(rendered, "text/html");
	});
	app.MapGet("{**slug}", async (string slug, CancellationToken ctx) =>
	{
		var generator = new MystSampleGenerator();
		slug = slug.Replace(".html", ".md");
		if (!generator.DocumentationSet.FlatMappedFiles.TryGetValue(slug, out var documentationFile))
			return Results.NotFound();

		switch (documentationFile)
		{
			case MarkdownFile markdown:
			{
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

	app.Run();
});
app.Run(args);

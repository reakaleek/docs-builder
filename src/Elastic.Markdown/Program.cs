using System.Text.Json.Serialization;
using ConsoleAppFramework;
using Elastic.Markdown;
using Elastic.Markdown.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
	var generator = new MystSampleGenerator();
	var builder = WebApplication.CreateSlimBuilder(args);

	builder.Services.ConfigureHttpJsonOptions(options =>
	{
		options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
	});

	var app = builder.Build();

	app.UseStaticFiles(new StaticFileOptions
	{
		FileProvider = new PhysicalFileProvider(Path.Combine(Paths.Root.FullName, "docs", "source", "_static_template")),
		RequestPath = "/_static"
	});

	var todosApi = app.MapGroup("/");
	todosApi.MapGet("/", async (CancellationToken ctx) =>
	{
		if (!generator.DocumentationSet.Map.TryGetValue("index.md", out var documentationFile))
			return Results.NotFound();

		var parsed = await generator.MarkdownConverter.ParseAsync(documentationFile.SourceFile, ctx);
		var html = generator.MarkdownConverter.CreateHtml(parsed);
		var rendered = await generator.HtmlWriter.RenderLayout(html, ctx);
		return Results.Content(rendered, "text/html");
	});

	app.Run();
});
app.Run(args);

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}

using ConsoleAppFramework;
using Documentation.Builder;
using Documentation.Builder.Http;
using Elastic.Markdown;

var app = ConsoleApp.Create();
app.UseFilter<CommandTimings>();

app.Add("generate", async Task (string? path = null, string? output = null, CancellationToken ctx = default) =>
{
	var generator = DocumentationGenerator.Create(path, output);
	await generator.GenerateAll(ctx);
});

app.Add("serve", async Task (string? path = null, CancellationToken ctx = default) =>
{
	var host = new DocumentationWebHost(path, args);
	await host.RunAsync(ctx);
});

app.Run(args);

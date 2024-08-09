using ConsoleAppFramework;
using Elastic.Markdown;

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

app.Run(args);

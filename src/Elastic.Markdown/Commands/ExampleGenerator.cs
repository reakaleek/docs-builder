using System.Reflection;

namespace Elastic.Markdown.Commands;

public class Template(string name, string contents)
{
	public string Name { get; } = name;
	public string Contents { get; } = contents;
}

public class ExampleGenerator(int? count, string? path)
{
	private int Count { get; } = count ?? 1_000;
	public string OutputPath { get; } = path ?? ".artifacts/docset-source";

	public async Task Build(CancellationToken ctx = default)
	{
		var dir = new DirectoryInfo(OutputPath);
		if (OutputPath.StartsWith(".artifacts") && dir.Exists)
			dir.Delete(true);
		dir.Create();

		var random = new Random();
		var folders = RandomSubFolderStructure();
		var templates = await GetTemplates(ctx);

		await GenerateExamples(templates, random, folders, Count - 3, ctx);
		//generate 3 docs at root
		await GenerateExamples(templates, random, [""], 3, ctx);

		Console.WriteLine($"Generated {Count} example markdown files in '{OutputPath}'");
	}

	private async Task GenerateExamples(List<Template> templates, Random random, List<string> folders, int count, CancellationToken ctx)
	{
		foreach (var i in Enumerable.Range(0, count))
		{
			var template = templates[random.Next(0, templates.Count)];
			var folder = folders[random.Next(0, folders.Count)];
			var file = template.Name.Replace(".md", $"-{i}.md");
			var path = new FileInfo(Path.Combine(OutputPath, folder, file));
			Directory.CreateDirectory(path.Directory!.FullName);
			await File.WriteAllTextAsync(path.FullName, template.Contents, ctx);
		}
	}

	private static List<string> RandomSubFolderStructure()
	{
		var randomDepth = new Random();
		var folders = Enumerable.Range(0, 10)
			.Select(_ => randomDepth.Next(1, 4))
			.Select(depth =>
				Enumerable.Range(1, depth)
					.Select(_ => (char)('A' + randomDepth.Next(0, 26)))
					.Select(c => $"{c}".ToLowerInvariant())
					.ToArray())
			.Select(Path.Join)
			.ToList();

		return folders;
	}

	private static async Task<List<Template>> GetTemplates(CancellationToken ctx)
	{
		var assembly = Assembly.GetExecutingAssembly();
		var templates = assembly
			.GetManifestResourceNames()
			.Select(async n => await ReadEmbeddedTemplates(assembly, n, ctx))
			.ToList();

		var templateMap = new List<Template>();
		await foreach (var template in templates.WhenEach(ctx))
			templateMap.Add(template);
		return templateMap;
	}

	private static async Task<Template> ReadEmbeddedTemplates(Assembly assembly, string name, CancellationToken ctx)
	{
		await using var stream = assembly.GetManifestResourceStream(name)!;
		using var textReader = new StreamReader(stream);
		var text = await textReader.ReadToEndAsync(ctx);
		return new Template(name.Replace("docset-templates/", ""), text);
	}

}

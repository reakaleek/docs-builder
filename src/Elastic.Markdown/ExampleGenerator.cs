using System.Reflection;

namespace Elastic.Markdown;

public class ExampleGenerator(int? count, string? path)
{
	private int Count { get; } = count ?? 1_000;
	public string OutputPath { get; } = path ?? ".artifacts/docset-source";

	public async Task Build()
	{
		var dir = new DirectoryInfo(OutputPath);
		if (OutputPath.StartsWith(".artifacts") && dir.Exists)
			dir.Delete(true);
		dir.Create();

		var random = new Random();
		var folders = RandomSubFolderStructure();
		var templates = await GetTemplates();

		await GenerateExamples(templates, random, folders, Count);
		//generate 3 docs at root
		await GenerateExamples(templates, random, [""], 3);
	}

	private async Task GenerateExamples(List<Template> templates, Random random, List<string> folders, int count)
	{
		foreach (var i in Enumerable.Range(0, count))
		{
			var template = templates[random.Next(0, templates.Count)];
			var folder = folders[random.Next(0, folders.Count)];
			var file = template.Name.Replace(".md", $"-{i}.md");
			var path = new FileInfo(Path.Combine(OutputPath, folder, file));
			Console.WriteLine(path.FullName);
			Directory.CreateDirectory(path.Directory!.FullName);
			await File.WriteAllTextAsync(path.FullName, template.Contents);
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

	private static async Task<List<Template>> GetTemplates()
	{
		var assembly = Assembly.GetExecutingAssembly();
		var templates = assembly
			.GetManifestResourceNames()
			.Select(async n => await ReadEmbeddedTemplates(assembly, n))
			.ToList();

		var templateMap = new List<Template>();
		await foreach (var template in templates.WhenEach())
			templateMap.Add(template);
		return templateMap;
	}

	private static async Task<Template> ReadEmbeddedTemplates(Assembly assembly, string name)
	{
		await using var stream = assembly.GetManifestResourceStream(name)!;
		using var textReader = new StreamReader(stream);
		var text = await textReader.ReadToEndAsync();
		return new Template(name.Replace("docset-templates/", ""), text);
	}

}

namespace Elastic.Markdown;

public class Template(string name, string contents)
{
	public string Name { get; } = name;
	public string Contents { get; } = contents;
};

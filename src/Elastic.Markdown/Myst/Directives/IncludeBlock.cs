using System.IO.Abstractions;

namespace Elastic.Markdown.Myst.Directives;

public class IncludeBlock(DirectiveBlockParser parser, Dictionary<string, string> properties, MystMarkdownParserContext context)
	: DirectiveBlock(parser, properties)
{
	public IFileSystem FileSystem { get; } = context.Parser.FileSystem;

	public IDirectoryInfo DocumentationSourcePath { get; } = context.Parser.SourcePath;

	public IFileInfo IncludeFromPath { get; } = context.Path;

	public YamlFrontMatter? FrontMatter { get; } = context.FrontMatter;

	public string? IncludePath { get; private set; }

	public bool Literal { get; private set; }

	public bool Found { get; private set; }

	//TODO add all options from
	//https://mystmd.org/guide/directives#directive-include
	public override void FinalizeAndValidate()
	{
		var includePath = Arguments; //todo validate
		Literal = bool.TryParse(Properties.GetValueOrDefault("literal"), out var b) && b;
		if (includePath is null)
		{
			//TODO emit empty error
		}
		else
		{
			IncludePath = Path.Combine(IncludeFromPath.Directory!.FullName, includePath);
			if (FileSystem.File.Exists(IncludePath))
				Found = true;
		}


	}
}


public class LiteralIncludeBlock(DirectiveBlockParser parser, Dictionary<string, string> properties, MystMarkdownParserContext context)
	: IncludeBlock(parser, properties, context);

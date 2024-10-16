namespace Elastic.Markdown.Myst.Directives;

public class MermaidBlock(DirectiveBlockParser blockParser, Dictionary<string, string> properties)
	: DirectiveBlock(blockParser, properties)
{
	public override void FinalizeAndValidate()
	{
	}
}

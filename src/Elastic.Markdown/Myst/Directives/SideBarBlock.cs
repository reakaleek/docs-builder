namespace Elastic.Markdown.Myst.Directives;

public class SideBarBlock(DirectiveBlockParser parser, Dictionary<string, string> properties)
	: DirectiveBlock(parser, properties)
{
	public override void FinalizeAndValidate()
	{
	}
}

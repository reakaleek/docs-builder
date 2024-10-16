namespace Elastic.Markdown.Myst.Directives;

public class SideBarBlock(DirectiveBlockParser blockParser, Dictionary<string, string> properties)
	: DirectiveBlock(blockParser, properties)
{
	public override void FinalizeAndValidate()
	{
	}
}

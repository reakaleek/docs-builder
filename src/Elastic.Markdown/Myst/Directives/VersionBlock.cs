namespace Elastic.Markdown.Myst.Directives;

public class VersionBlock(DirectiveBlockParser blockParser, string directive, Dictionary<string, string> properties)
	: DirectiveBlock(blockParser, properties)
{
	public string Directive => directive;
	public string Class => directive.Replace("version", "");

	public string Title
	{
		get
		{
			var title = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(directive.Replace("version", "version "));
			if (!string.IsNullOrEmpty(Arguments))
				title += $" {Arguments}";

			return title;
		}
	}

	public override void FinalizeAndValidate()
	{
	}
}

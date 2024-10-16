namespace Elastic.Markdown.Myst.Directives;

public class CodeBlock(DirectiveBlockParser blockParser, string directive, Dictionary<string, string> properties)
	: DirectiveBlock(blockParser, properties)
{
	public string Directive => directive;
	public string? Caption { get; private set; }
	public string? CrossReferenceName { get; private set; }

	public string Language
	{
		get
		{
			var language = (Directive is "code" or "code-block" or "sourcecode" ? Arguments : Info) ?? "unknown";
			return language;

		}
	}

	public override void FinalizeAndValidate()
	{
		Caption = Properties.GetValueOrDefault("caption");
		CrossReferenceName = Properties.GetValueOrDefault("name");
	}
}

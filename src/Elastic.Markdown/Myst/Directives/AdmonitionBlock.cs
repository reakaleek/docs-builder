namespace Elastic.Markdown.Myst.Directives;

public class AdmonitionBlock(DirectiveBlockParser blockParser, string admonition, Dictionary<string, string> properties)
	: DirectiveBlock(blockParser, properties)
{
	public string Admonition => admonition == "admonition" ? Classes?.Trim() ?? "note" : admonition;
	public string? Classes { get; protected set; }
	public string? CrossReferenceName  { get; private set; }
	public bool? DropdownOpen  { get; private set; }

	public string Title
	{
		get
		{
			var t = Admonition == "seealso" ? "see also" : Admonition;
			var title = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(t);
			if (admonition is "admonition" && !string.IsNullOrEmpty(Arguments))
				title = Arguments;
			else if (!string.IsNullOrEmpty(Arguments))
				title += $" {Arguments}";
			return title;
		}
	}

	public override void FinalizeAndValidate()
	{
		Classes = Properties.GetValueOrDefault("class");
		CrossReferenceName = Properties.GetValueOrDefault("name");
		ParseBool("open", b => DropdownOpen = b);
	}
}


public class DropdownBlock(DirectiveBlockParser blockParser, Dictionary<string, string> properties)
	: AdmonitionBlock(blockParser, "admonition", properties)
{
	// ReSharper disable once RedundantOverriddenMember
	public override void FinalizeAndValidate()
	{
		base.FinalizeAndValidate();
		Classes = $"dropdown {Classes}";
	}
}

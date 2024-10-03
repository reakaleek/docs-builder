namespace Elastic.Markdown.IO;

public static class Paths
{
	private static DirectoryInfo RootDirectoryInfo()
	{
		var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
		while (directory != null && !directory.GetFiles("*.sln").Any())
			directory = directory.Parent;
		return directory ?? new DirectoryInfo(Directory.GetCurrentDirectory());
	}

	public static readonly DirectoryInfo Root = RootDirectoryInfo();
}

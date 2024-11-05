// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information
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

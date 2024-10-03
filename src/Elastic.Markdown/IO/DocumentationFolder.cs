using Markdig.Helpers;

namespace Elastic.Markdown.IO;

public class DocumentationFolder
{
	public MarkdownFile? Index { get; }
	private MarkdownFile[] Files { get; }
	private DocumentationFolder[] Nested { get; }

	public OrderedList<MarkdownFile> FilesInOrder { get; private set; }
	public OrderedList<DocumentationFolder> GroupsInOrder { get; private set; }
	public int Level { get; }
	public string? FolderName { get; }

	public DocumentationFolder(Dictionary<string, MarkdownFile[]> markdownFiles, int level, string folderName)
	{
		Level = level;
		FolderName = folderName;

		var files = markdownFiles
			.Where(k => k.Key.EndsWith(".md")).SelectMany(g => g.Value)
			.Where(file => file.ParentFolders.Count == level)
			.ToArray();


		Files = files
			.Where(file => file.FileName != "index.md")
			.ToArray();

		FilesInOrder = new OrderedList<MarkdownFile>(Files);

		Index = files.FirstOrDefault(f => f.FileName == "index.md");

		var newLevel = level + 1;
		var groups = new List<DocumentationFolder>();
		foreach (var kv in markdownFiles.Where(kv=> !kv.Key.EndsWith(".md")))
		{
			var folder = kv.Key;
			var folderFiles = kv.Value
				.Where(file => file.ParentFolders.Count > level)
				.Where(file => file.ParentFolders[level] == folder).ToArray();
			var mapped = folderFiles
				.GroupBy(file =>
				{
					var path = file.ParentFolders.Count > newLevel ? file.ParentFolders[newLevel] : file.FileName;
					return path;
				})
				.ToDictionary(k => k.Key, v => v.ToArray());
			var documentationGroup  = new DocumentationFolder(mapped, newLevel, folder);
			groups.Add(documentationGroup);

		}
		Nested = groups.ToArray();
		GroupsInOrder = new OrderedList<DocumentationFolder>(Nested);
	}

	public bool HoldsCurrent(MarkdownFile current) =>
		Index == current || Files.Contains(current) || Nested.Any(n => n.HoldsCurrent(current));

	private bool _resolved;
	public async Task Resolve(Cancel ctx = default)
	{
		if (_resolved) return;

		await Parallel.ForEachAsync(Files, ctx, async (file, token) => await file.ParseAsync(token));
		await Parallel.ForEachAsync(Nested, ctx, async (group, token) => await group.Resolve(token));

		//foreach (var f in Files) await f.ParseAsync(ctx);
		//foreach (var n in Nested) await n.Resolve(ctx);

		await (Index?.ParseAsync(ctx) ?? Task.CompletedTask);
		if (Index?.TocTree == null)
			return;

		var tree = Index.TocTree;
		var fileList = new OrderedList<MarkdownFile>();
		var groupList = new OrderedList<DocumentationFolder>();

		foreach (var link in tree)
		{
			var file = Files.FirstOrDefault(f => f.RelativePath.EndsWith(link.Link));
			if (file != null)
			{
				file.TocTitle = link.Title;
				fileList.Add(file);
				continue;
			}

			var group = Nested.FirstOrDefault(f => f.Index != null && f.Index.RelativePath.EndsWith(link.Link));
			if (group != null)
			{
				groupList.Add(group);
				if (group.Index != null && !string.IsNullOrEmpty(link.Title))
					group.Index.TocTitle = link.Title;
			}
			else if (group == null || file == null)
			{

			}
		}

		FilesInOrder = fileList;
		GroupsInOrder = groupList;
		_resolved = true;
	}
}

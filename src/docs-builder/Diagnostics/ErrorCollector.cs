using System.Diagnostics.CodeAnalysis;
using System.Text;
using Cysharp.IO;
using Elastic.Markdown.Diagnostics;
using Errata;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Diagnostic = Elastic.Markdown.Diagnostics.Diagnostic;

namespace Documentation.Builder.Diagnostics;

public class FileSourceRepository : ISourceRepository
{
	public bool TryGet(string id, [NotNullWhen(true)] out Source? source)
	{
		using var reader = new Utf8StreamReader(id);
		var text = Encoding.UTF8.GetString(reader.ReadToEndAsync().GetAwaiter().GetResult());
		source = new Source(id, text);
		return true;
	}
}

public class ConsoleDiagnosticsCollector(ILoggerFactory loggerFactory)
	: DiagnosticsCollector(loggerFactory, [])
{
	private readonly List<Diagnostic> _items = new();

	private readonly ILogger<ConsoleDiagnosticsCollector> _logger =
		loggerFactory.CreateLogger<ConsoleDiagnosticsCollector>();

	protected override void HandleItem(Diagnostic diagnostic) => _items.Add(diagnostic);

	public override async Task StopAsync(Cancel ctx)
	{
		_logger.LogError("Stopping...");
		// Create a new report
		var report = new Report(new FileSourceRepository());
		foreach (var item in _items)
		{
			var d = item.Severity switch
			{
				Severity.Error =>
					Errata.Diagnostic.Error(item.Message)
					.WithLabel(new Label(item.File, new Location(item.Line, item.Position ?? 0), "bad substitution")
					.WithLength(8)
					.WithPriority(1)
					.WithColor(Color.Red))

				,
				Severity.Warning =>
					Errata.Diagnostic.Warning(item.Message),
				_ => Errata.Diagnostic.Info(item.Message)
			};
			report.AddDiagnostic(d);
			/*
		report.AddDiagnostic(
			Errata.Diagnostic.Error("Operator '/' cannot be applied to operands of type 'string' and 'int'")
				.WithCode("CS0019")
				.WithNote("Try changing the type")
				.WithLabel(new Label("Demo/Files/Program.cs", new Location(15, 23), "This is of type 'int'")
					.WithLength(3)
					.WithPriority(1)
					.WithColor(Color.Yellow))
				.WithLabel(new Label("Demo/Files/Program.cs", new Location(15, 27), "Division is not possible")
					.WithPriority(3)
					.WithColor(Color.Red))
				.WithLabel(new Label("Demo/Files/Program.cs", new Location(15, 29), "This is of type 'string'")
					.WithLength(3)
					.WithPriority(2)
					.WithColor(Color.Blue)));
					*/
		}
		// Render the report
		report.Render(AnsiConsole.Console);
		AnsiConsole.WriteLine();
		await Task.CompletedTask;
	}
}

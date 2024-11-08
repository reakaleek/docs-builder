using Elastic.Markdown.Myst;
using Markdig.Helpers;
using Markdig.Parsers;

namespace Elastic.Markdown.Diagnostics;

public static class ProcessorDiagnosticExtensions
{
	public static void EmitError(this InlineProcessor processor, int line, int position, string message)
	{
		var d = new Diagnostic
		{
			Severity = Severity.Error,
			File = processor.GetContext().Path.FullName,
			Position = position,
			Line = line,
			Message = message
		};
		processor.GetBuildContext().Collector.Channel.Write(d);
	}
}

using Elastic.Markdown.Myst;
using Markdig.Helpers;
using Markdig.Parsers;

namespace Elastic.Markdown.Diagnostics;

public static class ProcessorDiagnosticExtensions
{
	public static void EmitError(this InlineProcessor processor, int line, int column, int length, string message)
	{
		var d = new Diagnostic
		{
			Severity = Severity.Error,
			File = processor.GetContext().Path.FullName,
			Column = column,
			Line = line,
			Message = message,
			Length = length
		};
		processor.GetBuildContext().Collector.Channel.Write(d);
	}
}

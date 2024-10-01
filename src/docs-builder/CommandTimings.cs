using System.Diagnostics;
using ConsoleAppFramework;

namespace Documentation.Builder;

internal class CommandTimings(ConsoleAppFilter next) : ConsoleAppFilter(next)
{
	public override async Task InvokeAsync(ConsoleAppContext context, CancellationToken cancellationToken)
	{
		Console.WriteLine($":: {context.CommandName} :: Starting");
		var sw = Stopwatch.StartNew();
		try
		{
			await Next.InvokeAsync(context, cancellationToken);
		}
		finally
		{
			sw.Stop();
			Console.WriteLine($":: {context.CommandName} :: Finished in '{sw.Elapsed}");
		}
	}
}

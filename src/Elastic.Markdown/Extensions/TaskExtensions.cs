using System.Runtime.CompilerServices;

namespace Elastic.Markdown;

public static class TaskExtensions
{
	// Temporarily until dotnet 9 is released, this is not ordered by completion
	public static async IAsyncEnumerable<T> WhenEach<T>(this IEnumerable<Task<T>> tasks, [EnumeratorCancellation] CancellationToken ctx)
	{
		foreach (var task in tasks)
		{
			if (ctx.IsCancellationRequested) yield break;
			yield return await task;

		}
	}
}

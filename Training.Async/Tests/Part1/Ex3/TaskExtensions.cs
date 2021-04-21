using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Tests.Part1.Ex3
{
	public static class TaskExtensions
	{
		public static CultureAwaiter WithCurrentCulture(this Task task)
		{
			return new CultureAwaiter(task);
		}

		public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan)
		{
			return Task.Delay(timeSpan).GetAwaiter();
		}

		public static TaskAwaiter GetAwaiter(this int millisecondsDelay)
		{
			return TimeSpan.FromMilliseconds(millisecondsDelay).GetAwaiter();
		}

		public static TaskAwaiter GetAwaiter(this DateTimeOffset dateTimeOffset)
		{
			return (dateTimeOffset - DateTimeOffset.UtcNow).GetAwaiter();
		}

		public static TaskAwaiter<int> GetAwaiter(this Process process)
		{
			var tsc = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

			process.EnableRaisingEvents = true;
			process.Exited += (sender, args) =>
			{
				if (!(sender is Process senderProcess))
				{
					return;
				}

				tsc.SetResult(senderProcess.ExitCode);
			};

			return tsc.Task.GetAwaiter();
		}
	}
}
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace AwaiterDemoAppDotNet452
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine(
				$"Current thread ID {Thread.CurrentThread.ManagedThreadId} with culture {Thread.CurrentThread.CurrentCulture.Name}");

			Thread.CurrentThread.CurrentCulture = new CultureInfo("pl-PL");

			Console.WriteLine(
				$"After change current thread ID {Thread.CurrentThread.ManagedThreadId} with culture {Thread.CurrentThread.CurrentCulture.Name}");

			Console.WriteLine();

			await Task.Run(() =>
			{
				Console.WriteLine(
					$"Async thread ID {Thread.CurrentThread.ManagedThreadId} with culture {Thread.CurrentThread.CurrentCulture.Name}");
			});

			Console.WriteLine(
				$"Current thread ID {Thread.CurrentThread.ManagedThreadId} with culture {Thread.CurrentThread.CurrentCulture.Name}");
		}
	}
}

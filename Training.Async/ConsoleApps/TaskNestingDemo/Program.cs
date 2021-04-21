using System;
using System.Threading;
using System.Threading.Tasks;

namespace TasksNestingDemoApp
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine($"Starting tasks - Thread {Thread.CurrentThread.ManagedThreadId}");

			await Task.Factory.StartNew(() =>
			{
				Console.WriteLine(
					$"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

				Task.Factory.StartNew(() =>
				{
					Thread.Sleep(1000);
					Console.WriteLine(
						$"Starting child task1 {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");
				}, TaskCreationOptions.AttachedToParent);

				Task.Factory.StartNew(() =>
				{
					Thread.Sleep(2000);
					Console.WriteLine(
						$"Starting child task2 {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");
				}, TaskCreationOptions.DenyChildAttach);

				Task.Factory.StartNew(() =>
				{
					Thread.Sleep(2000);
					Console.WriteLine(
						$"Starting child task3 {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");
				}, TaskCreationOptions.DenyChildAttach);
			});

			Console.WriteLine($"Ending tasks - Thread {Thread.CurrentThread.ManagedThreadId}");

			Console.WriteLine("Press key to close the app");
			Console.ReadKey();
		}
	}
}

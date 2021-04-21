using System;
using System.Threading;
using System.Threading.Tasks;

namespace TasksBasicsDemoApp
{
	class Program
	{
		static void Main(string[] args)
		{
			var taskVoidByFactory = Task.Factory.StartNew(() =>
			{
				Console.WriteLine($"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");
			});

			var taskWithValueByFactory = Task<int?>.Factory.StartNew(() =>
			{
				Console.WriteLine($"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

				return Task.CurrentId;
			});

			var taskVoidByRunMethod = Task.Factory.StartNew(() =>
			{
				Console.WriteLine($"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");
			});

			var taskWithValueByRunMethod = Task<int?>.Factory.StartNew(() =>
			{
				Console.WriteLine($"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

				return Task.CurrentId;
			});

			Task.WaitAll(
				taskVoidByRunMethod,
				taskWithValueByFactory,
				taskVoidByRunMethod,
				taskWithValueByRunMethod);

			Console.WriteLine($"Task result is {taskWithValueByRunMethod.Result}");
		}
	}
}

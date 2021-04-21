using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SemaphoreDemoApp
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var cts = new CancellationTokenSource();
			var token = cts.Token;

			var semaphoreObject = new Semaphore(initialCount: 2, maximumCount: 2, name: "Training.SemaphoreDemoApp");

			var task1 = Task.Run(() =>
			{
				var callerId = Guid.NewGuid().ToString("N");

				while (token.IsCancellationRequested == false)
				{
					semaphoreObject.WaitOne();

					Console.WriteLine($"Caller {callerId} reads the file");

					ReadFile(callerId).Wait(token);

					Thread.Sleep(1000);

					Console.WriteLine($"Caller {callerId} release the file");
					semaphoreObject.Release();
				}
			}, token);

			var task2 = Task.Run(() =>
			{
				var callerId = Guid.NewGuid().ToString("N");

				while (token.IsCancellationRequested == false)
				{
					semaphoreObject.WaitOne();

					Console.WriteLine($"Caller {callerId} reads the file");

					ReadFile(callerId).Wait(token);

					Thread.Sleep(1000);

					Console.WriteLine($"Caller {callerId} release the file");
					semaphoreObject.Release();
				}
			}, token);

			var task3 = Task.Run(() =>
			{
				var callerId = Guid.NewGuid().ToString("N");

				while (token.IsCancellationRequested == false)
				{
					semaphoreObject.WaitOne();

					Console.WriteLine($"Caller {callerId} reads the file");

					ReadFile(callerId).Wait(token);

					Thread.Sleep(1000);

					Console.WriteLine($"Caller {callerId} release the file");
					semaphoreObject.Release();
				}
			}, token);

			cts.CancelAfter(5000);

			await Task.WhenAll(task1, task2, task3);

			Console.WriteLine("Press key to exit!");
			Console.ReadKey();
		}

		private static async Task<string> ReadFile(string callerId)
		{
			var filePath = Path.Combine("Files", "data.txt");
			return await File.ReadAllTextAsync(filePath);
		}
	}
}

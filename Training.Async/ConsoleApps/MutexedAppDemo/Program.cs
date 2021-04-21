using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MutexDemoApp
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var callerId = Guid.NewGuid().ToString("N");

			var cts = new CancellationTokenSource();
			var token = cts.Token;

			var task = Task.Run(() =>
			{
				var mutex = new Mutex(false, "Training.MutexDemoApp");

				while (token.IsCancellationRequested == false)
				{
					mutex.WaitOne();

					Console.WriteLine($"Caller {callerId} updates file");

					UpdateFile(callerId).Wait(token);

					Thread.Sleep(1000);

					Console.WriteLine($"Caller {callerId} release the file");
					mutex.ReleaseMutex();
				}
			}, token);

			await Task.Delay(5000, token);
			Console.WriteLine($"Caller {callerId} has completed");
			cts.Cancel();

			await task;

			Console.WriteLine("Press key to exit!");
			Console.ReadKey();
		}

		private static async Task UpdateFile(string callerId)
		{
			var filePath = Path.Combine("Files", "data.txt");
			var dataLines = await File.ReadAllLinesAsync(filePath);

			var lastLine = dataLines.Any()
				? dataLines.Last()
				: "x:0";

			var oldValue = Convert.ToInt32(lastLine.Split(":")[1]);
			var newValue = oldValue + 1;

			await File.AppendAllLinesAsync(filePath, new[] {$"{callerId}:{newValue}"});
		}
	}
}

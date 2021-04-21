using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Medallion.Threading.SqlServer;

namespace DistributedLockDemoApp
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var connectionString = "Server=.;Database=TestLockDb;User=TestLockDbUser;Password=lock@123;";

			var callerId = Guid.NewGuid().ToString("N");

			var cts = new CancellationTokenSource();
			var token = cts.Token;

			cts.CancelAfter(5000);

			var @lock = new SqlDistributedLock("MyLockName", connectionString);

			while (token.IsCancellationRequested == false)
			{
				await using (await @lock.AcquireAsync(cancellationToken: token))
				{
					Console.WriteLine($"Caller {callerId} updates file");

					UpdateFile(callerId).Wait(token);

					Thread.Sleep(1000);

					Console.WriteLine($"Caller {callerId} release the file");
				}
			}

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

			await File.AppendAllLinesAsync(filePath, new[] { $"{callerId}:{newValue}" });
		}
	}
}

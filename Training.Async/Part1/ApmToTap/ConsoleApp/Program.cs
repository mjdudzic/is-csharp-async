using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine($"Starting program - Thread {Thread.CurrentThread.ManagedThreadId}");

			var dataService = new DataService(Path.Combine(Directory.GetCurrentDirectory(), "TestData"));

			var asyncResult = dataService.BeginDataRead("test2.txt", null, null);

			var data = dataService.EndDataRead(asyncResult);

			Console.WriteLine($"File contains text with length: {data.Length}");
			//Console.WriteLine($"{data}");

			var data2 = await dataService.GetDataAsync("test2.txt");

			Console.WriteLine($"File contains text with length: {data2.Length}");
			//Console.WriteLine($"{data2}");

		}
	}
}

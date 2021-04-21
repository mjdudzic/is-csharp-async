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

			var dataFromFile1Task = dataService.GetData("test1.txt");

			var dataFromFile2Task = dataService.GetData("test2.txt");

			var dataFromFile2 = await dataFromFile2Task;

			var dataFromFile1 = await dataFromFile1Task;

			Console.WriteLine($"File 1 contains text with length: {dataFromFile1.Length}");
			Console.WriteLine($"File 2 contains text with length: {dataFromFile2.Length}");

			Console.WriteLine($"Closing program - Thread {Thread.CurrentThread.ManagedThreadId}");
		}
	}
}

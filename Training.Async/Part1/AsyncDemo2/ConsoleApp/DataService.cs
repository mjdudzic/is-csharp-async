using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
	public class DataService
	{
		private readonly string _filesPath;

		public DataService(string filesPath)
		{
			_filesPath = filesPath;
		}

		public async Task<string> GetData(string fileName)
		{
			Console.WriteLine($"Getting data for {fileName} - Thread {Thread.CurrentThread.ManagedThreadId}");

			var filePath = Path.Combine(_filesPath, fileName);
			
			if (File.Exists(filePath) == false)
			{
				return null;
			}

			//await Task.Delay(1000);
			var data = await File.ReadAllTextAsync(filePath);

			Console.WriteLine($"Returning data for {fileName} - Thread {Thread.CurrentThread.ManagedThreadId}");
			return data;
		}
	}
}
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Tests.Part1.Ex2
{
	public class DataService
	{
		private readonly string _filesPath;
		private readonly ITestOutputHelper _testOutputHelper;

		public DataService(
			string filesPath,
			ITestOutputHelper testOutputHelper)
		{
			_filesPath = filesPath;
			_testOutputHelper = testOutputHelper;
		}

		public async Task<string> GetData(string fileName)
		{
			_testOutputHelper.WriteLine($"Getting data for {fileName} - Thread {Thread.CurrentThread.ManagedThreadId}");

			var filePath = Path.Combine(_filesPath, fileName);
			
			if (File.Exists(filePath) == false)
			{
				return null;
			}

			//await Task.Delay(1000);
			var data = await File.ReadAllTextAsync(filePath);

			_testOutputHelper.WriteLine($"Returning data for {fileName} - Thread {Thread.CurrentThread.ManagedThreadId}");

			return data;
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part5
{
	public class AsyncDisposeTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public AsyncDisposeTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task SyncAndAsyncEnumerableTest()
		{
			var fileStorePath = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "Temp");

			await WriteToFileWithSyncDispose(Path.Combine(fileStorePath, $"{Guid.NewGuid():N}.txt"));
			await WriteToFileWithAsyncDispose(Path.Combine(fileStorePath, $"{Guid.NewGuid():N}.txt"));
		}

		private async Task WriteToFileWithSyncDispose(string filePath)
		{
			using (var fs = new FileStream(filePath, FileMode.Create))
			{
				using (var sw = new StreamWriter(fs))
				{
					await sw.WriteAsync("Welcome in async world");
				}
			}
		}

		private async Task WriteToFileWithAsyncDispose(string filePath)
		{
			await using var fs = new FileStream(filePath, FileMode.Create);
			await using var sw = new StreamWriter(fs);
			await sw.WriteAsync("Welcome in async world");
		}
	}
}

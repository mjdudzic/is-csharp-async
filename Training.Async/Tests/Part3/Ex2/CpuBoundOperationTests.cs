using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part3.Ex2
{
	public class CpuBoundOperationTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly HttpClient _httpClient = new HttpClient();

		public CpuBoundOperationTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task ThreadPoolTaskTest()
		{
			var task = Task.Run(() =>
			{
				Thread.Sleep(100);
				// Do some really time consuming operation

				return true;
			});

			var result = await task;

			result.Should().BeTrue();
		}
	}
}

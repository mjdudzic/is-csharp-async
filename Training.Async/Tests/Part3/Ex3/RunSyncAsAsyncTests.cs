using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Nito.AsyncEx;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part3.Ex3
{
	public class RunSyncAsAsyncTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly HttpClient _httpClient = new HttpClient();

		public RunSyncAsAsyncTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task AsyncOperationTest()
		{
			var service = new DataService(_testOutputHelper);

			var data = await service.GetJokeAsync();

			data.Should().NotBeNullOrWhiteSpace();
		}

		[Fact]
		public void SyncOperationTest()
		{
			AsyncContext.Run(() =>
			{
				var service = new DataService(_testOutputHelper);

				var data = service.GetJoke2();

				data.Should().NotBeNullOrWhiteSpace();
			});
		}
	}
}

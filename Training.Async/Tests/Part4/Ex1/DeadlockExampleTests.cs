using System.Net.Http;
using FluentAssertions;
using Nito.AsyncEx;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part4.Ex1
{
	public class DeadlockExampleTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly HttpClient _httpClient = new HttpClient();

		public DeadlockExampleTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public void DeadlockedOperationThatSucceedsInTest()
		{
			var service = new DataService();

			var data = service.GetJokeAsync().Result;

			data.Should().NotBeNullOrWhiteSpace();
		}

		[Fact]
		public void DeadlockedOperationThatFailsInTest()
		{
			AsyncContext.Run(() =>
			{
				var service = new DataService();

				var data = service.GetJokeAsync().Result;

				data.Should().NotBeNullOrWhiteSpace();
			});
		}
	}
}

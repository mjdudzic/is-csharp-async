using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part3.Ex5
{
	public class EapToTapTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly HttpClient _httpClient = new HttpClient();

		public EapToTapTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public void EapOperationTest()
		{
			var data = string.Empty;

			var webClient = new WebClient();
			webClient.DownloadStringCompleted += (s, e) =>
			{
				data = e.Result;
			};
			webClient.DownloadStringAsync(new Uri("https://api.chucknorris.io/jokes/random"));

			while (webClient.IsBusy)
			{
				Thread.Sleep(100);
			}

			data.Should().NotBeNullOrWhiteSpace();
		}

		[Fact]
		public async Task EapOperationRunByUsingTapTest()
		{
			var dataService = new EapDataService();

			var data = await dataService.GetDataAsync(new Uri("https://api.chucknorris.io/jokes/random"));

			data.Should().NotBeNullOrWhiteSpace();
		}
	}
}

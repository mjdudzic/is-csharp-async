using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part3.Ex2
{
	public class IoBoundOperationTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly HttpClient _httpClient = new HttpClient();

		public IoBoundOperationTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task ApiRequestTest()
		{
			var response = await _httpClient
				.GetAsync("https://api.chucknorris.io/jokes/random");

			response.IsSuccessStatusCode.Should().BeTrue();
		}

		[Fact]
		public async Task FileReadTest()
		{
			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "test1.txt");

			var data = await File.ReadAllTextAsync(filePath);

			data.Should().NotBeNullOrWhiteSpace();
		}
	}
}

using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part3.Ex5
{
	public class ApmToTapTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly HttpClient _httpClient = new HttpClient();

		public ApmToTapTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public void ApmOperationTest()
		{
			var dataService = new ApmDataService(Path.Combine(Directory.GetCurrentDirectory(), "TestData"));

			var asyncResult = dataService.BeginDataRead("test1.txt", null, null);

			var data = dataService.EndDataRead(asyncResult);

			_testOutputHelper.WriteLine($"File contains text with length: {data.Length}");

			data.Should().NotBeNullOrWhiteSpace();
		}

		[Fact]
		public async Task ApmOperationRunByUsingTapTest()
		{
			var dataService = new ApmDataService(Path.Combine(Directory.GetCurrentDirectory(), "TestData"));

			var data = await dataService.GetDataAsync("test1.txt");

			_testOutputHelper.WriteLine($"File contains text with length: {data.Length}");

			data.Should().NotBeNullOrWhiteSpace();
		}
	}
}

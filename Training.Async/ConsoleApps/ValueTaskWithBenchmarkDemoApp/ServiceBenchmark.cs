using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace ValueTaskWithBenchmarkDemoApp
{
	[MemoryDiagnoser]
	public class ServiceBenchmark
	{
		private static readonly DataService DataService = new DataService();

		[Benchmark]
		public async Task<string> GetDataWithTask()
		{
			return await DataService.GetJokeWithTask();
		}

		[Benchmark]
		public async Task<string> GetDataWithValueTask()
		{
			return await DataService.GetJokeWithValueTask();
		}
	}
}
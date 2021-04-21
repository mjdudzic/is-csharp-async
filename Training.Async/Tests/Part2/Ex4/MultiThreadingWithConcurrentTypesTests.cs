using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part2.Ex4
{
	public class MultiThreadingWithConcurrentTypesTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly Fixture _fixture = new Fixture();

		public MultiThreadingWithConcurrentTypesTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public void SaveDataItemsIntoTheadSafeCollectionTest()
		{
			var dataItemsCountToObtain = 4;

			var dataCollection = new List<string>();

			//var dataCollection = new ConcurrentBag<string>();

			var httpClient = new HttpClient();

			var result = Parallel.For(0, dataItemsCountToObtain, async (index, state) =>
			{
				_testOutputHelper.WriteLine(
					$"Sending request {index} - Thread {Thread.CurrentThread.ManagedThreadId}");

				var data = GetData(httpClient).Result;

				dataCollection.Add(data);
			});

			//Thread.Sleep(1000);
			dataCollection.Count.Should().Be(dataItemsCountToObtain);
		}

		[Fact]
		public async Task ManageProcessingQueueBetweenThreadsTest()
		{
			var cts = new CancellationTokenSource();
			var token = cts.Token;
			
			var queue = new ConcurrentQueue<Guid>();

			var orderIdsToProcess = _fixture.CreateMany<Guid>(5);

			var consumerTask = Task.Run(() =>
			{
				while (token.IsCancellationRequested == false)
				{
					if (queue.TryDequeue(out var orderToProcessId))
					{
						_testOutputHelper.WriteLine($"Dequeue order {orderToProcessId}");

						continue;
					}

					_testOutputHelper.WriteLine("No order to dequeue");

					Thread.Sleep(500);
				}

				_testOutputHelper.WriteLine("Order consumer cancelled");
			});

			Parallel.ForEach(orderIdsToProcess, orderId =>
			{
				Thread.Sleep(new Random().Next(1000, 2000));

				_testOutputHelper.WriteLine($"Enqueue order {orderId}");
				queue.Enqueue(orderId);
			});

			cts.CancelAfter(1000);

			await consumerTask;
		}

		private async Task<string> GetData(HttpClient httpClient)
		{
			await Task.Delay(100);

			return (await httpClient
				.GetFromJsonAsync<ApiResponse>(
					new Uri("https://postman-echo.com/delay/1")))
				?.Data;
		}

		private class ApiResponse
		{
			[JsonPropertyName("delay")]
			public string Data { get; set; }
		}
	}
}

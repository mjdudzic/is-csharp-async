using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nito.AsyncEx;
using Tests.Part4.Ex1;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part4.Ex2
{
	public class EntityFrameworkAsyncTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly Fixture _fixture;
		private readonly WeatherForecastDbContext _context;

		public EntityFrameworkAsyncTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
			_fixture = new Fixture();

			_context = new WeatherForecastDbContext(
				new DbContextOptionsBuilder<WeatherForecastDbContext>()
					.UseSqlServer("Server=.;Database=TrainingAsyncDb;User=TrainingAsyncDbUser;Password=lock@123;")
					.Options);
		}

		[Fact]
		public async Task GetCollectionAsync()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var data = await _context
				.WeatherForecasts
				.OrderBy(i => i.Date)
				.ToListAsync();

			_testOutputHelper.WriteLine($"Obtained {data.Count} records");

			data.Count.Should().BeGreaterOrEqualTo(0);

			_testOutputHelper.WriteLine($"Ending test - Thread {Thread.CurrentThread.ManagedThreadId}");
		}

		[Fact]
		public async Task GetItemAsync()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var item = await _context
				.WeatherForecasts
				.OrderBy(i => i.Date)
				.FirstOrDefaultAsync(i => i.Date > DateTime.Now.AddDays(-1));

			_testOutputHelper.WriteLine($"Obtained item id is {item?.Id}");

			_testOutputHelper.WriteLine($"Ending test - Thread {Thread.CurrentThread.ManagedThreadId}");
		}

		[Fact]
		public async Task AddItemAsync()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var newEntity = _fixture.Create<WeatherForecast>();
			newEntity.Id = default;

			await _context.AddAsync(newEntity);
			//_context.Add(newEntity);
			await _context.SaveChangesAsync();

			var item = await _context
				.WeatherForecasts
				.FirstOrDefaultAsync(i => i.Id == newEntity.Id);

			item.Should().NotBeNull();

			_testOutputHelper.WriteLine($"Ending test - Thread {Thread.CurrentThread.ManagedThreadId}");
		}

		[Fact]
		public async Task RunOperationsInParallel()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var task1 = _context
				.WeatherForecasts
				.OrderBy(i => i.Date)
				.ToListAsync();

			var task2 =  _context
				.WeatherForecasts
				.OrderBy(i => i.Date)
				.FirstOrDefaultAsync(i => i.Date > DateTime.Now.AddDays(-1));

			Func<Task> act = async () =>
			{
				await Task.WhenAll(task1, task2);
			};

			await act.Should().ThrowAsync<InvalidOperationException>();

			_testOutputHelper.WriteLine($"Ending test - Thread {Thread.CurrentThread.ManagedThreadId}");
		}
	}
}

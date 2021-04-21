using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part2.Ex4
{
	public class MultiThreadingWithLockingTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		private readonly object _lockObject = new object();

		private double _todayTotalPrice;
		private int _todayTotalPriceInt;

		public MultiThreadingWithLockingTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task MultiThreadingWithoutLockExampleTest()
		{
			await RunMultiThreadOperationsWithoutLock();

			_todayTotalPrice.Should().BeGreaterThan(0);
		}

		[Fact]
		public async Task MultiThreadingWithLockExampleTest()
		{
			await RunMultiThreadOperationsWithLock();

			_todayTotalPrice.Should().BeGreaterThan(0);
		}

		[Fact]
		public async Task MultiThreadingWithMonitorExampleTest()
		{
			await RunMultiThreadOperationsWithMonitor();

			_todayTotalPrice.Should().BeGreaterThan(0);
		}

		[Fact]
		public async Task MultiThreadingWithInterlockExampleTest()
		{
			await RunMultiThreadOperationsWithInterlock();

			_testOutputHelper.WriteLine($"Total price is {_todayTotalPriceInt}");
			_todayTotalPriceInt.Should().BeGreaterThan(0);
		}

		private async Task RunMultiThreadOperationsWithoutLock()
		{
			var task1 = Task.Factory.StartNew(() =>
			{
				var currentTotalPrice = _todayTotalPrice;
				Thread.Sleep(100);
				
				_todayTotalPrice += new Random().Next(100, 10000);

				_testOutputHelper.WriteLine(
					$"Thread {Thread.CurrentThread.ManagedThreadId} changed total price from {currentTotalPrice} to {_todayTotalPrice}");

			}, TaskCreationOptions.LongRunning);

			var task2 = Task.Factory.StartNew(() =>
			{
				var currentTotalPrice = _todayTotalPrice;
				Thread.Sleep(100);
				
				_todayTotalPrice += new Random().Next(100, 10000);

				_testOutputHelper.WriteLine(
					$"Thread {Thread.CurrentThread.ManagedThreadId} changed total price from {currentTotalPrice} to {_todayTotalPrice}");

			}, TaskCreationOptions.LongRunning);

			await Task.WhenAll(task1, task2);
		}

		private async Task RunMultiThreadOperationsWithLock()
		{
			var task1 = Task.Factory.StartNew(() =>
			{
				lock (_lockObject)
				{
					var currentTotalPrice = _todayTotalPrice;
					Thread.Sleep(100);
					
					_todayTotalPrice += new Random().Next(100, 10000);

					_testOutputHelper.WriteLine(
						$"Thread {Thread.CurrentThread.ManagedThreadId} changed total price from {currentTotalPrice} to {_todayTotalPrice}");
				}

			}, TaskCreationOptions.LongRunning);

			var task2 = Task.Factory.StartNew(() =>
			{
				lock (_lockObject)
				{
					var currentTotalPrice = _todayTotalPrice;
					Thread.Sleep(100);
					
					_todayTotalPrice += new Random().Next(100, 10000);

					_testOutputHelper.WriteLine(
						$"Thread {Thread.CurrentThread.ManagedThreadId} changed total price from {currentTotalPrice} to {_todayTotalPrice}");
				}

			}, TaskCreationOptions.LongRunning);

			await Task.WhenAll(task1, task2);
		}

		private async Task RunMultiThreadOperationsWithMonitor()
		{
			var task1 = Task.Factory.StartNew(() =>
			{
				Monitor.Enter(_lockObject);
				try
				{
					var currentTotalPrice = _todayTotalPrice;
					Thread.Sleep(100);
					
					_todayTotalPrice += new Random().Next(100, 10000);

					_testOutputHelper.WriteLine(
						$"Thread {Thread.CurrentThread.ManagedThreadId} changed total price from {currentTotalPrice} to {_todayTotalPrice}");
				}
				finally
				{
					Monitor.Exit(_lockObject);
				}

			}, TaskCreationOptions.LongRunning);

			var task2 = Task.Factory.StartNew(() =>
			{
				Monitor.Enter(_lockObject);
				try
				{
					var currentTotalPrice = _todayTotalPrice;
					Thread.Sleep(100);
					
					_todayTotalPrice += new Random().Next(100, 10000);

					_testOutputHelper.WriteLine(
						$"Thread {Thread.CurrentThread.ManagedThreadId} changed total price from {currentTotalPrice} to {_todayTotalPrice}");
				}
				finally
				{
					Monitor.Exit(_lockObject);
				}

			}, TaskCreationOptions.LongRunning);

			await Task.WhenAll(task1, task2);
		}

		private async Task RunMultiThreadOperationsWithInterlock()
		{
			var task1 = Task.Factory.StartNew(() =>
			{
				Thread.Sleep(100);

				var newValue = Interlocked.Add(ref _todayTotalPriceInt, new Random().Next(100, 10000));

				_testOutputHelper.WriteLine(
					$"Thread {Thread.CurrentThread.ManagedThreadId} changed total price to {newValue}");

			}, TaskCreationOptions.LongRunning);

			var task2 = Task.Factory.StartNew(() =>
			{
				Thread.Sleep(100);

				var newValue = Interlocked.Add(ref _todayTotalPriceInt, new Random().Next(100, 10000));

				_testOutputHelper.WriteLine(
					$"Thread {Thread.CurrentThread.ManagedThreadId} changed total price to {newValue}");

			}, TaskCreationOptions.LongRunning);

			await Task.WhenAll(task1, task2);
		}

	}
}

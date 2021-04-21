using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDebuggingDemoApp
{
	class Program
	{
		private static readonly object LockObject = new object();

		private static readonly object LockObjectA = new object();
		private static readonly object LockObjectB = new object();

		private static double _todayTotalPrice;

		static async Task Main(string[] args)
		{
			var task1 = Task.Factory.StartNew(() =>
			{
				lock (LockObject)
				{
					var currentTotalPrice = _todayTotalPrice;
					Thread.Sleep(100);

					_todayTotalPrice += new Random().Next(100, 10000);

					Console.WriteLine(
						$"Thread {Thread.CurrentThread.ManagedThreadId} changed total price from {currentTotalPrice} to {_todayTotalPrice}");
				}

			}, TaskCreationOptions.LongRunning);

			var task2 = Task.Factory.StartNew(() =>
			{
				lock (LockObject)
				{
					var currentTotalPrice = _todayTotalPrice;
					Thread.Sleep(100);

					_todayTotalPrice += new Random().Next(100, 10000);

					Console.WriteLine(
						$"Thread {Thread.CurrentThread.ManagedThreadId} changed total price from {currentTotalPrice} to {_todayTotalPrice}");
				}

			}, TaskCreationOptions.LongRunning);

			//var task3 = Task.Run(() =>
			//{
			//	lock (LockObjectA)
			//	{
			//		Thread.Sleep(1000);
			//		lock (LockObjectB)
			//		{
			//			Thread.Sleep(1000);
			//			Console.WriteLine(
			//				$"Thread {Thread.CurrentThread.ManagedThreadId} is doing something");
			//		}
			//	}
			//});

			//var task4 = Task.Run(() =>
			//{
			//	lock (LockObjectB)
			//	{
			//		Thread.Sleep(1000);
			//		lock (LockObjectA)
			//		{
			//			Thread.Sleep(1000);
			//			Console.WriteLine(
			//				$"Thread {Thread.CurrentThread.ManagedThreadId} is doing something");
			//		}
			//	}
			//});

			await StartJob();

			await Task.WhenAll(task1, task2);
		}

		private static async Task<string> StartJob()
		{
			return await CallService();
		}

		private static async Task<string> CallService()
		{
			return await GetData();
		}

		private static async Task<string> GetData()
		{
			string data = null;
			return await Task.FromResult(data.ToLower());
		}
	}
}

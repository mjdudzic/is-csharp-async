using System;
using System.Threading;

namespace ThreadsDemoApp
{
	class Program
	{
		static void Main(string[] args)
		{
			var thread1 = new Thread(RunSuccessOperation);
			var thread2= new Thread(RunFailedOperation);

			//thread1.IsBackground = true;
			//thread2.IsBackground = true;

			thread1.Start();
			thread2.Start();

			//thread1.Join();
			//thread2.Join();
		}

		private static void RunSuccessOperation()
		{
			Console.WriteLine($"Running operation {nameof(RunSuccessOperation)}");
		}

		private static void RunFailedOperation()
		{
			Console.WriteLine($"Running operation {nameof(RunFailedOperation)}");
			throw new Exception("something went wrong!");
		}
	}
}

using System;
using System.Threading;

namespace ThreadPoolDemoApp
{
	class Program
	{
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException
				+= OnUnhandleException;

			ThreadPool.QueueUserWorkItem(RunSuccessOperation);
			ThreadPool.QueueUserWorkItem(RunFailedOperation);

			Console.WriteLine("Press Enter to terminate!");
			Console.ReadLine();
		}

		private static void RunSuccessOperation(object state)
		{
			Console.WriteLine($"Running operation {nameof(RunSuccessOperation)}");
		}

		private static void RunFailedOperation(object state)
		{
			Console.WriteLine($"Running operation {nameof(RunFailedOperation)}");
			throw new Exception("something went wrong!");
		}

		private static void OnUnhandleException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine($"Is terminated? : {e.IsTerminating}");
			Console.WriteLine($"Reason is : {e.ExceptionObject}");
		}
	}
}

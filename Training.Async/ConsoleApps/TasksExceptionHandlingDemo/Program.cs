using System;
using System.Threading.Tasks;

namespace TasksExceptionHandlingDemoApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Do();
			Console.ReadLine();
		}

		private static void Do()
		{
			// Handler for UnobservedTaskException event; it does not ensure the UnobservedTaskException is now "observed"
			// to observe the task exception, we have to do e.SetObserved() in the below handler code
			TaskScheduler.UnobservedTaskException += (s, e) =>
			{
				Console.WriteLine($"Unobserved Task Exception : {e.Exception.Message}");
				//to actually observe the task, uncomment the below line of code
				//e.SetObserved();
			};
			try
			{
				ThrowsAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Caught in try/catch  : {ex.Message}");
			}
			// invoking garbage collector explicitly
			Console.WriteLine("GC..");
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

        private static async Task ThrowsAsync()
		{
			Console.WriteLine("Throwing...");
			throw new Exception("Boom!");
		}
    }
}

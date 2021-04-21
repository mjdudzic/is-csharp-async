using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AwaiterDemoAppDotNet452
{
	public class CultureAwaiter : INotifyCompletion
	{
		private readonly TaskAwaiter _sourceTaskAwaiter;
		private CultureInfo _sourceTaskCulture;

		public CultureAwaiter(Task task)
		{
			if (task == null)
			{
				throw new ArgumentNullException(nameof(task));
			}

			_sourceTaskAwaiter = task.GetAwaiter();
		}

		public CultureAwaiter GetAwaiter() { return this; }

		public bool IsCompleted => _sourceTaskAwaiter.IsCompleted;

		public void OnCompleted(Action continuation)
		{
			_sourceTaskCulture = Thread.CurrentThread.CurrentCulture;
			_sourceTaskAwaiter.OnCompleted(continuation);
		}

		public void GetResult()
		{
			if (_sourceTaskCulture != null)
			{
				Thread.CurrentThread.CurrentCulture = _sourceTaskCulture;
			}

			_sourceTaskAwaiter.GetResult();
		}
    }
}
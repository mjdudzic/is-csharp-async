using System.Threading.Tasks;

namespace AwaiterDemoAppDotNet452
{
	public static class TaskExtensions
	{
		public static CultureAwaiter WithCurrentCulture(this Task task)
		{
			return new CultureAwaiter(task);
		}
	}
}
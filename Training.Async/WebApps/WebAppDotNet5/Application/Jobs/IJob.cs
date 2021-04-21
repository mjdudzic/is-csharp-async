using System.Threading.Tasks;
using Hangfire.Server;

namespace WebAppDotNet5.Application.Jobs
{
	public interface IJob<in T>
	{
		Task Execute(T jobData, PerformContext context);
	}
}
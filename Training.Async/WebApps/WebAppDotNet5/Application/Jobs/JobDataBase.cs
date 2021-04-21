using System.Collections.Generic;

namespace WebAppDotNet5.Application.Jobs
{
	public abstract class JobDataBase
	{
		public IDictionary<string, string> TracingKeys { get; set; }
	}
}
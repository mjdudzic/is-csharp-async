using System;
using System.Net;
using System.Threading.Tasks;

namespace Tests.Part3.Ex5
{
	public class EapDataService
	{
		public Task<string> GetDataAsync(Uri url)
		{
			var tcs = new TaskCompletionSource<string>();
			var webClient = new WebClient();
			
			webClient.DownloadStringCompleted += (s, e) =>
			{
				if (e.Error != null)
				{
					tcs.TrySetException(e.Error);
				}
				else if (e.Cancelled)
				{
					tcs.TrySetCanceled();
				}
				else
				{
					tcs.TrySetResult(e.Result);
				}
			};
			
			webClient.DownloadStringAsync(url);
			
			return tcs.Task;
		}
	}
}
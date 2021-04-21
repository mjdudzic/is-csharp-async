using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Library
{
	public class ResourceHelper
	{
		private readonly HttpClient _httpClient;

		public ResourceHelper(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task SaveResource(Uri uri, string filePath)
		{
			var data = await _httpClient.GetStringAsync(uri);

			//throw new Exception("test");

			await File.WriteAllTextAsync(filePath, data);
		}
	}
}

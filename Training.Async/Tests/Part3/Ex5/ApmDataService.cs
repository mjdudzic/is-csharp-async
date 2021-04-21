using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Part3.Ex5
{
	public class ApmDataService : IDisposable
	{
		private readonly string _filesPath;
		private FileStream _fileStream;
		private byte[] _buffer;

		public ApmDataService(string filesPath)
		{
			_filesPath = filesPath;
		}

		public IAsyncResult BeginDataRead(string fileName, AsyncCallback callback, object state)
		{
			Console.WriteLine($"Getting data for {fileName} - Thread {Thread.CurrentThread.ManagedThreadId}");

			var filePath = Path.Combine(_filesPath, fileName);

			var fileInfo = new FileInfo(filePath);
			if (fileInfo.Exists == false)
			{
				return null;
			}

			_buffer = new byte[(int)fileInfo.Length];
			_fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, FileOptions.Asynchronous);
			
			var result = _fileStream.BeginRead(_buffer, 0, (int)fileInfo.Length, callback, state);

			return result;
		}

		public string EndDataRead(IAsyncResult asyncResult)
		{
			_fileStream.EndRead(asyncResult);

			Console.WriteLine($"Returning data for {_fileStream.Name} - Thread {Thread.CurrentThread.ManagedThreadId}");

			return Encoding.UTF8.GetString(_buffer);
		}

		public void Dispose()
		{
			_fileStream.Dispose();
		}
	}

	public static class DataServiceExt
	{
		public static Task<string> GetDataAsync(this ApmDataService apmDataService, string fileName)
		{
			return Task<string>.Factory.FromAsync(apmDataService.BeginDataRead, apmDataService.EndDataRead, fileName, null);
		}
	}
}
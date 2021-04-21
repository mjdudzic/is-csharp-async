using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading;
using Xunit.Abstractions;

namespace Tests.Part1.Ex3
{
	public class JokeAwaitable : IAwaitable
	{
		private volatile bool _dataRetrieved;
		private readonly HttpClient _httpClient;
		private readonly ITestOutputHelper _testOutputHelper;
		private TaskAwaiter<ApiResponse>? _apiCallAwaiter;

		public bool IsDataRetrieved => _dataRetrieved;
		public event Action DataRetrieved;
		public string RandomJoke { get; private set; }

		public JokeAwaitable(
			HttpClient httpClient,
			ITestOutputHelper testOutputHelper)
		{
			_httpClient = httpClient;
			_testOutputHelper = testOutputHelper;
		}

		public IAwaiter GetAwaiter() => new JokeAwaiter(this, _testOutputHelper);

		public void TryComplete()
		{
			if (_dataRetrieved)
			{
				return;
			}

			if (_apiCallAwaiter != null && _apiCallAwaiter.Value.IsCompleted)
			{
				_testOutputHelper.WriteLine($"Running {nameof(JokeAwaitable)}-> Sending event data is ready");
				
				_dataRetrieved = true;
				RandomJoke = _apiCallAwaiter.Value.GetResult().JokeText;
				DataRetrieved?.Invoke();
				return;
			}

			_testOutputHelper.WriteLine($"Running {nameof(JokeAwaitable)}-> Getting data from API");
			
			_apiCallAwaiter = _httpClient
				.GetFromJsonAsync<ApiResponse>(new Uri("https://api.chucknorris.io/jokes/random"))
				.GetAwaiter();
		}

		private class ApiResponse
		{
			[JsonPropertyName("value")]
			public string JokeText { get; set; }
		}
	}

	public class JokeAwaiter : IAwaiter
	{
		private readonly JokeAwaitable _awaitable;
		private readonly ITestOutputHelper _testOutputHelper;
		private string _randomJoke;

		public JokeAwaiter(
			JokeAwaitable awaitable,
			ITestOutputHelper testOutputHelper)
		{
			_awaitable = awaitable;
			_testOutputHelper = testOutputHelper;
		}

		public void OnCompleted(Action continuation)
		{
			_testOutputHelper.WriteLine($"Running {nameof(JokeAwaiter)}-> {nameof(OnCompleted)}");

			if (IsCompleted)
			{
				continuation();
				return;
			}

			var capturedContext = SynchronizationContext.Current;
			_awaitable.DataRetrieved += () =>
			{
				SetResult();

				if (capturedContext != null)
				{
					_testOutputHelper.WriteLine($"Running {nameof(JokeAwaiter)}-> {nameof(OnCompleted)} continuation on captured context");
					capturedContext.Post(_ => continuation(), null);
				}
				else
				{
					_testOutputHelper.WriteLine($"Running {nameof(JokeAwaiter)}-> {nameof(OnCompleted)} continuation without context");
					continuation();
				}
			};

			GetResult();
		}

		private void SetResult()
		{
			_randomJoke = _awaitable.RandomJoke;
		}

		public bool IsCompleted => _awaitable.IsDataRetrieved;

		public string GetResult()
		{
			_testOutputHelper.WriteLine($"Running {nameof(JokeAwaiter)}-> {nameof(GetResult)} at thread {Thread.CurrentThread.ManagedThreadId}");
			
			if (IsCompleted)
			{
				return _randomJoke;
			}

			while (!IsCompleted)
			{
				Thread.Sleep(100);
				_awaitable.TryComplete();
			}
			return _randomJoke;
		}
	}
}
using System;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace Tests.Part1.Ex3
{
	public class TestAwaitable
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public TestAwaitable(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		public TestAwaiter GetAwaiter() => new TestAwaiter(_testOutputHelper);
	}

	public class TestAwaiter : INotifyCompletion
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public TestAwaiter(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		public void OnCompleted(Action continuation)
		{
			_testOutputHelper.WriteLine("Running TestAwaiter->OnCompleted");
			continuation();
		}

		public bool IsCompleted => true;

		public void GetResult()
		{
			_testOutputHelper.WriteLine("Running TestAwaiter->GetResult");
		}
	}
}
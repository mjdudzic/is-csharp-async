using System;
using System.Runtime.CompilerServices;

namespace Tests.Part1.Ex3
{
	public interface IAwaiter : INotifyCompletion
	{
		bool IsCompleted { get; }
		string GetResult();
	}
}
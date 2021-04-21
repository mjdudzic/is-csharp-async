using System;
using System.Collections;
using System.Collections.Generic;

namespace Library
{
	public class AppResourceCollection : IEnumerable<string>
	{
		public IEnumerator<string> GetEnumerator()
		{
			yield return Guid.NewGuid().ToString("N");
			yield return Guid.NewGuid().ToString("N");
			yield return Guid.NewGuid().ToString("N");
			//yield return GetException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private string GetException()
		{
			throw new Exception("test");
		}
	}
}
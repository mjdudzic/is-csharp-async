namespace Tests.Part1.Ex3
{
	public interface IAwaitable
	{
		IAwaiter GetAwaiter();
	}
}
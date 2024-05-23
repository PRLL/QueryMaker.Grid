namespace QueryMakerLibrary.Grid.Common;

public static class Callbacks
{
	public readonly struct FilterOptions(string SearchTerm, bool search)
	{
		internal readonly string SearchTerm { get; init; } = SearchTerm;
		internal readonly bool Search { get; init; } = search;
	}
}
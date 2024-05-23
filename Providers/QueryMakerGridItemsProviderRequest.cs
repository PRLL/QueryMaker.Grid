namespace QueryMakerLibrary.Grid.Providers;

public readonly struct QueryMakerGridItemsProviderRequest(QueryMaker query)
{
	public readonly QueryMaker Query { get; private init; } = query;
}

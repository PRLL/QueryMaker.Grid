using System.Threading.Tasks;

namespace QueryMakerLibrary.Grid.Providers;

public delegate ValueTask<QueryMakerGridItemsProviderResult<TGridItem>> QueryMakerGridDataProvider<TGridItem>(
	QueryMakerGridItemsProviderRequest query);
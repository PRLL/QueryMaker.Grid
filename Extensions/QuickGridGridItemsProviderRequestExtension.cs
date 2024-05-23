using Microsoft.AspNetCore.Components.QuickGrid;
using static QueryMakerLibrary.Components.Sort;

namespace QueryMakerLibrary.Grid.Extensions;

internal static class QuickGridGridItemsProviderRequestExtension
{
	internal static QueryMaker ToQueryMaker<T>(
		this GridItemsProviderRequest<T> quickGridRequest,
		string index = "")
	{
		QueryMaker query = new();

		// add sorting
		foreach (SortedProperty item
			in quickGridRequest.GetSortByProperties())
		{
			query.AndThenSortBy(new (item.PropertyName,
				item.Direction == SortDirection.Ascending
					? SortDirections.Ascending
					: SortDirections.Descending));
		}

		// add paging
		return query.WithPage(new(quickGridRequest.StartIndex,
			quickGridRequest.Count ?? 2, index));
	}
}
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components;
using QueryMakerLibrary.Grid.Common;
using QueryMakerLibrary.Grid.Extensions;
using System.Threading.Tasks;
using QueryMakerLibrary.Grid.Providers;
using QueryMakerLibrary.Grid.Contexts;

namespace QueryMakerLibrary.Grid.Components;


[CascadingTypeParameter("TGridItem")]
public partial class QueryMakerGrid<TGridItem> : QuickGrid<TGridItem>
{
	[Parameter, EditorRequired]
	public QueryMakerGridDataProvider<TGridItem> DataProvider { get; set; } = default!;

	[Parameter]
	public QueryMakerGridPaginationState? PaginationState { get; set; } = null;

	private readonly QueryMakerGridContext<TGridItem> _queryMakerGridContext;

	private string PaginationIndex { get; set; } = string.Empty;

	public QueryMakerGrid() : base()
	{
		_queryMakerGridContext = new(
			async () => await RefreshDataAsync(),
			async (request) => await DataProvider(request),
			async () => await ShowColumnOptionsAsync(null!));
	}

	protected override async Task OnParametersSetAsync()
	{
		if (PaginationState is not null)
		{
			PaginationIndex = PaginationState.Index;
			Pagination = PaginationState;
		}

		await base.OnParametersSetAsync();

		ItemsProvider =
			async request => await GetResult(request);
	}

	private async Task<GridItemsProviderResult<TGridItem>> GetResult(GridItemsProviderRequest<TGridItem> request)
	{
		QueryMakerGridItemsProviderResult<TGridItem> result =
			await _queryMakerGridContext.GetItemsAsync(
				request.ToQueryMaker(PaginationIndex));

		return GridItemsProviderResult.From(result.Items, result.TotalAmmount);
	}

	private RenderFragment RenderBase() => base.BuildRenderTree;
}
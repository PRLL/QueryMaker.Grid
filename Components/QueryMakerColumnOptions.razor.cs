
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using QueryMakerLibrary.Grid.Common;
using QueryMakerLibrary.Grid.Contexts;
using QueryMakerLibrary.Grid.Providers;
using static QueryMakerLibrary.Components.Filter;

namespace QueryMakerLibrary.Grid.Components;

public partial class QueryMakerColumnOptions<TGridItem, TProp> : ComponentBase
{
	[CascadingParameter]
	internal QueryMakerGridContext<TGridItem> QueryMakerGridContext
	{
		get => _queryMakerGridContext
			?? throw new NullReferenceException($"{nameof(QueryMakerGridContext)} cannot be null");
		set => _queryMakerGridContext = value;
	}

	[Parameter, EditorRequired]
	public required string PropertyName { get; set; }

	[Parameter, EditorRequired]
	public required string Title { get; set; }

	[Parameter]
	public bool Virtualize { get; set; } = false;

	[Parameter]
	public int VirtualizeLoadDelay { get; set; } = 250;

	[Parameter]
	public int VirtualizeItemsPerPage { get; set; } = 200;

	[Parameter]
	public bool SearchWhileTyping { get; set; } = true;

	private QueryMakerGridContext<TGridItem>? _queryMakerGridContext = null;

	public QueryMakerColumnOptions()
	{
		QueryMakerColumnOptionsContext = new(
			((item) => IsChecked(item)),
			((item) => SetSelectedValue(item)));
	}

	protected override async Task OnParametersSetAsync()
	{
		QueryMakerGridContext.SetProperty<TProp>(PropertyName);

		SelectedValues = GetSelectedOptions();
		ManualCustomFilters = GetManualCustomFilters();

		if (!Virtualize)
		{
			await LoadOptions();
		}
	}

	private List<ManualCustomFilter> ManualCustomFilters { get; set; } = new();

	private ManualCustomFilter NewManualCustomFilter { get; set; } = new(FilterActions.Contains);

	private QueryMakerColumnOptionsContext<TProp> QueryMakerColumnOptionsContext { get; init; }

	private bool IsLoading { get; set; } = true;

	private bool AddFilterManually { get; set; } = false;

	private PropertyFilter<TProp> PropertyFilter
	{
		get => (PropertyFilter<TProp>)QueryMakerGridContext.GetFilterHandler<TProp>(PropertyName);
	}

	private string PlaceHolder { get => "Search " + Title; }

	private string? SearchTerm { get; set; } = null;

	private ItemsProviderResult<TProp> VirtualizeState = new();

	private Virtualize<TProp>? VirtualizeInstance = null;

	private List<TProp> SelectedValues = new();

	private Guid CurrentLoadVirtualizeOptionsOperationId = Guid.Empty;

	private async ValueTask<ItemsProviderResult<TProp>> LoadVirtualizedOptions(
		ItemsProviderRequest request)
	{
		Guid loadVirtualizeOptionsOperationId = Guid.NewGuid();
		CurrentLoadVirtualizeOptionsOperationId = loadVirtualizeOptionsOperationId;

		await Task.Delay(VirtualizeLoadDelay);

		if (CurrentLoadVirtualizeOptionsOperationId == loadVirtualizeOptionsOperationId)
		{
			await LoadOptions(request.StartIndex, request.Count);
		}

		return VirtualizeState;
	}

	private async Task LoadOptions(int skip = 0, int take = 0)
	{
		IsLoading = true;

		bool addSearchTerm = !string.IsNullOrEmpty(SearchTerm);

		QueryMaker query = new(
			filter: addSearchTerm
				? new(PropertyName, FilterActions.Contains, SearchTerm)
				: null,
			sort: new(PropertyName),
			page: new(skip, take),
			select: new(new string[1]{ PropertyName }, true));

		QueryMakerGridItemsProviderResult<TProp> result =
			await QueryMakerGridContext.GetColumnOptionsAsync<TProp>(query, PropertyName);

		List<TProp> availableOptions = result.Items.ToList();
		VirtualizeState = new ItemsProviderResult<TProp>(
			availableOptions, result.TotalAmmount);

		IsLoading = false;
	}

	private async Task FilterOptions(string searchTerm, bool search)
	{
		SearchTerm = searchTerm;

		if (search)
		{
			if (Virtualize)
			{
				await (VirtualizeInstance
					?? throw new NullReferenceException(nameof(VirtualizeInstance) + " was null"))
					.RefreshDataAsync();
			}
			else
			{
				await LoadOptions();
			}
		}
	}

	private void SetSelectedValue(TProp value)
	{
		if (SelectedValues.Contains(value))
		{
			SelectedValues.Remove(value);
		}
		else
		{
			SelectedValues.Add(value);
		}
	}

	private async Task SetSelectedValuesAndReloadGrid()
	{
		SetOptions(SelectedValues);

		await QueryMakerGridContext.HideColumnOptionsTask();
		await QueryMakerGridContext.RefreshDataAsync();
	}

	private bool IsChecked(TProp value)
	{
		return SelectedValues.Contains(value);
	}

	private void SwitchFiltersType()
	{
		AddFilterManually = !AddFilterManually;
	}

	private async Task ClearFilters()
	{
		if (AddFilterManually)
		{
			ManualCustomFilters = new();
		}
		else
		{
			SelectedValues = new();
		}

		await SetSelectedValuesAndReloadGrid();
	}

	private void DeleteManualCustomFilter(Guid id)
	{
		ManualCustomFilters.RemoveAll(x => x.Id == id);
	}

	private void AddNewManualCustomFilter()
	{
		ManualCustomFilters.Add(NewManualCustomFilter);

		NewManualCustomFilter = new(FilterActions.Contains);
	}

	private readonly Type UnderlyingType = Nullable.GetUnderlyingType(typeof(TProp)) ?? typeof(TProp);

	#region Property Filter Methods

	internal List<ManualCustomFilter> GetManualCustomFilters()
		=> PropertyFilter.GetManualCustomFilters().ToList();

	internal List<TProp> GetSelectedOptions()
		=> PropertyFilter.GetCustomFilterValues(FilterActions.Equal).ToList();

	internal void SetOptions(List<TProp> values)
	{
		PropertyFilter.SetMultipleCustomFilterValues(FilterActions.Equal, values.ToHashSet());
		PropertyFilter.SetManualCustomFilters(ManualCustomFilters);
	}

	#endregion Property Filter Methods
}
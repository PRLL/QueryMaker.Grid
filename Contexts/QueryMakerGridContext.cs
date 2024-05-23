using static QueryMakerLibrary.Components.Filter;
using QueryMakerLibrary.Components;
using QueryMakerLibrary.Grid.Interfaces;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using QueryMakerLibrary.Grid.Providers;
using QueryMakerLibrary.Grid.Common;

namespace QueryMakerLibrary.Grid.Contexts;

internal readonly struct QueryMakerGridContext<TGridItem>
{
	internal delegate ValueTask RefreshDataAsyncTask();
	internal delegate ValueTask HideColumnOptionsAsyncTask();

	private readonly RefreshDataAsyncTask _refreshDataAsyncTask;
	private readonly QueryMakerGridDataProvider<TGridItem> _dataProvider;
	private readonly HideColumnOptionsAsyncTask _hideColumnOptionsAsyncTask;
	private readonly List<IPropertyFilter> _propertyFilters;

	internal QueryMakerGridContext(
		RefreshDataAsyncTask refreshDataAsyncTask,
		QueryMakerGridDataProvider<TGridItem> dataProvider,
		HideColumnOptionsAsyncTask hideColumnOptionsAsyncTask)
	{
		_refreshDataAsyncTask = refreshDataAsyncTask;
		_dataProvider = dataProvider;
		_hideColumnOptionsAsyncTask = hideColumnOptionsAsyncTask;
		_propertyFilters = new();
	}

	#region Methods

	internal void SetProperty<TProp>(string propertyName)
	{
		if (!_propertyFilters.Any(x => x.PropertyName == propertyName))
		{
			_propertyFilters.Add(
				new PropertyFilter<TProp>(propertyName));
		}
	}

	internal async Task<QueryMakerGridItemsProviderResult<TProp>> GetColumnOptionsAsync<TProp>(
		QueryMaker query, string propertyName)
	{
		QueryMakerGridItemsProviderResult<TGridItem> result =
			await GetItemsAsync(query, propertyName);

		return new (
			result.Items.Select(QueryMakerGridContext<TGridItem>.CreateSelectPropertyExpression<TProp>(propertyName)).ToList(),
			result.TotalAmmount);
	}

	private static Func<TGridItem, TProp> CreateSelectPropertyExpression<TProp>(string propertyName)
	{
		ParameterExpression parameterExpression =
			Expression.Parameter(typeof(TGridItem), "o");

		return Expression.Lambda<Func<TGridItem, TProp>>(
			Expression.Property(parameterExpression, propertyName),
			parameterExpression).Compile();
	}

	internal IPropertyFilter GetFilterHandler<TProp>(string propertyName)
	{
		return _propertyFilters.First(x => x.PropertyName == propertyName);
	}

	internal async ValueTask RefreshDataAsync() => await _refreshDataAsyncTask();

	internal async ValueTask<QueryMakerGridItemsProviderResult<TGridItem>> GetItemsAsync(
		QueryMaker query, string property = "")
	{
		List<Filter> subFilters = new();
		foreach (IPropertyFilter propertyFilter
			in _propertyFilters
				.Where(x => x.PropertyName != property))
		{
			Filter? filter = propertyFilter.GenerateQueryMakerFilter();
			if (filter is not null)
			{
				subFilters.Add(filter);
			}
		}

		if (subFilters.Count > 0)
		{
			query.Filter ??= new(){ IsJoiner = true };

			query.Filter.SubFiltersOperation = FilterOperations.AndAlso;
			query.Filter.SubFilters = subFilters.ToArray();
		}

		return await _dataProvider(new(query));
	}

	internal async ValueTask HideColumnOptionsTask() => await _hideColumnOptionsAsyncTask();

	#endregion Methods
}
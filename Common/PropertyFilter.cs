
using QueryMakerLibrary.Grid.Interfaces;
using QueryMakerLibrary.Components;
using static QueryMakerLibrary.Components.Filter;
using System.Collections.Generic;
using System.Linq;
using System;

namespace QueryMakerLibrary.Grid.Common;

internal readonly struct PropertyFilter<TProp>(string propertyName) : IPropertyFilter
{
	public readonly string PropertyName { get; init; } = propertyName;

	private readonly List<ManualCustomFilter> ManualCustomFilters { get; init; } = new();

	private readonly List<CustomFilter> CustomFilters { get; init; } = CustomFilterOptions.FilterActionValues
		.Select(x => new CustomFilter(x)).ToList();

	#region Methods

	private readonly CustomFilter GetPropertyCustomFilter(FilterActions action)
	{
		return CustomFilters.Single(x => x.Action == action);
	}

	private readonly IEnumerable<ManualCustomFilter> GetManualCustomFilter(FilterActions action)
	{
		return ManualCustomFilters.Where(x => x.Action == action);
	}

	internal readonly void SetMultipleCustomFilterValues(FilterActions action,
		HashSet<TProp> values)
	{
		GetPropertyCustomFilter(action).Values.Clear();

		foreach(TProp value in values)
		{
			GetPropertyCustomFilter(action).Values.Add(value);
		}
	}

	internal readonly void SetSingleCustomFilterValue(FilterActions action, TProp value)
	{
		if (!GetPropertyCustomFilter(action).Values.Contains(value))
		{
			GetPropertyCustomFilter(action).Values.Add(value);
		}
		else
		{
			GetPropertyCustomFilter(action).Values.Remove(value);
		}
	}

	internal readonly bool IsValueSet(FilterActions action, TProp value)
	{
		return CustomFilters
			.Any(cf => cf.Action == action
				&& cf.Values.Contains(value));
	}

	internal readonly void RemoveValues(FilterActions action,
		List<TProp> valuesToRemove)
	{
		GetPropertyCustomFilter(action)
		.Values
		.RemoveWhere(value =>
			!valuesToRemove.Contains(value));
	}

	public readonly Filter? GenerateQueryMakerFilter()
	{
		bool addedFirstFilter = false;
		Filter filter = new();

		foreach (FilterActions action in CustomFilterOptions.FilterActionValues)
		{
			HashSet<object?> combinedValues = CombineValues(action);
			if (combinedValues.Count != 0)
			{
				if (!addedFirstFilter)
				{
					filter = new(PropertyName,
						action, combinedValues, false);
					addedFirstFilter = true;
				}
				else
				{
					filter.OrElse(new Filter(PropertyName,
						action, combinedValues, false));
				}
			}
		}

		return addedFirstFilter
			? filter
			: null;
	}

	internal readonly void ClearAllCustomFilters()
		=> CustomFilters.Clear();

	internal readonly IEnumerable<TProp> GetCustomFilterValues(FilterActions action)
	{
		return new List<TProp>(GetPropertyCustomFilter(action)
			.Values);
	}

	internal readonly IEnumerable<ManualCustomFilter> GetManualCustomFilters()
	{
		return new List<ManualCustomFilter>(ManualCustomFilters);
	}

	internal readonly void SetManualCustomFilters(IEnumerable<ManualCustomFilter> manualCustomFilters)
	{
		ManualCustomFilters.Clear();
		ManualCustomFilters.AddRange(manualCustomFilters);
	}

	internal readonly HashSet<object?> CombineValues(FilterActions action)
	{
		return Enumerable.Union(GetPropertyCustomFilter(action).Values.Select(x => (object?)x),
			GetManualCustomFilter(action).Select(x => x.Value)).ToHashSet();
	}

	#endregion Methods

	#region Classes

	private readonly struct CustomFilter(FilterActions action)
	{
		internal readonly FilterActions Action { get; init; } = action;
		internal readonly HashSet<TProp> Values { get; init; } = new();
	}

	#endregion Classes
}

public sealed class ManualCustomFilter
{
	internal ManualCustomFilter(FilterActions action)
	{
		Action = action;
	}

	internal Guid Id { get; init; } = Guid.NewGuid();
	internal FilterActions Action { get; set; } = FilterActions.Contains;
	internal string Value { get; set; } = string.Empty;
}
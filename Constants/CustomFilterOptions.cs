using System;
using System.Collections.Generic;
using System.Linq;
using static QueryMakerLibrary.Components.Filter;

internal static class CustomFilterOptions
{
	internal static readonly FilterActions[] FilterActionValues = Enum.GetValues<FilterActions>();

	internal static readonly IEnumerable<FilterActions> StringOptions =
		FilterActionValues.Where(f => f != FilterActions.GreaterThan && f != FilterActions.LessThan
			&& f != FilterActions.GreaterThanOrEqual && f != FilterActions.LessThanOrEqual);
}
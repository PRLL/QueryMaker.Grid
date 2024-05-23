using System;
using static QueryMakerLibrary.Components.Filter;

namespace QueryMakerLibrary.Grid.Extensions;

internal static class QueryMakerFilterActionsExtension
{
	internal static string GetName(this FilterActions filterAction)
	{
		return filterAction switch
		{
			FilterActions.Contains => "Contains",
			FilterActions.StartsWith => "Starts With",
			FilterActions.EndsWith => "Ends With",
			FilterActions.Equal => "Equal",
			FilterActions.GreaterThan => "Greater Than",
			FilterActions.LessThan => "Less Than",
			FilterActions.GreaterThanOrEqual => "Greater Than Or Equal",
			FilterActions.LessThanOrEqual => "Less Than Or Equal",
			FilterActions.NotContains => "Not Contains",
			FilterActions.NotStartsWith => "Not Starts With",
			FilterActions.NotEndsWith => "Not Ends With",
			FilterActions.NotEqual => "Not Equal",
			_ => throw new ArgumentOutOfRangeException($"{filterAction} has no name assigned")
		};
	}
}
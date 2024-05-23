using QueryMakerLibrary.Components;

namespace QueryMakerLibrary.Grid.Interfaces;

internal interface IPropertyFilter
{
	internal string PropertyName { get; init; }
	internal Filter? GenerateQueryMakerFilter();
}
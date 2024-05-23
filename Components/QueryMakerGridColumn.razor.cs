using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using System.Linq.Expressions;

namespace QueryMakerLibrary.Grid.Components;

public partial class QueryMakerGridColumn<TGridItem, TProp> : PropertyColumn<TGridItem, TProp>
{
	[Parameter]
	public bool RenderQueryMakerGridColumnOptions { get; set; } = true;

	[Parameter]
	public bool Virtualize { get; set; } = false;

	[Parameter]
	public int VirtualizeLoadDelay { get; set; } = 250;

	[Parameter]
	public int VirtualizeItemsPerPage { get; set; } = 200;

	[Parameter]
	public bool SearchWhileTyping { get; set; } = true;

	public string PropertyName { get => ((MemberExpression)Property.Body).Member.Name; }
}
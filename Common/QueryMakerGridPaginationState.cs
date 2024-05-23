using Microsoft.AspNetCore.Components.QuickGrid;

namespace QueryMakerLibrary.Grid.Common;

/// <summary>
/// Class which inherits QuickGrid's <see cref="PaginationState" /> class
/// and adds <see cref="Index" /> property.
/// </summary>
public sealed class QueryMakerGridPaginationState : PaginationState
{
	private string _index = string.Empty;

	/// <summary>
	/// Initializes a new instance of the <see cref="QueryMakerGridPaginationState" /> class.
	/// </summary>
	/// <param name="itemsPerPage">
	/// <para>The number of items on each page.</para>
	/// </param>
	/// <param name="index">
	/// <para>Name of property to use as index for pagination.</para>
	/// <para>Defaults to empty string.</para>
	/// </param>
	public QueryMakerGridPaginationState(int itemsPerPage, string index = "")
		: base()
	{
		ItemsPerPage = itemsPerPage;
		Index = index;
	}

	/// <summary>
	/// <para>Name of property to use as index for pagination.</para>
	/// <para>Defaults to empty string.</para>
	/// </summary>
	public string Index { get => _index; set => _index = value ?? string.Empty; }
}
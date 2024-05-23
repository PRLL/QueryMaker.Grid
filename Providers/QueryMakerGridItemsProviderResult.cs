using System.Collections.Generic;
using System.Linq;

namespace QueryMakerLibrary.Grid.Providers;

/// <summary>
/// Class with resulting values used for <see cref="Components.QueryMakerGrid{TGridItem}.GetDataProvider"/> />.
/// </summary>
/// <param name="items">
/// <para>Resulting items after pagination.</para>
/// </param>
/// <param name="totalAmmount">
/// <para>Total ammount of items withouth pagination.</para>
/// </param>
public readonly struct QueryMakerGridItemsProviderResult<TGridItem>(
	IEnumerable<TGridItem> items, int totalAmmount)
{
	/// <summary>
	/// Resulting items after pagination.
	/// </summary>
	public ICollection<TGridItem> Items { get; init; } = items.ToList();

	/// <summary>
	/// Total ammount of items withouth pagination.
	/// </summary>
	public int TotalAmmount { get; init; } = totalAmmount;
}
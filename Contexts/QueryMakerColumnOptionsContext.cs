namespace QueryMakerLibrary.Grid.Contexts;


public readonly struct QueryMakerColumnOptionsContext<TProp>
{
	private readonly IsCheckedTask _isChecked;
	private readonly AddCustomFilterAndReloadGridTask _addCustomFilterAndReloadGrid;

	internal QueryMakerColumnOptionsContext(
		IsCheckedTask isChecked,
		AddCustomFilterAndReloadGridTask addCustomFilterAndReloadGrid)
	{
		_isChecked = isChecked;
		_addCustomFilterAndReloadGrid = addCustomFilterAndReloadGrid;
	}

	internal bool IsChecked(TProp value)
		=> _isChecked(value);

	internal void AddCustomFilterAndReloadGrid(TProp value)
		=> _addCustomFilterAndReloadGrid(value);

	internal delegate bool IsCheckedTask(TProp value);
	internal delegate void AddCustomFilterAndReloadGridTask(TProp value);
}
using Microsoft.AspNetCore.Components.Web;

namespace QueryMakerLibrary.Grid.Extensions;

public static class KeyboardEventArgsExtension
{
	public static bool PressedEnter(this KeyboardEventArgs e)
	{
		return e.Code == "Enter" || e.Code == "NumpadEnter";
	}
}
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public static class ICheckings
{
	/// <summary>
	/// Checks if current cursor is IBeam.
	/// </summary>
	public static bool IsICursor()
	{
	    var h = Cursors.IBeam.Handle;
	    WinAPI.CURSORINFO cInfo;
	    cInfo.cbSize = Marshal.SizeOf(typeof(WinAPI.CURSORINFO));
	    WinAPI.GetCursorInfo(out cInfo);
	    return cInfo.hCursor == h;
	}	
}

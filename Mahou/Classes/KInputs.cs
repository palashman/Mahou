using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

static class KInputs
{
	/// <summary>
	/// Creates INPUT from key and state.
	/// </summary>
	/// <param name="key">Key to be converted to INPUT.</param>
	/// <param name="down">State of key(down=true, up=false)</param>
	/// <returns>WinAPI.INPUT</returns>
    public static WinAPI.INPUT AddKey(Keys key, bool down)
    {
        var vk = (UInt16)key;
        var input = new WinAPI.INPUT
        {
            Type = WinAPI.INPUT_KEYBOARD,
            Data =
            {
                Keyboard = new WinAPI.KEYBDINPUT
                {
                    Vk = vk,
                    Flags = IsExtended(key) ? (down ? (WinAPI.KEYEVENTF_EXTENDEDKEY) : (WinAPI.KEYEVENTF_KEYUP | WinAPI.KEYEVENTF_EXTENDEDKEY)) : (down ? 0 : WinAPI.KEYEVENTF_KEYUP),
                    Scan = (ushort)WinAPI.MapVirtualKey(vk, 0),
                    ExtraInfo = IntPtr.Zero,
                    Time = 0
                }
            }
        };
        return input;
    }
    /// <summary>
    /// Returns true if key is extended, else false.
    /// </summary>
    /// <param name="key">Key to be checked.</param>
    /// <returns>bool</returns>
    public static bool IsExtended(Keys key) //Check for extended keys
    {
		return key == Keys.Menu ||
			key == Keys.LMenu ||
			key == Keys.RMenu ||
			key == Keys.Control ||
			key == Keys.RControlKey ||
			key == Keys.Insert ||
			key == Keys.Delete ||
			key == Keys.Home ||
			key == Keys.End || 
			key == Keys.Prior ||
			key == Keys.Next || 
			key == Keys.Right ||
			key == Keys.Up || 
			key == Keys.Left ||
			key == Keys.Down || 
			key == Keys.NumLock || 
			key == Keys.Cancel ||
			key == Keys.Snapshot || 
			key == Keys.Divide;
    }
    /// <summary>
    /// Creates array of INPUT from string.
    /// </summary>
    /// <param name="str">String to be converted into INPUT[].</param>
    /// <returns>INPUT[]</returns>
    public static WinAPI.INPUT[] AddString(string str)
    {
        var result = new List<WinAPI.INPUT>();
        char[] inputs = str.ToCharArray();
        foreach (var s in inputs)
        {
            var down = new WinAPI.INPUT
            {
                Type = WinAPI.INPUT_KEYBOARD,
                Data =
                {
                    Keyboard = new WinAPI.KEYBDINPUT
                    {
                        Vk = 0,
                        Flags = WinAPI.KEYEVENTF_UNICODE,
                        Scan = (UInt16)s,
                        ExtraInfo = IntPtr.Zero,
                        Time = 0
                    }
                }
            };
            var up = new WinAPI.INPUT
            {
                Type = WinAPI.INPUT_KEYBOARD,
                Data =
                {
                    Keyboard = new WinAPI.KEYBDINPUT
                    {
                        Vk = 0,
                        Flags = WinAPI.KEYEVENTF_UNICODE | WinAPI.KEYEVENTF_KEYUP,
                        Scan = s,
                        ExtraInfo = IntPtr.Zero,
                        Time = 0
                    }
                }
            };
            if (s == '\n')
            {
                down = AddKey(Keys.Return, true);
                up = AddKey(Keys.Return, false);
            }
            result.Add(down);
            result.Add(up);
        }
        return result.ToArray();
    }
    /// <summary>
    /// Makes input INPUT's in "inputs" variable.
    /// </summary>
    /// <param name="inputs">Array of INPUT to be inputted.</param>
    public static void MakeInput(WinAPI.INPUT[] inputs) //Simply, sends input
    {
    	var done = WinAPI.SendInput((UInt32)inputs.Length+1, inputs, Marshal.SizeOf(typeof(WinAPI.INPUT)));
    	if (done != inputs.Length)
    		Mahou.Logging.Log("ERROR during send input, lenght: " +done+ ", Win32ERR: " + Marshal.GetLastWin32Error());
    }
}

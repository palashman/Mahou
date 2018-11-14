using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

static class KInputs {
	/// <summary>
	/// Creates INPUT from key and state.
	/// </summary>
	/// <param name="key">Key to be converted to INPUT.</param>
	/// <param name="down">State of key(down=true, up=false)</param>
	/// <returns>WinAPI.INPUT</returns>
    public static WinAPI.INPUT AddKey(Keys key, bool down) {
        var vk = (UInt16)key;
        var scan = (ushort)WinAPI.MapVirtualKey(vk, 0);
        System.Diagnostics.Debug.WriteLine("ADDED VK: " +vk + " KEY: " + key + " scan: " + scan);
        var input = new WinAPI.INPUT {
            Type = WinAPI.INPUT_KEYBOARD,
            Data = {
                Keyboard = new WinAPI.KEYBDINPUT {
                    Vk = vk,
                    Flags = IsExtended(key) ? (down ? (WinAPI.KEYEVENTF_EXTENDEDKEY) :
                                               (WinAPI.KEYEVENTF_KEYUP | WinAPI.KEYEVENTF_EXTENDEDKEY)) : 
                    							(down ? 0 : WinAPI.KEYEVENTF_KEYUP),
                    Scan = scan,
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
    public static bool IsExtended(Keys key) { //Check for extended keys
		return key == Keys.RMenu ||
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
			key == Keys.Return || 
			key == Keys.Divide;
    }
    public static string GetWordByIndex(string LINE, int index) {
    	var WORDS = Mahou.KMHook.SplitWords(LINE);
    	string word = "";
    	var len = 0;
    	for (int i = 0; i != WORDS.Length; i++) {
    		if (index >= len && index <= (len += WORDS[i].Length)) {
    			word = WORDS[i];
//    			System.Diagnostics.Debug.WriteLine("WORD AT INDEX "+index + " IS => ["+word+"]");
    		}
    	}
		return word;
    }
    /// <summary>
    /// Creates array of INPUT from string.
    /// </summary>
    /// <param name="str">String to be converted into INPUT[].</param>
    /// <returns>INPUT[]</returns>
    public static WinAPI.INPUT[] AddString(string str) {
        var result = new List<WinAPI.INPUT>();
        var index = 0;
        foreach (var s in str) {
        	bool uselt1_vk, uselt2_vk;
        	ushort resultvk = 0;
        	short lt1_vk = WinAPI.VkKeyScanEx(s, Mahou.MahouUI.MAIN_LAYOUT1);
        	uselt1_vk = lt1_vk != -1;
        	short lt2_vk = WinAPI.VkKeyScanEx(s, Mahou.MahouUI.MAIN_LAYOUT2);
        	uselt2_vk = lt2_vk != -1;
        	if (uselt1_vk && uselt2_vk) {
        		var guess = Mahou.KMHook.WordGuessLayout(GetWordByIndex(str, index));
//        		System.Diagnostics.Debug.WriteLine("ST:"+guess.Item2);
        		var lt_guess = guess.Item2;
        		resultvk = (ushort)WinAPI.VkKeyScanEx(s, lt_guess);
        	} else if (uselt1_vk)
        		resultvk = (ushort)lt1_vk;
        	else if (uselt2_vk) 
        		resultvk = (ushort)lt2_vk;
        	bool resultvk_state = ((resultvk >> 8) & 0xff) == 1;
        	if (resultvk_state)
        		result.Add(KInputs.AddKey(Keys.RShiftKey, true));
            var down = new WinAPI.INPUT {
                Type = WinAPI.INPUT_KEYBOARD,
                Data = {
                    Keyboard = new WinAPI.KEYBDINPUT {
                    	Vk = Mahou.MMain.mahou.GuessKeyCodeFix ? resultvk : (ushort)0,
                        Flags = (UInt32)(WinAPI.KEYEVENTF_UNICODE),
                        Scan = (UInt16)s,
                        ExtraInfo = IntPtr.Zero,
                        Time = 0
                    }
                }
            };
            var up = new WinAPI.INPUT {
                Type = WinAPI.INPUT_KEYBOARD,
                Data = {
                    Keyboard = new WinAPI.KEYBDINPUT {
                    	Vk = Mahou.MMain.mahou.GuessKeyCodeFix ? resultvk : (ushort)0,
                        Flags = (UInt32)(WinAPI.KEYEVENTF_UNICODE | WinAPI.KEYEVENTF_KEYUP),
                        Scan = (UInt16)s,
                        ExtraInfo = IntPtr.Zero,
                        Time = 0
                    }
                }
            };
            if (s == '\n') {
                down = AddKey(Keys.Return, true);
                up = AddKey(Keys.Return, false);
            }
            result.Add(down);
            result.Add(up);
        	if (resultvk_state)
        		result.Add(KInputs.AddKey(Keys.RShiftKey, false));
            index++;
        }
        return result.ToArray();
    }
    /// <summary>
    /// Makes input INPUT's in "inputs" variable.
    /// </summary>
    /// <param name="inputs">Array of INPUT to be inputted.</param>
    public static void MakeInput(WinAPI.INPUT[] inputs) { //Simply, sends input
    	var done = WinAPI.SendInput((UInt32)inputs.Length, inputs, Marshal.SizeOf(typeof(WinAPI.INPUT)));
    	System.Diagnostics.Debug.WriteLine("VK SENDED: " + inputs[0].Data.Keyboard.Vk);
    	if (done != inputs.Length)
    		Mahou.Logging.Log("ERROR during send input, lenght: " +done+ ", Win32ERR: " + Marshal.GetLastWin32Error());
    }
}
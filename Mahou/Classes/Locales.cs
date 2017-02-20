using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mahou
{
	static class Locales
	{
		/// <summary>
		/// Returns current layout id in foreground window.
		/// </summary>
		/// <returns>uint</returns>
		public static uint GetCurrentLocale() //Gets current locale in active window
		{
			uint tid = WinAPI.GetWindowThreadProcessId(ActiveWindow(), IntPtr.Zero);
			IntPtr layout = WinAPI.GetKeyboardLayout(tid);
			//Produces TOO much logging, disabled.
            //Logging.Log("Current locale id is [" + (uint)(layout.ToInt32() & 0xFFFF) + "].");
			return (uint)layout;
		}
		/// <summary>
		/// Returns focused or foreground window.
		/// </summary>
		/// <returns>IntPtr(HWND)</returns>
		public static IntPtr ActiveWindow()
		{
			IntPtr awHandle = IntPtr.Zero;
			var gui = new WinAPI.GUITHREADINFO();
			gui.cbSize = Marshal.SizeOf(gui);
			WinAPI.GetGUIThreadInfo(WinAPI.GetWindowThreadProcessId(WinAPI.GetForegroundWindow(), IntPtr.Zero), ref gui);

			awHandle = gui.hwndFocus;
			if (awHandle == IntPtr.Zero) {
				awHandle = WinAPI.GetForegroundWindow();
			} 
			return awHandle;
		}
		/// <summary>
		/// Returns all installed in system layouts. 
		/// </summary>
		/// <returns></returns>
		public static Locale[] AllList()
		{
			int count = 0;
			var locs = new List<Locale>();
			foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages) {
				count++;
				locs.Add(new Locale {
					Lang = lang.LayoutName,
					uId = (uint)lang.Handle
				});
			}
			return locs.ToArray();
		}
		/// <summary>
		/// Gets Locale from localeString.
		/// </summary>
		/// <param name="localeString">String which contains layout name and uid.</param>
		/// <returns>Locale</returns>
		public static Locale GetLocaleFromString(string localeString) {
			var getLocale = new Regex(@"^(.+)\((\d+)");
			var lang = getLocale.Match(localeString).Groups[1].Value;
			uint id = 0;
			if (!UInt32.TryParse(getLocale.Match(localeString).Groups[2].Value, out id)) 
				throw new Exception("Locale string ["+localeString+"] does not contain a layout uID.");
			return new Locale() { Lang = lang, uId = id};
		}
		/// <summary>
		/// Check if you have enough layouts(>2).
		/// </summary>
		public static void IfLessThan2()
		{
			if (AllList().Length < 2) {
				MessageBox.Show("This program switches texts by system's layouts(locales/languages), please add at least 2!\nProgram will exit.",
				                "You have too less layouts(locales/languages)!!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				Application.Exit();
			}
		}
		/// <summary>
		/// Contains layout name [Lang], and layout id [uId].  
		/// </summary>
		public struct Locale
		{
			public string Lang { get; set; }
			public uint uId { get; set; }
		}
	}
}

using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace Mahou {
	static class jklXHidServ {
		public static uint cycleEmuDesiredLayout = 0;
		public static bool start_cyclEmuSwitch = false;
		public static int jkluMSG = -1;
		static IntPtr HWND = IntPtr.Zero;
		static WinAPI.WndProc WNDPROC_DELEGATE;
	    static public void Destroy() {
			if (HWND != IntPtr.Zero) {
				var serv = WinAPI.FindWindow("_HIDDEN_HWND_SERVER", "_HIDDEN_HWND_SERVER");
				if (serv != IntPtr.Zero)
					WinAPI.PostMessage(serv, WinAPI.WM_QUIT, 0, 0);
		        WinAPI.DestroyWindow(HWND);
		        HWND = IntPtr.Zero;
			}
	    }
	    public static void Init() {
			if (HWND == IntPtr.Zero) {
		        WNDPROC_DELEGATE = jklWndProc;
		        var wnd_class = new WinAPI.WNDCLASS();
		        wnd_class.lpszClassName = "_XHIDDEN_HWND_SERVER";
		        wnd_class.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(WNDPROC_DELEGATE);
		        UInt16 cls_reg = WinAPI.RegisterClassW(ref wnd_class);
		        int last_error = Marshal.GetLastWin32Error();
		        if (cls_reg == 0 && last_error != WinAPI.ERROR_CLASS_ALREADY_EXISTS) {
		            Logging.Log("Could not register window class, for jkl Hidden Server, err: " + last_error, 1);
		        }
		        HWND = WinAPI.CreateWindowExW(0, "_XHIDDEN_HWND_SERVER", "_XHIDDEN_HWND_SERVER", 0, 0, 0, 0, 0,
		                                      IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
				try {
		        	System.Diagnostics.Process.Start(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jkl.exe"));
				} catch { Logging.Log("jkl.exe not found!", 1); }
				Thread.Sleep(50);
				try {
					jkluMSG = Convert.ToInt32(System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "umsg.id")));
				} catch (Exception e) { Logging.Log("Error with umsg.id, details:\r\n" + e.Message + "\r\n" + e.StackTrace, 1); }
				KMHook.DoLater(() => {
				               	for (int i = MMain.locales.Length; i != 0; i--)
				               		KMHook.CycleEmulateLayoutSwitch();
				               }, 350);
			}
	    }	
	    static IntPtr jklWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)  {
			if (msg == jkluMSG) {
				uint layout = (uint)lParam;
				MahouUI.GlobalLayout = MahouUI.currentLayout = layout;
				Logging.Log("Layout changed to " + layout);
				System.Diagnostics.Debug.WriteLine(start_cyclEmuSwitch+"." + cycleEmuDesiredLayout +".Layout changed to " + layout);
				if (start_cyclEmuSwitch) {
					if (layout != cycleEmuDesiredLayout)
						KMHook.CycleEmulateLayoutSwitch();
					else
						start_cyclEmuSwitch = false;
				}
			}
	        return WinAPI.DefWindowProcW(hWnd, msg, wParam, lParam);
	    }
	}
}
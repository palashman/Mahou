using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace Mahou {
	static class jklXHidServ {
		public static uint cycleEmuDesiredLayout = 0;
		public static bool start_cyclEmuSwitch = false;
		public static int jkluMSG = -1;
		/// <summary>0=exe, 1=dll, 2=x86.exe, 3=x86.dll</summary>
		public static bool[] jklFEX = new bool[5];
		public static string jklInfoStr = "";
		static IntPtr HWND = IntPtr.Zero;
		static WinAPI.WndProc WNDPROC_DELEGATE;
	    static public void Destroy() {
			if (HWND != IntPtr.Zero) {
				var serv = WinAPI.FindWindow("_HIDDEN_HWND_SERVER", "_HIDDEN_HWND_SERVER");
				if (serv != IntPtr.Zero)
					WinAPI.PostMessage(serv, WinAPI.WM_QUIT, 0, 0);
				// Multiple CreateWindowEx & WM_DESTROY causes NullReference exception in NATIVE CODE!!
				// So its disabled for now... Create window 1 time and not destroy it.
//				WinAPI.PostMessage(HWND, WinAPI.WM_DESTROY, 0, 0); 
//		        HWND = IntPtr.Zero;
			}
	    }
		public static bool jklExist() {
			var pth = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jkl");
			jklFEX[0] = File.Exists(pth+".exe");
			jklFEX[1] = File.Exists(pth+".dll");
			jklFEX[2] = File.Exists(pth+"x86.exe");
			jklFEX[3] = File.Exists(pth+"x86.dll");
			if (jklFEX[0] && jklFEX[1] && jklFEX[2] && jklFEX[3])
				return true;
			jklInfoStr = "jkl.exe " + (jklFEX[0] ? "" : MMain.Lang[Languages.Element.Not] + " ") + MMain.Lang[Languages.Element.Exist] + "\r\n";
			jklInfoStr += "jkl.dll " + (jklFEX[1] ? "" : MMain.Lang[Languages.Element.Not] + " ") + MMain.Lang[Languages.Element.Exist] + "\r\n";
			jklInfoStr += "jklx86.exe " + (jklFEX[2] ? "" : MMain.Lang[Languages.Element.Not] + " ") + MMain.Lang[Languages.Element.Exist] + "\r\n";
			jklInfoStr += "jklx86.dll " + (jklFEX[3] ? "" : MMain.Lang[Languages.Element.Not] + " ") + MMain.Lang[Languages.Element.Exist] + "\r\n";
			return false;
		}
	    public static void Init() {
			if (HWND == IntPtr.Zero) {
		        WNDPROC_DELEGATE = jklWndProc;
		        var wnd_class = new WinAPI.WNDCLASS();
		        wnd_class.lpszClassName = "_XHIDDEN_HWND_SERVER";
		        wnd_class.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(WNDPROC_DELEGATE);
		        UInt16 cls_reg = WinAPI.RegisterClassW(ref wnd_class);
		        int last_error = Marshal.GetLastWin32Error();
		        if (cls_reg == 0 && last_error != 0) {
		            Logging.Log("Could not register window class, for jkl Hidden Server, err: " + last_error, 1);
		        }
		        HWND = WinAPI.CreateWindowExW(0, "_XHIDDEN_HWND_SERVER", "_XHIDDEN_HWND_SERVER", 0, 0, 0, 0, 0,
		                                      IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			}
			if (jklExist()) {
	        	Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jkl.exe"));
				Thread.Sleep(50);
				jkluMSG = Convert.ToInt32(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "umsg.id")));
				KMHook.DoLater(() => CycleAllLayouts(Locales.ActiveWindow()), 350);
			} else {
				Logging.Log(jklInfoStr, 1);
			}
	    }
		public static void CycleAllLayouts(IntPtr hwnd) {
			for (int i = MMain.locales.Length; i != 0; i--) {
				WinAPI.SendMessage(hwnd, (int)WinAPI.WM_INPUTLANGCHANGEREQUEST, 0, (int)WinAPI.HKL_NEXT);
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
using System;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

/// <summary>
/// This class contains only WinAPI functions.
/// </summary>
public static class WinAPI
{
	#region Constants
    #region KInputs requirements
    public const int INPUT_KEYBOARD = 1;
    public const uint HKL_NEXT = 1;
    public const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
    public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
    public const uint KEYEVENTF_KEYUP = 0x0002;
    public const uint KEYEVENTF_UNICODE = 0x0004;
    public const uint KEYEVENTF_SCANCODE = 0x0008;
	#endregion
	#region NativeClipboard requirements
	public const int CF_TEXT = 1;
	public const int CF_BITMAP = 2;
	public const int CF_SYLK = 4;
	public const int CF_DIF = 5;
	public const int CF_TIFF = 6;
	public const int CF_OEMTEXT = 7;
	public const int CF_DIB = 8;
	public const int CF_PALETTE = 9;
	public const int CF_PENDATA = 10;
	public const int CF_RIFF = 11;
	public const int CF_WAVE = 12;
	public const int CF_UNICODETEXT = 13;
    public const uint GMEM_DDESHARE = 0x2000;
    public const uint GMEM_MOVEABLE = 0x2;
	#endregion
	#region KMHook requirements
	public static uint WM_LBUTTONDOWN = 0x0201;
	public static uint WM_LBUTTONUP = 0x0202;
	public static uint WM_MOUSEMOVE = 0x0200;
	public static uint WM_MOUSEWHEEL = 0x020A;
	public static uint WM_RBUTTONDOWN = 0x0204;
	public static uint WM_RBUTTONUP = 0x0205;
	public static uint WM_MBUTTONDOWN = 0x0207;
	public static uint WM_MBUTTONUP = 0x0208;
	public static int WH_KEYBOARD_LL = 13;
	public static int WH_MOUSE_LL = 14;
	public static uint WM_KEYDOWN = 0x0100;
	public static uint WM_KEYUP = 0x0101;
	public static uint WM_SYSKEYDOWN = 0x0104;
	public static uint WM_SYSKEYUP = 0x0105;
	#endregion
	#region LangDisplay requirements
	public const int SW_SHOWNOACTIVATE = 4;
	public const int HWND_TOPMOST = -1;
	public const uint SWP_NOACTIVATE = 0x0010;
	public const int WS_EX_TRANSPARENT = 0x20;
	public const int WS_EX_LAYERED = 0x80000;
	#endregion
	#endregion
	#region DLL Imports
	#region Configs requires
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    public static extern long WritePrivateProfileString(string section,
    string key, string val, string filePath);
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    public static extern int GetPrivateProfileString(string section,
    string key, string def, StringBuilder retVal, int size, string filePath);
    #endregion
    #region ICheckings requires
	[DllImport("user32.dll")]
	public static extern bool GetCursorInfo(out CURSORINFO pci);
	#endregion
    #region KInputs requires
    [DllImport("user32.dll", SetLastError = true)]
    public static extern UInt32 SendInput(UInt32 numberOfInputs, INPUT[] inputs, Int32 sizeOfInputStructure);
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int MapVirtualKey(uint uCode, uint uMapType);
    #endregion
    #region KMHook requires
	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int extraInfo);	
	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern IntPtr SetWindowsHookEx(int idHook,
		WinAPI.LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);
	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool UnhookWindowsHookEx(IntPtr hhk);
	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
		IntPtr wParam, IntPtr lParam);
	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern IntPtr GetModuleHandle(string lpModuleName);
	[return: MarshalAs(UnmanagedType.Bool)]
	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool PostMessage(IntPtr hhwnd, uint msg, uint wparam, uint lparam);
	[DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
		StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);
	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	public static extern short VkKeyScanEx(char ch, IntPtr dwhkl);
    #endregion
	#region Locales/CaretPos requires
	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern IntPtr GetKeyboardLayout(uint WindowsThreadProcessID);
	
	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern IntPtr GetForegroundWindow();
	
	[DllImport("user32.dll")]
	public static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);
	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetGUIThreadInfo(uint hTreadID, ref GUITHREADINFO lpgui);
	#endregion
    #region NativeClipboard requires 
    [DllImport("user32.dll")]
    public static extern IntPtr GetClipboardData(uint uFormat);
    [DllImport("user32.dll")]
    public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool EmptyClipboard();
    [DllImport("user32.dll")]
    public static extern bool OpenClipboard(IntPtr hWndNewOwner);
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool CloseClipboard();
    [DllImport("user32.dll")]
    public static extern bool IsClipboardFormatAvailable(uint format);
    [DllImport("kernel32.dll")]
    public static extern IntPtr GlobalLock(IntPtr hMem);
    [DllImport("kernel32.dll")]
    public static extern IntPtr GlobalUnlock(IntPtr hMem);
    [DllImport("kernel32.dll")]
    public static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);
    [DllImport("kernel32.dll")]
    public static extern UIntPtr GlobalSize(IntPtr hMem);
    [DllImport("kernel32.dll")]
    public static extern uint EnumClipboardFormats(uint format);
    #endregion
    #region MahouForm requires
	[DllImport("user32.dll")]
	public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, int vk);
	#endregion
	#region CaretPos requires
	[DllImport("user32.dll", SetLastError=true)]
	public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
	[DllImport("user32.dll", SetLastError=true)]
	public static extern bool GetCaretPos(out Point lpPoint);
	[DllImport("user32.dll", SetLastError=true)]
	public static extern IntPtr GetFocus();
	[DllImport("kernel32.dll", SetLastError=true)]
	public static extern uint GetCurrentThreadId();
	[DllImport("user32.dll", SetLastError=true)]
	public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
	[DllImport("user32.dll", SetLastError=true)]
	public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
	#endregion
	#region LangDisplay requires
	[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
	public static extern bool SetWindowPos(
		int hWnd,             // Window handle
		int hWndInsertAfter,  // Placement-order handle
		int X,                // Horizontal position
		int Y,                // Vertical position
		int cx,               // Width
		int cy,               // Height
		uint uFlags);         // Window positioning flags
	[DllImport("user32.dll")]
	public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
	#endregion
	#endregion
    #region Required structs
    #region ICheckings required structs
	[StructLayout(LayoutKind.Sequential)]
	public struct POINT
	{
	    public Int32 x;
	    public Int32 y;
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct CURSORINFO
	{
	    public Int32 cbSize;
	    public Int32 flags;
	    public IntPtr hCursor; 
	    public POINT ptScreenPos; 
	}	
    #endregion
    #region KMHook required delegate
    public delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
    #endregion
    #region KInputs required structs
    #pragma warning disable 649
    public struct INPUT
    {
        public UInt32 Type;
        public KEYBOARDMOUSEHARDWARE Data;
    }
    [StructLayout(LayoutKind.Explicit)]
   	/// <summary>
   	/// This is KEYBOARD-MOUSE-HARDWARE union INPUT won't work if you remove MOUSE or HARDWARE.
   	/// </summary>
    public struct KEYBOARDMOUSEHARDWARE
    {
        [FieldOffset(0)]
        public KEYBDINPUT Keyboard;
        [FieldOffset(0)]
        public HARDWAREINPUT Hardware;
        [FieldOffset(0)]
        public MOUSEINPUT Mouse;
    }
    public struct KEYBDINPUT
    {
        public UInt16 Vk;
        public UInt16 Scan;
        public UInt32 Flags;
        public UInt32 Time;
        public IntPtr ExtraInfo;
    }
    public struct MOUSEINPUT
    {
        public Int32 X;
        public Int32 Y;
        public UInt32 MouseData;
        public UInt32 Flags;
        public UInt32 Time;
        public IntPtr ExtraInfo;
    }
    public struct HARDWAREINPUT
    {
        public UInt32 Msg;
        public UInt16 ParamL;
        public UInt16 ParamH;
    }
	#pragma warning restore 649
    #endregion
	#region Locales/CaretPos required structs
	/// <summary>
	/// Contains x and y positions of upper-left and lower-right corners.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left;        // x position of upper-left corner
		public int Top;         // y position of upper-left corner
		public int Right;       // x position of lower-right corner
		public int Bottom;      // y position of lower-right corner
		public RECT(int lt, int tp, int rt, int btm) {
			Left = lt; Top = tp; 
			Right = rt; Bottom = btm;
		}
	}
	
	public struct GUITHREADINFO
	{
		public int cbSize;
		public int flags;
		public IntPtr hwndActive;
		public IntPtr hwndFocus;
		public IntPtr hwndCapture;
		public IntPtr hwndMenuOwner;
		public IntPtr hwndMoveSize;
		public IntPtr hwndCaret;
		public RECT rectCaret;
	}
	#endregion
    #endregion
}


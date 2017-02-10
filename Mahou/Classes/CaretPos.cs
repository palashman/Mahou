using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
namespace Mahou
{
	public static class CaretPos
	{
		/// <summary>
		/// Attaches two threads.
		/// </summary>
		/// <param name="idAt">Thread [id1] which attach.</param>
		/// <param name="idAtTo">Thread [id2] where [id1] will be attached to.</param>
		/// <param name="fAt">true = Attach, false = detach.</param>
		public static bool AttachThreads(uint idAt, uint idAtTo, bool fAt)
		{
			var atde = "At";
			if (!fAt)
				atde = "De";
			if (WinAPI.AttachThreadInput(idAt, idAtTo, fAt)) {
				Logging.Log(atde + "tached threads."); 
				return true;
			}
			Logging.Log("\t" + atde + "ttach failded, error code: ["+Marshal.GetLastWin32Error()+"].", 1); 
			return false;
		}
		/// <summary>
		/// Gets caret position in window(after thread attach).
		/// </summary>
		/// <param name="lpPo"></param>
		public static bool GetCaretPosition(out Point lpPo)
		{
			if (WinAPI.GetCaretPos(out lpPo)) {
				Logging.Log("GetCaretPos Point: x["+lpPo.X+"], y["+lpPo.Y+"].");
				return true;
			}
			Logging.Log("\tgCP failded, error code: ["+Marshal.GetLastWin32Error()+"].", 1);
			return false;
		}
		/// <summary>
		/// Gets window RECT.
		/// </summary>
		/// <param name="hwnd">Window handle, from which get RECT.</param>
		/// <param name="lpRe">RECT variable which will receive value.</param>
		public static bool GetWinRect(IntPtr hwnd, out WinAPI.RECT lpRe)
		{
			if (WinAPI.GetWindowRect(hwnd, out lpRe)) {
				Logging.Log("GetWinRect Rect: left["+lpRe.Left+"], top["+lpRe.Top+"], right["+lpRe.Right+"], bottom["+lpRe.Bottom+"].");
				return true;
			}
			Logging.Log("\tgetWinRe failded, error code: ["+Marshal.GetLastWin32Error()+"].", 1);
			return false;
		}
		/// <summary>
		/// Gets caret position from focused window or from focused control in window.
		/// </summary>
		/// <returns>Point</returns>
		public static Point GetCaretPointToScreen(out Point caretOnlyPos)
		{
			var LuckyNone = new Point(77777,77777);
			caretOnlyPos = LuckyNone;
			var _cThr_id = WinAPI.GetCurrentThreadId();
			Logging.Log("Current Thread Id: ["+_cThr_id+"].");
			var _fw = WinAPI.GetForegroundWindow();
			Logging.Log("ForeWin's HWND: ["+_fw+"].");
			uint _fwGWTPI_ret = 0; 
			var _fwThr_id = WinAPI.GetWindowThreadProcessId(_fw, out _fwGWTPI_ret);
			Logging.Log("ForeWin ThrId_ret: "+_fwGWTPI_ret+", ForeWin ThrId: "+_fwThr_id+".");
			if (_fwThr_id != _cThr_id) {
				if (!AttachThreads(_fwThr_id, _cThr_id, true))
					return LuckyNone;
				var _fwFCS = WinAPI.GetFocus();
				Logging.Log("ForeWin's focus: ["+_fwFCS+"].");
				var _pntCR = new Point(0, 0);
				var _fwFCS_Re = new WinAPI.RECT(0, 0, 0, 0);
				var _clsNMb = new System.Text.StringBuilder(256);
				WinAPI.GetClassName(_fw, _clsNMb, _clsNMb.Capacity);
				Logging.Log("Focused window classname = ["+_clsNMb+"].");
				var _clsNMfw = _clsNMb.ToString();
				if (_fwFCS != IntPtr.Zero && _fwFCS != _fw) {
					Logging.Log("Getting caret pos from main ForeWin's focused control("+_fwFCS+").");
					WinAPI.GetClassName(_fwFCS, _clsNMb, _clsNMb.Capacity);
					Logging.Log("Focused control classname = ["+_clsNMb+"].");
					if (!AttachThreads(_fwThr_id, _cThr_id, false))
						return LuckyNone;
					GetWinRect(_fwFCS, out _fwFCS_Re);
					uint _fwFCS_GWTPI_ret = 0; 
					var _fwFCS_Thr_id = WinAPI.GetWindowThreadProcessId(_fw, out _fwFCS_GWTPI_ret);
					Logging.Log("ForeWin focus ThrId_ret: "+_fwFCS_GWTPI_ret+", ForeWin focus ThrId: "+_fwFCS_Thr_id+".");
					if (!AttachThreads(_fwFCS_Thr_id, _cThr_id, true))
						return LuckyNone;
					GetCaretPosition(out _pntCR);
				} else {
					Logging.Log("Getting caret pos from main ForeWin.");
					GetCaretPosition(out _pntCR);
					GetWinRect(_fw, out _fwFCS_Re);
					if (!AttachThreads(_fwThr_id, _cThr_id, false) || _pntCR.Equals(new Point(0,0)))
						return LuckyNone;
				}
				// Do not display caret for these classes:
				var _clsNM = _clsNMb.ToString();
				if (new Regex("[Ll][Ii][Ss][Tt][Bb][Oo][Xx]").IsMatch(_clsNM) ||
				    new Regex("[Bb][Uu][Tt][Tt][Oo][Nn]").IsMatch(_clsNM) ||
			  	    new Regex("[Cc][Hh][Ee][Cc][Kk][Bb][Oo][Xx]").IsMatch(_clsNM) ||
				    new Regex("[Cc][Oo][Mm][Bb][Oo][Bb][Oo][Xx]").IsMatch(_clsNM) ||
				    new Regex("[Ll][Ii][Ss][Tt][Vv][Ii][Ee][Ww]").IsMatch(_clsNM) ||
				    new Regex("[Pp][Aa][Gg][Ee][Cc][Oo][Nn][Tt][rR][oO][lL]").IsMatch(_clsNM) ||
				    new Regex("[Ww][Ii][Nn][Dd][Oo][Ww]").IsMatch(_clsNM) ||
				    new Regex("[Ss][Yy][Ss][Ll][Ii][Nn][Kk]").IsMatch(_clsNM) ||
				    new Regex("[Tt][Rr][Ee][Ee]").IsMatch(_clsNM) ||
				    new Regex("[Hh][Ee][Ll][Pp][Ff][Oo][Rr][Mm]").IsMatch(_clsNM) ||
				    _clsNM == "msctls_trackbar32"|| _clsNM.Contains("wxWindow") ||
				    _clsNM == "SysTabControl32" || _clsNM == "DirectUIHWND" ||
				    _clsNM == "Static" ||  _clsNM == "NetUIHWND" || _clsNMfw == "MSPaintApp")
					return LuckyNone;
				if (_clsNM.Contains("SharpDevelop.exe")) {
					_pntCR.Y += 28; _pntCR.X += 3;
				}
				Logging.Log("Get caret position finished succesfully.", 0);
				caretOnlyPos = _pntCR;
				return new Point(_fwFCS_Re.Left + _pntCR.X, _fwFCS_Re.Top + _pntCR.Y);
			}
			return LuckyNone;
		}
	}
}
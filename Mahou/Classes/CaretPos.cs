using System;
using System.Drawing;
using System.Runtime.InteropServices;

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
		public static void AttachThreads(uint idAt, uint idAtTo, bool fAt)
		{
			var atde = "At";
			if (!fAt)
				atde = "De";
			if (WinAPI.AttachThreadInput(idAt, idAtTo, fAt))
				Logging.Log(atde + "tached threads.");
			else
				Logging.Log("\t" + atde + "ttach failded, error code: ["+Marshal.GetLastWin32Error()+"].", 1);
		}
		/// <summary>
		/// Gets caret position in window(after thread attach).
		/// </summary>
		/// <param name="lpPo"></param>
		public static void GetCaretPosition(out Point lpPo)
		{
			if (WinAPI.GetCaretPos(out lpPo))
				Logging.Log("GetCaretPos Point: x["+lpPo.X+"], y["+lpPo.Y+"].");
			else
				Logging.Log("\tgCP failded, error code: ["+Marshal.GetLastWin32Error()+"].", 1);
		}
		/// <summary>
		/// Gets window RECT.
		/// </summary>
		/// <param name="hwnd">Window handle, from which get RECT.</param>
		/// <param name="lpRe">RECT variable which will receive value.</param>
		public static void GetWinRect(IntPtr hwnd, out WinAPI.RECT lpRe)
		{
			if (WinAPI.GetWindowRect(hwnd, out lpRe))
				Logging.Log("GetWinRect Rect: left["+lpRe.Left+"], top["+lpRe.Top+"], right["+lpRe.Right+"], bottom["+lpRe.Bottom+"].");
			else
				Logging.Log("\tgetWinRe failded, error code: ["+Marshal.GetLastWin32Error()+"].", 1);
		}
		/// <summary>
		/// Gets caret position from focused window or from focused control in window.
		/// </summary>
		/// <returns>Point</returns>
		public static Point GetCaretPointToScreen(out Point caretOnlyPos)
		{
			var _fw = WinAPI.GetForegroundWindow();
			Logging.Log("ForeWin's HWND: ["+_fw+"].");
			var _cThr_id = WinAPI.GetCurrentThreadId();
			Logging.Log("Current Thread Id: ["+_cThr_id+"].");
			uint _fwGWTPI_ret = 0; 
			var _fwThr_id = WinAPI.GetWindowThreadProcessId(_fw, out _fwGWTPI_ret);
			Logging.Log("ForeWin ThrId_ret: "+_fwGWTPI_ret+", ForeWin ThrId: "+_fwThr_id+".");
			AttachThreads(_fwThr_id, _cThr_id, true);
			var _fwFCS = WinAPI.GetFocus();
			Logging.Log("ForeWin's focus: ["+_fwFCS+"].");
			var _pntCR = new Point(0, 0);
			var _fwFCS_Re = new WinAPI.RECT(0, 0, 0, 0);
			if (_fwFCS != IntPtr.Zero && _fwFCS != _fw) {
				Logging.Log("Getting caret pos from main ForeWin's focused control("+_fwFCS+").");
				AttachThreads(_fwThr_id, _cThr_id, false);
				GetWinRect(_fwFCS, out _fwFCS_Re);
				uint _fwFCS_GWTPI_ret = 0; 
				var _fwFCS_Thr_id = WinAPI.GetWindowThreadProcessId(_fw, out _fwFCS_GWTPI_ret);
				Logging.Log("ForeWin focus ThrId_ret: "+_fwFCS_GWTPI_ret+", ForeWin focus ThrId: "+_fwFCS_Thr_id+".");
				AttachThreads(_fwFCS_Thr_id, _cThr_id, true);
				GetCaretPosition(out _pntCR);
			} else {
				Logging.Log("Getting caret pos from main ForeWin.");
				GetCaretPosition(out _pntCR);
				GetWinRect(_fw, out _fwFCS_Re);
			}
//			var _pntSCR = new Point(0,0);
//			WinAPI.ClientToSCR(_fw, ref _pntSCR);
			AttachThreads(_fwThr_id, _cThr_id, false);
			Logging.Log("Done.", 1);
			caretOnlyPos = _pntCR;
			return new Point(_fwFCS_Re.Left + _pntCR.X, _fwFCS_Re.Top + _pntCR.Y);
		}
	}
}
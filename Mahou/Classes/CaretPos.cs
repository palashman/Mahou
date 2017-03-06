using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
namespace Mahou
{
	public static class CaretPos
	{
		/// <summary>
		/// Gets caret position from focused window or from focused control in window.
		/// </summary>
		/// <returns>Point</returns>
		public static Point GetCaretPointToScreen(out Point caretOnlyPos)
		{
			var LuckyNone = new Point(77777,77777);
			caretOnlyPos = LuckyNone;
			var _cThr_id = WinAPI.GetCurrentThreadId();
			var _pntCR = new Point(0, 0);
			var _fwFCS_Re = new WinAPI.RECT(0, 0, 0, 0);
			var _fw = WinAPI.GetForegroundWindow();
			uint dummy = 0; 
			var _fwThr_id = WinAPI.GetWindowThreadProcessId(_fw, out dummy);
			IntPtr _fwFCS = IntPtr.Zero;
			var _clsNMb = new System.Text.StringBuilder(256);
			string _clsNMfw = "";
			Logging.Log("_c HWND: [" +MMain.mahou.Handle+ "], _c ThrId: ["+_cThr_id+"], "+"_fw HWND: ["+_fw+"]"+", _fw ThrId: "+_fwThr_id+".");
			if (_fwThr_id != _cThr_id) {
				if (WinAPI.AttachThreadInput(_fwThr_id, _cThr_id, true)) {
					_fwFCS = WinAPI.GetFocus();
					WinAPI.GetClassName(_fw, _clsNMb, _clsNMb.Capacity);
					_clsNMfw = _clsNMb.ToString();
					if (_fwFCS != IntPtr.Zero && _fwFCS != _fw) {
						Logging.Log("_fcs: ["+_fwFCS+"]."+"_fw classname = ["+_clsNMb+"].");
						WinAPI.GetClassName(_fwFCS, _clsNMb, _clsNMb.Capacity);
						Logging.Log("_fcs classname = ["+_clsNMb+"].");
						WinAPI.GetWindowRect(_fwFCS, out _fwFCS_Re);
						WinAPI.GetCaretPos(out _pntCR);
					} else {
						WinAPI.GetCaretPos(out _pntCR);
						WinAPI.GetWindowRect(_fw, out _fwFCS_Re);
						if (_pntCR.Equals(new Point(0,0)))
							return LuckyNone;
					}
					WinAPI.AttachThreadInput(_fwThr_id, _cThr_id, false);
				} else
					return LuckyNone;
				Logging.Log("CaretPos = x["+_pntCR.X+"], y["+_pntCR.Y+"].");	
				// Do not display caret for these classes:
				var _clsNM = _clsNMb.ToString();
				if (new Regex("[L][I][S][T][B][O][X]", RegexOptions.IgnoreCase).IsMatch(_clsNM) ||
				    new Regex("[B][U][T][T][O][N]", RegexOptions.IgnoreCase).IsMatch(_clsNM) ||
			  	    new Regex("[C][H][E][C][K][B][O][X]", RegexOptions.IgnoreCase).IsMatch(_clsNM) ||
				    new Regex("[C][O][M][B][O][B][O][X]", RegexOptions.IgnoreCase).IsMatch(_clsNM) ||
				    new Regex("[L][I][S][T][V][I][E][W]", RegexOptions.IgnoreCase).IsMatch(_clsNM) ||
				    new Regex("[P][A][G][E][C][O][N][T][r][o][l]", RegexOptions.IgnoreCase).IsMatch(_clsNM) ||
				    (new Regex("[W][I][N][D][O][W]", RegexOptions.IgnoreCase).IsMatch(_clsNM) && _clsNM != "MozillaWindowClass") ||
				    new Regex("[S][Y][S][L][I][N][K]", RegexOptions.IgnoreCase).IsMatch(_clsNM) ||
				    new Regex("[T][R][E][E]", RegexOptions.IgnoreCase).IsMatch(_clsNM) ||
				    new Regex("[H][E][L][P][F][O][R][M]", RegexOptions.IgnoreCase).IsMatch(_clsNM) ||
				    new Regex("[T][M][A][I][N][F][O][R][M]", RegexOptions.IgnoreCase).IsMatch(_clsNM) ||
				    new Regex("[B][T][N]", RegexOptions.IgnoreCase).IsMatch(_clsNM) || _clsNM.Contains("Afx:") || 
				    _clsNM == "msctls_trackbar32"|| _clsNM.Contains("wxWindow") ||
				    _clsNM == "SysTabControl32" || _clsNM == "DirectUIHWND" ||
				    _clsNM == "Static" ||  _clsNM == "NetUIHWND" || _clsNMfw == "MSPaintApp" ||
					_clsNM == "PotPlayer" || _clsNM == "MDIClient")
					return LuckyNone;
				if (_clsNM.Contains("SharpDevelop.exe")) {
					_pntCR.Y += 28; _pntCR.X += 3;
				}
				Logging.Log("Get caret position finished successfully.", 0);
				caretOnlyPos = _pntCR;
				return new Point(_fwFCS_Re.Left + _pntCR.X, _fwFCS_Re.Top + _pntCR.Y);
			}
			return LuckyNone;
		}
	}
}
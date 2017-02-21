using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mahou
{
	public partial class LangDisplay : Form
	{
		/// <summary>
		/// Used to determine if lang display is used for caret display.
		/// </summary>
		public bool transparentBG, caretDisplay, mouseDisplay;
		public string lastText = "NO";
		public LangDisplay()
		{
			InitializeComponent();
			SetVisInvis();
			this.SetStyle(ControlStyles.DoubleBuffer, true);
		}
		/// <summary>
		/// Change lang display text.
		/// </summary>
		/// <param name="to">Text to be changed to.</param>
		public void ChangeLD(string to)
		{
			if (to == lastText) return;
			lbLang.Text = lastText = to;
			if (!transparentBG) return;
			Invalidate();
			Update();
		}
		/// <summary>
		/// Toggles lang display label visibility.
		/// </summary>
		public void SetVisInvis() {
			lbLang.Visible = !transparentBG;
			Invalidate();
			Update();
		}
		/// <summary>
		/// Refresh language text.
		/// </summary>
		public void RefreshLang()
		{
			if (!Visible) return;
			if (MMain.mahou.LDCaretTransparentBack_temp && caretDisplay)
				transparentBG = true;
			else if (MMain.mahou.LDMouseTransparentBack_temp && mouseDisplay)
				transparentBG = true;
			else transparentBG = false;
			if (MMain.mahou.DiffColorsForLayouts) {
				if (MMain.mahou.Layout1TransparentBack_temp && caretDisplay)
					transparentBG = true;
				else if (MMain.mahou.Layout2TransparentBack_temp && mouseDisplay)
					transparentBG = true;
				else transparentBG = false;
				var cLuid = Locales.GetCurrentLocale();
				if (cLuid == Locales.GetLocaleFromString(MMain.mahou.cbb_MainLayout1.SelectedItem.ToString()).uId) {
					lbLang.ForeColor = MMain.mahou.Layout1Fore_temp;
					lbLang.BackColor = MMain.mahou.Layout1Back_temp;
					lbLang.Font = MMain.mahou.Layout1Font_temp;
					lbLang.Height = MMain.mahou.Layout1Height_temp;
					lbLang.Width = MMain.mahou.Layout1Width_temp;
				} else if (cLuid == Locales.GetLocaleFromString(MMain.mahou.cbb_MainLayout2.SelectedItem.ToString()).uId) {
					lbLang.ForeColor = MMain.mahou.Layout2Fore_temp;
					lbLang.BackColor = MMain.mahou.Layout2Back_temp;
					lbLang.Font = MMain.mahou.Layout2Font_temp;
					lbLang.Height = MMain.mahou.Layout2Height_temp;
					lbLang.Width = MMain.mahou.Layout2Width_temp;
				}			
			}
			if (transparentBG)
				TransparencyKey = BackColor = lbLang.BackColor = Color.Pink;
			Size = lbLang.Size;
			var cLid = (int)(Locales.GetCurrentLocale() & 0xffff);
			if (cLid > 0) {
				var clangname = new System.Globalization.CultureInfo(cLid);
				ChangeLD(clangname.ThreeLetterISOLanguageName.Substring(0, 1).ToUpper() + clangname.ThreeLetterISOLanguageName.Substring(1));
			} else {
				Logging.Log("Language tooltip text NOT changed, locale id = [" + cLid + "].", 2);
			}
		}
		/// <summary>
		/// Change font and colors of lang display.
		/// </summary>
		/// <param name="fnt">Font to be changed to.</param>
		/// <param name="fore">Text color to be changed to.</param>
		/// <param name="back">Background color to be changed to.</param>
		public void ChangeColors(Font fnt, Color fore, Color back)
		{
			lbLang.ForeColor = fore;
			lbLang.BackColor = back;
			lbLang.Font = fnt;
		}
		/// <summary>
		/// Change size of lang display.
		/// </summary>
		/// <param name="height">Height to be set to.</param>
		/// <param name="width">Width to be set to.</param>
		public void ChangeSize(int height, int width)
		{
			lbLang.Height = height;
			lbLang.Width = width;
		}
		/// <summary>
		/// Show lang display form without activation.
		/// </summary>
		public void ShowInactiveTopmost()
		{
			if (Visible) return;
			WinAPI.ShowWindow(Handle, WinAPI.SW_SHOWNOACTIVATE);
			WinAPI.SetWindowPos(Handle.ToInt32(), WinAPI.HWND_TOPMOST,
				Left, Top, Width, Height,
				WinAPI.SWP_NOACTIVATE);
		}
		/// <summary>
		/// Hide lang display window.
		/// </summary>
		public void HideWnd()
		{
			if (!Visible) return;
			WinAPI.ShowWindow(Handle, 0);
		}
		protected override CreateParams CreateParams {
			get {
				var Params = base.CreateParams;
				// Hides form from everywhere (taskbar/task switcher/etc.).
				Params.ExStyle |= WinAPI.WS_EX_TOOLWINDOW;
				// Add click through window ability.
				Params.ExStyle |= WinAPI.WS_EX_LAYERED | WinAPI.WS_EX_TRANSPARENT;
				return Params;
			}
		}
		/// <summary>
		/// Transparent background text rendering.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (!transparentBG) { base.OnPaint(e); return; }
			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
			e.Graphics.DrawString(lbLang.Text, lbLang.Font, new SolidBrush(lbLang.ForeColor), 0, 0);
			base.OnPaint(e);
		}
	}
}

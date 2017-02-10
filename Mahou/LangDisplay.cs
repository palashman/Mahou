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
		public bool caretDisplay = false;
		public LangDisplay()
		{
			InitializeComponent();
			SetVisInvis();
		}
		/// <summary>
		/// Change lang display text.
		/// </summary>
		/// <param name="to">Text to be changed to.</param>
		public void ChangeLD(string to)
		{
			lbLang.Text = to;
			if (MMain.MyConfs.ReadBool("TTipUI", "TransparentBack") || caretDisplay) {
				Invalidate();
				Update();
			}
		}
		/// <summary>
		/// Toggles lang display label visibility.
		/// </summary>
		public void SetVisInvis()
		{
			lbLang.Visible = !MMain.MyConfs.ReadBool("TTipUI", "TransparentBack");
		}
		/// <summary>
		/// Refresh language text.
		/// </summary>
		public void RefreshLang()
		{
			if (MMain.MyConfs.ReadBool("Functions", "DiffLayoutColors")) {
				var cLid = Locales.GetCurrentLocale();
				if (cLid == MMain.MyConfs.ReadUInt("Locales", "locale1uId")) {
					lbLang.ForeColor = ColorTranslator.FromHtml(MMain.MyConfs.Read("TTipUI", "L1DiffFGColor"));
					lbLang.BackColor = ColorTranslator.FromHtml(MMain.MyConfs.Read("TTipUI", "L1DiffBGColor"));
					lbLang.Font = (Font)MMain.mahou.moreConfigs.fcv.ConvertFromString(MMain.MyConfs.Read("TTipUI", "L1DiffFont"));
				} else if (cLid == MMain.MyConfs.ReadUInt("Locales", "locale2uId")) {
					lbLang.ForeColor = ColorTranslator.FromHtml(MMain.MyConfs.Read("TTipUI", "L2DiffFGColor"));
					lbLang.BackColor = ColorTranslator.FromHtml(MMain.MyConfs.Read("TTipUI", "L2DiffBGColor"));
					lbLang.Font = (Font)MMain.mahou.moreConfigs.fcv.ConvertFromString(MMain.MyConfs.Read("TTipUI", "L2DiffFont"));			
				}
			}
			Size = lbLang.Size;
//			Width += 4;
			if (MMain.MyConfs.ReadBool("TTipUI", "TransparentBack"))
				TransparencyKey = BackColor = lbLang.BackColor = Color.Pink;
			var lcid = (int)(Locales.GetCurrentLocale() & 0xffff);
			if (lcid > 0) {
				var clangname = new System.Globalization.CultureInfo(lcid);
				ChangeLD(clangname.ThreeLetterISOLanguageName.Substring(0, 1).ToUpper() + clangname.ThreeLetterISOLanguageName.Substring(1));
			} else {
				Logging.Log("Language tooltip text NOT changed, locale id = [" + lcid + "].", 2);
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
			WinAPI.ShowWindow(Handle, 0);
		}
		protected override CreateParams CreateParams {
			get {
				var Params = base.CreateParams;
				// Hides form from everywhere(taskbar/task switcher/etc.).
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
			if (MMain.MyConfs.ReadBool("TTipUI", "TransparentBack")) {
				e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
				e.Graphics.DrawString(lbLang.Text, lbLang.Font, new SolidBrush(lbLang.ForeColor), 0, 0);
			}
			base.OnPaint(e);
		}
	}
}

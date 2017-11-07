using System.Drawing;
using System.Windows.Forms;

namespace Mahou
{
	public partial class LangDisplay : Form
	{
		/// <summary>
		/// Used to determine if lang display is used for caret display.
		/// </summary>
		public bool lastTransparentBG, transparentBG, caretDisplay, mouseDisplay, DisplayFlag, onInit = true, Empty;
		public string lastText = "NO";
		Size lastsize = new Size(0,0);
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
			if (DisplayFlag) {
				lbLang.Visible = false;
				MahouUI.RefreshFLAG();
				if (MMain.mahou.MouseTTAlways && mouseDisplay) // fix for tray stuck due to variable "LayoutChanged" which being changed by this mouse tooltip always
					MMain.mahou.icon.trIcon.Icon = Icon.FromHandle(MahouUI.FLAG.GetHicon());
				BackgroundImage = MahouUI.FLAG;
				TransparencyKey = BackColor = Color.Pink;
				Invalidate();
				Update();
			}
			if (!transparentBG) return;
			Invalidate();
			Update();
		}
		public void DisplayUpper(bool Upper) {
			var lastvis = pct_UpperArrow.Visible;
			if (Upper && !pct_UpperArrow.Visible)
				pct_UpperArrow.Visible = true;
			else if (!Upper && pct_UpperArrow.Visible)
				pct_UpperArrow.Visible = false;
			if (lastvis != pct_UpperArrow.Visible) {
				ReSize();
				if (Upper) {
					pct_UpperArrow.Left = Width;
					pct_UpperArrow.Top = (Height - 16)/2+1;
					Width = Width + 16;
				}
				lastsize = Size;
			}
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
			uint cLid = Locales.GetCurrentLocale();
			if (DisplayFlag && MMain.mahou.DiffAppearenceForLayouts) {
				if (cLid == MahouUI.MAIN_LAYOUT1 && MMain.mahou.Layout1TText.Length < 2)
					lbLang.Text = MMain.mahou.Layout1TText;
				else if (cLid == MahouUI.MAIN_LAYOUT2 && MMain.mahou.Layout2TText.Length < 2)
					lbLang.Text = MMain.mahou.Layout2TText;
				Empty = (MMain.mahou.DiffAppearenceForLayouts && lbLang.Text == " ");
			}
			if (!Visible || Empty) return;
			if (cLid == 0)
				cLid = MahouUI.currentLayout;
			if (MMain.mahou.OneLayout)
				cLid = MahouUI.GlobalLayout;
			if (MMain.mahou.LDCaretTransparentBack_temp && caretDisplay)
				transparentBG = true;
			else if (MMain.mahou.LDMouseTransparentBack_temp && mouseDisplay)
				transparentBG = true;
			else transparentBG = false;
			var notTwo = false;
			if ((cLid & 0xffff) > 0) {
				var clangname = new System.Globalization.CultureInfo((int)(cLid & 0xffff));
				if (MMain.mahou.DiffAppearenceForLayouts && !DisplayFlag) {
					if (cLid == MahouUI.MAIN_LAYOUT1) {
						ChangeColors(MMain.mahou.Layout1Font_temp, MMain.mahou.Layout1Fore_temp, 
						             MMain.mahou.Layout1Back_temp, MMain.mahou.Layout1TransparentBack_temp);
						ChangeSize(MMain.mahou.Layout1Height_temp, MMain.mahou.Layout1Width_temp);
						ChangeLD(MMain.mahou.Layout1TText);
					} else if (cLid == MahouUI.MAIN_LAYOUT2) {
						ChangeColors(MMain.mahou.Layout2Font_temp, MMain.mahou.Layout2Fore_temp, 
						             MMain.mahou.Layout2Back_temp, MMain.mahou.Layout2TransparentBack_temp);
						ChangeSize(MMain.mahou.Layout2Height_temp, MMain.mahou.Layout2Width_temp);
						ChangeLD(MMain.mahou.Layout2TText);
					} else notTwo = true;
				} else notTwo = true;
				if (notTwo) {
					if (mouseDisplay) {
						ChangeColors(MMain.mahou.LDMouseFont_temp, MMain.mahou.LDMouseFore_temp,
						             MMain.mahou.LDMouseBack_temp, MMain.mahou.LDMouseTransparentBack_temp);
						ChangeSize(MMain.mahou.LDMouseHeight_temp, MMain.mahou.LDMouseWidth_temp);
					}
					if (caretDisplay) {
						ChangeColors(MMain.mahou.LDCaretFont_temp, MMain.mahou.LDCaretFore_temp, 
						             MMain.mahou.LDCaretBack_temp, MMain.mahou.LDCaretTransparentBack_temp);
						ChangeSize(MMain.mahou.LDCaretHeight_temp, MMain.mahou.LDCaretWidth_temp);
					}
					ChangeLD(clangname.ThreeLetterISOLanguageName.Substring(0, 1).ToUpper() + clangname.ThreeLetterISOLanguageName.Substring(1));
				}
				if (!MMain.mahou.DiffAppearenceForLayouts || lbLang.Text == "") 
					ChangeLD(clangname.ThreeLetterISOLanguageName.Substring(0, 1).ToUpper() + clangname.ThreeLetterISOLanguageName.Substring(1));
				if (transparentBG)
					TransparencyKey = BackColor = lbLang.BackColor = pct_UpperArrow.BackColor = Color.Pink;
				if (lastTransparentBG != transparentBG)
					SetVisInvis();
				lastTransparentBG = transparentBG;
				if ((caretDisplay && !MahouUI.caretLTUpperArrow) || (mouseDisplay && !MahouUI.mouseLTUpperArrow) || onInit) {
					ReSize(); onInit = false;
				}
			} else {
				Logging.Log("Language tooltip text NOT changed, locale id = [" + cLid + "].", 2);
			}
		}
		void ReSize() {
			if (DisplayFlag) {
				if (lastsize == BackgroundImage.Size) return;
				Size = BackgroundImage.Size;
			}
			else {
				if (lastsize == lbLang.Size) return;
				Size = lbLang.Size;
			}
			lastsize = Size;
		}
		/// <summary>
		/// Change font and colors of lang display.
		/// </summary>
		/// <param name="fnt">Font to be changed to.</param>
		/// <param name="fore">Text color to be changed to.</param>
		/// <param name="back">Background color to be changed to.</param>
		/// <param name="tBG">Transparent background.</param>
		public void ChangeColors(Font fnt, Color fore, Color back, bool tBG)
		{
			transparentBG = tBG;
			lbLang.ForeColor = fore;
			pct_UpperArrow.BackColor = lbLang.BackColor = back;
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
			if (!transparentBG || DisplayFlag) { base.OnPaint(e); return; }
			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
			e.Graphics.DrawString(lbLang.Text, lbLang.Font, new SolidBrush(lbLang.ForeColor), 0, 0);
			base.OnPaint(e);
		}
	}
}

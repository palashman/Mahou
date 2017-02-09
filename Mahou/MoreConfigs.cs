using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
namespace Mahou
{
	public partial class MoreConfigs : Form
	{
		#region Varibales
		int tmpSIKey = 0, page = 0, tempConMorWorKey = 0;
		string tmpSIMods = "None", tempConMorWorMods = "None";
		ColorDialog clrd = new ColorDialog();
		FontDialog fntd = new FontDialog();
		public FontConverter fcv = new FontConverter();
		public string snipfile = Path.Combine(Mahou.Update.nPath, "snippets.txt");
		public bool Active;
		#endregion
		#region Button/Form/etc. events
		public MoreConfigs()
		{
			InitializeComponent();
		}
		void MoreConfigs_Load(object sender, EventArgs e)
		{
			nudXpos.Minimum = nudYpos.Minimum = -100;
			RefreshLocales();
			load();
			DisEna();
			RefreshLanguage();
			tmpRestore();
			tbHKSymIgn.Text = MMain.mahou.OemReadable((MMain.MyConfs.Read("Hotkeys", "HKSymIgnMods").ToString().Replace(",", " +") + " + " +
				MMain.mahou.Remake((Keys)MMain.MyConfs.ReadInt("Hotkeys", "HKSymIgnKey")).Replace("None + ", "")));
			tbConMorWor.Text = MMain.mahou.OemReadable((MMain.MyConfs.Read("Hotkeys", "HKConvertMoreMods").ToString().Replace(",", " +") + " + " +
				MMain.mahou.Remake((Keys)MMain.MyConfs.ReadInt("Hotkeys", "HKConvertMore")).Replace("None + ", "")));
			Deactivate += (se, ea) => {
				Active = false;
			};
		}
		void MoreConfigs_FormClosing(object sender, FormClosingEventArgs e)
		{
			Close(e);
		}
		void MoreConfigs_Activated(object sender, EventArgs e)
		{
			Active = true;
			RefreshLanguage();
		}
		void btnOK_Click(object sender, EventArgs e)
		{
			Save();
			Close();
		}
		void btnNO_Click(object sender, EventArgs e)
		{
			Close();
		}
		void btShowColorDialog(object sender, EventArgs e)
		{
			var btn = sender as Button;
			if (clrd.ShowDialog() == DialogResult.OK)
				btn.BackColor = clrd.Color;
		}
		void btShowFontDialog(object sender, EventArgs e)
		{
			var btn = sender as Button;
			fntd.Font = btn.Font;
			if (fntd.ShowDialog() == DialogResult.OK)
				btn.Font = fntd.Font;
		}
		void BtnDbgInfClick(object sender, EventArgs e)
		{
			try {
				string debuginfo = "### MAHOU DEBUG INFO";
				debuginfo += "\r\n" + "- Mahou-v"+Application.ProductVersion;
				debuginfo += "\r\n" + "- OS = [" + Environment.OSVersion + "]";
				debuginfo += "\r\n" + "- x64 = [" + Environment.Is64BitOperatingSystem + "]";
				debuginfo += "\r\n" + "- .Net = [" + Environment.Version +"]";
				debuginfo += "\r\n" + "#### All installed layouts:\r\n";
				foreach (var l in MMain.lcnmid) {
					debuginfo += l + "\r\n";
				}
				debuginfo += "#### Mahou.ini:\r\n```ini\r\n"+ File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mahou.ini")) + "```";
				if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "snippets.txt")))
				    debuginfo += "\r\n" + "#### Snippets:\r\n```\r\n" + File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "snippets.txt")) + "```";
				Clipboard.SetText(debuginfo);
				var tmr = new Timer();
				tmr.Tick += (_,__) => { 
					btnDbgInf.Text = MMain.UI[67];
					tmr.Stop();
				};
				tmr.Interval = 2000;
				btnDbgInf.Text = MMain.UI[68];
				tmr.Start();
				
				Logging.Log("Debug info copied.");
			}
			catch(Exception er) { 
				Logging.Log("Error during DEBUG INFO copy, details:\r\n" +er.Message +"\r\n"+er.StackTrace);
			}
		}
		void DisEnaOnCheckedChanged(object sender, EventArgs e)
		{
			DisEna();
		}
		void tbHKSymIgn_KeyDown(object sender, KeyEventArgs e)
		{
			if (MMain.MyConfs.ReadBool("DoubleKey", "Use")) {
				tbHKSymIgn.Text = MMain.mahou.OemReadable(MMain.mahou.Remake(e.KeyCode));
				tmpSIMods = "None";
				tmpSIKey = (int)e.KeyCode;				
			} else {
				tbHKSymIgn.Text = MMain.mahou.OemReadable((e.Modifiers.ToString().Replace(",", " +") + " + " +
				MMain.mahou.Remake(e.KeyCode)).Replace("None + ", ""));
				tmpSIMods = e.Modifiers.ToString().Replace(",", " +");
				switch ((int)e.KeyCode) {
					case 16:
					case 17:
					case 18:
						tmpSIKey = 0;
						break;
					default:
						tmpSIKey = (int)e.KeyCode;
						break;
				}
			}
		}
		void TbConMorWorKeyDown(object sender, KeyEventArgs e)
		{
			if (MMain.MyConfs.ReadBool("DoubleKey", "Use")) {
				tbConMorWor.Text = MMain.mahou.OemReadable(MMain.mahou.Remake(e.KeyCode));
				tempConMorWorMods = "None";
				tempConMorWorKey = (int)e.KeyCode;				
			} else {
				tbConMorWor.Text = MMain.mahou.OemReadable((e.Modifiers.ToString().Replace(",", " +") + " + " +
				MMain.mahou.Remake(e.KeyCode)).Replace("None + ", ""));
				tempConMorWorMods = e.Modifiers.ToString().Replace(",", " +");
				switch ((int)e.KeyCode) {
					case 16:
					case 17:
					case 18:
						tempConMorWorKey = 0;
						break;
					default:
						tempConMorWorKey = (int)e.KeyCode;
						break;
				}
			}
		}
		void BtnEMoreClick(object sender, EventArgs e)
		{
			page++;
			Logging.Log("More Configs page changed page = "+ page+".");
			switch (page) {
				case 0:
					pEExtra.Visible = false;
					pEMore.Visible = false;
					break;
				case 1:
					pEMore.Visible = true;
					pEExtra.Visible = false;
					break;
				case 2:
					pEMore.Visible = false;
					pEExtra.Visible = true;
					page = -1;
					break;
			}
			DisEna();
			RefreshLanguage();
		}
		#endregion
		#region Functions
		/// <summary>
		/// Closes window without destruction.
		/// </summary>
		/// <param name="e">Form closing event argumnets.</param>
		void Close(FormClosingEventArgs e) // 
		{
			if (e.CloseReason == CloseReason.UserClosing) {
				e.Cancel = true;
				Visible = false;
			}
			DisEna();
		}
		/// <summary>
		/// Disables or Enables some controls.
		/// </summary>
		void DisEna()
		{
			MMain.mahou.IfNotExist();
			lbLCto.Enabled = lbRCto.Enabled = cbLCLocalesList.Enabled = cbRCLocalesList.Enabled = cbUseLRC.Checked;
			nudRefreshRate.Enabled = cbOnChange.Enabled = lblRefRate.Enabled = lbColors.Enabled = 
				btCol1.Enabled = btCol2.Enabled = btFont.Enabled = lbSize.Enabled = lbPosition.Enabled =
				nudTTWidth.Enabled = nudTTHeight.Enabled = nudXpos.Enabled = nudYpos.Enabled = cbDisplayLang.Checked;
			lbDDelay.Enabled = nudDoubleDelay.Enabled = cbDoublePress.Checked;
			tbSnippets.Enabled = cbUseSnippets.Checked;
			btL1DifClrBGClr.Enabled = btL1DifClrFGClr.Enabled = btL1DifClrFont.Enabled = lblLayout1DifClr.Enabled =
				lblLayout2DifClr.Enabled = btL2DifClrBGClr.Enabled = btL2DifClrFGClr.Enabled = btL2DifClrFont.Enabled =
				cbDifLayoClr.Checked;
			btCrtCol1.Enabled = btCrtCol2.Enabled = btCrtFont.Enabled = nudCrtTTHeight.Enabled = lbCrtH.Enabled =
				lbCrtW.Enabled = lbCrtX.Enabled = lbCrtY.Enabled = lblCrtRefRate.Enabled = nudCrtTTWidth.Enabled =
				nudCrtXpos.Enabled = nudCrtYpos.Enabled = nudCrtRefreshRate.Enabled = cbCaretLangDisplay.Checked;
			btCol2.Enabled = btCrtCol2.Enabled = btL1DifClrBGClr.Enabled = btL2DifClrBGClr.Enabled = !cbTrBLT.Checked;
			MMain.mahou.RemoveAddCtrls();
		}
		/// <summary>
		/// Saves configurations.
		/// </summary>
		void Save()
		{
			MMain.mahou.IfNotExist();
			var getLocale = new Regex(@"^(.+)\((\d+)");
			MMain.MyConfs.Write("ExtCtrls", "LCLocaleName", getLocale.Match(cbLCLocalesList.Text).Groups[1].Value);
			MMain.MyConfs.Write("ExtCtrls", "RCLocaleName", getLocale.Match(cbRCLocalesList.Text).Groups[1].Value);
			MMain.MyConfs.Write("ExtCtrls", "LCLocale", getLocale.Match(cbLCLocalesList.Text).Groups[2].Value);
			MMain.MyConfs.Write("ExtCtrls", "RCLocale", getLocale.Match(cbRCLocalesList.Text).Groups[2].Value);
			MMain.MyConfs.Write("ExtCtrls", "UseExtCtrls", cbUseLRC.Checked.ToString());
			MMain.MyConfs.Write("EnabledHotkeys", "HKSymIgnEnabled", cbSymIgn.Checked.ToString());
			MMain.MyConfs.Write("Functions", "MoreTries", cbMoreTries.Checked.ToString());
			MMain.MyConfs.Write("Functions", "TriesCount", nudMTCount.Value.ToString());
			MMain.MyConfs.Write("Functions", "DLRefreshRate", nudRefreshRate.Value.ToString());
			MMain.MyConfs.Write("Functions", "DisplayLang", cbDisplayLang.Checked.ToString());
			MMain.MyConfs.Write("Functions", "ExperimentalCSSwitch", cbExCSSwitch.Checked.ToString());
			MMain.MyConfs.Write("Functions", "Snippets", cbUseSnippets.Checked.ToString());
			MMain.MyConfs.Write("TTipUI", "Height", nudTTHeight.Value.ToString());
			MMain.MyConfs.Write("TTipUI", "Width", nudTTWidth.Value.ToString());
			MMain.MyConfs.Write("TTipUI", "Font", fcv.ConvertToString(btFont.Font));
			MMain.MyConfs.Write("TTipUI", "xpos", nudXpos.Value.ToString());
			MMain.MyConfs.Write("TTipUI", "ypos", nudYpos.Value.ToString());
			MMain.MyConfs.Write("TTipUI", "TransparentBack", cbTrBLT.Checked.ToString());
			MMain.MyConfs.Write("TTipUI", "DLForeColor", ColorTranslator.ToHtml(btCol1.BackColor));
			MMain.MyConfs.Write("TTipUI", "DLBackColor", ColorTranslator.ToHtml(btCol2.BackColor));
			MMain.MyConfs.Write("TTipUI", "CrtHeight", nudCrtTTHeight.Value.ToString());
			MMain.MyConfs.Write("TTipUI", "CrtWidth", nudCrtTTWidth.Value.ToString());
			MMain.MyConfs.Write("TTipUI", "CrtFont", fcv.ConvertToString(btCrtFont.Font));
			MMain.MyConfs.Write("TTipUI", "Crtxpos", nudCrtXpos.Value.ToString());
			MMain.MyConfs.Write("TTipUI", "Crtypos", nudCrtYpos.Value.ToString());
			MMain.MyConfs.Write("TTipUI", "CrtDLForeColor", ColorTranslator.ToHtml(btCrtCol1.BackColor));
			MMain.MyConfs.Write("TTipUI", "CrtDLBackColor", ColorTranslator.ToHtml(btCrtCol2.BackColor));
			MMain.MyConfs.Write("TTipUI", "L1DiffFGColor", ColorTranslator.ToHtml(btL1DifClrFGClr.BackColor));
			MMain.MyConfs.Write("TTipUI", "L1DiffBGColor", ColorTranslator.ToHtml(btL1DifClrBGClr.BackColor));
			MMain.MyConfs.Write("TTipUI", "L1DiffFont", fcv.ConvertToString(btL1DifClrFont.Font));
			MMain.MyConfs.Write("TTipUI", "L2DiffFGColor", ColorTranslator.ToHtml(btL2DifClrFGClr.BackColor));
			MMain.MyConfs.Write("TTipUI", "L2DiffBGColor", ColorTranslator.ToHtml(btL2DifClrBGClr.BackColor));
			MMain.MyConfs.Write("TTipUI", "L2DiffFont", fcv.ConvertToString(btL2DifClrFont.Font));
			MMain.MyConfs.Write("Functions", "CrtDisplayLang", cbCaretLangDisplay.Checked.ToString());
			MMain.MyConfs.Write("Functions", "DiffLayoutColors", cbDifLayoClr.Checked.ToString());
			MMain.MyConfs.Write("Functions", "CrtDLRefreshRate", nudCrtRefreshRate.Value.ToString());
			MMain.MyConfs.Write("Functions", "DTTOnChange", cbOnChange.Checked.ToString());
			MMain.MyConfs.Write("Functions", "ScrollTip", cbScrollLight.Checked.ToString());
			MMain.MyConfs.Write("Functions", "UpdatesEnabled", cbCheckForUPD.Checked.ToString());
			MMain.MyConfs.Write("Functions", "Logging", cbLogging.Checked.ToString());
			MMain.MyConfs.Write("DoubleKey", "Delay", nudDoubleDelay.Value.ToString());
			MMain.MyConfs.Write("DoubleKey", "Use", cbDoublePress.Checked.ToString());
			File.WriteAllText(snipfile, tbSnippets.Text);
			KMHook.ReInitSnippets();
			MMain.mahou.RefreshLangDisplays();
			if (!string.IsNullOrEmpty(tmpSIMods) && tmpSIKey != 0)
				MMain.MyConfs.Write("Hotkeys", "HKSymIgnMods", tmpSIMods);
			if (tmpSIKey != 0)
				MMain.MyConfs.Write("Hotkeys", "HKSymIgnKey", tmpSIKey.ToString());
			else
				MessageBox.Show(MMain.Msgs[6], MMain.Msgs[5], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			
			if (!string.IsNullOrEmpty(tempConMorWorMods) && tempConMorWorKey != 0)
				MMain.MyConfs.Write("Hotkeys", "HKConvertMoreMods", tempConMorWorMods);
			if (tempConMorWorKey != 0)
				MMain.MyConfs.Write("Hotkeys", "HKConvertMore", tempConMorWorKey.ToString());
			else
				MessageBox.Show(MMain.Msgs[6], MMain.Msgs[5], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			MMain.mahou.ICheck.Interval = Convert.ToInt32(nudRefreshRate.Value);
			MMain.mahou.res.Interval = Convert.ToInt32(nudRefreshRate.Value) * 7;
			MMain.mahou.crtCheck.Interval = Convert.ToInt32(nudCrtRefreshRate.Value);
			MMain.mahou.ToggleTimers();
			Logging.Log("More Configs configurations saved.");
			MMain.mahou.RefreshIconAll();
			MMain.mahou.InitializeHotkeys();

		}
		/// <summary>
		/// Loads configurations.
		/// </summary>
		void load()
		{
			MMain.mahou.IfNotExist();
			try {
				cbLCLocalesList.SelectedIndex = String.IsNullOrEmpty(MMain.MyConfs.Read("ExtCtrls", "LCLocaleName")) ? 0 
					: MMain.lcnmid.IndexOf(MMain.MyConfs.Read("ExtCtrls", "LCLocaleName") + "(" + MMain.MyConfs.Read("ExtCtrls", "LCLocale") + ")");
				cbRCLocalesList.SelectedIndex = String.IsNullOrEmpty(MMain.MyConfs.Read("ExtCtrls", "RCLocaleName")) ? 1 
					: MMain.lcnmid.IndexOf(MMain.MyConfs.Read("ExtCtrls", "RCLocaleName") + "(" + MMain.MyConfs.Read("ExtCtrls", "RCLocale") + ")");
			} catch {
				MessageBox.Show(MMain.Msgs[9], MMain.Msgs[5], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				RefreshLocales();
				cbLCLocalesList.SelectedIndex = 0;
				cbRCLocalesList.SelectedIndex = 1;
			}
			cbUseLRC.Checked = MMain.MyConfs.ReadBool("ExtCtrls", "UseExtCtrls");
			cbSymIgn.Checked = MMain.MyConfs.ReadBool("EnabledHotkeys", "HKSymIgnEnabled");
			cbMoreTries.Checked = MMain.MyConfs.ReadBool("Functions", "MoreTries");
			nudMTCount.Value = MMain.MyConfs.ReadInt("Functions", "TriesCount");
			nudRefreshRate.Value = MMain.MyConfs.ReadInt("Functions", "DLRefreshRate");
			cbDisplayLang.Checked = MMain.MyConfs.ReadBool("Functions", "DisplayLang");
			cbExCSSwitch.Checked = MMain.MyConfs.ReadBool("Functions", "ExperimentalCSSwitch");
			cbUseSnippets.Checked = MMain.MyConfs.ReadBool("Functions", "Snippets");
			nudTTHeight.Value = MMain.MyConfs.ReadInt("TTipUI", "Height");
			nudTTWidth.Value = MMain.MyConfs.ReadInt("TTipUI", "Width");
			btFont.Font = (Font)fcv.ConvertFromString(MMain.MyConfs.Read("TTipUI", "Font"));
			nudXpos.Value = MMain.MyConfs.ReadInt("TTipUI", "xpos");
			nudYpos.Value = MMain.MyConfs.ReadInt("TTipUI", "ypos");
			cbTrBLT.Checked = MMain.MyConfs.ReadBool("TTipUI", "TransparentBack");
			btCol1.BackColor = ColorTranslator.FromHtml(MMain.MyConfs.Read("TTipUI", "DLForeColor"));
			btCol2.BackColor = ColorTranslator.FromHtml(MMain.MyConfs.Read("TTipUI", "DLBackColor"));			
			nudCrtTTHeight.Value = MMain.MyConfs.ReadInt("TTipUI", "CrtHeight");
			nudCrtTTWidth.Value = MMain.MyConfs.ReadInt("TTipUI", "CrtWidth");
			btCrtFont.Font = (Font)fcv.ConvertFromString(MMain.MyConfs.Read("TTipUI", "CrtFont"));
			nudCrtXpos.Value = MMain.MyConfs.ReadInt("TTipUI", "Crtxpos");
			nudCrtYpos.Value = MMain.MyConfs.ReadInt("TTipUI", "Crtypos");
			btCrtCol1.BackColor = ColorTranslator.FromHtml(MMain.MyConfs.Read("TTipUI", "CrtDLForeColor"));
			btCrtCol2.BackColor = ColorTranslator.FromHtml(MMain.MyConfs.Read("TTipUI", "CrtDLBackColor"));
			btL1DifClrFGClr.BackColor = ColorTranslator.FromHtml(MMain.MyConfs.Read("TTipUI", "L1DiffFGColor"));
			btL1DifClrBGClr.BackColor = ColorTranslator.FromHtml(MMain.MyConfs.Read("TTipUI", "L1DiffBGColor"));
			btL1DifClrFont.Font = (Font)fcv.ConvertFromString(MMain.MyConfs.Read("TTipUI", "L1DiffFont"));
			btL2DifClrFGClr.BackColor = ColorTranslator.FromHtml(MMain.MyConfs.Read("TTipUI", "L2DiffFGColor"));
			btL2DifClrBGClr.BackColor = ColorTranslator.FromHtml(MMain.MyConfs.Read("TTipUI", "L2DiffBGColor"));
			btL2DifClrFont.Font = (Font)fcv.ConvertFromString(MMain.MyConfs.Read("TTipUI", "L2DiffFont"));
			cbCaretLangDisplay.Checked  = MMain.MyConfs.ReadBool("Functions", "CrtDisplayLang");
			cbDifLayoClr.Checked = MMain.MyConfs.ReadBool("Functions", "DiffLayoutColors");
			nudCrtRefreshRate.Value = MMain.MyConfs.ReadInt("Functions", "CrtDLRefreshRate");
			nudDoubleDelay.Value = MMain.MyConfs.ReadInt("DoubleKey", "Delay");
			cbDoublePress.Checked = MMain.MyConfs.ReadBool("DoubleKey", "Use");
			cbOnChange.Checked = MMain.MyConfs.ReadBool("Functions", "DTTOnChange");
			cbScrollLight.Checked = MMain.MyConfs.ReadBool("Functions", "ScrollTip");
			cbCheckForUPD.Checked = MMain.MyConfs.ReadBool("Functions", "UpdatesEnabled");
			cbLogging.Checked = MMain.MyConfs.ReadBool("Functions", "Logging");
			if (File.Exists(snipfile)) {
				tbSnippets.Text = File.ReadAllText(snipfile);
				KMHook.ReInitSnippets();
			}
			Logging.Log("More Configs configurations loaded.");
		}
		/// <summary>
		/// Restores temporary variables.
		/// </summary>
		void tmpRestore()
		{
			MMain.mahou.IfNotExist();
			tmpSIKey = MMain.MyConfs.ReadInt("Hotkeys", "HKSymIgnKey");
			tmpSIMods = MMain.MyConfs.Read("Hotkeys", "HKSymIgnMods");
			tempConMorWorKey = MMain.MyConfs.ReadInt("Hotkeys", "HKConvertMore");
			tempConMorWorMods = MMain.MyConfs.Read("Hotkeys", "HKConvertMoreMods");
		}
		/// <summary>
		/// Refreshes locales in comboboxes.
		/// </summary>
		void RefreshLocales() 
		{
			Locales.IfLessThan2();
			MMain.locales = Locales.AllList();
			cbLCLocalesList.Items.Clear();
			cbRCLocalesList.Items.Clear();
			MMain.lcnmid.Clear();
			foreach (Locales.Locale lc in MMain.locales) {
				cbLCLocalesList.Items.Add(lc.Lang + "(" + lc.uId + ")");
				cbRCLocalesList.Items.Add(lc.Lang + "(" + lc.uId + ")");
				MMain.lcnmid.Add(lc.Lang + "(" + lc.uId + ")");
			}
		}
		/// <summary>
		/// Refreshes controls text's language.
		/// </summary>
		void RefreshLanguage()
		{
			cbUseLRC.Text = MMain.UI[36];
			lbLCto.Text = MMain.UI[37];
			lbRCto.Text = MMain.UI[38];
			Text = MMain.UI[39];
			cbSymIgn.Text = MMain.UI[40];
			cbMoreTries.Text = MMain.UI[41];
			btnNO.Text = MMain.UI[19];
			cbDisplayLang.Text = MMain.UI[45];
			lblRefRate.Text = lblCrtRefRate.Text = MMain.UI[46];
			lbColors.Text = MMain.UI[47];
			btFont.Text = btL2DifClrFont.Text = btL1DifClrFont.Text = btCrtFont.Text = MMain.UI[52];
			lbSize.Text = MMain.UI[53];
			lbPosition.Text = MMain.UI[54];
			btnEMore.Text = !pEExtra.Visible ? MMain.UI[55] : MMain.UI[56];
			cbDoublePress.Text = MMain.UI[57];
			lbDDelay.Text = MMain.UI[58];
			cbExCSSwitch.Text = MMain.UI[59];
			cbTrBLT.Text = MMain.UI[60];
			cbUseSnippets.Text = MMain.UI[61];
			cbOnChange.Text = MMain.UI[62];
			cbScrollLight.Text 	= MMain.UI[63];
			cbCheckForUPD.Text = MMain.UI[64];
			lbConMorWor.Text = MMain.UI[65];
			cbLogging.Text = MMain.UI[66];
			btnDbgInf.Text = MMain.UI[67];
			lblLayout1DifClr.Text = MMain.UI[16] + " 1:";
			lblLayout2DifClr.Text = MMain.UI[16] + " 2:";
			cbCaretLangDisplay.Text = MMain.UI[69];
			cbDifLayoClr.Text = MMain.UI[70];
			Logging.Log("More Configs UI language refreshed.");
		}
		#endregion
		#region Tooltips
		void cbLCLocalesList_MouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbLCLocalesList.Text;
			HelpTT.Show(MMain.TTips[16], cbLCLocalesList);
		}
		void cbRCLocalesList_MouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbRCLocalesList.Text;
			HelpTT.Show(MMain.TTips[17], cbRCLocalesList);
		}
		void cbUseLRC_MouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbUseLRC.Text;
			HelpTT.Show(MMain.TTips[18], cbUseLRC);
		}
		void cbSymIgn_MouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbSymIgn.Text;
			HelpTT.Show(MMain.TTips[20], cbSymIgn);
		}
		void cbMoreTries_MouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbMoreTries.Text;
			HelpTT.Show(MMain.TTips[21], cbMoreTries);
		}
		void CbDisplayLangMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbDisplayLang.Text;
			HelpTT.Show(MMain.TTips[22], cbDisplayLang);
		}
		void LblRefRateMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = lblRefRate.Text;
			HelpTT.Show(MMain.TTips[23], lblRefRate);
		}
		void LbColorsMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = lbColors.Text;
			HelpTT.Show(MMain.TTips[24], lbColors);
	
		}
		void LbSizeMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = lbSize.Text;
			HelpTT.Show(MMain.TTips[25], lbSize);
		}
		void LbPositionMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = lbPosition.Text;
			HelpTT.Show(MMain.TTips[26], lbPosition);
		}
		void CbDoublePressMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbDoublePress.Text;
			HelpTT.Show(MMain.TTips[27], cbDoublePress);	
		}
		void LbDDelayMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = lbDDelay.Text;
			HelpTT.Show(MMain.TTips[28], lbDDelay);	
		}
		void CbExCSSwitchMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbExCSSwitch.Text;
			HelpTT.Show(MMain.TTips[29], cbExCSSwitch);	
		}
		void CbTrBLTMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbTrBLT.Text;
			HelpTT.Show(MMain.TTips[30], cbTrBLT);	
		}
		void CbUseSnippetsMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbUseSnippets.Text;
			HelpTT.Show(MMain.TTips[31], cbUseSnippets);	
		}
		void CbOnChangeMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbOnChange.Text;
			HelpTT.Show(MMain.TTips[32], cbOnChange);	
		}
		void CbScrollLightMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbScrollLight.Text;
			HelpTT.Show(MMain.TTips[33], cbScrollLight);	
		}
		void CbCheckForUPDMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbCheckForUPD.Text;
			HelpTT.Show(MMain.TTips[34], cbCheckForUPD);		
		}
		void LbConMorWorMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = lbConMorWor.Text;
			HelpTT.Show(MMain.TTips[35], lbConMorWor);		
		}
		void TbConMorWorMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = lbConMorWor.Text;
			HelpTT.Show(MMain.TTips[35], tbConMorWor);	
		}
		void CbLoggingMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbLogging.Text;
			HelpTT.Show(MMain.TTips[36], cbLogging);	
		}
		void CbCaretLangDisplayMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbCaretLangDisplay.Text;
			HelpTT.Show(MMain.TTips[37], cbCaretLangDisplay);
		}
		void CbDifLayoClrMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = cbDifLayoClr.Text;
			HelpTT.Show(MMain.TTips[38], cbDifLayoClr);
		}
		void LblCrtRefRateMouseHover(object sender, EventArgs e)
		{
			HelpTT.ToolTipTitle = lblCrtRefRate.Text;
			HelpTT.Show(MMain.TTips[39], lblCrtRefRate);
		}
		void btColForegroundMouseHover(object sender, EventArgs e)
		{
			var bt = sender as Button;
			HelpTT.ToolTipTitle = bt.BackColor.ToString().Replace("Color ", "").Replace("A=255, ","");
			HelpTT.Show(MMain.TTips[40], bt);
		}
		void btColBackgroundMouseHover(object sender, EventArgs e)
		{
			var bt = sender as Button;
			HelpTT.ToolTipTitle = bt.BackColor.ToString().Replace("Color ", "").Replace("A=255, ","");
			HelpTT.Show(MMain.TTips[41], bt);
		}
		#endregion
	}
}

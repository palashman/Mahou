using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Mahou
{
	static class KMHook  { // Keyboard & Mouse Listeners & Event hook		#region Variables
		public static string __ANY__ = "***ANY***", last_snip;
		public static bool win, alt, ctrl, shift,
			win_r, alt_r, ctrl_r, shift_r,
			shiftRP, ctrlRP, altRP, winRP, //RP = Re-Press
			awas, swas, cwas, wwas, afterEOS, afterEOL, //*was = alt/shift/ctrl was
			keyAfterCTRL, keyAfterALT, keyAfterSHIFT,
			clickAfterCTRL, clickAfterALT, clickAfterSHIFT,
			hotkeywithmodsfired, csdoing, incapt, waitfornum, 
			IsHotkey, ff_chr_wheeled, preSnip, LMB_down, RMB_down, MMB_down,
			dbl_click, click, selfie;
		public static System.Windows.Forms.Timer click_reset = new System.Windows.Forms.Timer();
		public static int skip_mouse_events, skip_spec_keys, cursormove = -1, guess_tries;
		static uint cs_layout_last = 0;
		static string lastClipText = "", busy_on = "";
		static List<Keys> tempNumpads = new List<Keys>();
		public static List<char> c_snip = new List<char>();
		public static System.Windows.Forms.Timer doublekey = new System.Windows.Forms.Timer();
		public static List<YuKey> c_word_backup = new List<YuKey>();
		public static List<YuKey> c_word_backup_last = new List<YuKey>();
		public static List<IntPtr> PLC_HWNDs = new List<IntPtr>();
		/// <summary> Created for faster check if program is excluded, when checkin too many times(in hooks, timers etc.). </summary>
		public static List<IntPtr> EXCLUDED_HWNDs = new List<IntPtr>(); 
		public static Stopwatch pif = new Stopwatch();
		public static List<IntPtr> NOT_EXCLUDED_HWNDs = new List<IntPtr>(); 
		public static List<IntPtr> ConHost_HWNDs = new List<IntPtr>();
		public static string[] snipps = new []{ "mahou", "eml" };
		public static string[] exps = new [] {
			"Mahou (魔法) - Magical layout switcher.",
			"BladeMight@gmail.com"
		};
		public static string[] as_wrongs;
		public static string[] as_corrects;
		static Dictionary<string, string> DefaultTransliterationDict = new Dictionary<string, string>() { 
				{"Щ", "SCH"}, {"щ", "sch"}, {"Ч", "CH"}, {"Ш", "SH"}, {"Ё", "JO"}, {"ВВ", "W"},
				{"Є", "EH"}, {"ю", "yu"}, {"я", "ya"}, {"є", "eh"}, {"Ж", "ZH"},
				{"ч", "ch"}, {"ш", "sh"}, {"Й", "JJ"}, {"ж", "zh"},
				{"Э", "EH"}, {"Ю", "YU"}, {"Я", "YA"}, {"й", "jj"}, {"ё", "jo"}, 
				{"э", "eh"}, {"вв", "w"}, {"кь", "q"}, {"КЬ", "Q"},
				{"ь", "j"}, {"№", "#"}, {"А", "A"}, {"Б", "B"},
				{"В", "V"}, {"Г", "G"}, {"Д", "D"}, {"Е", "E"}, {"З", "Z"}, 
				{"И", "I"}, {"К", "K"}, {"Л", "L"}, {"М", "M"}, {"Н", "N"},
				{"О", "O"}, {"П", "P"}, {"Р", "R"}, {"С", "S"}, {"Т", "T"},
				{"У", "U"}, {"Ф", "F"}, {"Х", "H"}, {"Ц", "C"}, {"Ъ", "'"}, 
				{"а", "a"}, {"б", "b"}, {"в", "v"}, {"г", "g"}, {"д", "d"},
				{"з", "z"}, {"и", "i"}, {"к", "k"}, {"л", "l"}, {"м", "m"},
				{"н", "n"}, {"о", "o"}, {"п", "p"}, {"р", "r"}, {"с", "s"}, 
				{"у", "u"}, {"ф", "f"}, {"х", "h"}, {"ц", "c"}, {"ъ", ":"},
				{"Ы", "Y"}, {"Ь", "J"}, {"е", "e"}, {"т", "t"}, {"ы", "y"}
		};
		static Dictionary<string, string> transliterationDict = new Dictionary<string, string>(DefaultTransliterationDict);
		#endregion
		#region Keyboard, Mouse & Event hooks callbacks
		public static void ListenKeyboard(int vkCode, uint MSG, short Flags = 0) {
			if (MMain.mahou.CaretLangTooltipEnabled)
				ff_chr_wheeled = false;
			if (vkCode > 254) return;
			var down = ((MSG == WinAPI.WM_SYSKEYDOWN) ? true : false) || ((MSG == WinAPI.WM_KEYDOWN) ? true : false);
			var Key = (Keys)vkCode; // "Key" will further be used instead of "(Keys)vkCode"
			if (MMain.c_words.Count == 0) {
				MMain.c_words.Add(new List<YuKey>());
			}
			if ((Key < Keys.D0 || Key > Keys.D9) && waitfornum && (uint)Key != MMain.mahou.HKConMorWor.VirtualKeyCode && down)
				MMain.mahou.FlushConvertMoreWords();
			#region Checks modifiers that are down
			switch (Key) {
				case Keys.LShiftKey:   shift = down; break;
				case Keys.LControlKey: ctrl = down; break;
				case Keys.LMenu:       alt = down; break;
				case Keys.LWin:        win = down; break;
				case Keys.RShiftKey:   shift_r = down; break;
				case Keys.RControlKey: ctrl_r = down; break;
				case Keys.RMenu:       alt_r = down; break;
				case Keys.RWin:        win_r = down; break;
			}
			// Additional fix for scroll tip.
			if (MMain.mahou.ScrollTip && Key == Keys.Scroll && down) {
				DoSelf(() => {
					KeybdEvent(Keys.Scroll, 0);
					KeybdEvent(Keys.Scroll, 2);
	              });
			}
			uint mods = 0;
			if (alt || alt_r)
				mods += WinAPI.MOD_ALT;
			if (ctrl || ctrl_r)
				mods += WinAPI.MOD_CONTROL;
			if (shift || shift_r)
				mods += WinAPI.MOD_SHIFT;
			if (win || win_r)
				mods += WinAPI.MOD_WIN;
			if (MMain.mahou.HasHotkey(new Hotkey(false, (uint)Key, mods, 77))) {
				IsHotkey = true;
			} else
				IsHotkey = false;
			Logging.Log("Pressed hotkey?: "+IsHotkey+" => ["+Key+"+"+mods+"] .");
			if ((Key >= Keys.D0 || Key <= Keys.D9) && waitfornum)
				IsHotkey = true;
			if (MahouUI.OnceSpecific && !down) {
				MahouUI.OnceSpecific = false;
			}
			var printable = ((Key >= Keys.D0 && Key <= Keys.Z) || // This is 0-9 & A-Z
			                 Key >= Keys.Oem1 && Key <= Keys.OemBackslash || // Other printable
							(Control.IsKeyLocked(Keys.NumLock) && ( // while numlock is on
						     Key >= Keys.NumPad0 && Key <= Keys.NumPad9)) || // Numpad numbers 
						     Key == Keys.Decimal || Key == Keys.Subtract || Key == Keys.Multiply ||
						     Key == Keys.Divide || Key == Keys.Add); // Numpad symbols
			var printable_mod = !win && !win_r && !alt && !alt_r && !ctrl && !ctrl_r; // E.g. only shift is PrintAble
			//Key log
			Logging.Log("Catched Key=[" + Key + "] with VKCode=[" + vkCode + "] and message=[" + (int)MSG + "], modifiers=[" + 
			            (shift ? "L-Shift" : "") + (shift_r ? "R-Shift" : "") + 
			            (alt ? "L-Alt" : "") + (alt_r ? "R-Alt" : "") + 
			            (ctrl ? "L-Ctrl" : "") + (ctrl_r ? "R-Ctrl" : "") + 
			            (win ? "L-Win" : "") + (win_r ? "R-Win" : "") + "].");
			// Anti C-A-DEL C & A stuck rule
			if (Key == Keys.Delete) {
				if (ctrl && alt)
					ctrl = alt = false;
				if (ctrl && alt_r)
					ctrl = alt_r = false;
				if (ctrl_r && alt_r)
					ctrl_r = alt_r = false;
				if (ctrl_r && alt)
					ctrl_r = alt = false;
			}
			// Anti win-stuck rule
			if (Key == Keys.L) {
				if (win)
					win = false;
				if (win_r)
					win_r = false;
			}
			// Clear currentLayout in MMain.mahou rule
			if (!MahouUI.UseJKL)
				if (((win || alt || ctrl || win_r || alt_r || ctrl_r) && Key == Keys.Tab) ||
				    win && (Key != Keys.None && 
				            Key != Keys.LWin && 
				            Key != Keys.RWin)) // On any Win+[AnyKey] hotkey
					MahouUI.currentLayout = 0;
			if (!down && (
			    ((alt || ctrl || alt_r || ctrl_r) && (Key == Keys.Shift || Key == Keys.LShiftKey || Key == Keys.RShiftKey)) ||
			     shift && (Key == Keys.Menu || Key == Keys.LMenu || Key == Keys.RMenu) ||
			     (Environment.OSVersion.Version.Major == 10 && (win || win_r) && Key == Keys.Space))) {
				if (!MahouUI.UseJKL) {
					var time = 200;
					if (Environment.OSVersion.Version.Major == 10)
						time = 50;
					MahouUI.currentLayout = 0;
					DoLater(() => { MahouUI.GlobalLayout = MahouUI.currentLayout = Locales.GetCurrentLocale(); }, time);
				}
			}
			#endregion
			#region
			var upper = false;
			if (MahouUI.LangPanelUpperArrow || MahouUI.mouseLTUpperArrow || MahouUI.caretLTUpperArrow)
				upper = IsUpperInput();
			if (MMain.mahou.LangPanelDisplay)
				if (MahouUI.LangPanelUpperArrow)
					MMain.mahou._langPanel.DisplayUpper(upper);
			if (MMain.mahou.MouseLangTooltipEnabled)
				if (MahouUI.mouseLTUpperArrow)
					MMain.mahou.mouseLangDisplay.DisplayUpper(upper);
			if (MMain.mahou.CaretLangTooltipEnabled)
				if (MahouUI.caretLTUpperArrow)
					MMain.mahou.caretLangDisplay.DisplayUpper(upper);
			#endregion
			#region InputHistory
			char sym = '\0';
			if (MahouUI.WriteInputHistory) {
				if ((printable || Key == Keys.Enter || Key == Keys.Space) && printable_mod && down) {
					sym = getSym(vkCode);
					WriteToHistory(sym);
				}
				if (Key == Keys.Back && printable_mod && down) {
					if (MahouUI.InputHistoryBackSpaceWriteType == 0) {
						WriteToHistory("<Back>");
					} else 
						RemLastHistory();
				}
			}
			#endregion
			#region Snippets
			if (MMain.mahou.SnippetsEnabled && !ExcludedProgram()) {
				if (printable && printable_mod && down) {
					if (sym == '\0')
						sym = getSym(vkCode);
					c_snip.Add(sym);
					Logging.Log("Added ["+ sym + "] to current snippet.");
					Debug.WriteLine("added " + sym);
				}
				var seKey = Keys.Space;
				if (MMain.mahou.SnippetsExpandType == "Tab")
					seKey = Keys.F14;
				if (Key == seKey || seKey == Keys.F14)
					preSnip = true;
				if (MSG == WinAPI.WM_KEYUP) {
					var snip = "";
					foreach (var ch in c_snip) {
						snip += ch;
//						Debug.WriteLine(ch);
					}
					var matched = false;
					Debug.WriteLine("Snip " + snip + ", last: " + last_snip);
					if (Key == seKey) {
		            	matched = CheckSnippet(snip);
		            	if (!matched)
		            		matched = CheckSnippet(last_snip+" "+snip);
						if (matched || preSnip)
							c_snip.Clear();
					}
					if (MahouUI.AutoSwitchEnabled && !matched && as_wrongs != null && Key == Keys.Space) {
		            	matched = CheckAutoSwitch(snip, c_word_backup);
		            	if (!matched) {
		            		var snip2x = last_snip+" "+snip;
		            		Debug.WriteLine("SNIp2x! " + snip2x);
		            		var SPace = new List<YuKey>(){ new YuKey() { key = Keys.Space, altnum = false, upper = false } };
		            		var dash = new List<YuKey>(){ new YuKey() { key = Keys.OemMinus, altnum = false, upper = false } };
		            		var last2words = c_word_backup_last.Concat(dash)
		            			.Concat(c_word_backup).ToList();
		            		CheckAutoSwitch(snip2x, last2words);
		            	}
						c_snip.Clear();
					}
					if (Key == seKey) {
						last_snip = snip;
					}
				}
			}
			#endregion
			#region Release Re-Pressed keys
			if (hotkeywithmodsfired && !down &&
			   ((Key == Keys.LShiftKey || Key == Keys.LMenu || Key == Keys.LControlKey || Key == Keys.LWin) ||
			     (Key == Keys.RShiftKey || Key == Keys.RMenu || Key == Keys.RControlKey || Key == Keys.RWin))) {
				hotkeywithmodsfired = false;
				mods = 0;
				if (cwas) {
					cwas = false;
					mods += WinAPI.MOD_CONTROL;
				}
				if (swas) {
					swas = false;
					mods += WinAPI.MOD_SHIFT;
				}
				if (awas) {
					awas = false;
					mods += WinAPI.MOD_ALT;
				}
				if (wwas) {
					wwas = false;
					mods += WinAPI.MOD_WIN;
				}
				SendModsUp((int)mods);
			}
			#endregion
			#region One key layout switch
			if (!down)
				if (Key == Keys.LControlKey || Key == Keys.RControlKey)
					clickAfterCTRL = false;
				if (Key != Keys.LMenu && Key != Keys.RMenu)
					clickAfterALT = false;
				if (Key != Keys.LShiftKey && Key != Keys.RShiftKey)
					clickAfterSHIFT = false;
			if (MMain.mahou.ChangeLayouByKey) {
					if (((Key == Keys.LControlKey || Key == Keys.RControlKey) && !MahouUI.CtrlInHotkey) ||
					    ((Key == Keys.LShiftKey || Key == Keys.RShiftKey) && !MahouUI.ShiftInHotkey) ||
					    ((Key == Keys.LMenu || Key == Keys.RMenu) && !MahouUI.AltInHotkey) ||
					    ((Key == Keys.LWin || Key == Keys.RWin) && !MahouUI.WinInHotkey) ||
					    Key == Keys.CapsLock || Key == Keys.F18 || vkCode == 240 || Key == Keys.Tab) {
						SpecificKey(Key, MSG, vkCode);
				}
				if ((ctrl || ctrl_r) && (Key != Keys.LControlKey && Key != Keys.RControlKey && Key != Keys.ControlKey || clickAfterCTRL))
					keyAfterCTRL = true;
				else 
					keyAfterCTRL = false;
				if ((alt || alt_r) && (Key != Keys.LMenu && Key != Keys.RMenu && Key != Keys.Menu || clickAfterALT))
					keyAfterALT = true;
				else 
					keyAfterALT = false;
				if ((shift || shift_r) && (Key != Keys.LShiftKey && Key != Keys.RShiftKey && Key != Keys.Shift || clickAfterSHIFT))
					keyAfterSHIFT = true;
				else 
					keyAfterSHIFT = false;
			}
			#endregion
			if ((ctrl||win||alt||ctrl_r||win_r||alt_r) && Key == Keys.Tab) {
				ClearWord(true, true, true, "Any modifier + Tab");
			}
			#region Other, when KeyDown
			if (MSG == WinAPI.WM_KEYDOWN && !waitfornum && !IsHotkey) {
				if (Key == Keys.Back) { //Removes last item from current word when user press Backspace
					if (MMain.c_word.Count != 0) {
						MMain.c_word.RemoveAt(MMain.c_word.Count - 1);
					}
					if (MMain.c_words.Count > 0) {
						if (MMain.c_words[MMain.c_words.Count - 1].Count - 1 > 0) {
							Logging.Log("Removed key [" + MMain.c_words[MMain.c_words.Count - 1][MMain.c_words[MMain.c_words.Count - 1].Count - 1].key + "] from last word in words.");
							MMain.c_words[MMain.c_words.Count - 1].RemoveAt(MMain.c_words[MMain.c_words.Count - 1].Count - 1);
						} else {
							Logging.Log("Removed one empty word from current words.");
							MMain.c_words.RemoveAt(MMain.c_words.Count - 1);
						}
					}
					if (MMain.mahou.SnippetsEnabled) {
						if (c_snip.Count != 0) {
							c_snip.RemoveAt(c_snip.Count - 1);
							Logging.Log("Removed one character from current snippet.");
						}
					}
				}
				//Pressing any of these Keys will empty current word, and snippet
				if (Key == Keys.Home || Key == Keys.End ||
				    (Key == Keys.Tab && MMain.mahou.SnippetsExpandType != "Tab" && snipps.Length > 0) || Key == Keys.PageDown || Key == Keys.PageUp ||
				   Key == Keys.Left || Key == Keys.Right || Key == Keys.Down || Key == Keys.Up ||
				   Key == Keys.BrowserSearch || 
				   ((ctrl||win||alt||ctrl_r||win_r||alt_r) && (Key != Keys.Menu  && //Ctrl modifier and key which is not modifier
							Key != Keys.LMenu &&
							Key != Keys.RMenu &&
							Key != Keys.LWin &&
							Key != Keys.ShiftKey &&
							Key != Keys.RShiftKey &&
							Key != Keys.LShiftKey &&
							Key != Keys.RWin &&
							Key != Keys.ControlKey &&
							Key != Keys.LControlKey &&
							Key != Keys.RControlKey ))) { 
					ClearWord(true, true, true, "Pressed combination of key and modifiers(not shift) or key that changes caret position.");
				}
				if (Key == Keys.Space) {
					Logging.Log("Adding one new empty word to words, and adding to it [Space] key.");
					MMain.c_words.Add(new List<YuKey>());
					MMain.c_words[MMain.c_words.Count - 1].Add(new YuKey() { key = Keys.Space });
					if (MMain.mahou.AddOneSpace && MMain.c_word.Count != 0 &&
					   MMain.c_word[MMain.c_word.Count - 1].key != Keys.Space) {
						Logging.Log("Eat one space passed, next space will clear last word.");
						MMain.c_word.Add(new YuKey() { key = Keys.Space });
						afterEOS = true;
					} else {
						ClearWord(true, false, false, "Pressed space");
						afterEOS = false;
					}
				}
				if (Key == Keys.Enter) {
					if (MMain.mahou.Add1NL && MMain.c_word.Count != 0 && 
					    MMain.c_word[MMain.c_word.Count - 1].key != Keys.Enter) {
						Logging.Log("Eat one New Line passed, next Enter will clear last word.");
						MMain.c_word.Add(new YuKey() { key = Keys.Enter });
						MMain.c_words[MMain.c_words.Count - 1].Add(new YuKey() { key = Keys.Enter });
						afterEOL = true;
					} else {
						ClearWord(true, true, true, "Pressed enter");
						afterEOL = false;
					}
				}
				if (printable && printable_mod) {
					if (afterEOS) { //Clears word after Eat ONE space
						ClearWord(true, false, false, "Clear last word after 1 space");
						afterEOS = false;
					}
					if (afterEOL) { //Clears word after Eat ONE enter
						ClearWord(true, false, false, "Clear last word after 1 enter");
						afterEOL = false;
					}
					var upr = false;
					if (shift || shift_r || Control.IsKeyLocked(Keys.CapsLock)) {
						upr = true;
					}
					MMain.c_word.Add(new YuKey() {
						key = Key,
						upper = upr
					});
					MMain.c_words[MMain.c_words.Count - 1].Add(new YuKey() {
						key = Key,
						upper = upr
					});
					Logging.Log("Added [" + Key + "]^"+upr);
				}
			}
			#endregion
			#region Alt+Numpad (fully workable)
			if (incapt &&
			   (Key == Keys.RMenu || Key == Keys.LMenu || Key == Keys.Menu) && !down) {
				Logging.Log("Capture of numpads ended, captured [" + tempNumpads.Count + "] numpads.");
				if (tempNumpads.Count > 0) { // Prevents zero numpads(alt only) keys
					MMain.c_word.Add(new YuKey() {
						altnum = true,
						numpads = new List<Keys>(tempNumpads)//new List => VERY important here!!!
					});                                      //It prevents pointer to tempNumpads, which is cleared.
					MMain.c_words[MMain.c_words.Count - 1].Add(new YuKey() {
						altnum = true,
						numpads = new List<Keys>(tempNumpads)
					});
				}
				tempNumpads.Clear();
				incapt = false;
			}
			if (!incapt && (alt || alt_r) && down) {
				Logging.Log("Alt is down, starting capture of Numpads...");
				incapt = true;
			}
			if ((alt || alt_r) && incapt) {
				if (Key >= Keys.NumPad0 && Key <= Keys.NumPad9 && !down) {
					tempNumpads.Add(Key);
				}
			}
			#endregion
			#region Reset Modifiers in Hotkeys
			MahouUI.ShiftInHotkey = MahouUI.AltInHotkey = MahouUI.WinInHotkey = MahouUI.CtrlInHotkey = false;
			#endregion
			preSnip = false;
			#region Update LD
			MMain.mahou.UpdateLDs();
			#endregion
		}
		public static void ListenMouse(ushort MSG) {
			if ((MSG == (ushort)WinAPI.RawMouseButtons.MouseWheel)) {
				if (MMain.mahou.caretLangDisplay.Visible && MMain.mahou.CaretLangTooltipEnabled) {
					var _fw = WinAPI.GetForegroundWindow();
					var _clsNMb = new StringBuilder(40);
					WinAPI.GetClassName(_fw, _clsNMb, _clsNMb.Capacity);
					var clsNM = _clsNMb.ToString();
					if (clsNM == "MozillaWindowClass" || clsNM.Contains("mozilla") || clsNM.Contains("Chrome_WidgetWin"))
						ff_chr_wheeled = true;
				}
			}
			if (MSG == (ushort)WinAPI.RawMouseButtons.LeftDown || MSG == (ushort)WinAPI.RawMouseButtons.RightDown) {
				if (ctrl || ctrl_r)
					clickAfterCTRL = true;
				if (shift || shift_r)
					clickAfterSHIFT = true;
				if (alt || alt_r)
					clickAfterALT = true;
				if (!MahouUI.UseJKL)
					MahouUI.currentLayout = 0;
				ClearWord(true, true, true, "Mouse click");
			}
			#region Double click show translate
			if (MahouUI.TrEnabled)
				if (MahouUI.TrOnDoubleClick) {
					if (MSG == (ushort)WinAPI.RawMouseButtons.LeftUp || MSG == (ushort)WinAPI.RawMouseButtons.RightUp) {
						if (dbl_click) {
							Debug.WriteLine("DBL");
							MahouUI.ShowSelectionTranslation(true);
							dbl_click = click = false;
						}
					}
					if (MSG == (ushort)WinAPI.RawMouseButtons.LeftDown || MSG == (ushort)WinAPI.RawMouseButtons.RightDown) {
						if (!click) {
							pif.Start();
							click = true;
							click_reset.Interval = SystemInformation.DoubleClickTime;
							click_reset.Tick += (_, __) => {
								click = false;
								Debug.WriteLine("Slow second click!");
								click_reset.Stop();
								click_reset.Dispose();
								click_reset = new System.Windows.Forms.Timer();
							};
							click_reset.Start();
							Debug.WriteLine("First click, reset after: " + SystemInformation.DoubleClickTime);
						} else {
							var el = pif.ElapsedMilliseconds;
							pif.Reset();
							if (el <= 5) {
								Debug.WriteLine("Too fast ["+el+"ms], probably buggy...");
								click_reset.Stop();
								click_reset.Dispose();
								click_reset = new System.Windows.Forms.Timer();
								click = false;
							} else {
								Debug.WriteLine("Second click, after: [" + el + "ms] + kill reset + waiting to Up button");
								click_reset.Stop();
								click_reset.Dispose();
								click_reset = new System.Windows.Forms.Timer();
								dbl_click = true;
								click = false;
							}
						}
					}
				}
			#endregion
			if (MMain.mahou.LDUseWindowsMessages) {
				if (MSG == (ushort)WinAPI.RawMouseButtons.LeftDown)
					LMB_down = true;
				else if (MSG == (ushort)WinAPI.RawMouseButtons.LeftUp)
					LMB_down = false;
				if (MSG == (ushort)WinAPI.RawMouseButtons.RightDown)
					RMB_down = true;
				else if (MSG == (ushort)WinAPI.RawMouseButtons.RightUp)
					RMB_down = false; 
				if (MSG == (ushort)WinAPI.RawMouseButtons.MiddleDown)
					MMB_down = true;
				else if (MSG == (ushort)WinAPI.RawMouseButtons.MiddleUp)
					MMB_down = false;
				if (MSG == (ushort)WinAPI.RawMouseButtons.MouseWheel ||
					MSG == (ushort)WinAPI.RawMouseButtons.LeftUp ||
					MSG == (ushort)WinAPI.RawMouseButtons.RightUp ||
					MSG == (ushort)WinAPI.RawMouseButtons.MiddleUp) {
					if (MMain.mahou.LDForCaret) {
						MMain.mahou.UpdateCaredLD();
					}
				}
				if (MSG == (ushort)WinAPI.RawMouseButtons.LeftUp ||
					MSG == (ushort)WinAPI.RawMouseButtons.RightUp ||
					MSG == (ushort)WinAPI.RawMouseButtons.MiddleUp)
					if (MMain.mahou.CaretLangTooltipEnabled)
						ff_chr_wheeled = false;
				if (skip_mouse_events-- == 0 || skip_mouse_events == 0) {
					skip_mouse_events = MahouUI.LD_MouseSkipMessagesCount;
					if (MSG == (ushort)WinAPI.RawMouseFlags.MoveRelative) {
						if (MMain.mahou.LDForMouse) {
							MMain.mahou.UpdateMouseLD();
						}
						if ((LMB_down || RMB_down || MMB_down)) {
							if (MMain.mahou.LDForCaret) {
								MMain.mahou.UpdateCaredLD();
							}
						}
					}
				}
			}
		}
		public static void LDEventHook(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject,
		                                     int idChild, uint dwEventThread, uint dwmsEventTime) {
			if (MMain.mahou.LDUseWindowsMessages) {
				if (eventType == WinAPI.EVENT_OBJECT_FOCUS) {
					if (MMain.mahou != null)
						MMain.mahou.UpdateLDs();
				}
			}
		}
		public static void EventHookCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject,
		                                       int idChild, uint dwEventThread, uint dwmsEventTime) {
			if (MMain.mahou.PersistentLayoutOnWindowChange) {
				var proc = Locales.ActiveWindowProcess();
				var cont = PLC_HWNDs.Contains(hwnd);
				if (!cont || !MMain.mahou.PersistentLayoutOnlyOnce) {
					if (MMain.mahou.PersistentLayoutForLayout1)
						MMain.mahou.PersistentLayoutCheck(MMain.mahou.PersistentLayout1Processes, MahouUI.MAIN_LAYOUT1, proc.ProcessName);
					if (MMain.mahou.PersistentLayoutForLayout2)
						MMain.mahou.PersistentLayoutCheck(MMain.mahou.PersistentLayout2Processes, MahouUI.MAIN_LAYOUT2, proc.ProcessName);
				}
				if (MMain.mahou.PersistentLayoutOnlyOnce && !cont)
					PLC_HWNDs.Add(hwnd);
			}
			if (MahouUI.UseJKL) {
				if (ConHost_HWNDs.Contains(hwnd)) {
					Logging.Log("[JKL] > Known ConHost window: " + hwnd);
					jklXHidServ.CycleAllLayouts(hwnd);
				} else {
					var strb = new StringBuilder(350);
					WinAPI.GetClassName(hwnd, strb, strb.Capacity);
					if (strb.ToString() == "ConsoleWindowClass" || strb.ToString() == "Chrome_WidgetWin_1") {
						Logging.Log("[JKL] > ["+hwnd+"] = ConHost window, remembering...");
						ConHost_HWNDs.Add(hwnd);
						jklXHidServ.CycleAllLayouts(hwnd);
					} else {
						MahouUI.currentLayout = /*MahouUI.GlobalLayout =*/ Locales.GetCurrentLocale();
						Logging.Log("[JKL] > Updating currentLayout on window activate to ["+MahouUI.currentLayout+"]...");
					}
				}
			}
			uint hwndLayout = Locales.GetCurrentLocale(hwnd);
			Logging.Log("Hwnd " + hwnd + ", layout: " + hwndLayout + ", Mahou layout: " + MahouUI.GlobalLayout);		
			if (MMain.mahou.OneLayout)
				if (hwndLayout != MahouUI.GlobalLayout) {
					var title = new StringBuilder(128);
					WinAPI.GetWindowText(hwnd, title, 127);
					DoLater(() => {
						Logging.Log("Layout in this window ["+title+"] was different, changing layout to Mahou global layout.");
						ChangeToLayout(hwnd, MahouUI.GlobalLayout);
			       	 }, 100);
				}
		}
		#endregion
		#region Functions/Struct
		static bool CheckAutoSwitch(string snip, List<YuKey> word) {
			var matched = false;
			var snil = snip.ToLowerInvariant();
			foreach (var element in word) {
				Debug.WriteLine(element.key);
			}
			for (int i = 0; i < as_wrongs.Length; i++) {
				if (as_corrects.Length > i) {
//					if (snip == as_wrongs[i]) {
//						ExpandSnippet(snip, as_corrects[i], MMain.mahou.AutoSwitchSpaceAfter, MMain.mahou.AutoSwitchSwitchToGuessLayout);
//						break;
//					} else {
	    			if (as_wrongs[i] == null)
	    				break;
						if (snip.Length == as_wrongs[i].Length) {
							if (snil == as_wrongs[i].ToLowerInvariant()) {
	        					if (MahouUI.SoundOnAutoSwitch)
	        						MahouUI.SoundPlay();
	        					if (MahouUI.SoundOnAutoSwitch2)
	        						MahouUI.Sound2Play();
	        					SendBack();
	        					var snl = WordGuessLayout(snil).Item2;
	        					var asl = WordGuessLayout(as_corrects[i]).Item2;
	        					var skipLS = (snl == asl);
	        					Debug.WriteLine("snl: " +snil + ", l:" +snl + "\nas_crI: " + as_corrects[i] + ", l: " +asl + "\nSKIP: " +skipLS);
	        					var ofk = false;
	        					if (!skipLS) {
	        						ChangeToLayout(Locales.ActiveWindow(), asl);
	        						if (MahouUI.UseJKL && MMain.mahou.SwitchBetweenLayouts && MMain.mahou.EmulateLS) {
										jklXHidServ.OnLayoutAction = asl;
	        							jklXHidServ.ActionOnLayout = () => {
				        					StartConvertWord(word.ToArray(), Locales.GetCurrentLocale(), true);
											ExpandSnippet(snip, as_corrects[i], MMain.mahou.AutoSwitchSpaceAfter, MMain.mahou.AutoSwitchSwitchToGuessLayout, true);
										};
	        						} else ofk = true;
	        					} else ofk = true;
	        					if (ofk) {
		        					StartConvertWord(word.ToArray(), Locales.GetCurrentLocale(), true);
									ExpandSnippet(snip, as_corrects[i], MMain.mahou.AutoSwitchSpaceAfter, MMain.mahou.AutoSwitchSwitchToGuessLayout, true);
	        					}
								matched = true;
								break;
							}
						}
//					}
				} else {
					Logging.Log("Auto-switch word ["+snip+"] has no expansion, snippet is not finished or its expansion commented.", 1);
				}
			}
			return matched;
		}
		static void SendBack() {
			KInputs.MakeInput(new [] {
                   		KInputs.AddKey(Keys.Back, true),
                   		KInputs.AddKey(Keys.Back, false)});
		}
		static bool CheckSnippet(string snip) {
			var matched = false;
			Logging.Log("Current snippet is [" + snip + "].");
			for (int i = 0; i < snipps.Length; i++) {
				if (snipps[i] == null) break;
				if (snipps[i].Contains(__ANY__)) {
					var any = "";
					var pins = snipps[i];
					var len = pins.Length;
					var at = pins.IndexOf(__ANY__, StringComparison.InvariantCulture);
					var aft = at+__ANY__.Length;
//					Debug.WriteLine("aftst:"+pins[aft]);
					var laf = len-aft;
					if (snip.Length < laf+at) {
						Debug.WriteLine("Too small snip, to use with "+__ANY__);
						continue;
					}
//					Debug.WriteLine("at:"+at+",aft:"+aft+",laf:"+laf);
					bool yay = true;
					if (at <= snip.Length)
						for (int f = 0; f != at; f++) {
							if (snip[f] != pins[f]) yay = false;
						}
					for (int f = 0; f != laf; f++) {
						var t = f + (pins.Length-laf);
						var g = f + (snip.Length-laf);
//						Debug.WriteLine("Calc: " + g + ", " + t +  ", " + at + ", " + laf);
						if (g > snip.Length || g < 0) continue;
//						Debug.WriteLine("Cht: " + snip[g] + ", " + pins[t]);
						if (snip[g] != pins[t]) yay = false;
					}
					if (yay) {
    					if (MahouUI.SoundOnSnippets)
    						MahouUI.SoundPlay();
    					if (MahouUI.SoundOnSnippets2)
    						MahouUI.Sound2Play();
						any = snip.Substring(at, (snip.Length-laf-at));
//						Debug.WriteLine("Yay!" + any);
						Logging.Log("Current snippet [" + snip + "] matched with "+__ANY__+" existing snippet [" + exps[i] + "].");
						var exp = exps[i].Replace(__ANY__, any);
//						Debug.WriteLine("exp: " + exp);
						ExpandSnippet(snip, exp, MMain.mahou.SnippetSpaceAfter, MMain.mahou.SnippetsSwitchToGuessLayout);
						break;
					}
//		    		Debug.WriteLine("ANY " + yay);
			    }
				if (snip == snipps[i]) {
					if (exps.Length > i) {
    					if (MahouUI.SoundOnSnippets)
    						MahouUI.SoundPlay();
    					if (MahouUI.SoundOnSnippets2)
    						MahouUI.Sound2Play();
						Logging.Log("Current snippet [" + snip + "] matched existing snippet [" + exps[i] + "].");
						ExpandSnippet(snip, exps[i], MMain.mahou.SnippetSpaceAfter, MMain.mahou.SnippetsSwitchToGuessLayout);
						matched = true;
					} else {
						Logging.Log("Snippet ["+snip+"] has no expansion, snippet is not finished or its expansion commented.", 1);
					}
					break;
				}
			}
			return matched;
		}
		static void RemLastHistory() {
			var txt = System.IO.File.ReadAllText(System.IO.Path.Combine(MahouUI.nPath, "history.txt"));
			if (txt.Length<1) return;
			txt = txt.Substring(0, txt.Length-1);
			System.IO.File.WriteAllText(System.IO.Path.Combine(MahouUI.nPath, "history.txt"), txt);
		}
		static void WriteToHistory(char c) {
			var sw = System.IO.File.AppendText(System.IO.Path.Combine(MahouUI.nPath, "history.txt"));
			sw.Write(c);
			sw.Close();
		}
		static void WriteToHistory(string s) {
			var sw = System.IO.File.AppendText(System.IO.Path.Combine(MahouUI.nPath, "history.txt"));
			sw.Write(s);
			sw.Close();
		}
		static char getSym(int vkCode) {
			var stb = new StringBuilder(10);
			var byt = new byte[256];
			if (shift || shift_r || Control.IsKeyLocked(Keys.CapsLock)) {
				byt[(int)Keys.ShiftKey] = 0xFF;
			}
			uint layout = Locales.GetCurrentLocale() & 0xffff;
			if (MahouUI.UseJKL) {
				if (layout != (MahouUI.currentLayout & 0xffff)) {
					if (IsConhost())
						layout = MahouUI.currentLayout & 0xffff;
				}
			}
			WinAPI.ToUnicodeEx((uint)vkCode, (uint)vkCode, byt, stb, stb.Capacity, 0, (IntPtr)layout);
			if (stb.Length > 0) {
				var c = stb.ToString()[0];
				return c;	
			}
			return '\0';
		}
		public static void ReloadTSDict() {
			var tsdict = new Dictionary<string, string>();
			var tsdictp = System.IO.Path.Combine(MahouUI.nPath, "TSDict.txt");
			if (System.IO.File.Exists(tsdictp)) {
				var lines = System.IO.File.ReadAllLines(tsdictp);
				for (int i = 0; i != lines.Length; i++) {
					var line = lines[i];
					if (line.Contains("|")) {
				    	var lr = line.Split('|');
				    	tsdict[lr[0]] = lr[1];
//				    	Debug.WriteLine("Added to TSDict: " +lr[0] +" <=> " + lr[1]);
					} else {
						Logging.Log("Wrong Transliteration Dict line #"+i+", => " +line);
				    	tsdict = null;
				    	break;
					}
				}
			} else {
				var raw = "";
				foreach (var kv in DefaultTransliterationDict) {
					raw += kv.Key+"|"+kv.Value+"\r\n";
				}
				System.IO.File.WriteAllText(tsdictp, raw);
			}
			if (tsdict != null && tsdict.Count != 0) {
				Logging.Log("Succesfully initialized Transliteration Dictionary from ["+tsdictp+"].");
				transliterationDict = tsdict;
			} else {
				Logging.Log(tsdictp+" missing or wrong syntax reset to default transliteration Dictionary.");
				transliterationDict = DefaultTransliterationDict;
			}
		}
		static void ExpandSnippet(string snip, string expand, bool spaceAft, bool switchLayout, bool ignoreExpand = false) {
			DoSelf(() => {
				try {
		       		Debug.WriteLine("Snippet: " +snip);
					if (switchLayout) {
						var guess = WordGuessLayout(expand);
						Logging.Log("Changing to guess layout [" + guess.Item2 + "] after snippet ["+ guess.Item1 + "].");
						ChangeToLayout(Locales.ActiveWindow(), guess.Item2);
					}
					if (!ignoreExpand) {
       					for (int e = -1 + (MMain.mahou.SnippetsExpandType == "Tab" ? 1 : 0); e < snip.Length; e++) {
		       				SendBack();
						}
						Logging.Log("Expanding snippet [" + snip + "] to [" + expand + "].");
		       			ExpandSnippetWithExpressions(expand);
						ClearWord(true, true, false, "Cleared due to snippet expansion");
//						KInputs.MakeInput(KInputs.AddString(expand));
					}
					if (spaceAft)
						KInputs.MakeInput(KInputs.AddString(" "));
					DoLater(() => MMain.mahou.Invoke((MethodInvoker)delegate {
						MMain.mahou.UpdateLDs();
					}), 500);
		       	} catch(Exception e) {
					Logging.Log("Some snippets configured wrong, check them, error:\r\n" + e.Message +"\r\n" + e.StackTrace+"\r\n", 1);
					// If not use TASK, form(MessageBox) won't accept the keys(Enter/Escape/Alt+F4).
					var msg = new [] {"", ""};
					msg[0] = MMain.Lang[Languages.Element.MSG_SnippetsError];
					msg[1] = MMain.Lang[Languages.Element.Error];
					var tsk = new System.Threading.Tasks.Task(() => MessageBox.Show(msg[0], msg[1], MessageBoxButtons.OK, MessageBoxIcon.Error));
					tsk.Start();
					KInputs.MakeInput(KInputs.AddString(snip));
				}
              });
		}
		#region in Snippets expressions  
		static readonly string[] expressions = new []{ "__date", "__time", "__version", "__system", "__title", "__keyboard", "__execute", "__cursorhere", "__paste", "__mahouhome" };
		#endregion
		static void ExpandSnippetWithExpressions(string expand) {
			string ex = "", args = "", raw = "", err = "";
			bool args_getting = false, is_expr = false, escaped = false;
			int expr_start = -1;
			bool contains_expr = false;
			foreach (var expr in expressions) {
				if (expand.Contains(expr)) {
					contains_expr = true;
					break;
				}
			}
			if (!contains_expr) {
				KInputs.MakeInput(KInputs.AddString(expand));
				return;
			}
			for (int i = 0; i!=expand.Length; i++) {
				var args_get = false;
				var e = expand[i]; 
//				Debug.WriteLine("i:"+i+", e:"+e);
				if (!is_expr)
					ex += e;
				else err+=e;
				if (is_expr && e == ')') { // Escape closing
					if (expand[i-1] == '\\') {
						Logging.Log("Escaped \")\" at position: "+i);
						if (args.Length >2)
							args = args.Substring(0, args.Length-1);
					} else {
						if (args_getting) {
							args_getting = false;
							args_get = true;
	//						Debug.WriteLine("end of args of: " + fun + " -> " +i);
						} else {
							Logging.Log("Expression \"(\" missing, but \")\" were there, in ["+ex+"], at position: "+expr_start+" in ["+expand+"]");
							KInputs.MakeInput(KInputs.AddString(ex+err));
							is_expr = false;
							args_get = false;
							escaped = false;
							args = ex = raw = "";
						}
					}
				}
				if (args_getting)
					args += e;
				if (is_expr && e == '(' && !args_getting) {
					args_getting = true; 
//					Debug.WriteLine("start of args of: " + fun + " -> " +i);
				}
				var maybe_fun = false;
				if (!args_getting && !string.IsNullOrEmpty(ex) && !is_expr) {
					foreach (var expr in expressions) {
						if (expr.StartsWith(ex, StringComparison.InvariantCulture)) {
							maybe_fun = true;
				    		if (expr == ex) {
								expr_start = i - (ex.Length-1);
								escaped = false;
								if (expr_start-1<0)
									escaped = false;
								else if (expand[expr_start-1] == '\\')
									escaped = true;
								is_expr = !escaped;
//								Debug.WriteLine("expr: " +expr+" equals " + ex + ", expr_start: " + expr_start + " is_expr: " + is_expr);
								err = "";
								break;
				    		}
						} else
							maybe_fun = false;
//						Debug.WriteLine("Try: " +fun+" > " + expr + (maybe_fun ? " OK" : " NO"));
						if (maybe_fun) break;
					}
				}
				if (is_expr && i == expand.Length-1 && !args_get) {
					Logging.Log("Expression [" + ex +"] missing its end \")\", at positon: " + expr_start +" in: [" + expand + "].", 2);
					KInputs.MakeInput(KInputs.AddString(ex+err+args));
					err = "";
				}
				if (args_get && !escaped) {
					Logging.Log("Executing expression: " + ex + " with args: [" + args + "]");
					var curlefts = expand.Length - i -1;
					ExecExpression(ex, args, curlefts);
					is_expr = false;
					args_get = false;
					args = ex = "";
				}
				if (!args_getting && !maybe_fun && !is_expr) {
					if (!escaped) {
//						Debug.WriteLine("Not even start of any expression: " + ex);
						raw += ex;
					}
					ex = "";
					maybe_fun = false;
					is_expr = false;
					expr_start = -1;
				}
				if (!string.IsNullOrEmpty(raw)) {
//					Debug.WriteLine("Inputting raw: ["+raw+"]");
					KInputs.MakeInput(KInputs.AddString(raw));
					raw = "";
				}
				if (escaped) {
					Logging.Log("Ignored espaced expression: " + ex);
					SendBack();
					KInputs.MakeInput(KInputs.AddString(ex));
					is_expr = false;
					args_get = false;
					escaped = false;
					args = ex = raw = "";
				}
			}
			if (cursormove != -1) {
	        	for (int i=0; i != cursormove; i++) {
	        		KInputs.MakeInput(new []{ KInputs.AddKey(Keys.Left, true), 
	        		                  	KInputs.AddKey(Keys.Left, false)
	        		                  });
	        	}
			}
			cursormove = -1;
				
		}
		static void ExecExpression(string expr, string args, int curlefts = -1) {
			switch (expr) {
				case "__paste":
					Logging.Log("Pasting text from snippet.");
					Debug.WriteLine("Paste: " + args);
					GetClipStr();
					RestoreClipBoard(Regex.Replace(args, "\r?\n|\r", Environment.NewLine));
					KInputs.MakeInput(new []{ KInputs.AddKey(Keys.LControlKey, true), KInputs.AddKey(Keys.V, true),
					                  	KInputs.AddKey(Keys.LControlKey, false), KInputs.AddKey(Keys.V, false)});
					DoLater(() => RestoreClipBoard(), 300);
					break;
				case "__date":
				case "__time":
					var now = DateTime.Now;
					var format = args;
					if (string.IsNullOrEmpty(args)) {
						if (expr == "__date")
							format = "dd/MM/yyyy";
						else 
							format = "HH:mm:ss";
					}
					KInputs.MakeInput(KInputs.AddString(now.ToString(format)));
					break;
				case "__version":
					KInputs.MakeInput(KInputs.AddString(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));
					break;
				 case "__title":
					KInputs.MakeInput(KInputs.AddString(MMain.mahou.Text));
					 break;
				case "__system":
					KInputs.MakeInput(KInputs.AddString(Environment.OSVersion.ToString()));
					break;
				case "__keyboard":
					SimKeyboard(args);
					break;
				case "__execute":
					Execute(args);
					break;
				case "__mahouhome":
					KInputs.MakeInput(KInputs.AddString(MahouUI.nPath));
					break;
				case "__cursorhere":
					Debug.WriteLine("Curlefts: " +curlefts);
					cursormove = curlefts;
					break;
			}
		}
		static void Execute(string args) {
			string fil = "", arg ="";
			bool fil_get = false;
			for (int i = 0; i < args.Length; i++) {
				var c = args[i];
				Debug.WriteLine("c: " +c);
				if (!fil_get) {
					if (c == '|') {
						fil_get = true;
					} else fil += c;
				} else {
					arg += c;
				}
			}
			Logging.Log("Executing: executable: ["+fil+"] with args: ["+arg+"].");
			var p = new ProcessStartInfo();
			p.Arguments = arg;
			p.UseShellExecute = true;
			p.FileName = fil;
			try {
				Process.Start(p);
			} catch(Exception e) {
				Logging.Log("Execute error: " + e.Message);
			}
		}
		static void SimKeyboard(string args) {
			string[] multi_args;
			var all_keys = new List<List<Keys>>();
			if (args.Contains(" "))
				multi_args = args.Split(' ');
			else
				multi_args = new []{args};
			for (int i = 0; i!= multi_args.Length; i++) {
				var keys = new List<Keys>();
				var _args = multi_args[i];
				string[] multi_keys;
				if (_args.Contains("+"))
					multi_keys = _args.Split('+');
				else 
					multi_keys = new []{_args};
				for (int j = 0; j != multi_keys.Length; j++) {
					var key =  multi_keys[j].ToLower();
					foreach (Keys k in Enum.GetValues(typeof(Keys))) {
						var _n = k.ToString().ToLower()
							.Replace("menu", "alt").Replace("control", "ctrl")
							.Replace("d0", "0").Replace("d1", "1")
							.Replace("d2", "2").Replace("d3", "3")
							.Replace("d4", "4").Replace("d5", "5")
							.Replace("d6", "6").Replace("d7", "7")
							.Replace("d8", "9").Replace("d9", "9")
							.Replace("return", "enter").Replace("numpa", "numpad");
						if (_n == key+"key") { // controlkey, shiftkey
							Logging.Log("Added the " + _n);
							keys.Add(k);
							break;
						}
						if (key.Length>1) {
							if (key[0] == '[' && key[key.Length-1] == ']') {
								var scode = key.Substring(1,key.Length-2).ToLower();
								int code = -1;
								bool ok = false;
								if (scode.Contains("x")) {
									scode = scode.Replace("x", "");
									ok = Int32.TryParse(scode, System.Globalization.NumberStyles.HexNumber, null, out code);
								} else {
									ok = Int32.TryParse(scode, out code);
								}
								if (ok)
									if (code == (int)k) { 
										Logging.Log("Added the key by code: " + code + ", key: " + k);
										keys.Add(k);
										break;
									}
							}
						}
						if (key == "esc") {
							Logging.Log("Added the short escape: " + key);
							keys.Add(Keys.Escape);
							break;
						}
						if (key == "win") {
							Logging.Log("Added the lwin as base of: " + _n);
							keys.Add(Keys.LWin);
							break;
						}
						if (_n == key) {
							Logging.Log("Added the " + _n);
							keys.Add(k);
							break;
						}
					}
				}
				all_keys.Add(keys);
			}
			foreach (var keys in all_keys) {
				foreach (var key in keys) {
					Logging.Log("Pressing: " +key);
					KInputs.MakeInput(new []{KInputs.AddKey(key, true)});
				}
				foreach (var key in keys) {
					Logging.Log("Releasing: " +key);
					KInputs.MakeInput(new []{KInputs.AddKey(key, false)});
				}
				Thread.Sleep(5);
			}
			Thread.Sleep(30);
		}
		public static void DoLater(Action act, int timeout) {
			System.Threading.Tasks.Task.Factory.StartNew(() => {
			                                             	Thread.Sleep(timeout);
			                                             	act();
			                                             });
		}
//		public static void SetNextLayout() {
//			var CUR = Locales.GetCurrentLocale();
//			var CUR_IND = MMain.locales.ToList().FindIndex(lid => lid.uId == CUR);
//			CUR_IND++;
//			if (CUR_IND >= MMain.locales.Length)
//				CUR_IND = 0;
//			Debug.WriteLine("NEXT LAYOUT: " + MMain.locales[CUR_IND].Lang + " IND " + CUR_IND  + " LEN " + MMain.locales.Length + " CUR " + CUR) ;
//		}
		static bool IsUpperInput() {
			bool caps = Control.IsKeyLocked(Keys.CapsLock);
			if (MahouUI.CapsLockDisablerTimer)
				caps = false;
			if (((shift || shift_r) && !caps) || (!(shift || shift_r) && caps))
				return true;
			if (((shift || shift_r) && caps) || (!(shift || shift_r) && !caps))
				return false;
			return false;
		}
		public static bool ExcludedProgram() {
			if (MMain.mahou == null) return false;
			var hwnd = WinAPI.GetForegroundWindow();
		if (NOT_EXCLUDED_HWNDs.Contains(hwnd)) {
				Logging.Log("This program was been checked already, it is not excluded hwnd: " + hwnd);
				return false;
			}
			if (!EXCLUDED_HWNDs.Contains(hwnd)) {
				uint pid;
				WinAPI.GetWindowThreadProcessId(hwnd, out pid);
				Process prc = null;
				try { 
					prc = Process.GetProcessById((int)pid);
					if (prc == null) return false;
					if (MMain.mahou.ExcludedPrograms.Replace(Environment.NewLine, " ").ToLower().Contains(prc.ProcessName.ToLower().Replace(" ", "_") + ".exe")) {
						Logging.Log(prc.ProcessName + ".exe->excluded");
						EXCLUDED_HWNDs.Add(hwnd);
						return true;
					}
				} catch { Logging.Log("Process with id ["+pid+"] not exist...", 1); }
			} else {
				Logging.Log("Excluded program by excluded program saved hwnd: " + hwnd);
				return true;
			}
			NOT_EXCLUDED_HWNDs.Add(hwnd);
			return false;
		}
		static void SpecificKey(Keys Key, uint MSG, int vkCode = 0) {
//			Debug.WriteLine("SPK:" + skip_spec_keys);
			if (skip_spec_keys > 0) {
				skip_spec_keys--;
				if (skip_spec_keys < 0)
					skip_spec_keys = 0;
				return;
			}
//			Debug.WriteLine("Speekky->" + Key);
			for (int i = 1; i!=5; i++) {
				if ((MSG == WinAPI.WM_KEYUP || MSG == WinAPI.WM_SYSKEYUP || vkCode == 240)) {
		       		var specificKey = (int)typeof(MahouUI).GetField("Key"+i).GetValue(MMain.mahou);
					if (MMain.mahou.ChangeLayoutInExcluded || !ExcludedProgram()) {
						#region Switch between layouts with one key
						bool F18 = Key == Keys.F18;
						bool GJIME = false;
						if (specificKey == 8) // Shift+CapsLock
							if (vkCode == 240) { // Google Japanese IME's  Shift+CapsLock repam fix
								skip_spec_keys++; // Skip next CapsLock up event
								GJIME = true;
							}
						if ((Key == Keys.CapsLock && !shift && !shift_r && !alt && !alt_r && !ctrl && !ctrl_r && specificKey == 1) ||
						    (Key == Keys.CapsLock && (shift || shift_r) && !alt && !alt_r && !ctrl && !ctrl_r && specificKey == 8) )
							if (Control.IsKeyLocked(Keys.CapsLock))
								DoSelf(() => { KeybdEvent(Keys.CapsLock, 0); KeybdEvent(Keys.CapsLock, 2); });
						var speclayout = (string)typeof(MahouUI).GetField("Layout"+i).GetValue(MMain.mahou);
						if (String.IsNullOrEmpty(speclayout)) {
						    Logging.Log("No layout for Layout"+i + " variable.");
						    return;
					    }
						if (speclayout == MMain.Lang[Languages.Element.SwitchBetween]) {
							if (specificKey == 12 && Key == Keys.Tab && !ctrl && !ctrl_r && !shift_r && !shift && !win && !win_r && !alt && !alt_r) {
								Logging.Log("Changing layout by Tab key.");
								ChangeLayout();
						    	return;
							}
							if (specificKey == 11 && (
								(Key == Keys.LShiftKey && ctrl) || (Key == Keys.RShiftKey && ctrl_r) ||
								(Key == Keys.LControlKey && shift) || (Key == Keys.RControlKey && shift_r)) && !win && !win_r && !alt && !alt_r) {
								Logging.Log("Changing layout by Ctrl+Shift key.");
								ChangeLayout();
						    	return;
							}
							if (specificKey == 10 && (
								(Key == Keys.LShiftKey && alt) || (Key == Keys.RShiftKey && alt_r) ||
								(Key == Keys.LMenu && shift) || (Key == Keys.RMenu && shift_r)) && !win && !win_r && !ctrl && !ctrl_r) {
								Logging.Log("Changing layout by Alt+Shift key.");
								ChangeLayout();
						    	return;
							}
							if (specificKey == 8 && (Key == Keys.CapsLock || F18 || GJIME) && (shift || shift_r) && !alt && !alt_r && !ctrl && !ctrl_r) {
								Logging.Log("Changing layout by Shift+CapsLock"+(GJIME?"(KeyCode: 240, Google Japanese IME's Shift+CapsLock remap)":"")+(F18?"(F18)":"")+" key.");
								ChangeLayout();
						    	return;
							} else 
							if (!shift && !shift_r && !alt && !alt_r && !ctrl && !ctrl_r && specificKey == 1 && 
								    (Key == Keys.CapsLock || F18)) {
								ChangeLayout();
								Logging.Log("Changing layout by CapsLock"+(F18?"(F18)":"")+" key.");
						    	return;
							}
							if (specificKey == 2 && Key == Keys.LControlKey && !keyAfterCTRL) {
								Logging.Log("Changing layout by L-Ctrl key.");
								ChangeLayout();
						    	return;
							}
							if (specificKey == 3 && Key == Keys.RControlKey && !keyAfterCTRL) {
								Logging.Log("Changing layout by R-Ctrl key.");
								ChangeLayout();
						    	return;
							}
							if (specificKey == 4 && Key == Keys.LShiftKey && !keyAfterSHIFT) {
								Logging.Log("Changing layout by L-Shift key.");
								ChangeLayout();
						    	return;
							}
							if (specificKey == 5 && Key == Keys.RShiftKey && !keyAfterSHIFT) {
								Logging.Log("Changing layout by R-Shift key.");
								ChangeLayout();
						    	return;
							}
							if (specificKey == 6 && Key == Keys.LMenu && !keyAfterALT) {
								Logging.Log("Changing layout by L-Alt key.");
								ChangeLayout();
						    	return;
							}
							if (specificKey == 7 && Key == Keys.RMenu && !keyAfterALT) {
								Logging.Log("Changing layout by R-Alt key.");
								ChangeLayout();
						    	return;
							}
							if (specificKey == 9 && Key == Keys.RMenu) {
								Logging.Log("Changing layout by AltGr key.");
								ChangeLayout();
						    	return;
							}
//							if (catched) {
//			       			    if (Key == Keys.LMenu)
//									DoSelf(()=>{ Thread.Sleep(150); KeybdEvent(Keys.LMenu, 0); KeybdEvent(Keys.LMenu, 2); });
//			       			    if (Key == Keys.RMenu)
//									DoSelf(()=>{ Thread.Sleep(150); KeybdEvent(Keys.RMenu, 0); KeybdEvent(Keys.RMenu, 2); });
//							}
							#endregion
						} else {
							#region By layout switch
							var matched = false;
							if (specificKey == 12 && Key == Keys.Tab && !ctrl && !ctrl_r && !shift_r && !shift && !win && !win_r && !alt && !alt_r) {
								Logging.Log("Switching to specific layout by Tab key.");
								ChangeToLayout(Locales.ActiveWindow(), Locales.GetLocaleFromString(speclayout).uId);
								matched = true;
						    	return;
							}
							if (specificKey == 10 && (
								(Key == Keys.LShiftKey && alt) || (Key == Keys.RShiftKey && alt_r) ||
								(Key == Keys.LMenu && shift) || (Key == Keys.RMenu && shift_r)) && !win && !win_r && !ctrl && !ctrl_r) {
								Logging.Log("Switching to specific layout by Alt+Shift key.");
								ChangeToLayout(Locales.ActiveWindow(), Locales.GetLocaleFromString(speclayout).uId);
								matched = true;
						    	return;
							}
							if (specificKey == 8 && (Key == Keys.CapsLock || F18 || GJIME) && (shift || shift_r) && !alt && !alt_r && !ctrl && !ctrl_r) {
								Logging.Log("Switching to specific layout by Shift+CapsLock"+(GJIME?"(KeyCode: 240, Google Japanese IME's Shift+CapsLock remap)":"")+(F18?"(F18)":"")+" key.");
								ChangeToLayout(Locales.ActiveWindow(), Locales.GetLocaleFromString(speclayout).uId);
								matched = true;
						    	return;
							} else
							if (specificKey == 1 && (Key == Keys.CapsLock || F18)) {
								Logging.Log("Switching to specific layout by Caps Lock"+(F18?"(F18)":"")+" key.");
								ChangeToLayout(Locales.ActiveWindow(), Locales.GetLocaleFromString(speclayout).uId);
								matched = true;
						    	return;
							}
							if (specificKey == 2 && Key == Keys.LControlKey && !keyAfterCTRL) {
								Logging.Log("Switching to specific layout by  L-Ctrl key.");
								ChangeToLayout(Locales.ActiveWindow(), Locales.GetLocaleFromString(speclayout).uId);
								matched = true;
						    	return;
							}
							if (specificKey == 3 && Key == Keys.RControlKey && !keyAfterCTRL) {
								Logging.Log("Switching to specific layout by R-Ctrl key.");
								ChangeToLayout(Locales.ActiveWindow(), Locales.GetLocaleFromString(speclayout).uId);
								matched = true;
						    	return;
							}
							if (specificKey == 4 && Key == Keys.LShiftKey && !keyAfterSHIFT) {
								Logging.Log("Switching to specific layout by L-Shift key.");
								ChangeToLayout(Locales.ActiveWindow(), Locales.GetLocaleFromString(speclayout).uId);
								matched = true;
						    	return;
							}
							if (specificKey == 5 && Key == Keys.RShiftKey && !keyAfterSHIFT) {
								Logging.Log("Switching to specific layout by R-Shift key.");
								ChangeToLayout(Locales.ActiveWindow(), Locales.GetLocaleFromString(speclayout).uId);
								matched = true;
						    	return;
							}
							if (specificKey == 6 && Key == Keys.LMenu && !keyAfterALT) {
								Logging.Log("Switching to specific layout by L-Alt key.");
								ChangeToLayout(Locales.ActiveWindow(), Locales.GetLocaleFromString(speclayout).uId);	
								matched = true;
								DoSelf(()=>{ KeybdEvent(Keys.LMenu, 0); KeybdEvent(Keys.LMenu, 2); });
						    	return;
							}
							if (specificKey == 7 && Key == Keys.RMenu && !keyAfterALT) {
								Logging.Log("Switching to specific layout by R-Alt key.");
								ChangeToLayout(Locales.ActiveWindow(), Locales.GetLocaleFromString(speclayout).uId);
								matched = true;
								DoSelf(()=>{ KeybdEvent(Keys.RMenu, 0); KeybdEvent(Keys.RMenu, 2); });
						    	return;
							}
							if (specificKey == 9 && Key == Keys.RMenu) {
								Logging.Log("Switching to specific layout by AltGr key.");
								ChangeToLayout(Locales.ActiveWindow(), Locales.GetLocaleFromString(speclayout).uId);
								matched = true;
								DoSelf(()=>{ KeybdEvent(Keys.RMenu, 0); KeybdEvent(Keys.RMenu, 2); });
						    	return;
							}
							try {
								if (matched) {
									Logging.Log("Available layout from string ["+speclayout+"] & id ["+i+"].");
									//Fix for alt-show-menu in programs
//				       			    if (Key == Keys.LMenu)
//										DoSelf(()=>{ KeybdEvent(Keys.LMenu, 0); KeybdEvent(Keys.LMenu, 2); });
//				       			    if (Key == Keys.RMenu)
//										DoSelf(()=>{ KeybdEvent(Keys.RMenu, 0); KeybdEvent(Keys.RMenu, 2); });
								}
							} catch { 
								Logging.Log("No layout available from string ["+speclayout+"] & id ["+i+"]."); 
							}
						}
						#endregion
				    }
          		}
			}
		}
		public static void ClearModifiers() {
			win = alt = ctrl = shift = win_r = alt_r = ctrl_r = shift_r = false;
			LLHook.ClearModifiers();
			SendModsUp((int)(WinAPI.MOD_ALT + WinAPI.MOD_CONTROL + WinAPI.MOD_SHIFT + WinAPI.MOD_WIN));
		}
		static void ClearWord(bool LastWord = false, bool LastLine = false, bool Snippet = false, string ClearReason = "") {
			string ReasonEnding = ".";
			if (MahouUI.LoggingEnabled && !String.IsNullOrEmpty(ClearReason))
				ReasonEnding = ", reason: [" + ClearReason + "].";
			if (LastWord) {
				c_word_backup_last = new List<YuKey>(c_word_backup);
				c_word_backup = new List<YuKey>(MMain.c_word);
				MMain.c_word.Clear();
				Logging.Log("Cleared last word" + ReasonEnding);
			}
			if (LastLine) {
				MMain.c_words.Clear();
				Logging.Log("Cleared last line" + ReasonEnding);
			}
			if (Snippet) {
				if (MMain.mahou.SnippetsEnabled) {
					c_snip.Clear();
					Logging.Log("Cleared current snippet" + ReasonEnding);
				}
			}
		}
		public static string[] SplitWords(string LINE) {
			var LIST = new List<string>();
			string left = LINE;
			int ind = left.IndexOf(' ');
			while ((ind = left.IndexOf(' ')) != -1) {
				ind = left.IndexOf(' ');
				if (ind == 0)
					ind = 1;
				var word = left.Substring(0, ind);
				left = left.Substring(ind, left.Length-ind);
//				Debug.WriteLine(word + "] " + ind + " [" + left);
				LIST.Add(word);
			}
			if (ind == -1 && !String.IsNullOrEmpty(left)) {
				LIST.Add(left);
//				Debug.WriteLine(left);
			}
			return LIST.ToArray();
		}
		/// <summary>
		/// Converts selected text.
		/// </summary>
		public static void ConvertSelection() {
			Debug.WriteLine("Start CS");
			try { //Used to catch errors
				DoSelf(() => {
					Logging.Log("Starting Convert selection.");
					string ClipStr = GetClipStr();
					ClipStr = Regex.Replace(ClipStr, @"(\d+)[,.?бю/]", "$1.");
					if (!String.IsNullOrEmpty(ClipStr)) {
						csdoing = true;
						Logging.Log("Starting conversion of [" + ClipStr + "].");
						SendBack();
						var result = "";
						int items = 0;
						if (MMain.mahou.ConvertSelectionLS && !MMain.mahou.OneLayoutWholeWord) {
							Logging.Log("Using CS-Switch mode.");
							var wasLocale = Locales.GetCurrentLocale() & 0xffff;
							var wawasLocale = wasLocale & 0xffff;
							ChangeLayout(true);
							uint nowLocale = 0;
							if(MMain.mahou.SwitchBetweenLayouts) {
								nowLocale = wasLocale == (MahouUI.MAIN_LAYOUT1 & 0xffff)
									? MahouUI.MAIN_LAYOUT2 & 0xffff
									: MahouUI.MAIN_LAYOUT1 & 0xffff;
								if (nowLocale == wasLocale && 
								    (MahouUI.currentLayout == MahouUI.MAIN_LAYOUT1 || 
								     MahouUI.currentLayout == MahouUI.MAIN_LAYOUT2)) {
									if (wasLocale != MahouUI.currentLayout)
										nowLocale = MahouUI.currentLayout;
								}
							} else {
								Thread.Sleep(10); nowLocale = GetNextLayout().uId & 0xffff;
							}
							var index = 0;
							foreach (char c in ClipStr) {
								items++;
								wasLocale = wawasLocale;
								var s = new StringBuilder(10);
								var sb = new StringBuilder(10);
								var yk = new YuKey();
								var scan = WinAPI.VkKeyScanEx(c, wasLocale);
								var state = ((scan >> 8) & 0xff);
								var bytes = new byte[255];
								if (state == 1)
									bytes[(int)Keys.ShiftKey] = 0xFF;
								var scan2 = WinAPI.VkKeyScanEx(c, nowLocale);
								var state2 = ((scan2 >> 8) & 0xff);
								var bytes2 = new byte[255];
								if (state2 == 1)
									bytes2[(int)Keys.ShiftKey] = 0xFF;
								if (MMain.mahou.ConvertSelectionLSPlus) {
									Logging.Log("Using Experimental CS-Switch mode.");
									WinAPI.ToUnicodeEx((uint)scan, (uint)scan, bytes, s, s.Capacity, 0, (IntPtr)wasLocale);
									Logging.Log("Char 1 is [" + s + "] in locale +[" + wasLocale + "].");
									if (ClipStr[index].ToString() == s.ToString()) {
										if (!SymbolIgnoreRules((Keys)(scan & 0xff), state == 1, wasLocale)) {
											Logging.Log("Making input of [" + scan + "] in locale +[" + nowLocale + "].");
											KInputs.MakeInput(KInputs.AddString(InAnother(c, wasLocale, nowLocale)));
										}
										index++;
										continue;
									}
									WinAPI.ToUnicodeEx((uint)scan2, (uint)scan2, bytes2, sb, sb.Capacity, 0, (IntPtr)nowLocale);
									Logging.Log("Char 2 is [" + sb + "] in locale +[" + nowLocale + "].");
									if (ClipStr[index].ToString() == sb.ToString()) {
										Logging.Log("Char 1, 2 and original are equivalent.");
										ChangeToLayout(Locales.ActiveWindow(), wasLocale);
										wasLocale = nowLocale;
										scan = scan2;
										state = state2;
									}
								}
								if (c == '\n') {
									yk.key = Keys.Enter;
									yk.upper = false;
								} else {
									if (scan != -1) {
										var key = (Keys)(scan & 0xff);
										bool upper = false || state == 1;
										yk = new YuKey() { key = key, upper = upper };
										Logging.Log("Key of char [" + c + "] = {" + key + "}, upper = +[" + state + "].");
									} else {
										yk = new YuKey() { key = Keys.None };
									}
								}
								if (yk.key == Keys.None) { // retype unrecognized as unicode
									var unrecognized = ClipStr[items - 1].ToString();
									WinAPI.INPUT unr = KInputs.AddString(unrecognized)[0];
									Logging.Log("Key of char [" + c + "] = not exist, using input as string.");
									KInputs.MakeInput(new [] { unr });
								} else {
									if (!SymbolIgnoreRules(yk.key, yk.upper, wasLocale)) {
										Logging.Log("Making input of [" + yk.key + "] key with upper = [" + yk.upper + "].");
										if (yk.upper)
											KInputs.MakeInput(new [] { KInputs.AddKey(Keys.LShiftKey, true) });
										KInputs.MakeInput(new [] { KInputs.AddKey(yk.key, true) });
										KInputs.MakeInput(new [] { KInputs.AddKey(yk.key, false) });
										if (yk.upper)
											KInputs.MakeInput(new [] { KInputs.AddKey(Keys.LShiftKey, false) });
									}
								}
								index++;
							}
						} else {
							var l1 = cs_layout_last;
							if (MMain.mahou.ConvertSelectionLS) {
								Logging.Log("Using Layout Switch in Convert Selection.");
								l1 = Locales.GetCurrentLocale();
								if (MahouUI.UseJKL)
									l1 = MahouUI.currentLayout;
								ChangeLayout();
							}
							var l2 = GetNextLayout(l1).uId;
							Debug.WriteLine("next: " +l2);
							var index = 0;
							if (MMain.mahou.OneLayoutWholeWord) {
								Logging.Log("Using one layout whole word convert selection mode.");
								var allWords = SplitWords(ClipStr);
								var word_index = 0;
								foreach (var w in allWords) {
									result += w == " " ? w : WordGuessLayout(w, l2).Item1;
									word_index +=1;
//									Debug.WriteLine("(" + w + ") ["+ result +"]");
									index++;
								}
							} else {
								Logging.Log("Using default convert selection mode.");
								for (int I=0; I!=ClipStr.Length; I++) {
									var sm = false;
									var c = ClipStr[I];
									if (c == 'ո' || c == 'Ո') {
										if (c == 'ո') sm = true;
										if (ClipStr.Length > I+1) {
											if (ClipStr[I+1] == 'ւ') {
												var shrt = l2 & 0xffff;
												var _shrt = l1 & 0xffff;
												if (shrt == 1033 || shrt == 1041) {
													result += sm ? "u" : "U";
													I++; continue;
												}
												if (_shrt == 1033 || _shrt == 1041) {
													result += sm ? "u" : "U";
													I++; continue;
												}
												if (shrt == 1049) {
													result += sm ? "г" : "Г";
													I++; continue;
												}
												if (_shrt == 1049) {
													result += sm ? "г" : "Г";
													I++; continue;
												}
											}
										}
									}
									var T = InAnother(c, l1 & 0xffff, l2 & 0xffff);
									for (int i = 0; i != MMain.locales.Length; i++) {
										var l = MMain.locales[i].uId;
										if (c == '\n')
											T = "\n";
										T = GermanLayoutFix(c);
										T = InAnother(c, l & 0xffff, l2 & 0xffff);
										if (T != "") 
											break;
										index++;
									}
									if (T == "")
										T = ClipStr[index].ToString();
									result += T;
								}
							}
							cs_layout_last = l2;
							Logging.Log("Conversion of string [" + ClipStr + "] from locale [" + l1 + "] into locale [" + l2 + "] became [" + result + "].");
							//Inputs converted text
							Logging.Log("Making input of [" + result + "] as string");
							KInputs.MakeInput(KInputs.AddString(result));
							items = result.Length;
						}
						ReSelect(items);
					}
					NativeClipboard.Clear();
					RestoreClipBoard();
				});
			} catch(Exception e) {
				Logging.Log("Convert Selection encountered error, details:\r\n" +e.Message+"\r\n"+e.StackTrace, 1);
			}
			Memory.Flush();
		}
		public static void TransliterateSelection() {
			try { //Used to catch errors
				Locales.IfLessThan2();
				DoSelf(() => {
					Logging.Log("Starting Transliterate selection.");
					string ClipStr = GetClipStr();
					if (!String.IsNullOrEmpty(ClipStr)) {
						string output = ClipStr;
						foreach (KeyValuePair<string, string> key in transliterationDict) {
							if (output.Contains(key.Key))
			                	output.Replace(key.Key, key.Value);
			            }
						if (ClipStr == output) {
							foreach (KeyValuePair<string, string> key in transliterationDict) {
								if (ClipStr.Contains(key.Value))
				                	ClipStr = ClipStr.Replace(key.Value, key.Key);
		                	}
							if (ClipStr == output)
							foreach (KeyValuePair<string, string> key in transliterationDict) {
								if (ClipStr.Contains(key.Key))
				                	ClipStr = ClipStr.Replace(key.Key, key.Value);
		                	}
							KInputs.MakeInput(KInputs.AddString(ClipStr));
						} else { KInputs.MakeInput(KInputs.AddString(output)); }
						ReSelect(ClipStr.Length);
					}
					NativeClipboard.Clear();
					RestoreClipBoard();
	            });
				} catch(Exception e) {
					Logging.Log("Transliterate Selection encountered error, details:\r\n" +e.Message+"\r\n"+e.StackTrace, 1);
				}
			Memory.Flush();
		}
		public static void ToTitleSelection() {
			try {
				DoSelf(() => {
					string ClipStr = GetClipStr();
					if (!String.IsNullOrEmpty(ClipStr)) {
						string[] ClipStrLines = ClipStr.Split('\n');
						int lines = 0;
						foreach (var line in ClipStrLines) {
							lines++;
							string[] ClipStrWords = SplitWords(line);
							int words = 0;
							foreach (var word in ClipStrWords) {
								words++;
								string Tword = "";
								if (word.Length > 0)
									Tword += word[0].ToString().ToUpper();
								if (word .Length > 1)
									foreach(char ch in word.Substring(1, word.Length - 1)) {
										Tword += char.ToLower(ch);
									}
								Logging.Log("Inputting word ["+Tword+"] as Title case");
								KInputs.MakeInput(KInputs.AddString(Tword));
							}
							if (lines != ClipStrLines.Length)
								KInputs.MakeInput(KInputs.AddString("\n"));
						}
						ReSelect(ClipStr.Length);
					}
					NativeClipboard.Clear();
					RestoreClipBoard();
	              });
			} catch(Exception e) {
				Logging.Log("To Title Selection encountered error, details:\r\n" +e.Message+"\r\n"+e.StackTrace, 1);
			}
			Memory.Flush();
		}
		public static void ToSwapSelection() {
			try {
				DoSelf(() => {
					string ClipStr = GetClipStr();
					if (!String.IsNullOrEmpty(ClipStr)) {
						string[] ClipStrLines = ClipStr.Split('\n');
						int lines = 0;
						foreach (var line in ClipStrLines) {
							lines++;
							string[] ClipStrWords = SplitWords(line);
							int words = 0;
							foreach (var word in ClipStrWords) {
								words++;
								string Sword = "";
								foreach(char ch in word) {
									if (char.IsUpper(ch))
										Sword += char.ToLower(ch);
									else if (char.IsLower(ch))
										Sword += char.ToUpper(ch);
									else
										Sword += ch;
								}
								Logging.Log("Inputting word ["+Sword+"] as sWAP case");
								KInputs.MakeInput(KInputs.AddString(Sword));
							}
							if (lines != ClipStrLines.Length)
								KInputs.MakeInput(KInputs.AddString("\n"));
						}
						ReSelect(ClipStr.Length);
					}
					NativeClipboard.Clear();
					RestoreClipBoard();
	              });
			} catch(Exception e) {
				Logging.Log("To sWAP Selection encountered error, details:\r\n" +e.Message+"\r\n"+e.StackTrace, 1);
			}
			Memory.Flush();
		}
		public static void ToRandomSelection() {
			try {
				DoSelf(() => {
					string ClipStr = GetClipStr();
					if (!String.IsNullOrEmpty(ClipStr)) {
						string[] ClipStrLines = ClipStr.Split('\n');
						int lines = 0;
						foreach (var line in ClipStrLines) {
							lines++;
							string[] ClipStrWords = SplitWords(line);
							int words = 0;
							foreach (var word in ClipStrWords) {
								words++;
								Random rand = new Random();
								string Rword = "";
								foreach(char ch in word) {
									if (rand.NextDouble() >= 0.5) {
										Rword += char.ToLower(ch);
									} else {
										Rword += char.ToUpper(ch);
									}
								}
								Logging.Log("Inputting word ["+Rword+"] as RaNdoM case");
								KInputs.MakeInput(KInputs.AddString(Rword));
							}
							if (lines != ClipStrLines.Length)
								KInputs.MakeInput(KInputs.AddString("\n"));
						}
						ReSelect(ClipStr.Length);
					}
					NativeClipboard.Clear();
					RestoreClipBoard();
	              });
			} catch(Exception e) {
				Logging.Log("To RaNdoM Selection encountered error, details:\r\n" +e.Message+"\r\n"+e.StackTrace, 1);
			}
			Memory.Flush();
		}
		static void ReSelect(int count) {
			if (MMain.mahou.ReSelect) {
				//reselects text
				Logging.Log("Reselecting text.");
				for (int i = count; i != 0; i--) {
					KInputs.MakeInput(new [] { 
						KInputs.AddKey(Keys.LShiftKey, true),
						KInputs.AddKey(Keys.Left, true),
						KInputs.AddKey(Keys.Left, false),
						KInputs.AddKey(Keys.LShiftKey, false)
					});
				}
			}
		}
		static string ArmenianSignleCharFix(string word, uint next_layout, uint this_layout = 0) {
			var shrt = next_layout & 0xffff;
			var _shrt = this_layout & 0xffff;
//			if (shrt == 1033 || shrt == 1041) // English/Japanese
			var repl = word;
//			Debug.WriteLine("Next: " + next_layout + ", word: " +word);
			if (shrt == 1067) // Armenian
				repl = word.Replace("u", "w6").Replace("U", "W6").Replace("г","ц6").Replace("Г","Ц6");
			if (_shrt == 1067) {
				if (shrt == 1033 || shrt == 1041) // English/Japanese
					repl = word.Replace("ու", "u").Replace("Ու", "U");
				if (shrt == 1049) //  Russian
					repl = word.Replace("ու", "Г").Replace("Ու", "Г");
			}
//			else if (shrt == 1049) // Russian
//				word.Replace("Ու", "Г").Replace("ու", "г");
			Debug.WriteLine("RELT: " + repl);
			return repl;
		}
		static string GermanLayoutFix(char c) {
			if (!MMain.mahou.QWERTZ_fix)
				return "";
			var T = "";
			switch (c) {
				case 'ä':
					T = "э"; break; 
				case 'э':
					T = "ä"; break;
				case 'ö':
					T = "ж"; break;
				case 'ж':
					T = "ö"; break;
				case 'ü':
					T = "х"; break;
				case 'х':
					T = "ü"; break;
				case 'Ä':
					T = "Э"; break;
				case 'Э':
					T = "Ä"; break;
				case 'Ö':
					T = "Ж"; break;
				case 'Ж':
					T = "Ö"; break;
				case 'Ü':
					T = "Х"; break;
				case 'Х':
					T = "Ü"; break;
				case 'Я':
					T = "Y"; break;
				case 'Y':
					T = "Я"; break;
				case 'Н':
					T = "Z"; break;
				case 'Z':
					T = "Н"; break;
				case 'я':
					T = "y"; break;
				case 'y':
					T = "я"; break;
				case 'н':
					T = "z"; break;
				case 'z':
					T = "н"; break;
				case '-':
					T = "ß"; break;
				default:
					T = ""; break;
			}
			Logging.Log("German fix T:" + T +  "/ c: " + c);
			return T;
		}
		static bool WaitForClip2BeFree() {
			Debug.WriteLine(">> WFC2F");
			IntPtr CB_Blocker = IntPtr.Zero;
			int tries = 0;
			do { 
				CB_Blocker = WinAPI.GetOpenClipboardWindow();
				if (CB_Blocker == IntPtr.Zero) break;
				Logging.Log("Clipboard blocked by process id ["+WinAPI.GetWindowThreadProcessId(CB_Blocker, IntPtr.Zero) +"].", 2);
				tries ++;
				if (tries > 3000) {
					Logging.Log("3000 Tries to wait for clipboard blocker ended, blocker didn't free'd clipboard |_|.", 2); return false;
				}
			} while (CB_Blocker != IntPtr.Zero);
			Debug.WriteLine(">> WFC2F t: " + tries);
			return true;
		}
		public static bool RestoreClipBoard(string special = "") {
			Debug.WriteLine(">> RC");
			var restore = special;
			bool spc = true;
			if (String.IsNullOrEmpty(restore)) {
				restore = lastClipText;
				spc = false;
			}
			Logging.Log((spc?"Special ":"")+"Restoring clipboard text: ["+restore+"].");
			if (WaitForClip2BeFree()) {
				try { Clipboard.SetDataObject(restore, true, 5, 120); return true; } 
				catch { Logging.Log("Error during clipboard "+(spc?"Special ":"")+"text restore after 5 tries.", 2); return false; }
			}
			return false;
		}
		/// <summary>
		/// Sends RCtrl + Insert to selected get text, and returns that text by using WinAPI.GetText().
		/// </summary>
		/// <returns>string</returns>
		static string MakeCopy()  {
			Debug.WriteLine(">> MC");
			KInputs.MakeInput(new [] {
				KInputs.AddKey(Keys.RControlKey, true),
				KInputs.AddKey(Keys.Insert, true),
				KInputs.AddKey(Keys.Insert, false),
				KInputs.AddKey(Keys.RControlKey, false)
			});
			Thread.Sleep(30);
			return NativeClipboard.GetText();
		}
		public static string GetClipStr() {
			Debug.WriteLine(">> GCS");
			Locales.IfLessThan2();
			string ClipStr = "";
			// Backup & Restore feature, now only text supported...
			if (MMain.MahouActive() && MMain.mahou.ActiveControl is TextBox)
				return (MMain.mahou.ActiveControl as TextBox).SelectedText;
			Logging.Log("Taking backup of clipboard text if possible.");
			lastClipText = NativeClipboard.GetText();
//			Thread.Sleep(50);
			if (!String.IsNullOrEmpty(lastClipText))
				lastClipText = Clipboard.GetText();
//			This prevents from converting text that already exist in Clipboard
//			by pressing "Convert Selection hotkey" without selected text.
			NativeClipboard.Clear();
			Logging.Log("Getting selected text.");
			if (MMain.mahou.SelectedTextGetMoreTries)
				for (int i = 0; i != MMain.mahou.SelectedTextGetMoreTriesCount; i++) {
					if (WaitForClip2BeFree()) {
							ClipStr = MakeCopy();
							if (!String.IsNullOrEmpty(ClipStr))
								break;
					}
				}
			else {
				if (WaitForClip2BeFree()) {
					ClipStr = MakeCopy();
					if (String.IsNullOrEmpty(ClipStr))
						ClipStr = MakeCopy();
				}
			}
			if (String.IsNullOrEmpty(ClipStr))
				return "";
			return Regex.Replace(ClipStr, "\r?\n|\r", "\n");
		}
		/// <summary>
		/// Re-presses modifiers you hold when hotkey fired(due to SendModsUp()).
		/// </summary>
		public static void RePress()  {
			DoSelf(() => {
				//Repress's modifiers by RePress variables
				if (shiftRP) {
					KeybdEvent(Keys.LShiftKey, 0);
					swas = true;
					shiftRP = false;
				}
				if (altRP) {
					KeybdEvent(Keys.LMenu, 0);
					awas = true;
					altRP = false;
				}
				if (ctrlRP) {
					KeybdEvent(Keys.LControlKey, 0);
					cwas = true;
					ctrlRP = false;
				}
				if (winRP) {
					KeybdEvent(Keys.LWin, 0);
					wwas = true;
					winRP = false;
				}
			       });
		}
		/// <summary>
		/// Do action without RawInput listeners(e.g. not catch).
		/// Useful with SendInput or keybd_event functions.
		/// </summary>
		/// <param name="self_action">Action that will be done without RawInput listeners, Hotkeys and low-level hook.</param>
		public static void DoSelf(Action self_action) {
			if (selfie) {
				Logging.Log("Inside "+busy_on+"called: "+self_action.Method.Name);
				self_action();
			} else {
				Debug.WriteLine(">> DS" + self_action.Method.Name);
				MMain.mahou.Invoke((MethodInvoker)delegate {
                   	if (MMain.mahou.RemapCapslockAsF18) { LLHook.UnSet(); } MMain.mahou.UnregisterHotkeys(); });
				MMain.rif.Invoke((MethodInvoker)delegate{MMain.rif.RegisterRawInputDevices(IntPtr.Zero, WinAPI.RawInputDeviceFlags.Remove);});
				selfie = true;
				busy_on = self_action.Method.Name;
				self_action();
				MMain.mahou.Invoke((MethodInvoker)delegate {
                   	if (MMain.mahou.RemapCapslockAsF18) { LLHook.Set(); } MMain.mahou.RegisterHotkeys(); });
				MMain.rif.Invoke((MethodInvoker)delegate{MMain.rif.RegisterRawInputDevices(MMain.rif.Handle);});
				selfie = false;
				Debug.WriteLine(">> ES" + self_action.Method.Name);
			}
		}
		public static void StartConvertWord(YuKey[] YuKeys, uint wasLocale, bool skipsnip = false) {
			Logging.Log("Start Convert Word len: ["+YuKeys.Length+"], wl:"+wasLocale+", ss:"+skipsnip);
			DoSelf(() => {
				Debug.WriteLine(">> ST CLW");
				var backs = YuKeys.Length;
				// Fix for cmd exe pause hotkey leaving one char. 
				var clsNM = new StringBuilder(256);
				if (Environment.OSVersion.Version.Major >= 9 && 
				    clsNM.ToString() == "ConsoleWindowClass" && (
					MMain.mahou.HKCLast.VirtualKeyCode == (int)Keys.Pause))
					backs++;
				Debug.WriteLine(">> LC Aft. " + (MMain.locales.Length * 20));
				Logging.Log("Deleting old word, with lenght of [" + YuKeys.Length + "].");
				for (int e = 0; e < backs; e++) {
					SendBack();
				}
				if (MMain.mahou.UseDelayAfterBackspaces)
					Thread.Sleep(MMain.mahou.DelayAfterBackspaces);
				if(!skipsnip)
					c_snip.Clear();
				for (int i = 0; i < YuKeys.Length; i++) {
					if (YuKeys[i].altnum) {
						Logging.Log("An YuKey with [" + YuKeys[i].numpads.Count + "] numpad(s) passed.");
						KInputs.MakeInput(new[] {
							KInputs.AddKey(Keys.LMenu, true)
						});
						foreach (var numpad in YuKeys[i].numpads) {
							Logging.Log(numpad + " is being inputted.");
							KInputs.MakeInput(new[] {
								KInputs.AddKey(numpad, true)
							});
							KInputs.MakeInput(new[] {
								KInputs.AddKey(numpad, false)
							});
						}
						KInputs.MakeInput(new[] {
							KInputs.AddKey(Keys.LMenu, false)
						});
					} else {
						Logging.Log("An YuKey with state passed, key = {" + YuKeys[i].key + "}, upper = [" + YuKeys[i].upper + "].");
						var upp = YuKeys[i].upper && !Control.IsKeyLocked(Keys.CapsLock);
						if (upp)
							KInputs.MakeInput(new[] {
								KInputs.AddKey(Keys.LShiftKey, true)
							});
						if (!SymbolIgnoreRules(YuKeys[i].key, YuKeys[i].upper, wasLocale)) {
							KInputs.MakeInput(new[] {
								KInputs.AddKey(YuKeys[i].key, true)
							});
							KInputs.MakeInput(new[] {
								KInputs.AddKey(YuKeys[i].key, false)
							});
						}
						if (upp)
							KInputs.MakeInput(new[] {
								KInputs.AddKey(Keys.LShiftKey, false)
							});
						var c = new StringBuilder();
						var byu = new byte[256];
						if (YuKeys[i].upper) {
							byu[(int)Keys.ShiftKey] = 0xFF;
						}
						if (!skipsnip) {
							var loc = (Locales.GetCurrentLocale() & 0xffff);
							if (MahouUI.UseJKL)
								loc = MahouUI.currentLayout & 0xffff;
							WinAPI.ToUnicodeEx((uint)YuKeys[i].key, (uint)WinAPI.MapVirtualKey((uint)YuKeys[i].key, 0), byu, c, (int)5, (uint)0, (IntPtr)loc);
							c_snip.Add(c[0]);
						}
					}
				}
				Debug.WriteLine("XX CLW_END");
			});
		}
		/// <summary>
		/// Converts last word/line/words.
		/// </summary>
		/// <param name="c_">List of YuKeys to be converted.</param>
		public static void ConvertLast(List<YuKey> c_) {
			try { //Used to catch errors, since it called as Task
				Debug.WriteLine("Start CL");
				Debug.WriteLine(c_.Count + " LL");
				if (c_.Count <= 0)
					return;
				Locales.IfLessThan2();
				YuKey[] YuKeys = c_.ToArray();
				Logging.Log("Starting to convert word.");
				if (MahouUI.SoundOnConvLast)
					MahouUI.SoundPlay();
				if (MahouUI.SoundOnConvLast2)
					MahouUI.Sound2Play();
				var wasLocale = Locales.GetCurrentLocale() & 0xFFFF;
				var desl = ChangeLayout(true);
				if (MahouUI.UseJKL && MMain.mahou.SwitchBetweenLayouts && MMain.mahou.EmulateLS) {
					Debug.WriteLine("JKL-ed CLW");
					jklXHidServ.OnLayoutAction = desl;
					jklXHidServ.ActionOnLayout = () => StartConvertWord(YuKeys, wasLocale);
				} else
					StartConvertWord(YuKeys, wasLocale);
			} catch (Exception e) {
				Logging.Log("Convert Last encountered error, details:\r\n" +e.Message+"\r\n"+e.StackTrace, 1);
			}
			Debug.WriteLine("===============> Fin");
			MMain.mahou.UpdateLDs();
			Memory.Flush();
		}
		/// <summary>
		/// Rules to ignore symbols in ConvertLast() function.
		/// </summary>
		/// <param name="key">Key to be checked.</param>
		/// <param name="upper">State of key to be checked.</param>
		/// <param name="wasLocale">Last layout id.</param>
		/// <returns></returns>
		static bool SymbolIgnoreRules(Keys key, bool upper, uint wasLocale) {
			Logging.Log("Passing Key = ["+key+"]+["+(upper ? "UPPER" : "lower") + "] with WasLayoutID = ["+wasLocale+"] through symbol ignore rules.");
			if (MMain.mahou.HKSymIgn.Enabled &&
			    MMain.mahou.SymIgnEnabled &&
			    (wasLocale == 1033 || wasLocale == 1041) &&
			    ((Locales.AllList().Length < 3 && !MMain.mahou.SwitchBetweenLayouts) ||
			    MMain.mahou.SwitchBetweenLayouts) && (
			        key == Keys.Oem5 ||
			        key == Keys.OemOpenBrackets ||
			        key == Keys.Oem6 ||
			        key == Keys.Oem1 ||
			        key == Keys.Oem7 ||
			        key == Keys.Oemcomma ||
			        key == Keys.OemPeriod ||
			        key == Keys.OemQuestion)) {
				if (upper && key == Keys.OemOpenBrackets)
					KInputs.MakeInput(KInputs.AddString("{"));
				if (!upper && key == Keys.OemOpenBrackets)
					KInputs.MakeInput(KInputs.AddString("["));

				if (upper && key == Keys.Oem5)
					KInputs.MakeInput(KInputs.AddString("|"));
				if (!upper && key == Keys.Oem5)
					KInputs.MakeInput(KInputs.AddString("\\"));

				if (upper && key == Keys.Oem6)
					KInputs.MakeInput(KInputs.AddString("}"));
				if (!upper && key == Keys.Oem6)
					KInputs.MakeInput(KInputs.AddString("]"));

				if (upper && key == Keys.Oem1)
					KInputs.MakeInput(KInputs.AddString(":"));
				if (!upper && key == Keys.Oem1)
					KInputs.MakeInput(KInputs.AddString(";"));

				if (upper && key == Keys.Oem7)
					KInputs.MakeInput(KInputs.AddString("\""));
				if (!upper && key == Keys.Oem7)
					KInputs.MakeInput(KInputs.AddString("'"));

				if (upper && key == Keys.Oemcomma)
					KInputs.MakeInput(KInputs.AddString("<"));
				if (!upper && key == Keys.Oemcomma)
					KInputs.MakeInput(KInputs.AddString(","));

				if (upper && key == Keys.OemPeriod)
					KInputs.MakeInput(KInputs.AddString(">"));
				if (!upper && key == Keys.OemPeriod)
					KInputs.MakeInput(KInputs.AddString("."));

				if (upper && key == Keys.OemQuestion)
					KInputs.MakeInput(KInputs.AddString("?"));
				if (!upper && key == Keys.OemQuestion)
					KInputs.MakeInput(KInputs.AddString("/"));
				Memory.Flush();
				return true;
			} else
				return false;
		}
		public static bool IsConhost() {
			var strb = new StringBuilder(256);
			WinAPI.GetClassName(WinAPI.GetForegroundWindow(), strb, strb.Capacity);
			return strb.ToString().Contains("ConsoleWindowClass");
		}
		/// <summary>
		/// Changes current layout.
		/// </summary>
		public static uint ChangeLayout(bool quiet = false) {
			uint desired = 0;
			Debug.WriteLine(">> LC + SELF");
			DoSelf(() => {
				if (!quiet) {
					if (MahouUI.SoundOnLayoutSwitch)
						MahouUI.SoundPlay();
					if (MahouUI.SoundOnLayoutSwitch2)
						MahouUI.Sound2Play();
				}
				if (Locales.ActiveWindowProcess().ProcessName.ToLower() == "HD-Frontend".ToLower()) {
					KInputs.MakeInput(new [] { 
					                  	KInputs.AddKey(Keys.LControlKey, true),
					                  	KInputs.AddKey(Keys.Space, true),
					                  	KInputs.AddKey(Keys.Space, false),
					                  	KInputs.AddKey(Keys.LControlKey, false)});
					Thread.Sleep(13);
				} else {
					if (MMain.mahou.SwitchBetweenLayouts) {
						uint last = 0;
						bool conhost = false;
						if (MahouUI.UseJKL) {
							conhost = IsConhost();
						}
						for (int i=MMain.locales.Length; i!=0; i--) {
							var nowLocale = Locales.GetCurrentLocale();
							if (MahouUI.UseJKL) {
								if (nowLocale == 0 || conhost)
									nowLocale = MahouUI.currentLayout;
								if (last == nowLocale && nowLocale != 0) {
									nowLocale = MahouUI.currentLayout;
									desired = 0;
								}
							}
							if (nowLocale == desired)
								break;
							uint notnowLocale = nowLocale == MahouUI.MAIN_LAYOUT1
				                ? MahouUI.MAIN_LAYOUT2
				                : MahouUI.MAIN_LAYOUT1;
							last = nowLocale;
							ChangeToLayout(Locales.ActiveWindow(), notnowLocale, conhost);
							desired = notnowLocale;
							if (MMain.mahou.EmulateLS)
								break;
						}
					} else {
						if (MMain.mahou.EmulateLS) {
							CycleEmulateLayoutSwitch();
						} else {
							CycleLayoutSwitch();
						}
					}
				}
			});
			return desired;
		}
		/// <summary>
		/// Calls functions to change layout based on EmulateLS variable.
		/// </summary>
		/// <param name="hwnd">Target window to change its layout.</param>
		/// <param name="LayoutId">Desired layout to switch to.</param>
		public static void ChangeToLayout(IntPtr hwnd, uint LayoutId, bool conhost = false) {
			Debug.WriteLine(">> CTL");
			if (MMain.mahou.EmulateLS) 
				EmulateChangeToLayout(LayoutId, conhost);
			 else
			 	NormalChangeToLayout(hwnd, LayoutId, conhost);
		}
		/// <summary>
		/// Changing layout to LayoutId in hwnd with PostMessage and WM_INPUTLANGCHANGEREQUEST.
		/// </summary>
		/// <param name="hwnd">Target window to change its layout.</param>
		/// <param name="LayoutId">Desired layout to switch to.</param>
		static void NormalChangeToLayout(IntPtr hwnd, uint LayoutId, bool conhost = false) {
			Debug.WriteLine(">> N-CTL");
			Logging.Log("Changing layout using normal mode, WinAPI.SendMessage [WinAPI.WM_INPUTLANGCHANGEREQUEST] with LParam ["+LayoutId+"].");
			int tries = 0;
			uint last = 0;
			var loc = Locales.GetCurrentLocale();
			//Cycles while layout not changed
			do {
				if (MahouUI.UseJKL)
					if ((loc == last && loc != 0) || conhost)
						loc = MahouUI.currentLayout;
				WinAPI.SendMessage(hwnd, (int)WinAPI.WM_INPUTLANGCHANGEREQUEST, 0, (UIntPtr)LayoutId);
				Thread.Sleep(10);//Give some time to switch layout
				tries++;
				if (tries == MMain.locales.Length) {
					Logging.Log("Tries break, probably failed layout changing...",1);
					break;
				}
				last = loc;
			} while (loc != LayoutId);
//			if (!MahouUI.UseJKL) // Wow, gives no sense!!
				MahouUI.currentLayout = MahouUI.GlobalLayout = LayoutId;
		}
		static bool failed = true;
		/// <summary>
		/// Changing layout to LayoutId by emulating windows layout switch hotkey. 
		/// </summary>
		/// <param name="LayoutId">Desired layout to switch to.</param>
		static void EmulateChangeToLayout(uint LayoutId, bool conhost = false) {
			Debug.WriteLine(">> E-CTL");
			var last = MahouUI.currentLayout;
			if (last == LayoutId) {
				if (!conhost && last == Locales.GetCurrentLocale()) {
					Debug.WriteLine("Layout already " + LayoutId);
					return;
				}
				Debug.WriteLine("False, layout isn't actually #"+last);
			}
			Logging.Log("Changing to specific layout ["+LayoutId+"] by emulating layout switch.");
			for (int i = MMain.locales.Length; i != 0; i--) {
				uint loc = Locales.GetCurrentLocale();
//				Debug.WriteLine(loc + " " + last);
				if (MahouUI.UseJKL && ((loc == 0 || loc == last) || conhost)) {
					jklXHidServ.start_cyclEmuSwitch = true;
					jklXHidServ.cycleEmuDesiredLayout = LayoutId;
					Debug.WriteLine("LI: " + LayoutId);
					CycleEmulateLayoutSwitch();
					break;
				} else {
//					Debug.WriteLine(i+".LayoutID: " + LayoutId + ", loc: " +loc);
					if (loc == LayoutId) {
						failed = false;
						break;
					}
					CycleEmulateLayoutSwitch();
					Thread.Sleep(30);
				}
				last = loc;
				if (!failed)
					break;
			}
			if (!MahouUI.UseJKL)
				if (!failed) {
					MahouUI.currentLayout = MahouUI.GlobalLayout = LayoutId;
				} else
					Logging.Log("Changing to layout [" + LayoutId + "] using emulation failed after # of layouts tries,\r\nmaybe you have more that 16 layouts, disabled change layout hotkey in windows, or working in console window(use getconkbl.dll)?", 1);
			failed = true;
		}
		/// <summary>
		/// Changing layout by emulating windows layout switch hotkey
		/// </summary>
		public static void CycleEmulateLayoutSwitch() {
			Debug.WriteLine(">> CELS");
			if (MMain.mahou.EmulateLSType == "Alt+Shift") {
				Logging.Log("Changing layout using cycle mode by simulating key press [Alt+Shift].");
				//Emulate Alt+Shift
				KInputs.MakeInput(new [] {
					KInputs.AddKey(Keys.LMenu, true),
					KInputs.AddKey(Keys.LShiftKey, true),
					KInputs.AddKey(Keys.LShiftKey, false),
					KInputs.AddKey(Keys.LMenu, false)
				});
			} else if (MMain.mahou.EmulateLSType == "Ctrl+Shift") {
				Logging.Log("Changing layout using cycle mode by simulating key press [Ctrl+Shift].");
				//Emulate Ctrl+Shift
				KInputs.MakeInput(new [] {
					KInputs.AddKey(Keys.LControlKey, true),
					KInputs.AddKey(Keys.LShiftKey, true),
					KInputs.AddKey(Keys.LShiftKey, false),
					KInputs.AddKey(Keys.LControlKey, false)
				});
			} else {
				Logging.Log("Changing layout using cycle mode by simulating key press [Win+Space].");
				//Emulate Win+Space
				KInputs.MakeInput(new [] {
					KInputs.AddKey(Keys.LWin, true),
					KInputs.AddKey(Keys.Space, true),
					KInputs.AddKey(Keys.Space, false),
					KInputs.AddKey(Keys.LWin, false)
				});
				Thread.Sleep(20); //Important!
			}
			if (!MahouUI.UseJKL)
				DoLater(() => { MahouUI.currentLayout = MahouUI.GlobalLayout = Locales.GetCurrentLocale(); }, 10);
		}
		public static Locales.Locale GetNextLayout(uint before = 0) {
			Debug.WriteLine(">> GNL");
			var loc = new Locales.Locale();
			uint last = 0;
			for (int i=MMain.locales.Length; i!=0; i--) {
				var br = false;
				var cur = Locales.GetCurrentLocale(); 
				if (last != 0 && cur != last)
					break;
				if (MahouUI.UseJKL)
					if (cur == 0 || cur == last)
						cur = MahouUI.currentLayout;
				if (before != 0)
					cur = before;
				Debug.WriteLine("Current: " +cur);
				if (MMain.mahou.SwitchBetweenLayouts) {
					if (cur == MahouUI.MAIN_LAYOUT1) 
						loc.uId = MahouUI.MAIN_LAYOUT2;
					else if (cur == MahouUI.MAIN_LAYOUT2) {
						loc.uId = MahouUI.MAIN_LAYOUT1;
					} else 
						loc.uId = MahouUI.MAIN_LAYOUT1;
					break;
				}
				Thread.Sleep(5);
				var curind = MMain.locales.ToList().FindIndex(lid => lid.uId == cur);
				for (int g=0; g != MMain.locales.Length; g++) {
					var l = MMain.locales[g];
					Debug.WriteLine("Checking: " + l.Lang + ", with "+cur);
					if (curind == MMain.locales.Length - 1) {
						Logging.Log("Locales BREAK!");
						loc = MMain.locales[0];
						br = true;
						break;
					}
					Logging.Log("LIDC = "+g +" curid = "+curind + " Lidle = " +(MMain.locales.Length - 1));
					if (l.Lang.Contains("Microsoft Office IME")) // fake layout
						continue;
					if (g > curind)
						if (l.uId != cur) {
							Logging.Log("Locales +1 Next BREAK on " + l.uId);
							loc = l;
							if (last !=0) // ensure its checked at least twice
								br = true;
							break;
					}
				}
				last = cur;
				if (br)
					break;
			}
			Debug.WriteLine("Next layout: " + loc.Lang);
			return loc;
		}
		/// <summary>
		/// Changing layout to next with PostMessage and WM_INPUTLANGCHANGEREQUEST and LParam HKL_NEXT.
		/// </summary>
		public static void CycleLayoutSwitch() {
			Debug.WriteLine(">> CLS");
			Logging.Log("Changing layout using cycle mode by sending Message [WinAPI.WM_INPUTLANGCHANGEREQUEST] with LParam [HKL_NEXT] using WinAPI.PostMessage to ActiveWindow");
			//Use WinAPI.PostMessage to switch to next layout
			ChangeToLayout(Locales.ActiveWindow(), GetNextLayout().uId);
		}
		/// <summary>
		/// Converts character(c) from layout(uID1) to another layout(uID2) by using WinAPI.ToUnicodeEx().
		/// </summary>
		/// <param name="c">Character to be converted.</param>
		/// <param name="uID1">Layout id 1(from).</param>
		/// <param name="uID2">Layout id 2(to)</param>
		/// <returns></returns>
		static string InAnother(char c, uint uID1, uint uID2)  { //Remakes c from uID1  to uID2			var cc = c;
			var chsc = WinAPI.VkKeyScanEx(cc, uID1);
			var state = (chsc >> 8) & 0xff;
			var byt = new byte[256];
			//it needs just 1 but,anyway let it be 10, i think that's better
			var s = new StringBuilder(10);
			//Checks if 'chsc' have upper state
			if (state == 1) {
				byt[(int)Keys.ShiftKey] = 0xFF;
			}
			//"Convert magic✩" is the string below
			var ant = WinAPI.ToUnicodeEx((uint)chsc, (uint)chsc, byt, s, s.Capacity, 0, (IntPtr)uID2);
			return chsc != -1 ? s.ToString() : "";
		}
		/// <summary>
		/// Simplified WinAPI.keybd_event() with extended recognize feature.
		/// </summary>
		/// <param name="key">Key to be inputted.</param>
		/// <param name="flags">Flags(state) of key.</param>
		public static void KeybdEvent(Keys key, int flags)  { // 			//Do not remove this line, it needed for "Left Control Switch Layout" to work properly
//			Thread.Sleep(15);
			WinAPI.keybd_event((byte)key, 0, flags | (KInputs.IsExtended(key) ? 1 : 0), 0);
		}
		public static void RePressAfter(int mods) {
			ctrlRP = Hotkey.ContainsModifier(mods, (int)WinAPI.MOD_CONTROL);
			shiftRP = Hotkey.ContainsModifier(mods, (int)WinAPI.MOD_SHIFT);
			altRP = Hotkey.ContainsModifier(mods, (int)WinAPI.MOD_ALT);
			winRP = Hotkey.ContainsModifier(mods, (int)WinAPI.MOD_WIN);
		}
		/// <summary>
		/// Sends modifiers up by modstoup array. 
		/// </summary>
		/// <param name="modstoup">Array of modifiers which will be send up. 0 = ctrl, 1 = shift, 2 = alt.</param>
		public static void SendModsUp(int modstoup)  { //			//These three below are needed to release all modifiers, so even if you will still hold any of it
			//it will skip them and do as it must.
			if (modstoup <= 0) return;
			Debug.WriteLine(">> SMU: " + Hotkey.GetMods(modstoup));
			DoSelf(() => {
				if (Hotkey.ContainsModifier(modstoup, (int)WinAPI.MOD_WIN)) {
					KMHook.KeybdEvent(Keys.LWin, 2); // Right Win Up
					KMHook.KeybdEvent(Keys.RWin, 2); // Left Win Up
					win = win_r = false;
					LLHook.SetModifier(WinAPI.MOD_WIN, false);
				}
				if (Hotkey.ContainsModifier(modstoup, (int)WinAPI.MOD_SHIFT)) {
					KMHook.KeybdEvent(Keys.RShiftKey, 2); // Right Shift Up
					KMHook.KeybdEvent(Keys.LShiftKey, 2); // Left Shift Up
					shift = shift_r = false;
					LLHook.SetModifier(WinAPI.MOD_SHIFT, false);
				}
				if (Hotkey.ContainsModifier(modstoup, (int)WinAPI.MOD_CONTROL)) {
					KMHook.KeybdEvent(Keys.RControlKey, 2); // Right Control Up
					KMHook.KeybdEvent(Keys.LControlKey, 2); // Left Control Up
					ctrl = ctrl_r = false;
					LLHook.SetModifier(WinAPI.MOD_CONTROL, false);
				}
				if (Hotkey.ContainsModifier(modstoup, (int)WinAPI.MOD_ALT)) {
					KMHook.KeybdEvent(Keys.RMenu, 2); // Right Alt Up
					KMHook.KeybdEvent(Keys.LMenu, 2); // Left Alt Up
					alt = alt_r = false;
					LLHook.SetModifier(WinAPI.MOD_ALT, false);
				}
				Logging.Log("Modifiers ["+modstoup+ "] sent up.");
              });
		}
		/// <summary>
		/// Checks if key is modifier, and calls SendModsUp() if it is.
		/// </summary>
		/// <param name="key">Key to be checked.</param>
		public static void IfKeyIsMod(Keys key) {
			uint mods = 0;
			switch (key) {
				case Keys.LControlKey:
				case Keys.RControlKey:
					mods += WinAPI.MOD_CONTROL;
					break;
				case Keys.LShiftKey:
				case Keys.RShiftKey:
					mods += WinAPI.MOD_SHIFT;
					break;
				case Keys.LMenu:
				case Keys.RMenu:
				case Keys.Alt:
					mods += WinAPI.MOD_ALT;
					break;
				case Keys.LWin:
				case Keys.RWin:
					mods += WinAPI.MOD_WIN;
					break;
			}
			if (mods > 0)
				SendModsUp((int)mods);
		}
		public static Tuple<string, uint> WordGuessLayout(string word, uint _target = 0) {
			uint layout = 0;
			string guess = "";
			uint target = 0;
			if (_target == 0) {
				if (MMain.mahou.SwitchBetweenLayouts) {
					var cur = Locales.GetCurrentLocale();
					if (MahouUI.UseJKL)
						cur = MahouUI.currentLayout;
					target = cur == MahouUI.MAIN_LAYOUT1 ? MahouUI.MAIN_LAYOUT2 : MahouUI.MAIN_LAYOUT1;
				} else 
					target = GetNextLayout().uId;
			} else target = _target;
			for (int i = 0; i != MMain.locales.Length; i++ ) {
				if (MMain.locales[i].Lang.Contains("Microsoft Office IME")) // fake layout
					continue;
				var l = MMain.locales[i].uId;
				var l2 = target;
				if (l == target) continue;
				int wordLMinuses = 0;
				int wordL2Minuses = 0;
				int minmin = 0;
				int thismin = 0;
				uint lay = 0;
				var wordL = "";
				var wordL2 = "";
				var result = "";
				Debug.WriteLine("Testing " +word+" against: " +l+" and "+l2);
				for (int I = 0; I!=word.Length; I++) {
					var c = word[I];
					var sm = false;
					if (c == 'ո' || c == 'Ո') {
						if (c == 'ո') sm = true;
						if (word.Length > I+1) {
							if (word[I+1] == 'ւ') {
								var shrt = l2 & 0xffff;
								var _shrt = l & 0xffff;
								if (shrt == 1033 || shrt == 1041) {
									wordL += sm ? "u" : "U";
									I++; continue;
								}
								if (_shrt == 1033 || _shrt == 1041) {
									wordL2 += sm ? "u" : "U";
									I++; continue;
								}
								if (shrt == 1049) {
									wordL += sm ? "г" : "Г";
									I++; continue;
								}
								if (_shrt == 1049) {
									wordL2 += sm ? "г" : "Г";
									I++; continue;
								}
							}
						}
					}
					var T3 = GermanLayoutFix(c);
					if (T3 != "") {
						wordL += T3;
						wordL2 += T3;
						continue;
					}
					if (c == '\n') {
						wordL += "\n";
						wordL2 += "\n";
						continue;
					}
					var T1 = InAnother(c, l & 0xffff, l2 & 0xffff);
					wordL += T1;
					if (T1 == "") wordLMinuses++;
					var T2 = InAnother(c, l2 & 0xffff, l & 0xffff);
					wordL2 += T2;
					if (T2 == "") wordL2Minuses++;
//					Debug.WriteLine("T1: "+ T1 + ", T2: "+ T2 + ", C: " +c);
					if (T2 == "" && T1 == "") {
//							Debug.WriteLine("Char ["+c+"] is not in any of two layouts ["+l+"], ["+l2+"] just rewriting.");
						wordL += word[I].ToString();
						wordL2 += word[I].ToString();
					}
				}
				if (wordLMinuses > wordL2Minuses) {
					thismin = wordL2Minuses;
					lay = l2;
					result = wordL2;
				}
				else {
					thismin = wordLMinuses;
					lay = l;
					result = wordL;
				}
//				Debug.WriteLine("End, " + lay + "|" +wordL + ", " + wordL2 + "|" +wordLMinuses + ", " +wordL2Minuses);
				if (wordLMinuses == wordL2Minuses) {
					thismin = wordLMinuses;
					lay = 0;
					result = word;
				}
				if (result.Length > guess.Length || (lay != 0 && thismin <= minmin)) {
					guess = result;
					layout = lay;
				}
				if (thismin < minmin)
					minmin = thismin;
				if (lay == target) break;
			}
			if (target == layout) 
				guess = word;
			if (layout == target) {
				guess_tries++;
				Debug.WriteLine("WARNING! Guess Try [#"+guess_tries+"], target layout and word layout are same!, taking next layout as target!");
				if (guess_tries < MMain.locales.Length+1) {
					target = GetNextLayout(target).uId;
					Debug.WriteLine("Retry with: layout: " +layout +", target: " + target);
					return WordGuessLayout(word, target);
				} else {
					guess_tries = 0;
				}
			} else {
				guess_tries = 0;
			}
			Debug.WriteLine("Word " + word + " layout is " + layout + " targeting: " + target +" guess: " + guess);
			return Tuple.Create(guess, layout);
		}
		public static Tuple<bool, int> SnippetsLineCommented(string snippets, int k) {
			if (k == 0 || (k-1 >= 0 && snippets[k-1].Equals('\n'))) { // only at every new line
				var end = snippets.IndexOf('\n', k);
				if (end==-1)
					end=snippets.Length;
				var l = end-k-1;
				if (end==-1)
					l = end-k;
				if (end == k)
					l = 0;
				var line = snippets.Substring(k, l);
				if (line.Length > 0) // Ingore empty lines
					if (line[0] == '#' || (line[0] == '/' && (line.Length > 1 && line[1] == '/'))) {
						Logging.Log("Ignored commented line in snippets:[" + line + "].");
						return new Tuple<bool, int>(true, line.Length-1);
					}
			}
			return new Tuple<bool, int>(false, 0);
		}
		public static void GetSnippetsData(string snippets, bool isSnip = true) {
			var leng = 0;
			if (isSnip)
				leng = MahouUI.SnippetsCount;
			else
				leng = MahouUI.AutoSwitchCount;
			string[] smalls = new string[leng+1024];
			string[]  bigs = new string[leng+1024];
			if (String.IsNullOrEmpty(snippets)) return;
			snippets = snippets.Replace("\r", "");
			int last_exp_len = 0, ids = 0, idb = 0, add_alias = 0;
			for (int k = 0; k < snippets.Length-6; k++) {
				var com = SnippetsLineCommented(snippets, k);
				if (com.Item1) {
					k+=com.Item2; // skip commented line, speedup!
					continue;
				}
				if ((last_exp_len <= 0 || last_exp_len-- == 0) && snippets[k].Equals('-') && snippets[k+1].Equals('>')) {
					var len = -1;
					var endl = snippets.IndexOf('\n', k+2);
					if (endl==-1)
						endl=snippets.Length;
//					Debug.WriteLine((k+2) + " X " +endl);
					string cool = snippets.Substring(k+2, endl - (k+2));
					if (cool.Length > 4)
						for (int i = 0; i != cool.Length-5; i ++) {
							if (cool[i].Equals('=') && cool[i+1].Equals('=') && cool[i+2].Equals('=') && cool[i+3].Equals('=') && cool[i+4].Equals('>')) {
								len = i;
							}
						}
					else 
						len = cool.Length;
					if (len == -1)
						len = endl-(k+2);
					var sm = snippets.Substring(k+2, len).Replace("\r", "");
					if (sm.Contains("|")) {
						var esm = sm.Replace("||", pipe_esc);
						foreach (var n in esm.Split('|')) {
							smalls[ids] = n.Replace(pipe_esc , "|");
//							Debug.WriteLine("ADded sm alias: " +ids + ", ++ " + smalls[ids]);
							ids++;
							add_alias++;
						}
					} else {
						smalls[ids] = sm;
						ids++;
					}
				}
				if (snippets[k].Equals('=') && snippets[k+1].Equals('=') && snippets[k+2].Equals('=') && snippets[k+3].Equals('=') && snippets[k+4].Equals('>')) {
					var endl = snippets.IndexOf('\n', k+2);
					if (endl==-1)
						endl=snippets.Length;
					var pool = snippets.Substring(k+5, endl - (k+5));
					if(isSnip)
						pool = snippets.Substring(k+5);
					StringBuilder pyust = new StringBuilder(); // Should be faster than string +=
					for (int g = 0; g != pool.Length-5; g++) {
						if (pool[g].Equals('<') && pool[g+1].Equals('=') && pool[g+2].Equals('=') && pool[g+3].Equals('=') && pool[g+4].Equals('='))
							break;
						pyust.Append(pool[g]);
					}
					last_exp_len = pyust.Length;
					if (add_alias != 0) {
						while (add_alias != 0) {
//							Debug.WriteLine("ADded exp alias: " +idb + ", ++ " + pyust);
							bigs[idb] = (pyust.ToString());
							idb++;
							add_alias--;
						}
					} else {
						bigs[idb] = (pyust.ToString());
						idb++;
					}
					k+=5;
				}
			}
			if (isSnip) {
//				snipps = exps = null;
//				Memory.Flush();
				snipps = smalls;
				exps = bigs;
			} else {
//				as_wrongs = as_corrects = null;
//				Memory.Flush();
				as_wrongs = smalls;
				as_corrects = bigs;
				
			}
		}
		/// <summary>
		/// Re-Initializes snippets.
		/// </summary>
		public static void ReInitSnippets() {
			if (System.IO.File.Exists(MahouUI.snipfile)) {
				var snippets = System.IO.File.ReadAllText(MahouUI.snipfile);
				Stopwatch watch = null;
				if (MahouUI.LoggingEnabled) {
					watch = new Stopwatch();
					watch.Start();
				}
				GetSnippetsData(snippets);
				if (MahouUI.LoggingEnabled) {
					watch.Stop();
					Logging.Log("Snippets init finished, elapsed ["+watch.Elapsed.TotalMilliseconds+"] ms.");
					watch.Reset();
					watch.Start();
				}
				if (MahouUI.AutoSwitchEnabled)
					GetSnippetsData(MahouUI.AutoSwitchDictionaryRaw, false);
				else {
					as_wrongs = as_corrects = null;
					Memory.Flush();
				}
				if (MahouUI.LoggingEnabled && MahouUI.AutoSwitchEnabled) {
					watch.Stop();
					Logging.Log("AutoSwitch dictionary init finished, elapsed ["+watch.Elapsed.TotalMilliseconds+"] ms.");
				}
			}
			Memory.Flush();
		}
		#region Snippets Aliases
		static readonly string pipe_esc = "__pipeEscape::";
		#endregion
		/// <summary>
		///  Contains key(Keys key), it state(bool upper), if it is Alt+[NumPad](bool altnum) and array of numpads(list of numpad keys).
		/// </summary>
		public struct YuKey {
			public Keys key;
			public bool upper;
			public bool altnum;
			public List<Keys> numpads;
		}
		#endregion
	}
}
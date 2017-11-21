using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Mahou
{
	/// <summary>Ini settings writer/reader in memory, not from disk.</summary>
	class INI {
		#region Variables
		/// <summary>Raw INI configs file</summary>
		public string Raw;
		/// <summary>Split into lines INI configs file</summary>
		public string[] lines;
		public bool DEBUG;
		#endregion
		
		public INI(string ini, bool dbg = false) {
			this.Raw = ini;
			this.lines = Raw.Replace("\r", "").Split('\n');
			this.DEBUG = dbg;
		}
		#region Debug
		public void log(string str) {
			if (!DEBUG) return;
			Logging.Log(str);
		}
		#endregion
		#region Has/Is
		public bool IsCommented(string line) {
			if (String.IsNullOrEmpty(line)) return false;
			if (line[0] == '!' || line[0] == ';') {
				log("Commented line: " + line);
				return true;
			}
			return false;
		}
		public int HasSection(string Section) {
			for (int a = 0; a != lines.Length; a++) {
				log("Line => " + lines[a]);
				if (IsCommented(lines[a]))
					continue;
				if (lines[a] == "["+Section+"]") {
					return a;
				}
			}
			return -1;
		}
		public int HasValue(int sect, string ValueName) {
			if (sect == -1) return -1;
			else {
				log("SECT LINE: " + sect);
				for (int i = sect+1; i != lines.Length; i++) {
					var line = lines[i];
					if (IsCommented(line))
						continue;
					if (line.Length <= 1) {
						log("--EMPTY LINE!");
						return -1;
					}
					if (line[0] == '[' && line[line.Length-1] == ']') {
						log("--NEXT SECT!");
						return -1;
					}
					var valeq = line.Split('=')[0];
					log(">>Value Line => " + line + " I: " + i);
					// log("ValEq: " + valeq);
					if (valeq == ValueName) {
						log("===Has value: " + ValueName);
						return i;
					}
				}
			}
			return -1;
		}
		#endregion
		#region Writing
		string[] AddLine(string NewLine, int pos, string[] source) {
			var _source = new string[source.Length+1];
			if (pos == -1) {
				_source[0] = NewLine;
				Array.Copy(source, pos+1, _source, pos+2, source.Length-1);
			} else { 
				if (pos != 0)
					Array.Copy(source, 0, _source, 0, pos);
				else 
					_source[0] = source[0];
				_source[pos] = source[pos];
				_source[pos+1] = NewLine;
				Array.Copy(source, pos+1, _source, pos+2, source.Length-1-pos);
			}
			return _source;
		}
		public void SetValue(string Section, string ValueName, string Value) {
			var sect = HasSection(Section);
			var val_line = HasValue(sect, ValueName);
			if (sect == -1) {
				log("  NO SUCH SECT! " + Section);
				lines = AddLine("["+Section+"]", sect, lines);
				sect = 0;
				val_line = -1;
			}
			if (val_line > -1) {
				lines[val_line] = ValueName + "=" + Value;
			}
			if (val_line == -1) {
				log("   NO SUCH VALUE! " + ValueName);
				lines = AddLine(ValueName + "=" + Value, sect, lines);
			}
			if (val_line == -1 || val_line > -1 || sect == -1) {
				Raw = string.Join(Environment.NewLine, lines);
				lines = Raw.Replace("\r", "").Split('\n');
			}
		}
		#endregion
		#region Reading
		public string GetValue(string Section, string ValueName) {
			log("Getting value :"+ValueName+": from section ["+Section+"].");
			var sect = HasSection(Section);
			var val_line = HasValue(sect, ValueName);
			if (val_line < 0) return "";
			return lines[val_line].Split('=')[1];
		}
		#endregion
	}
    class Configs {
    	public static bool forceAppData;
    	public static bool fine = false;
        /// <summary> Mahou.ini file path. </summary>
        public static string filePath = Path.Combine(MahouUI.nPath, "Mahou.ini");
        
        public INI _INI;
        /// <summary> Creates if it is not exist and test that configs file Mahou.ini its readable, on startup can create dialog about forced AppData configs if configs file failed to be created/readen. </summary>
        public static void CreateConfigsFile() {
        	if (File.Exists(Path.Combine(MahouUI.mahou_folder_appd,".force"))) {
        		filePath = Path.Combine(MahouUI.mahou_folder_appd, "Mahou.ini");
        		forceAppData = true;
        	}
        	bool create = true;
        	try {
	        	if (!File.Exists(filePath)) { //Create an UTF-16 configuration file
	                File.WriteAllText(filePath, "!Unicode(✔), Mahou settings file", Encoding.Unicode);
        		} else { 
			    	using (var sr = new StreamReader(filePath)) {
			    				sr.Read();
		        		}
        		}
        		fine = true;
        	} catch(Exception e) {
        		fine = false;
        		if (MessageBox.Show(MMain.Lang[Languages.Element.ConfigsCannot]+(create ? MMain.Lang[Languages.Element.Created] : MMain.Lang[Languages.Element.Readen])+", "+ MMain.Lang[Languages.Element.Error].ToLower() + ":\r\n" + e.Message + "\r\n" + MMain.Lang[Languages.Element.RetryInAppData],
        		                    MMain.Lang[Languages.Element.Error], MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes) {
        			if (!Directory.Exists(MahouUI.mahou_folder_appd))
        				Directory.CreateDirectory(MahouUI.mahou_folder_appd);
        			filePath = Path.Combine(MahouUI.mahou_folder_appd, "Mahou.ini");
        			File.Create(Path.Combine(MahouUI.mahou_folder_appd,".force"));
        			MMain.MyConfs = new Configs();
        		} else 
        		System.Diagnostics.Process.GetCurrentProcess().Kill();
        	}
        }
        /// <summary> Check if configs file readable. </summary>
        /// <returns>Read access.</returns>
	        public static bool Readable() {
        	try {
		    	using (var sr = new StreamReader(filePath)) {
	    				sr.Read();
        		}
        	} catch(Exception e) { Logging.Log("Configs file ["+filePath+"] cannot be readen, error:\r\n" + e.Message); return false; }
        	return true;
        }
        /// <summary> Initializes settings, if some of values or settings file, not exists it creates them with default value. </summary>
        public Configs() {
        	CreateConfigsFile();
        	ReadFromDisk();
        	#region FirstStart section
            CheckBool("FirstStart", "First", "true");
        	#endregion
            #region Functions section
            CheckBool("Functions", "AutoStartAsAdmin", "false");
            CheckBool("Functions", "TrayIconVisible", "true");
            CheckBool("Functions", "ConvertSelectionLayoutSwitching", "false");
            CheckBool("Functions", "ReSelect", "true");
            CheckBool("Functions", "RePress", "false");
            CheckBool("Functions", "AddOneSpaceToLastWord", "false");
            CheckBool("Functions", "AddOneEnterToLastWord", "false");
            CheckBool("Functions", "ConvertSelectionLayoutSwitchingPlus", "false");
            CheckBool("Functions", "ScrollTip", "false");
            CheckBool("Functions", "StartupUpdatesCheck", "false");
            CheckBool("Functions", "SilentUpdate", "false");
            CheckBool("Functions", "Logging", "false");
            CheckBool("Functions", "CapsLockTimer", "false");
            CheckBool("Functions", "TrayFlags", "true");
            CheckBool("Functions", "BlockMahouHotkeysWithCtrl", "false");
            CheckBool("Functions", "SymbolIgnoreModeEnabled", "false");
            CheckBool("Functions", "MCDServerSupport", "false");
            CheckBool("Functions", "OneLayoutWholeWord", "true");
            CheckBool("Functions", "GuessKeyCodeFix", "false");
            CheckBool("Functions", "AppDataConfigs", forceAppData.ToString());
            CheckBool("Functions", "RemapCapslockAsF18", "true");
            #endregion
			#region Layouts section
			CheckBool("Layouts", "SwitchBetweenLayouts", "true");
			CheckBool("Layouts", "OneLayout", "false");
			CheckBool("Layouts", "EmulateLayoutSwitch", "false");
            CheckString("Layouts", "EmulateLayoutSwitchType", "Alt+Shift");
			CheckBool("Layouts", "ChangeToSpecificLayoutByKey", "true");
			CheckString("Layouts", "MainLayout1", "");
			CheckString("Layouts", "MainLayout2", "");
			CheckInt("Layouts", "SpecificKey1", "1");
			CheckInt("Layouts", "SpecificKey2", "0");
			CheckInt("Layouts", "SpecificKey3", "0");
			CheckInt("Layouts", "SpecificKey4", "0");
			CheckString("Layouts", "SpecificLayout1", Languages.English[Languages.Element.SwitchBetween]);
			CheckString("Layouts", "SpecificLayout2", "");
			CheckString("Layouts", "SpecificLayout3", "");
			CheckString("Layouts", "SpecificLayout4", "");
			CheckInt("Layouts", "SpecificKeysType", "0");
			CheckString("Layouts", "SpecificKeySets", "set_0");
			CheckBool("Layouts", "QWERTZfix", "false");
			#endregion
			#region Persistent Layout
			CheckBool("PersistentLayout", "OnlyOnWindowChange", "false");
			CheckBool("PersistentLayout", "ChangeOnlyOnce", "false");
			CheckBool("PersistentLayout", "ActivateForLayout1", "false");
			CheckBool("PersistentLayout", "ActivateForLayout2", "false");
			CheckInt("PersistentLayout", "Layout1CheckInterval", "50");
			CheckInt("PersistentLayout", "Layout2CheckInterval", "50");
			CheckString("PersistentLayout", "Layout1Processes", "devenv.exe wdexpress.exe");
			CheckString("PersistentLayout", "Layout2Processes", "notepad++.exe winword.exe");
			#endregion
			#region Appearence section
            CheckBool("Appearence", "DisplayLangTooltipForMouse", "false");
            CheckBool("Appearence", "DisplayLangTooltipForMouseOnChange", "false");
            CheckBool("Appearence", "DisplayLangTooltipForCaret", "false");
            CheckBool("Appearence", "DisplayLangTooltipForCaretOnChange", "false");
            CheckBool("Appearence", "DifferentColorsForLayouts", "false");
            CheckBool("Appearence", "MouseLTAlways", "false");
			CheckString("Appearence", "Language", "English");
			// Language tooltip appearence for Layout 1
			CheckString("Appearence", "Layout1ForeColor", "#000000");
			CheckString("Appearence", "Layout1BackColor", "#FFFFFF");
            CheckBool("Appearence", "Layout1TransparentBackColor", "false");
			CheckString("Appearence", "Layout1Font", "Georgia; 8pt");
			CheckInt("Appearence", "Layout1Height", "14");
			CheckInt("Appearence", "Layout1Width", "26");
			CheckInt("Appearence", "Layout1PositionX", "8");
			CheckInt("Appearence", "Layout1PositionY", "0");
			// Language tooltip appearence for Layout 2
			CheckString("Appearence", "Layout2ForeColor", "#000000");
			CheckString("Appearence", "Layout2BackColor", "#FFFFFF");
            CheckBool("Appearence", "Layout2TransparentBackColor", "false");
			CheckString("Appearence", "Layout2Font", "Georgia; 8pt");
			CheckInt("Appearence", "Layout2Height", "14");
			CheckInt("Appearence", "Layout2Width", "26");
			CheckInt("Appearence", "Layout2PositionX", "8");
			CheckInt("Appearence", "Layout2PositionY", "0");
			// Language tooltip appearence for Mouse Language Tooltip
			CheckString("Appearence", "MouseLTForeColor", "#000000");
			CheckString("Appearence", "MouseLTBackColor", "#FFFFFF");
            CheckBool("Appearence", "MouseLTTransparentBackColor", "false");
			CheckString("Appearence", "MouseLTFont", "Georgia; 8pt");
			CheckInt("Appearence", "MouseLTHeight", "14");
			CheckInt("Appearence", "MouseLTWidth", "26");
			CheckInt("Appearence", "MouseLTPositionX", "8");
			CheckInt("Appearence", "MouseLTPositionY", "0");
			// Language tooltip appearence for Caret Language Tooltip
			CheckString("Appearence", "CaretLTForeColor", "#000000");
			CheckString("Appearence", "CaretLTBackColor", "#FFFFFF");
            CheckBool("Appearence", "MouseLTTransparentBackColor", "false");
            CheckBool("Appearence", "CaretLTTransparentBackColor", "false");
			CheckString("Appearence", "CaretLTFont", "Georgia; 8pt");
			CheckInt("Appearence", "CaretLTHeight", "14");
			CheckInt("Appearence", "CaretLTWidth", "26");
			CheckInt("Appearence", "CaretLTPositionX", "8");
			CheckInt("Appearence", "CaretLTPositionY", "12");
			// Language tooltip positions for Mahou Cared Display Server
			CheckInt("Appearence", "MCDS_Pos_X", "58");
			CheckInt("Appearence", "MCDS_Pos_Y", "13");
			CheckInt("Appearence", "MCDS_Top", "60");
			CheckInt("Appearence", "MCDS_Bottom", "45");
			// Language tooltips use flags
            CheckBool("Appearence", "MouseLTUseFlags", "false");
            CheckBool("Appearence", "CaretLTUseFlags", "false");
			// Different text for layouts
			CheckString("Appearence", "Layout1LTText", "");
			CheckString("Appearence", "Layout2LTText", "");
			// Upper arrows for lang displays
			CheckBool("Appearence", "MouseLTUpperArrow", "false");
			CheckBool("Appearence", "CaretLTUpperArrow", "false");
			// Windows Messages instead of timers
			CheckBool("Appearence", "WindowsMessages", "true");
			#endregion
			#region Timings section
			CheckInt("Timings", "LangTooltipForMouseRefreshRate", "25");
			CheckInt("Timings", "LangTooltipForCaretRefreshRate", "25");
			CheckInt("Timings", "DoubleHotkey2ndPressWait", "350");
			CheckInt("Timings", "FlagsInTrayRefreshRate", "100");
			CheckInt("Timings", "ScrollLockStateRefreshRate", "100");
			CheckInt("Timings", "CapsLockDisableRefreshRate", "100");
			CheckBool("Timings", "SelectedTextGetMoreTries", "false");
			CheckInt("Timings", "SelectedTextGetMoreTriesCount", "5");
			CheckString("Timings", "ExcludedPrograms", "LA.exe SomeProg.exe");
            CheckBool("Timings", "ChangeLayoutInExcluded", "true");
			CheckInt("Timings", "LangTooltipForMouseSkipMessages", "5");
			#endregion
			#region Snippets section
			CheckBool("Snippets", "SnippetsEnabled", "false");
			CheckBool("Snippets", "SpaceAfter", "false");
			CheckBool("Snippets", "SwitchToGuessLayout", "false");
			CheckString("Snippets", "SnippetExpandKey", "Space");
			#endregion
			#region AutoSwitch section
			CheckBool("AutoSwitch", "Enabled", "false");
			CheckBool("AutoSwitch", "SpaceAfter", "true");
			CheckBool("AutoSwitch", "SwitchToGuessLayout", "true");
			CheckBool("AutoSwitch", "DownloadInZip", "true");
			#endregion
			#region Hotkeys section
			// Toggle main window hotkey
			CheckBool("Hotkeys", "ToggleMainWindow_Enabled", "true");
			CheckBool("Hotkeys", "ToggleMainWindow_Double", "false");
			CheckString("Hotkeys", "ToggleMainWindow_Modifiers", "Win + Control + Shift + Alt");
			CheckInt("Hotkeys", "ToggleMainWindow_Key", "45");
			// Convert last word hotkey
			CheckBool("Hotkeys", "ConvertLastWord_Enabled", "true");
			CheckBool("Hotkeys", "ConvertLastWord_Double", "false");
			CheckString("Hotkeys", "ConvertLastWord_Modifiers", "");
			CheckInt("Hotkeys", "ConvertLastWord_Key", "19");
			// Convert selected text hotkey
			CheckBool("Hotkeys", "ConvertSelectedText_Enabled", "true");
			CheckBool("Hotkeys", "ConvertSelectedText_Double", "false");
			CheckString("Hotkeys", "ConvertSelectedText_Modifiers", "");
			CheckInt("Hotkeys", "ConvertSelectedText_Key", "145");
			// Convert last line hotkey
			CheckBool("Hotkeys", "ConvertLastLine_Enabled", "true");
			CheckBool("Hotkeys", "ConvertLastLine_Double", "false");
			CheckString("Hotkeys", "ConvertLastLine_Modifiers", "Shift");
			CheckInt("Hotkeys", "ConvertLastLine_Key", "19");
			// Convert last words hotkey
			CheckBool("Hotkeys", "ConvertLastWords_Enabled", "true");
			CheckBool("Hotkeys", "ConvertLastWords_Double", "false");
			CheckString("Hotkeys", "ConvertLastWords_Modifiers", "Shift");
			CheckInt("Hotkeys", "ConvertLastWords_Key", "122");
			// Toggle symbol ignore mode hotkey
			CheckBool("Hotkeys", "ToggleSymbolIgnoreMode_Enabled", "true");
			CheckBool("Hotkeys", "ToggleSymbolIgnoreMode_Double", "false");
			CheckString("Hotkeys", "ToggleSymbolIgnoreMode_Modifiers", "Shift + Control");
			CheckInt("Hotkeys", "ToggleSymbolIgnoreMode_Key", "122");
			// Selected text to title case hotkey
			CheckBool("Hotkeys", "SelectedTextToTitleCase_Enabled", "false");
			CheckBool("Hotkeys", "SelectedTextToTitleCase_Double", "true");
			CheckString("Hotkeys", "SelectedTextToTitleCase_Modifiers", "Shift");
			CheckInt("Hotkeys", "SelectedTextToTitleCase_Key", "0");
			// Selected text To random case hotkey
			CheckBool("Hotkeys", "SelectedTextToRandomCase_Enabled", "false");
			CheckBool("Hotkeys", "SelectedTextToRandomCase_Double", "true");
			CheckString("Hotkeys", "SelectedTextToRandomCase_Modifiers", "Alt");
			CheckInt("Hotkeys", "SelectedTextToRandomCase_Key", "0");
			// Selected text to swap case hotkey
			CheckBool("Hotkeys", "SelectedTextToSwapCase_Enabled", "false");
			CheckBool("Hotkeys", "SelectedTextToSwapCase_Double", "false");
			CheckString("Hotkeys", "SelectedTextToSwapCase_Modifiers", "Win");
			CheckInt("Hotkeys", "SelectedTextToSwapCase_Key", "190");
			// Selected text Transliteration hotkey
			CheckBool("Hotkeys", "SelectedTextTransliteration_Enabled", "false");
			CheckBool("Hotkeys", "SelectedTextTransliteration_Double", "false");
			CheckString("Hotkeys", "SelectedTextTransliteration_Modifiers", "Win");
			CheckInt("Hotkeys", "SelectedTextTransliteration_Key", "191");
			// Exit Mahou hotkey
			CheckBool("Hotkeys", "ExitMahou_Enabled", "true");
			CheckBool("Hotkeys", "ExitMahou_Double", "false");
			CheckString("Hotkeys", "ExitMahou_Modifiers", "Win + Control + Shift + Alt");
			CheckInt("Hotkeys", "ExitMahou_Key", "123");
			// Restart Mahou hotkey
			CheckBool("Hotkeys", "RestartMahou_Enabled", "true");
			CheckString("Hotkeys", "RestartMahou_Modifiers", "Win + Shift + Alt");
			CheckInt("Hotkeys", "RestartMahou_Key", "33");
			// Toggle Language Panel hotkey
			CheckBool("Hotkeys", "ToggleLangPanel_Enabled", "true");
			CheckBool("Hotkeys", "ToggleLangPanel_Double", "false");
			CheckString("Hotkeys", "ToggleLangPanel_Modifiers", "Shift");
			CheckInt("Hotkeys", "ToggleLangPanel_Key", "120");
			#endregion
			#region Language Panel
			CheckBool("LangPanel", "Display", "false");
			CheckInt("LangPanel", "Transparency", "90");
			CheckInt("LangPanel", "RefreshRate", "25");
			CheckString("LangPanel", "Position", "X0 Y0");
			CheckString("LangPanel", "ForeColor", "#000000");
			CheckString("LangPanel", "BackColor", "#FFFFFF");
			CheckString("LangPanel", "Font", "Microsoft Sans Serif; 8,25pt");
			CheckString("LangPanel", "BorderColor", "#8B5FFF");
			CheckBool("LangPanel", "BorderAeroColor", "true");
			CheckBool("LangPanel", "UpperArrow", "true");
			#endregion
			#region Updates
			CheckString("Updates", "Channel", "Stable");
			CheckString("Updates", "LatestCommit", "");
			#endregion
            #region Proxy section
            CheckString("Proxy", "ServerPort", "");
            CheckString("Proxy", "UserName", "");
            CheckString("Proxy", "Password", "");
            #endregion
            fine = true;
        }
        void CheckBool(string section, string key, string default_value) {
            bool bt = false; //bool temp
            if (!Boolean.TryParse(Read(section, key), out bt))
                Write(section, key, default_value);
        }
        void CheckInt(string section, string key, string default_value) {
            int it = 0; //int temp
            if (!Int32.TryParse(Read(section, key), out it))
                Write(section, key, default_value);
        }
        void CheckString(string section, string key, string default_value) {
            if (String.IsNullOrEmpty(Read(section, key)))
                Write(section, key, default_value);
        }
        /// <summary> Writes "value" to "key" in "section" in INI configuration. </summary>
        public void Write(string section, string key, string value) {
            _INI.SetValue(section, key, value);
        }
        /// <summary> Writes "value" to "key" in "section" in INI configuration, and saves to disk. </summary>
        public void WriteSave(string section, string key, string value) {
            _INI.SetValue(section, key, value);
            WriteToDisk();
        }
        /// <summary> Reads "value" from "key" in "section" from INI configuration. </summary>
        public string Read(string section, string key) {
        	return _INI.GetValue(section, key);
        }
        /// <summary>
        /// Reads "value" as int from "key" in "section" from INI configuration.
        /// </summary>
        public int ReadInt(string section, string key) {
            return Int32.Parse(_INI.GetValue(section, key));
        }
        /// <summary> Reads "value" as bool from "key" in "section" from INI configuration. </summary>
        public bool ReadBool(string section, string key) {
            return Boolean.Parse(_INI.GetValue(section, key).ToLower());
        }
        public void ReadFromDisk() {
        	_INI = new INI(File.ReadAllText(filePath));
        }
        public void WriteToDisk() {
        	File.WriteAllText(filePath, _INI.Raw);
        }
    }
}

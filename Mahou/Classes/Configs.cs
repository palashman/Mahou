using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Mahou
{
    class Configs
    {
        /// <summary>
        /// Mahou.ini file path.
        /// </summary>
        public static readonly string filePath = Path.Combine(Update.nPath, "Mahou.ini");
        /// <summary>
        /// Initializes settings, if some of values or settings file, not exists it creates them with default value.
        /// </summary>
        public Configs()
        {
            if (!File.Exists(filePath)) //Create an UTF-16 configuration file
            {
                File.WriteAllText(filePath, "!Unicode(✔), Mahou settings file", Encoding.Unicode);
            }
            int it = 0;      //int temp
            uint uit = 0;    //uint temp
            bool bt = false; //bool temp
            #region Hotkeys section
            if (!Int32.TryParse(this.Read("Hotkeys", "HKCLKey"), out it))
                this.Write("Hotkeys", "HKCLKey", "19"); //Hotkey convert last word

            if (String.IsNullOrEmpty(this.Read("Hotkeys", "HKCLMods")))
                this.Write("Hotkeys", "HKCLMods", "None"); //Hotkey convert last word modifiers

            if (!Int32.TryParse(this.Read("Hotkeys", "HKCSKey"), out it))
                this.Write("Hotkeys", "HKCSKey", "145"); //Hotkey convert selection

            if (String.IsNullOrEmpty(this.Read("Hotkeys", "HKCSMods")))
                this.Write("Hotkeys", "HKCSMods", "None"); //Hotkey convert selection modifiers

            if (!Int32.TryParse(this.Read("Hotkeys", "HKCLineKey"), out it))
                this.Write("Hotkeys", "HKCLineKey", "19"); //Hotkey convert line

            if (String.IsNullOrEmpty(this.Read("Hotkeys", "HKCLineMods"))) //Hotkey convert line modifiers
                this.Write("Hotkeys", "HKCLineMods", "Shift");

            if (String.IsNullOrEmpty(this.Read("Hotkeys", "OnlyKeyLayoutSwicth")))
                this.Write("Hotkeys", "OnlyKeyLayoutSwicth", "CapsLock"); //One key to switch layout

            if (!Int32.TryParse(this.Read("Hotkeys", "HKSymIgnKey"), out it))
                this.Write("Hotkeys", "HKSymIgnKey", "122"); //Hotkey Symbol ignore mode

            if (String.IsNullOrEmpty(this.Read("Hotkeys", "HKSymIgnMods"))) //Hotkey Symbol ignore mode modifiers
                this.Write("Hotkeys", "HKSymIgnMods", "Shift + Control + Alt");

            if (!Int32.TryParse(this.Read("Hotkeys", "HKConvertMore"), out it))
                this.Write("Hotkeys", "HKConvertMore", "122"); //Hotkey Convert more words

            if (String.IsNullOrEmpty(this.Read("Hotkeys", "HKConvertMoreMods"))) //Hotkey Convert more words modifiers
                this.Write("Hotkeys", "HKConvertMoreMods", "Shift + Control");
			#endregion
            #region Locales section
            if (!UInt32.TryParse(this.Read("Locales", "locale1uId"), out uit))
                this.Write("Locales", "locale1uId", ""); //Locale 1 id

            if (String.IsNullOrEmpty(this.Read("Locales", "locale1Lang")))
                this.Write("Locales", "locale1Lang", ""); //Locale 1 name

            if (!UInt32.TryParse(this.Read("Locales", "locale2uId"), out uit))
                this.Write("Locales", "locale2uId", ""); //Locale 2 id

            if (String.IsNullOrEmpty(this.Read("Locales", "locale2Lang")))
                this.Write("Locales", "locale2Lang", ""); //Locale 2 name

            if (String.IsNullOrEmpty(this.Read("Locales", "LANGUAGE")))
                this.Write("Locales", "LANGUAGE", "EN"); //Language of user interface, messages etc.
			#endregion
            #region Functions section
            if (!Boolean.TryParse(this.Read("Functions", "IconVisibility"), out bt))
                this.Write("Functions", "IconVisibility", "true"); //Tray icon visibility

            if (!Boolean.TryParse(this.Read("Functions", "CycleMode"), out bt))
                this.Write("Functions", "CycleMode", "false");

            if (!Boolean.TryParse(this.Read("Functions", "EmulateLayoutSwitch"), out bt))
                this.Write("Functions", "EmulateLayoutSwitch", "false");

            if (!Int32.TryParse(this.Read("Functions", "ELSType"), out it))
                this.Write("Functions", "ELSType", "0");

            if (!Boolean.TryParse(this.Read("Functions", "CSSwitch"), out bt))
                this.Write("Functions", "CSSwitch", "true");

            if (!Boolean.TryParse(this.Read("Functions", "BlockCTRL"), out bt))
                this.Write("Functions", "BlockCTRL", "false");

            if (!Boolean.TryParse(this.Read("Functions", "RePress"), out bt))
                this.Write("Functions", "RePress", "true");

            if (!Boolean.TryParse(this.Read("Functions", "EatOneSpace"), out bt))
                this.Write("Functions", "EatOneSpace", "false");
            
            if (!Boolean.TryParse(this.Read("Functions", "ReSelect"), out bt))
                this.Write("Functions", "ReSelect", "true");

            if (!Boolean.TryParse(this.Read("Functions", "SymIgnModeEnabled"), out bt))
                this.Write("Functions", "SymIgnModeEnabled", "false");

            if (!Boolean.TryParse(this.Read("Functions", "MoreTries"), out bt))
                this.Write("Functions", "MoreTries", "false");

            if (!Int32.TryParse(this.Read("Functions", "TriesCount"), out it))
                this.Write("Functions", "TriesCount", "5");

            if (!Boolean.TryParse(this.Read("Functions", "DisplayLang"), out bt))
                this.Write("Functions", "DisplayLang", "false");

            if (!Int32.TryParse(this.Read("Functions", "DLRefreshRate"), out it))
                this.Write("Functions", "DLRefreshRate", "50");

            if (!Boolean.TryParse(this.Read("Functions", "ExperimentalCSSwitch"), out bt))
                this.Write("Functions", "ExperimentalCSSwitch", "false");
            
            if (!Boolean.TryParse(this.Read("Functions", "Snippets"), out bt))
                this.Write("Functions", "Snippets", "false");
            
            if (!Boolean.TryParse(this.Read("Functions", "DTTOnChange"), out bt))
                this.Write("Functions", "DTTOnChange", "false");
            
            if (!Boolean.TryParse(this.Read("Functions", "ScrollTip"), out bt))
                this.Write("Functions", "ScrollTip", "false");
            
            if (!Boolean.TryParse(this.Read("Functions", "UpdatesEnabled"), out bt))
                this.Write("Functions", "UpdatesEnabled", "true");
            
            if (!Boolean.TryParse(this.Read("Functions", "Logging"), out bt))
                this.Write("Functions", "Logging", "false");
            
            if (!Boolean.TryParse(this.Read("Functions", "DiffLayoutColors"), out bt))
                this.Write("Functions", "DiffLayoutColors", "false");
            
            if (!Boolean.TryParse(this.Read("Functions", "CrtDisplayLang"), out bt))
                this.Write("Functions", "CrtDisplayLang", "false");

            if (!Int32.TryParse(this.Read("Functions", "CrtDLRefreshRate"), out it))
                this.Write("Functions", "CrtDLRefreshRate", "50");
			#endregion
            #region EnabledHotkeys section
            if (!Boolean.TryParse(this.Read("EnabledHotkeys", "HKCLEnabled"), out bt))
                this.Write("EnabledHotkeys", "HKCLEnabled", "true"); //Hotkey convert last word enabled

            if (!Boolean.TryParse(this.Read("EnabledHotkeys", "HKCSEnabled"), out bt))
                this.Write("EnabledHotkeys", "HKCSEnabled", "true"); //Hotkey convert selection enabled

            if (!Boolean.TryParse(this.Read("EnabledHotkeys", "HKCLineEnabled"), out bt))
                this.Write("EnabledHotkeys", "HKCLineEnabled", "true"); //Hotkey convert line enabled

            if (!Boolean.TryParse(this.Read("EnabledHotkeys", "HKSymIgnEnabled"), out bt))
                this.Write("EnabledHotkeys", "HKSymIgnEnabled", "true"); //Hotkey symbol ignore enabled
			#endregion
            #region ExtCtrls section
            if (!Boolean.TryParse(this.Read("ExtCtrls", "UseExtCtrls"), out bt))
                this.Write("ExtCtrls", "UseExtCtrls", "false"); //Use extended CTRLs feature

            if (!UInt32.TryParse(this.Read("ExtCtrls", "LCLocale"), out uit))
                this.Write("ExtCtrls", "LCLocale", ""); //Left CTRL switch to locale

            if (String.IsNullOrEmpty(this.Read("ExtCtrls", "LCLocaleName")))
                this.Write("ExtCtrls", "LCLocaleName", "");

            if (!UInt32.TryParse(this.Read("ExtCtrls", "RCLocale"), out uit))
                this.Write("ExtCtrls", "RCLocale", ""); //Right CTRL switch to locale

            if (String.IsNullOrEmpty(this.Read("ExtCtrls", "RCLocaleName")))
                this.Write("ExtCtrls", "RCLocaleName", "");
            #endregion
            #region Proxy section
            if (String.IsNullOrEmpty(this.Read("Proxy", "ServerPort")))
                this.Write("Proxy", "ServerPort", "");
            
            if (String.IsNullOrEmpty(this.Read("Proxy", "UserName")))
                this.Write("Proxy", "UserName", "");
            
            if (String.IsNullOrEmpty(this.Read("Proxy", "Password")))
                this.Write("Proxy", "Password", "");
            #endregion
            #region Tooltip UI sections
            if (!Int32.TryParse(this.Read("TTipUI", "Height"), out it))
                this.Write("TTipUI", "Height", "14"); //Lang Tooltip height
            
            if (!Int32.TryParse(this.Read("TTipUI", "Width"), out it))
                this.Write("TTipUI", "Width", "24"); //Lang Tooltip width
            
            if (String.IsNullOrEmpty(this.Read("TTipUI", "Font")))
                this.Write("TTipUI", "Font", "Georgia; 8pt"); //Lang Tooltip font & it size
            
            if (!Int32.TryParse(this.Read("TTipUI", "xpos"), out it))
                this.Write("TTipUI", "xpos", "8"); //Lang Tooltip x pos
            
            if (!Int32.TryParse(this.Read("TTipUI", "ypos"), out it))
                this.Write("TTipUI", "ypos", "0"); //Lang Tooltip y pos
            
            if (!Boolean.TryParse(this.Read("TTipUI", "TransparentBack"), out bt))
                this.Write("TTipUI", "TransparentBack", "false"); //Transparent Background in tooltip
            
            if (String.IsNullOrEmpty(this.Read("TTipUI", "DLForeColor")))
                this.Write("TTipUI", "DLForeColor", "#FFFFFF");

            if (String.IsNullOrEmpty(this.Read("TTipUI", "DLBackColor")))
                this.Write("TTipUI", "DLBackColor", "#000000");
            
            if (String.IsNullOrEmpty(this.Read("TTipUI", "CrtDLForeColor")))
                this.Write("TTipUI", "CrtDLForeColor", "#FFFFFF");

            if (String.IsNullOrEmpty(this.Read("TTipUI", "CrtDLBackColor")))
                this.Write("TTipUI", "CrtDLBackColor", "#000000");
            
            if (String.IsNullOrEmpty(this.Read("TTipUI", "L1DiffFGColor")))
                this.Write("TTipUI", "L1DiffFGColor", "#8B5FFF"); //Layout 1 different ForeGround color

            if (String.IsNullOrEmpty(this.Read("TTipUI", "L1DiffBGColor")))
                this.Write("TTipUI", "L1DiffBGColor", "#FFFFFF");
            
            if (String.IsNullOrEmpty(this.Read("TTipUI", "L1DiffFont")))
                this.Write("TTipUI", "L1DiffFont", "Georgia; 8pt"); //Layout 1 different font
            
            if (String.IsNullOrEmpty(this.Read("TTipUI", "L2DiffFGColor")))
                this.Write("TTipUI", "L2DiffFGColor", "#E1E100");

            if (String.IsNullOrEmpty(this.Read("TTipUI", "L2DiffBGColor")))
                this.Write("TTipUI", "L2DiffBGColor", "#000000"); //Layout 2 different BackGround color
            
            if (String.IsNullOrEmpty(this.Read("TTipUI", "L2DiffFont")))
                this.Write("TTipUI", "L2DiffFont", "Palatino Linotype; 9pt"); //Layout 2 different font
            
            if (!Int32.TryParse(this.Read("TTipUI", "CrtHeight"), out it))
                this.Write("TTipUI", "CrtHeight", "14"); //Caret Lang Tooltip height
            
            if (!Int32.TryParse(this.Read("TTipUI", "CrtWidth"), out it))
                this.Write("TTipUI", "CrtWidth", "24"); //Caret Lang Tooltip width
            
            if (String.IsNullOrEmpty(this.Read("TTipUI", "CrtFont")))
                this.Write("TTipUI", "CrtFont", "Georgia; 8pt"); //Caret Lang Tooltip font & it size
            
            if (!Int32.TryParse(this.Read("TTipUI", "Crtxpos"), out it))
                this.Write("TTipUI", "Crtxpos", "8"); //Caret Lang Tooltip x pos
            
            if (!Int32.TryParse(this.Read("TTipUI", "Crtypos"), out it))
                this.Write("TTipUI", "Crtypos", "0"); //Caret Lang Tooltip y pos
            #endregion
            #region DoubleKey section
            if (String.IsNullOrEmpty(this.Read("DoubleKey", "Use")))
                this.Write("DoubleKey", "Use", "false");
            
            if (!Int32.TryParse(this.Read("DoubleKey", "Delay"), out it))
                this.Write("DoubleKey", "Delay", "350");
            #endregion
        }
        /// <summary>
        /// Writes "value" to "key" in "section" in INI configuration.
        /// </summary>
        /// <param name="section">Section name in which key will be written.</param>
        /// <param name="key">Key name to be written.</param>
        /// <param name="value">Key's value to be written.</param>
        public void Write(string section, string key, string value)
        {
            WinAPI.WritePrivateProfileString(section, key, value, filePath);
        }
        /// <summary>
        /// Reads "value" from "key" in "section" from INI configuration.
        /// </summary>
        /// <param name="section">Section name in which key will be read.</param>
        /// <param name="key">Key's name in which value will be read.</param>
        /// <returns>string</returns>
        public string Read(string section, string key)
        {
            var SB = new StringBuilder(255);
            int i = WinAPI.GetPrivateProfileString(section, key, "", SB, 255, filePath);
            return SB.ToString();
        }
        /// <summary>
        /// Reads "value" as int from "key" in "section" from INI configuration.
        /// </summary>
        /// <param name="section">Section name in which key will be read.</param>
        /// <param name="key">Key's name in which value will be read.</param>
        /// <returns>int</returns>
        public int ReadInt(string section, string key) //Returns "key" value in "section" as int
        {
            var SB = new StringBuilder(255);
            int i = WinAPI.GetPrivateProfileString(section, key, "", SB, 255, filePath);
            return Int32.Parse(SB.ToString());
        }
        /// <summary>
        /// Reads "value" as uint from "key" in "section" from INI configuration .
        /// </summary>
        /// <param name="section">Section name in which key will be read.</param>
        /// <param name="key">Key's name in which value will be read.</param>
        /// <returns>uint</returns>
        public uint ReadUInt(string section, string key) //Returns "key" value in "section" as int
        {
            var SB = new StringBuilder(255);
            int i = WinAPI.GetPrivateProfileString(section, key, "", SB, 255, filePath);
            return UInt32.Parse(SB.ToString());
        }
        /// <summary>
        /// Reads "value" as bool from "key" in "section" from INI configuration.
        /// </summary>
        /// <param name="section">Section name in which key will be read.</param>
        /// <param name="key">Key's name in which value will be read.</param>
        /// <returns>bool</returns>
        public bool ReadBool(string section, string key) //Returns "key" value in "section" as bool
        {
            var SB = new StringBuilder(255);
            int i = WinAPI.GetPrivateProfileString(section, key, "", SB, 255, filePath);
            return Boolean.Parse(SB.ToString().ToLower());
        }
    }
}

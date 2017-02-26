using System;

namespace Mahou.Languages
{
	public static class English {
		#region Tabs
		public static readonly string tab_Functions = "Functions";
		public static readonly string tab_Layouts = "Layouts";
		public static readonly string tab_Appearence = "Appearence";
		public static readonly string tab_Timings = "Timings";
		public static readonly string tab_Snippets = "Snippets";
		public static readonly string tab_Hotkeys = "Hotkeys";
		public static readonly string tab_Updates = "Updates";
		public static readonly string tab_About = "About";
		#endregion
		#region Functions
		public static readonly string AutoStart = "Start with Windows.";
		public static readonly string TrayIcon = "Show tray icon.";
		public static readonly string ConvertSelectionLS = "Convert selection layout switching.";
		public static readonly string ReSelect = "Re-select text after conversion.";
		public static readonly string RePress = "Re-press modifiers after hotkey action.";
		public static readonly string Add1Space = "Add one space to last word.";
		public static readonly string ConvertSelectionLSPlus = "Convert selection layout switching+ (experimental).";
		public static readonly string HighlightScroll = "Highlight Scroll-Lock when layout 1 is active.";
		public static readonly string UpdatesCheck = "Check for updates at startup.";
		public static readonly string Logging = "Enable logging for debugging.";
		public static readonly string CapsTimer = "Activate Caps Lock disabler timer.";
		public static readonly string ContryFlags = "Display country flags in tray icon.";
		public static readonly string BlockCtrlHKs = "Block Mahou hotkeys with Ctrl.";
		#endregion
		#region Layouts
		public static readonly string SwitchBetween = "Switch between layouts";
		public static readonly string EmulateLS = "Emulate layout switching.";
		public static readonly string EmulateType = "Emulation type:";
		public static readonly string ChangeLayoutBy1Key = "Change to specific layout by keys:";
		#endregion
		#region Appearence
		public static readonly string LDMouseDisplay = "Display current language tooltip around mouse.";
		public static readonly string LDCaretDisplay = "Display current language tooltip around caret.";
		public static readonly string LDOnlyOnChange = "Display only on layout change.";
		public static readonly string LDDifferentAppearence = "Use different appearence for layouts.";
		public static readonly string Language = "Language:";
		public static readonly string LDAppearence = "Language tooltip appearence";
		public static readonly string LDAroundMouse = "Around mouse";
		public static readonly string LDAroundCaret = "Around caret";
		public static readonly string LDTransparentBG = "Transparent color";
		public static readonly string LDFont = "Font";
		public static readonly string LDFore = "Foreground color:";
		public static readonly string LDBack = "Background color:";
		public static readonly string LDSize = "Size";
		public static readonly string LDPosition = "Position";
		public static readonly string LDWidth = "Width";
		public static readonly string LDHeight = "Height";
		#endregion
		#region Timings
		public static readonly string LDForMouseRefreshRate = "Language tooltip around mouse refresh rate(ms):";
		public static readonly string LDForCaretRefreshRate = "Language tooltip around caret refresh rate(ms):";
		public static readonly string DoubleHKDelay = "Double hotkey wait time for second press(ms):";
		public static readonly string TrayFlagsRefreshRate = "Flags in tray icon refresh rate(ms):";
		public static readonly string ScrollLockRefreshRate = "Scroll Lock refresh rate(ms):";
		public static readonly string CapsLockRefreshRate = "Caps Lock update rate(ms):";
		public static readonly string MoreTriesToGetSelectedText = "Use more tries to get selected text:";
		#endregion
		#region Snippets
		public static readonly string SnippetsEnabled = "Enable snippets.";
		#endregion
		#region Hotkeys
		public static readonly string ToggleMainWnd = "Toggle settings window";
		public static readonly string ConvertLast = "Convert last word";
		public static readonly string ConvertSelected = "Convert selected text";
		public static readonly string ConvertLine = "Convert last line";
		public static readonly string ConvertWords = "Convert specific last words count";
		public static readonly string ToggleSymbolIgnore = "Toggle symbol ignore mode";
		public static readonly string SelectedToTitleCase = "Selected text words to Title Case";
		public static readonly string SelectedToRandomCase = "Selected text words to RanDoM cASe";
		public static readonly string SelectedToSwapCase = "Selected text words to sWAP cASE";
		public static readonly string SelectedTransliteration = "Selected text transliteration";
		public static readonly string ExitMahou = "Exit";
		public static readonly string Enabled = "Enabled";
		public static readonly string DoubleHK= "Double hotkey";
		#endregion
		#region Updates
		public static readonly string CheckForUpdates = "Check for updates:";
		public static readonly string UpdateMahou = "Update Mahou to <version>";
		public static readonly string DownloadUpdate = "Download update";
		public static readonly string ProxyConfig = "Proxy configuration";
		public static readonly string ProxyServer = "Server:Port";
		public static readonly string ProxyLogin = "Login";
		public static readonly string ProxyPass = "Password";
		#endregion
		#region About
		public static readonly string DbgInf = "Debug info";
		public static readonly string DbgInf_Copied = "Copied!";
		public static readonly string Site = "Site";
		public static readonly string Releases = "Releases";
		public static readonly string About = "Hotkeys:\n"+"Press Pause (by Default) to convert last inputted word.\n" +"" +
			"Press Scroll Lock (by Default) while selected text is focused to convert it.\n"+
			"Press Shift+Pause (by Default) to convert last inputted line.\n"+
            "Press Ctrl+Alt+Win+Shift+Insert to show Mahou main window.\n"+
			"Press Ctrl+Alt+Win+Shift+F12 to shutdown Mahou.\n\n"+
			"*Note that if you typing in not of selected in settings layouts,"+
			" conversion will switch typed text to Language 1 (Ignored if Switch between layouts is OFF).\n\n"+
          "**If you have problems with symbols conversion (selection) try \"switching languages (1=>2 & 2=>1)\""+
			" or \"Convert selection layout switching\"/ or Plus option."+
			"To reset settings just delete Mahou.ini in Mahou folder.\n\nRegards.";
		#endregion
		#region Misc
		public static readonly string Keys = "Keys";
		public static readonly string Key_Left = "Left";
		public static readonly string Key_Right = "Right";
		public static readonly string Layouts = "Layouts";
		public static readonly string Layout = "Layout";
		public static readonly string Hotkey = "Hotkey";
		public static readonly string UpdateFound = "New version avaible!";
		public static readonly string UpdateComplete = "Mahou succesfully updated!";
		public static readonly string Mahou = "Mahou(魔法) - magic layout switcher.";
		#endregion
		#region Buttons
		public static readonly string ButtonOK = "OK";
		public static readonly string ButtonApply = "Apply";
		public static readonly string ButtonCancel = "Cancel";
		#endregion
//		#region Tooltips
//		public static readonly string TT_SwitchBetween = "While this option disabled, [Convert word] and [Convert line] and [Convert selection with \"CS-Switch\" enabled]\n" +
//		                                  "will just cycle between all locales instead of switching between selected in settings.\nThis mode works for almost all programs.\n" +
//		                                  "If there is program in which [Convert word] or [Convert line] or [Convert selection with \"CS-Switch\" enabled] not work,\ntry with this option enabled.\n" +
//		                                  "If you have just 2 layouts (input languages) it is HIGHLY RECOMMENDED to turn it ON, and \"Emulate layout switching\" too to ON.";
//		public static readonly string TT_Link = "Go to GitHub repository to view source or report issue.";
//		public static readonly string TT_TrayIcon = "Toggles visibility of icon in a tray.\nIf it is hidden, to show configs window hit CTRL+ALT+SHIFT+INSERT or just run Mahou.exe again.";
//		public static readonly string TT_BlockCtrl = "Blocks hotkeys that use Control,\nwhen \"Switch layout by key\" is set to Left/Right Control.";
//		public static readonly string TT_EmulateLS = "If this option enabled, Cycle Mode will use Emulation of Alt+Shift/Win+Space instead \"sending window message\" that changes layout.";
//		public static readonly string TT_ConvertSelectionSwitch = "If this option enabled, Covert selection will use layout switching.\nAll characters will be rewritten as they must (no problems with symbols).\nThere also an CS-Switch+ in More Configs page 2.";
//		public static readonly string TT_RePress = "If enabled, modifiers will be repressed after conversion.\nDue to new hotkey management modifiers NEVER stuck (recommended).";
//		public static readonly string TT_Add1Space = "If enabled, pressing ONE space AFTER word will not clear last word.";
//		public static readonly string TT_ReSelect = "Enabling this, will reselect text after \"Convert selection\".";
//		public static readonly string TT_EmulationType = "Select type for Emulate change layout.\nWin+Space works only in Windows 10!!\nWin+Space also will work better in Metro apps.";
//		public static readonly string TT_SymbolIgnore = "If enabled, symbols []{};':\"./<>? will be ignored and will not be converted.\nWorks in Convert word, Convert line, Convert selection with Cycle Mode and CS-Switch enabled.\nWon't work if you have >2 layouts an Cycle Mode enabled!";
//		public static readonly string TT_MoreTriesToGetSelectedText = "Enables more tries to get selected text in Convert selection.";
//		public static readonly string TT_LDForMouse = "If enabled, mouse cursor hovers text it will display a small tip of what language is now using.";
//		public static readonly string TT_LDForMouseRefreshRate = "Speed of checking cursor type (if it equals \"I\" ).";
//		public static readonly string TT_LDSize = "Size of language tooltip.";
//		public static readonly string TT_LDPosition = "Position relative to mouse/caret position.";
//		public static readonly string TT_DoubleHK = "Enables double hotkey ability,\nand makes it possible to set modifies to hotkey.";
//		public static readonly string TT_DoubleHKDelay = "Time to wait second hotkey press (ms)";
//		public static readonly string TT_ConvertSelectionSwitchPlus = "Combines some abilities of Convert selection with enabled CS-Switch and when it disabled (Enable CS-Switch for it to work).\nIt can:\n1.Conversion from multiple languages at once.\n2.Ignore symbols feature work in it.\n3.Auto get language of text (not all symbols, the ones that exist in both layouts) can be rightly recognized if wrong layout selected from begin)\n4.Ability to convert symbols that exist in both layouts different if change layout before conversion.";
//		public static readonly string TT_LDTransparentBG = "Makes background of language tooltips transparent.\nDon't forget to change text color (because \"White\" is default).\nSome fonts may look bad, try changing to another (\"Georgia\" or \"Palatino Linotype\" is recommend).";
//		public static readonly string TT_Snippets = "If this enabled, pressing Space will expand small (which starts with \"->\") word, to big (which is between \"====>\" and \"<====\") word/text fragment.";
//		public static readonly string TT_LDOnlyOnChange = "Display language tooltip only on layout change.\nTime of displaying is 7x[Refresh rate].";
//		public static readonly string TT_ScrollTip = "Highlight Scroll Lock when active language 1, selected in main window.";
//		public static readonly string TT_StartupUpdateCheck = "If enabled Mahou will check for updates at startup.";
//		public static readonly string TT_ConvertWords = "Allow to convert specific last word count by pressing hotkey and then 0-9 (0 = 10) on keyboard.";
//		public static readonly string TT_Logging = "Designed ONLY to search for errors, BIG PERFORMANCE IMPACT, logs are saved in Mahou's folder, in folder Logs.";
//		public static readonly string TT_LDForCaret = "Displays language tooltip around caret (carriage).";
//		public static readonly string TT_LDDifferentAppearence = "If enabled language tooltips will use different color for different layouts (1&2).";
//		public static readonly string TT_CaretRefreshRate = "Speed of checking caret availability.";
//		public static readonly string TT_FGColor = "Foreground/Text color.";
//		public static readonly string TT_BGColor = "Background color.";
//		public static readonly string TT_CapsDis = "If enabled and change layout by key set to CapsLock then timer which will disable CapsLock if it is active will launch.";
//		public static readonly string TT_CountryFlags = "If enabled Mahou will display country flags in tray icon.\nYou can add or change flags to yours, to make this create folder Flags in folder where Mahou.exe is, and paste there png with name of 2 letters which you can see in language tooltip.";
//		#endregion
	}
	/// <summary>
	/// Russian language for MahouUI.
	/// </summary>
	public static class Russian
	{
		#region Tabs
		public static readonly string tab_Functions = "Функции";
		public static readonly string tab_Layouts = "Раскладки";
		public static readonly string tab_Appearence = "Вид";
		public static readonly string tab_Timings = "Тайминги";
		public static readonly string tab_Snippets = "Сниппеты";
		public static readonly string tab_Hotkeys = "Горячие клавиши";
		public static readonly string tab_Updates = "Обновления";
		public static readonly string tab_About = "О...";
		#endregion
		#region Functions
		public static readonly string AutoStart = "Запускать вместе с Windows.";
		public static readonly string TrayIcon = "Показывать иконку в трее.";
		public static readonly string ConvertSelectionLS = "Смена раскладки в Конверт выделения.";
		public static readonly string ReSelect = "Выделять заново текст после конвертации.";
		public static readonly string RePress = "Нажимать заново модификаторы после действия горячих клавиш.";
		public static readonly string Add1Space = "Добавлять 1 пробел в текущее слово.";
		public static readonly string ConvertSelectionLSPlus = "Смена раскладки в Конверт выделения+ (экспереметнально).";
		public static readonly string HighlightScroll = "Подсвечивать Scroll-Lock когда раскладка 1 активна.";
		public static readonly string UpdatesCheck = "Проверять обновления при запуске.";
		public static readonly string Logging = "Включить журналирование действий для поиска ошибок.";
		public static readonly string CapsTimer = "Включит таймер отключатель Caps-Lock.";
		public static readonly string ContryFlags = "Отображать флаги стран в трее.";
		public static readonly string BlockCtrlHKs = "Блокировать горячие клавиши Mahou содержащие Ctrl.";
		#endregion
		#region Layouts
		public static readonly string SwitchBetween = "Переключать между раскладками";
		public static readonly string EmulateLS = "Эмулировать переключение раскладки.";
		public static readonly string EmulateType = "Тип эмуляции:";
		public static readonly string ChangeLayoutBy1Key = "Переключать раскладки по клавишам:";
		#endregion
		#region Appearence
		public static readonly string LDMouseDisplay = "Отображать подсказку текущего языка рядом с мышью.";
		public static readonly string LDCaretDisplay = "Отображать подсказку текущего языка рядом с кареткой.";
		public static readonly string LDOnlyOnChange = "Отображать только при смене.";
		public static readonly string LDDifferentAppearence = "Использовать разный вид для раскладок.";
		public static readonly string Language = "Язык:";
		public static readonly string LDAppearence = "Вид подсказки языка";
		public static readonly string LDAroundMouse = "Возле мыши";
		public static readonly string LDAroundCaret = "Возле каретки";
		public static readonly string LDTransparentBG = "Прозрачный цвет";
		public static readonly string LDFont = "Шрифт";
		public static readonly string LDFore = "Цвет текста:";
		public static readonly string LDBack = "Цвет фона:";
		public static readonly string LDSize = "Размер";
		public static readonly string LDPosition = "Позиция";
		public static readonly string LDWidth = "Ширина";
		public static readonly string LDHeight = "Высота";
		#endregion
		#region Timings
		public static readonly string LDForMouseRefreshRate = "Скорость обновления подсказки языка возле мыши(мс):";
		public static readonly string LDForCaretRefreshRate = "Скорость обновления подсказки языка возле каретки(мс)";
		public static readonly string DoubleHKDelay = "Время ожидания следующего нажатия двойных горячих клавиш(мс):";
		public static readonly string TrayFlagsRefreshRate = "Скорость обновления флагов в трее(мс):";
		public static readonly string ScrollLockRefreshRate = "Скорость обновления Scroll Lock(мс):";
		public static readonly string CapsLockRefreshRate = "Скорость обновления Caps Lock(ms):";
		public static readonly string MoreTriesToGetSelectedText = "Использовать больше попыток взятия текста:";
		#endregion
		#region Snippets
		public static readonly string SnippetsEnabled = "Включить сниппеты.";
		#endregion
		#region Hotkeys
		public static readonly string ToggleMainWnd = "Переключить видимость главного окна";
		public static readonly string ConvertLast = "Конвертация последнего слова";
		public static readonly string ConvertSelected = "Конвертация выделенного текста";
		public static readonly string ConvertLine = "Конвертация последней линии";
		public static readonly string ConvertWords = "Конвертация нескольких слов";
		public static readonly string ToggleSymbolIgnore = "Переключить игнорование символов";
		public static readonly string SelectedToTitleCase = "Выделенные слов в Заглавный регистр";
		public static readonly string SelectedToRandomCase = "Выделенные слов в СЛУчАйнЫй регистр";
		public static readonly string SelectedToSwapCase = "Выделенные слов в оБРАТЫНЙ регистр";
		public static readonly string SelectedTransliteration = "Транслитерация выледенного текста";
		public static readonly string ExitMahou = "Выход";
		public static readonly string Enabled = "Включена";
		public static readonly string DoubleHK= "Двойная горячая клавиша";
		#endregion
		#region Updates
		public static readonly string CheckForUpdates = "Проверить обновления:";
		public static readonly string CheckingForUpdates = "Проверяю...";
		public static readonly string YouHaveLatest = "У Вас последняя версия.";
		public static readonly string TimeToUpdate = "Думаю пора обновиться.";
		public static readonly string UpdateMahou = "Обновить Mahou к <версии>";
		public static readonly string DownloadUpdate = "Скачать обновление";
		public static readonly string ProxyConfig = "Конфигурация прокси";
		public static readonly string ProxyServer = "Сервер:порт";
		public static readonly string ProxyLogin = "Логин";
		public static readonly string ProxyPass = "Пароль";
		#endregion
		#region About
		public static readonly string DbgInf = "Отладочная информация";
		public static readonly string DbgInf_Copied = "Скопировано!";
		public static readonly string Site = "Сайт";
		public static readonly string Releases = "Релизы";
		public static readonly string About = "Горячие клавиши:\nНажмите Pause (по умолчанию) для конвертации последнего введённого слова.\n"+
			"Нажмите Scroll (по умолчанию) пока выделенный текст в фокусе чтобы конвертировать его.\n"+
			"Нажмите Shift+Pause (по умолчанию) для конвертации последней введённой линии.\n"+
            "Нажмите Ctrl+Alt+Win+Shift+Insert чтобы показать/скрыть главное окно.\n"+
			"Нажмите Ctrl+Alt+Win+Shift+F12 чтобы завершить Mahou.\n"+
			"\n*Заметьте что если you вводите текст не из выбранных раскладок в настройках, то конвертация конвертирует текст в Язык 1 (Не актуально если включён Циклич. режим).\n\n"+
            "**Если у Вас проблемы с символами при Конвертации выделения попробуйте \"переключить языки местами (1=>2 & 2=>1)\" или включите \"Смена раскладки в конверсиях выделенного\" или плюс.\n\n"+
			"\n\nУдачи.";
		#endregion
		#region Misc
		public static readonly string Keys = "Клавиши";
		public static readonly string Key_Left = "Левый";
		public static readonly string Key_Right = "Правый";
		public static readonly string Layouts = "Раскладки";
		public static readonly string Layout = "Раскладка";
		public static readonly string Hotkey = "Горячая клавиша";
		public static readonly string UpdateFound = "Новая версия доступна!";
		public static readonly string UpdateComplete = "Mahou успешно обновлен!";
		public static readonly string ShowHide = "Показать/Скрыть";
		public static readonly string Mahou = "Mahou(魔法) - волшебный переключатель раскладок.";
		#endregion
		#region Buttons
		public static readonly string ButtonOK = "ОК";
		public static readonly string ButtonApply = "Применить";
		public static readonly string ButtonCancel = "Отмена";
		#endregion
//		#region Tooltips
//		public static readonly string TT_SwitchBetween = "Пока включена, [Конверт слова] and [Конверт линии] and [Конверт выделения с \"КВ-Ключ\" включённой]\n" +
//		                                  "будет переключать раскладку циклично, вместо переключения между выбранными в настройках.\nЭтот режим работает с бОльшим количеством программ.\n" +
//		                                  "Если есть программа в которой [Конверт слова] или [Конверт линии] или [Конверт выделения с \"КВ-Ключ\" включённой] не работают,\nто попробуйте включить эту функцию.\n" +
//		                                  "Если у вас только 2 раскладки ОЧЕНЬ РЕКОМЕНДУЕТСЯ включить ее, и \"Эму\".";
		//		public static readonly string TT_Link = "Go to GitHub repository to view source or report issue.";
		//		public static readonly string TT_TrayIcon = "Toggles visibility of icon in a tray.\nIf it is hidden, to show configs window hit CTRL+ALT+SHIFT+INSERT or just run Mahou.exe again.";
		//		public static readonly string TT_BlockCtrl = "Blocks hotkeys that use Control,\nwhen \"Switch layout by key\" is set to Left/Right Control.";
		//		public static readonly string TT_EmulateLS = "If this option enabled, Cycle Mode will use Emulation of Alt+Shift/Win+Space instead \"sending window message\" that changes layout.";
		//		public static readonly string TT_ConvertSelectionSwitch = "If this option enabled, Covert selection will use layout switching.\nAll characters will be rewritten as they must (no problems with symbols).\nThere also an CS-Switch+ in More Configs page 2.";
		//		public static readonly string TT_RePress = "If enabled, modifiers will be repressed after conversion.\nDue to new hotkey management modifiers NEVER stuck (recommended).";
		//		public static readonly string TT_Add1Space = "If enabled, pressing ONE space AFTER word will not clear last word.";
		//		public static readonly string TT_ReSelect = "Enabling this, will reselect text after \"Convert selection\".";
		//		public static readonly string TT_EmulationType = "Select type for Emulate change layout.\nWin+Space works only in Windows 10!!\nWin+Space also will work better in Metro apps.";
		//		public static readonly string TT_SymbolIgnore = "If enabled, symbols []{};':\"./<>? will be ignored and will not be converted.\nWorks in Convert word, Convert line, Convert selection with Cycle Mode and CS-Switch enabled.\nWon't work if you have >2 layouts an Cycle Mode enabled!";
		//		public static readonly string TT_MoreTriesToGetSelectedText = "Enables more tries to get selected text in Convert selection.";
		//		public static readonly string TT_LDForMouse = "If enabled, mouse cursor hovers text it will display a small tip of what language is now using.";
		//		public static readonly string TT_LDForMouseRefreshRate = "Speed of checking cursor type (if it equals \"I\" ).";
		//		public static readonly string TT_LDSize = "Size of language tooltip.";
		//		public static readonly string TT_LDPosition = "Position relative to mouse/caret position.";
		//		public static readonly string TT_DoubleHK = "Enables double hotkey ability,\nand makes it possible to set modifies to hotkey.";
		//		public static readonly string TT_DoubleHKDelay = "Time to wait second hotkey press (ms)";
		//		public static readonly string TT_ConvertSelectionSwitchPlus = "Combines some abilities of Convert selection with enabled CS-Switch and when it disabled (Enable CS-Switch for it to work).\nIt can:\n1.Conversion from multiple languages at once.\n2.Ignore symbols feature work in it.\n3.Auto get language of text (not all symbols, the ones that exist in both layouts) can be rightly recognized if wrong layout selected from begin)\n4.Ability to convert symbols that exist in both layouts different if change layout before conversion.";
		//		public static readonly string TT_LDTransparentBG = "Makes background of language tooltips transparent.\nDon't forget to change text color (because \"White\" is default).\nSome fonts may look bad, try changing to another (\"Georgia\" or \"Palatino Linotype\" is recommend).";
		//		public static readonly string TT_Snippets = "If this enabled, pressing Space will expand small (which starts with \"->\") word, to big (which is between \"====>\" and \"<====\") word/text fragment.";
		//		public static readonly string TT_LDOnlyOnChange = "Display language tooltip only on layout change.\nTime of displaying is 7x[Refresh rate].";
		//		public static readonly string TT_ScrollTip = "Highlight Scroll Lock when active language 1, selected in main window.";
		//		public static readonly string TT_StartupUpdateCheck = "If enabled Mahou will check for updates at startup.";
		//		public static readonly string TT_ConvertWords= "Allow to convert specific last word count by pressing hotkey and then 0-9 (0 = 10) on keyboard.";
		//		public static readonly string TT_Logging = "Designed ONLY to search for errors, BIG PERFORMANCE IMPACT, logs are saved in Mahou's folder, in folder Logs.";
		//		public static readonly string TT_LDForCaret = "Displays language tooltip around caret (carriage).";
		//		public static readonly string TT_LDDifferentAppearence = "If enabled language tooltips will use different color for different layouts (1&2).";
		//		public static readonly string TT_CaretRefreshRate = "Speed of checking caret availability.";
		//		public static readonly string TT_FGColor = "Foreground/Text color.";
		//		public static readonly string TT_BGColor = "Background color.";
		//		public static readonly string TT_CapsDis = "If enabled and change layout by key set to CapsLock then timer which will disable CapsLock if it is active will launch.";
		//		public static readonly string TT_CountryFlags = "If enabled Mahou will display country flags in tray icon.\nYou can add or change flags to yours, to make this create folder Flags in folder where Mahou.exe is, and paste there png with name of 2 letters which you can see in language tooltip.";
//		#endregion
	}
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace Mahou
{
	class MMain
	{
		#region DLLs
		[DllImport("user32.dll")]
		public static extern uint RegisterWindowMessage(string message);
		#endregion
		#region Prevent another instance variables
		/// GGPU = Global GUID PC User
		public static string GGPU_Mutex = "Global\\" +"ec511418-1d57-4dbe-a0c3-c6022b33735b_" + Environment.UserDomainName + "_" + Environment.UserName;
		public static uint ao = RegisterWindowMessage("AlderyOpenedMahou!");
		#endregion
		#region All Main variables, arrays etc.
		public static List<KMHook.YuKey> c_word = new List<KMHook.YuKey>();
		public static List<List<KMHook.YuKey>> c_words = new List<List<KMHook.YuKey>>();
		public static IntPtr _evt_hookID = IntPtr.Zero;
		public static WinAPI.WinEventDelegate _evt_proc = KMHook.EventHookCallback;
		public static Locales.Locale[] locales = Locales.AllList();
		public static string _language = "";
		public static Dictionary<Languages.Element, string> Lang = Languages.English;
		public static Configs MyConfs;
		public static MahouUI mahou;
		public static RawInputForm rif;
		public static System.Threading.Timer _logTimer = new System.Threading.Timer((_) => { try { Logging.UpdateLog(); } catch (Exception e) { Logging.Log("Error updating log, details:\r\n" + e.Message);}}, null, 20, 300);
		public static List<string> lcnmid = new List<string>();
		#endregion
		[STAThread] //DO NOT REMOVE THIS
        public static void Main(string[] args)
		{
			//Catch any error during program runtime
			AppDomain.CurrentDomain.UnhandledException += (obj, arg) => {
				var e = (Exception)arg.ExceptionObject;
				Logging.Log("Unexpected error occurred, Mahou exited, error details:\r\n" + e.Message+"\r\n" + e.StackTrace, 1);
			};
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			if (System.Globalization.CultureInfo.InstalledUICulture.TwoLetterISOLanguageName == "ru")
				Lang = Languages.Russian;
			MyConfs = new Configs();
			if (Configs.forceAppData && Configs.fine)
				MyConfs.Write("Functions", "AppDataConfigs", "true");
			Logging.Log("Mahou started.");
			using (var mutex = new Mutex(false, GGPU_Mutex)) {
				if (!mutex.WaitOne(0, false)) {
					WinAPI.PostMessage((IntPtr)0xffff, ao, 0, 0);
					return;
				}
				if (MMain.MyConfs.ReadBool("Functions", "AppDataConfigs")) {
					var mahou_folder_appd = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mahou");
					if (!Directory.Exists(mahou_folder_appd))
						Directory.CreateDirectory(mahou_folder_appd);
					Configs.filePath = Path.Combine(mahou_folder_appd, "Mahou.ini");
					MyConfs = new Configs();
				} else 
					Configs.filePath = Path.Combine(MahouUI.nPath, "Mahou.ini");
				MahouUI.latest_save_dir = Configs.filePath;
				if (MyConfs.ReadBool("FirstStart", "First")) {
					if (System.Globalization.CultureInfo.InstalledUICulture.TwoLetterISOLanguageName == "ru") {
						MyConfs.Write("Appearence", "Language", "Русский");
						MahouUI.InitLanguage();
//						MyConfs.Write("Layouts", "SpecificLayout1", Lang[Languages.Element.SwitchBetween]);
						MyConfs.Write("Layouts", "SpecificKeySets", "set_1/20//"+Lang[Languages.Element.SwitchBetween]+"|set_2///");
						MyConfs.Write("FirstStart", "First", "False");
					}
				} else {
					MahouUI.InitLanguage();
				}
				RefreshLCnMID();
				//for first run, add your locale 1 & locale 2 to settings
				if (MyConfs.Read("Layouts", "MainLayout1") == "" && MyConfs.Read("Layouts", "MainLayout2") == "") {
					Logging.Log("Initializing locales.");
					MyConfs.Write("Layouts", "MainLayout1", lcnmid[0]);
					MyConfs.Write("Layouts", "MainLayout2", lcnmid[1]);
				}
				mahou = new MahouUI();
				rif = new RawInputForm();
				Locales.IfLessThan2();
				if (MyConfs.Read("Layouts", "MainLayout1") == "" && MyConfs.Read("Layouts", "MainLayout2") == "") {
					mahou.cbb_MainLayout1.SelectedIndex = 0;
					mahou.cbb_MainLayout2.SelectedIndex = 1;
				}
				Application.EnableVisualStyles(); // Huh i did not noticed that it was missing... '~'
				_evt_hookID = WinAPI.SetWinEventHook(WinAPI.EVENT_SYSTEM_FOREGROUND, WinAPI.EVENT_SYSTEM_FOREGROUND,
				                                     IntPtr.Zero, _evt_proc, 0, 0, WinAPI.WINEVENT_OUTOFCONTEXT);
				KMHook.CheckLayoutLater.Tick += (_, __) => { MahouUI.GlobalLayout = Locales.GetCurrentLocale(); KMHook.CheckLayoutLater.Stop();};
				if (args.Length != 0)
				if (args[0] == "_!_updated_!_") {
					Logging.Log("Mahou updated.");
					mahou.ToggleVisibility();
					MessageBox.Show(Lang[Languages.Element.UpdateComplete], Lang[Languages.Element.UpdateComplete], MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				MahouUI.GlobalLayout = MahouUI.currentLayout = Locales.GetLocaleFromString(mahou.MainLayout1).uId;
				Application.Run();
			}
		}
        public static void RefreshLCnMID() {
        	MMain.locales = Locales.AllList();
        	lcnmid.Clear();
			foreach (var lc in locales) {	
				lcnmid.Add(lc.Lang + "(" + lc.uId + ")");
			}
        }
		public static bool MahouActive() {
			var ActHandle = WinAPI.GetForegroundWindow();
			if (ActHandle == IntPtr.Zero) {
				return false;
			}
			var active = mahou.Handle == ActHandle;
			Logging.Log("Mahou is active = ["+active+"]" + ", Mahou handle [" + mahou.Handle +"], fore win handle ["+ActHandle+"]");
			return active;
		}
	}
}
// Created by BladeMight in 05.12.2016-18:55
using System;
using System.Diagnostics;
using System.IO;

namespace Mahou
{
	public static class Logging
	{
		readonly static string logdir = Path.Combine(MahouUI.nPath, "Logs");
		readonly static string log = Path.Combine(logdir, DateTime.Today.ToString("yyyy.MM.dd") + ".log");
		static int errcount = 0; // Logging errors count
		static bool messagebox = false;	// To prevent multiple messageboxes
		/// <summary>
		/// Write message to log.
		/// </summary>
		/// <param name="logmsg">Message that will be written.</param>
		/// <param name="msgtype">Type of message: 0(or nothing) = Info, 1 = Error, 2 = Warning.</param>
		public static void Log(string logmsg, int msgtype = 0)
		{			
			if (!MahouUI.LoggingEnabled) return;
			if (!File.Exists(Configs.filePath)) 
				MMain.MyConfs = new Configs();	
			if (!Directory.Exists(Path.Combine(MahouUI.nPath, "Logs")))
				Directory.CreateDirectory(logdir);
			var messagetype = "Info";
			var msgtime = DateTime.Now.ToString("hh:mm:ss.fff");
			switch (msgtype) {
				case 1:
					messagetype = "Error";
					break;
				case 2:
					messagetype = "Warning";
					break;
			}
			var tologmsg = msgtime + " [" + messagetype + "]\r\n                    " + logmsg + "\r\n";
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while (true) {
				try {
					#if DEBUG
						Debug.WriteLine(tologmsg);
					#else
					File.AppendAllText(log, tologmsg);
					#endif
					break;
				} catch (Exception e) {
					Debug.WriteLine(msgtime + " [LogError]\r\n                    " + e.Message + "\r\n" + e.StackTrace + "\r\n");
					if (errcount > 10 && !messagebox) {
						messagebox = true;
//							MessageBox.Show(MMain.Msgs[14] + "\r\n\r\n" + e.Message + "\r\n\r\n" + e.StackTrace, MMain.Msgs[5]);
						MMain.MyConfs.Write("Functions", "Logging", "False");
						errcount = 0;
						messagebox = false;
					}
					errcount++;
				}						
				if (stopwatch.ElapsedMilliseconds > 50) {
					Debug.WriteLine(msgtime + " Last logging canceled...");
					break;
				}
				//Wait to Retry
				System.Threading.Thread.Sleep(5);
			}
			stopwatch.Stop();
		}
	}
}

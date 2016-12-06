// Created by BladeMight in 05.12.2016-18:55
using System;
using System.IO;

namespace Mahou
{
	public static class Logging
	{
		readonly static string logdir = Path.Combine(Update.nPath, "Logs");
		readonly static string log = Path.Combine(logdir, DateTime.Today.ToString("yyyy.MM.dd") + ".log");
		/// <summary>
		/// Write message to log.
		/// </summary>
		/// <param name="logmsg">Message that will be written.</param>
		/// <param name="msgtype">Type of message: 0(or nothing) = Info, 1 = Error, 2 = Warning.</param>
		public static void Log(string logmsg, int msgtype = 0)
		{
			if (!Directory.Exists(Path.Combine(Update.nPath, "Logs")))
				Directory.CreateDirectory(logdir);
			if (MMain.MyConfs.ReadBool("Functions", "Logging")) { 
				var messagetype = "Info";
				var msgtime = DateTime.Now.ToString("hh:mm:ss.fff");
				switch (msgtype) {
					case 1:
						messagetype = "Error"; break;
					case 2:
						messagetype = "Warning"; break;
				}
				File.AppendAllText(log, msgtime + " [" + messagetype + "]\r\n                    " + logmsg + "\r\n");
			}
		}
	}
}

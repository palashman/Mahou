using System;
using System.Windows.Forms;

namespace Mahou {
    public class TrayIcon {
        public event EventHandler<EventArgs> Exit;
        public event EventHandler<EventArgs> EnaDisable;
        public event EventHandler<EventArgs> ShowHide;
        public event EventHandler<EventArgs> Restart;
        public NotifyIcon trIcon;
        ContextMenu cMenu;
        MenuItem Exi, ShHi, EnDis, Resta;
        /// <summary>Initializes new tray icon.</summary>
        /// <param name="visible">State of tray icon's visibility on initialize.</param>
        public TrayIcon(bool? visible = true) {
            trIcon = new NotifyIcon();
            cMenu = new ContextMenu();
            trIcon.Icon = Properties.Resources.MahouTrayHD;
            trIcon.Visible = visible == true;
            Exi = new MenuItem("Exit", ExitHandler);
            ShHi = new MenuItem("Show", ShowHideHandler);
            EnDis = new MenuItem("Enable", EnaDisableHandler);
            Resta = new MenuItem("Restart", RestartHandler);
            EnDis.Checked = true;
            cMenu.MenuItems.Add(ShHi);
            cMenu.MenuItems.Add(EnDis);
            cMenu.MenuItems.Add(Resta);
            cMenu.MenuItems.Add(Exi);
            trIcon.Text = "Mahou (魔法)\nA magical layout switcher.";
            trIcon.ContextMenu = cMenu;
            trIcon.MouseClick += (_,__) => { if (__.Button == MouseButtons.Left) ShowHideHandler(_,__); };
        }
        public void CheckShHi(bool shhi) {
        	ShHi.Checked = shhi;
        }
        public void CheckEnDis(bool endis) {
        	EnDis.Checked = endis;
        }
        /// <summary>Toggle Mahou, enable/disable event handler..</summary>
        void EnaDisableHandler(object sender, EventArgs e) {
            if (EnaDisable != null)
                EnaDisable(this, null);
        }
        /// <summary>Restart event handler..</summary>
        void RestartHandler(object sender, EventArgs e) {
            if (Restart != null)
                Restart(this, null);
        }
        /// <summary>Exit event handler..</summary>
        void ExitHandler(object sender, EventArgs e) {
            if (Exit != null)
                Exit(this, null);
        }
        /// <summary>ShowHide event handler..</summary>
        void ShowHideHandler(object sender, EventArgs e) {
            if (ShowHide != null)
                ShowHide(this, null);
        }
        /// <summary>Hides tray icon.</summary>
        public void Hide() {
        	if (MMain.mahou.TrayFlags || MMain.mahou.TrayText)
        		MMain.mahou.flagsCheck.Stop();
            trIcon.Visible = false;
        }
        /// <summary>Shows tray icon.</summary>
        public void Show() {
        	if (MMain.mahou.TrayFlags || MMain.mahou.TrayText)
        		MMain.mahou.flagsCheck.Start();
            trIcon.Visible = true;
        }
        /// <summary>Refreshes tray icon various text.</summary>
        /// <param name="TrText">Tray icon hover text.</param>
        /// <param name="ShHiText">Show/Hide tray icon menu item's text.</param>
        /// <param name="ExiText">Exit tray icon menu item's text.</param>
        public void RefreshText(string TrText, string ShHiText, string ExiText, string EnaDisText, string RestartText) {
            trIcon.Text = TrText;
            if (!String.IsNullOrEmpty(ShHiText))
            	ShHi.Text = ShHiText;
            if (!String.IsNullOrEmpty(ExiText))
            	Exi.Text = ExiText;
            if (!String.IsNullOrEmpty(EnaDisText))
            	EnDis.Text = EnaDisText;
            if (!String.IsNullOrEmpty(RestartText))
            	Resta.Text = RestartText;
        }
    }
}

using System;
using System.Windows.Forms;

namespace Mahou
{
    public class TrayIcon
    {
        public event EventHandler<EventArgs> Exit;
        public event EventHandler<EventArgs> ShowHide;
        public NotifyIcon trIcon;
        ContextMenu cMenu;
        MenuItem Exi, ShHi;
        /// <summary>
        /// Initializes new tray icon.
        /// </summary>
        /// <param name="visible">State of tray icon's visibility on initialize.</param>
        public TrayIcon(bool? visible = true)
        {
            trIcon = new NotifyIcon();
            cMenu = new ContextMenu();
            trIcon.Icon = Properties.Resources.MahouTrayHD;
            trIcon.Visible = visible == true;
            Exi = new MenuItem("Exit", ExitHandler);
            ShHi = new MenuItem("Show/Hide", ShowHideHandler);
            cMenu.MenuItems.Add(ShHi);
            cMenu.MenuItems.Add(Exi);
            trIcon.Text = "Mahou (魔法)\nA magical layout switcher.";
            trIcon.ContextMenu = cMenu;
            trIcon.MouseClick += (_,__) => { if (__.Button == MouseButtons.Left) ShowHideHandler(_,__); };
            trIcon.BalloonTipClicked += ExitHandler;
        }
        /// <summary>
        /// Exit event handler..
        /// </summary>
        void ExitHandler(object sender, EventArgs e)
        {
            if (Exit != null)
            {
                Exit(this, null);
            }
        }
        /// <summary>
        /// ShowHide event handler..
        /// </summary>
        void ShowHideHandler(object sender, EventArgs e)
        {
            if (ShowHide != null)
            {
                ShowHide(this, null);
            }
        }
        /// <summary>
        /// Hides tray icon.
        /// </summary>
        public void Hide()
        {
        	if (MMain.mahou.TrayFlags)
        		MMain.mahou.flagsCheck.Stop();
            trIcon.Visible = false;
        }
        /// <summary>
        /// Shows tray icon.
        /// </summary>
        public void Show()
        {
        	if (MMain.mahou.TrayFlags)
        		MMain.mahou.flagsCheck.Start();
            trIcon.Visible = true;
        }
        /// <summary>
        /// Refreshes tray icon various text.
        /// </summary>
        /// <param name="TrText">Tray icon hover text.</param>
        /// <param name="ShHiText">Show/Hide tray icon menu item's text.</param>
        /// <param name="ExiText">Exit tray icon menu item's text.</param>
        public void RefreshText(string TrText, string ShHiText, string ExiText)
        {
            trIcon.Text = TrText;
            ShHi.Text = ShHiText;
            Exi.Text = ExiText;
        }
    }
}

using ManagedWinapi.Windows;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MakeTopMost
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainAppContext());
        }
    }

    public class MainAppContext
        : ApplicationContext
    {
        public MainAppContext()
        {
            NotifyIcon trayIcon = new NotifyIcon() { Icon = Properties.Resources.TopMost, Text = "MakeTopMost", Visible = true };
            trayIcon.Click += (s, e) =>
                {
                    if (this.lastMenu != null)
                    {
                        this.lastMenu.Dispose();
                    }

                    this.lastMenu = this.CreateMenu();

                    NativeWindow trayIconWindow = (NativeWindow)typeof(NotifyIcon).GetField("window", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(trayIcon);
                    POINT cursorLocation = new POINT(); NativeMethods.GetCursorPos(out cursorLocation);
                    SystemWindow.ForegroundWindow = new SystemWindow(trayIconWindow.Handle);
                    NativeMethods.TrackPopupMenuEx(new HandleRef(this.lastMenu, this.lastMenu.Handle), 72, cursorLocation.X, cursorLocation.Y, new HandleRef(trayIconWindow, trayIconWindow.Handle), IntPtr.Zero);
                    NativeMethods.PostMessage(new HandleRef(trayIconWindow, trayIconWindow.Handle), 0, IntPtr.Zero, IntPtr.Zero);
                };

            this.ThreadExit += (s, e) =>
                {
                    trayIcon.Dispose();
                    if (this.lastMenu != null)
                    {
                        this.lastMenu.Dispose();
                    }
                };
        }

        private ContextMenu lastMenu;
        public ContextMenu CreateMenu()
        {
            SystemWindow foregroundWindow = SystemWindow.ForegroundWindow;
            return new ContextMenu(SystemWindow.FilterToplevelWindows(window => window.Visible && !string.IsNullOrWhiteSpace(window.Title) && window != foregroundWindow && window.Process.ProcessName != "explorer")
                .Select(window => new SystemWindowMenuItem(window))
                .Concat(new MenuItem[] { new MenuItem("-"), new MenuItem("E&xit", (s, e) => Application.Exit(), Shortcut.AltF4) }).ToArray());
        }

        public class SystemWindowMenuItem
            : MenuItem
        {
            public SystemWindowMenuItem(SystemWindow window)
            {
                this.Window = window;
                this.Text =  window.Title + " (" + window.Process.ProcessName + ")"; //window.Title;
                this.Checked = window.TopMost;
            }

            protected override void OnClick(EventArgs e)
            {
                if (this.Window.IsValid())
                {
                    this.Window.TopMost = !this.Window.TopMost;
                    this.Checked = this.Window.TopMost;
                    if (this.Window.TopMost)
                    {
                        SystemWindow.ForegroundWindow = this.Window;
                    }
                }

                base.OnClick(e);
            }

            public SystemWindow Window { get; private set; }
        }
    }
}
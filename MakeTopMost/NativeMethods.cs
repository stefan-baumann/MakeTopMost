using ManagedWinapi.Windows;
using System;
using System.Runtime.InteropServices;

namespace MakeTopMost
{
    public static class NativeMethods
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = false)]
        public static extern bool PostMessage(HandleRef hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool TrackPopupMenuEx(HandleRef hmenu, int fuFlags, int x, int y, HandleRef hwnd, IntPtr lptpm);
    }
}
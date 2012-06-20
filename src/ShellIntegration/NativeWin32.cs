// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.ShellIntegration
// 
//  <copyright file="NativeWin32.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
// 
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.ShellIntegration
{
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// The native win 32.
    /// </summary>
    internal static class NativeWin32
    {
        #region Constants and Fields

        /// <summary>
        /// The gw child.
        /// </summary>
        public const int GwChild = 5;

        /// <summary>
        /// The gw hwndfirst.
        /// </summary>
        public const int GwHwndfirst = 0;

        /// <summary>
        /// The gw hwndlast.
        /// </summary>
        public const int GwHwndlast = 1;

        /// <summary>
        /// The gw hwndnext.
        /// </summary>
        public const int GwHwndnext = 2;

        /// <summary>
        /// The gw hwndprev.
        /// </summary>
        public const int GwHwndprev = 3;

        /// <summary>
        /// The gw owner.
        /// </summary>
        public const int GwOwner = 4;

        /// <summary>
        /// The sc close.
        /// </summary>
        public const int ScClose = 0xF060;

        /// <summary>
        /// The wm syscommand.
        /// </summary>
        public const int WmSyscommand = 0x0112;

        #endregion

        #region Delegates

        /// <summary>
        /// The enum windows proc delegate.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="lParam">
        /// The l param.
        /// </param>
        public delegate int EnumWindowsProcDelegate(int hWnd, int lParam);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The enum windows.
        /// </summary>
        /// <param name="lpEnumFunc">
        /// The lp enum func.
        /// </param>
        /// <param name="lParam">
        /// The l param.
        /// </param>
        /// <returns>
        /// The enum windows.
        /// </returns>
        [DllImport("user32")]
        public static extern int EnumWindows(EnumWindowsProcDelegate lpEnumFunc, int lParam);

        /// <summary>
        /// The find window.
        /// </summary>
        /// <param name="lpClassName">
        /// The lp class name.
        /// </param>
        /// <param name="lpWindowName">
        /// The lp window name.
        /// </param>
        /// <returns>
        /// The find window.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int FindWindow(
            string lpClassName, 
            // class name
            string lpWindowName // window name
            );

        /// <summary>
        /// The get desktop window.
        /// </summary>
        /// <returns>
        /// The get desktop window.
        /// </returns>
        [DllImport("user32")]
        public static extern int GetDesktopWindow();

        /// <summary>
        /// The get parent.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <returns>
        /// The get parent.
        /// </returns>
        [DllImport("user32")]
        public static extern int GetParent(int hwnd);

        /// <summary>
        /// The get window.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="wCmd">
        /// The w cmd.
        /// </param>
        /// <returns>
        /// The get window.
        /// </returns>
        [DllImport("user32")]
        public static extern int GetWindow(int hwnd, int wCmd);

        /// <summary>
        /// The get window long ptr.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="nIndex">
        /// The n index.
        /// </param>
        /// <returns>
        /// The get window long ptr.
        /// </returns>
        [DllImport("user32", EntryPoint = "GetWindowLongA")]
        public static extern int GetWindowLongPtr(int hwnd, int nIndex);

        /// <summary>
        /// The get window text.
        /// </summary>
        /// <param name="h">
        /// The h.
        /// </param>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="nMaxCount">
        /// The n max count.
        /// </param>
        [DllImport("User32.Dll")]
        public static extern void GetWindowText(int h, StringBuilder s, int nMaxCount);

        /// <summary>
        /// The is window visible.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <returns>
        /// The is window visible.
        /// </returns>
        [DllImport("user32")]
        public static extern int IsWindowVisible(int hwnd);

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="wParam">
        /// The w param.
        /// </param>
        /// <param name="lParam">
        /// The l param.
        /// </param>
        /// <returns>
        /// The send message.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(
            int hWnd, 
            // handle to destination window
            uint msg, 
            // message
            int wParam, 
            // first message parameter
            int lParam // second message parameter
            );

        /// <summary>
        /// The set foreground window.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <returns>
        /// The set foreground window.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(
            int hWnd // handle to window
            );

        #endregion
    }
}
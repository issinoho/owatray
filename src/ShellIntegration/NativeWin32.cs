// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.ShellIntegration.NativeWin32.cs
//
//  <copyright file="NativeWin32.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2013 The Drunken Bakery. All rights reserved.
//  </copyright>
//
//  Author: Iain Smith
// ------------------------------------------------------------------
namespace DrunkenBakery.OWAtray.ShellIntegration
{
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    ///     The native win 32.
    /// </summary>
    internal static class NativeWin32
    {
        #region Constants

        /// <summary>
        ///     The gw child.
        /// </summary>
        public const int GwChild = 5;

        /// <summary>
        ///     The gw hwndfirst.
        /// </summary>
        public const int GwHwndfirst = 0;

        /// <summary>
        ///     The gw hwndlast.
        /// </summary>
        public const int GwHwndlast = 1;

        /// <summary>
        ///     The gw hwndnext.
        /// </summary>
        public const int GwHwndnext = 2;

        /// <summary>
        ///     The gw hwndprev.
        /// </summary>
        public const int GwHwndprev = 3;

        /// <summary>
        ///     The gw owner.
        /// </summary>
        public const int GwOwner = 4;

        /// <summary>
        ///     The sc close.
        /// </summary>
        public const int ScClose = 0xF060;

        /// <summary>
        ///     The wm syscommand.
        /// </summary>
        public const int WmSyscommand = 0x0112;

        #endregion

        #region Delegates

        /// <summary>
        /// The enum windows proc delegate.
        /// </summary>
        /// <param name="hwnd">
        /// The h wnd.
        /// </param>
        /// <param name="param">
        /// The l param.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public delegate int EnumWindowsProcDelegate(int hwnd, int param);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The enum windows.
        /// </summary>
        /// <param name="enumFunc">
        /// The lp enum func.
        /// </param>
        /// <param name="param">
        /// The l param.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32")]
        public static extern int EnumWindows(EnumWindowsProcDelegate enumFunc, int param);

        /// <summary>
        /// The find window.
        /// </summary>
        /// <param name="className">
        /// The lp class name.
        /// </param>
        /// <param name="windowName">
        /// The lp window name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int FindWindow(string className, string windowName);

        /// <summary>
        /// The get desktop window.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
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
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32")]
        public static extern int GetParent(int hwnd);

        /// <summary>
        /// The get window.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="cmd">
        /// The w cmd.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32")]
        public static extern int GetWindow(int hwnd, int cmd);

        /// <summary>
        /// The get window long ptr.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="index">
        /// The n index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32", EntryPoint = "GetWindowLongA")]
        public static extern int GetWindowLongPtr(int hwnd, int index);

        /// <summary>
        /// The get window text.
        /// </summary>
        /// <param name="h">
        /// The h.
        /// </param>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="maxCount">
        /// The n max count.
        /// </param>
        [DllImport("User32.Dll")]
        public static extern void GetWindowText(int h, StringBuilder s, int maxCount);

        /// <summary>
        /// The is window visible.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32")]
        public static extern int IsWindowVisible(int hwnd);

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="wnd">
        /// The h wnd.
        /// </param>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="paramW">
        /// The w param.
        /// </param>
        /// <param name="paramL">
        /// The l param.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(int wnd, uint msg, int paramW, int paramL);

        /// <summary>
        /// The set foreground window.
        /// </summary>
        /// <param name="wnd">
        /// The h wnd.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(int wnd);

        #endregion
    }
}
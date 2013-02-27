// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.Snarl.SnarlHelper.cs
//  
//  <copyright file="SnarlHelper.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2013 The Drunken Bakery. All rights reserved.
//  </copyright>
//  
//  Author: Iain Smith
// ------------------------------------------------------------------
namespace DrunkenBakery.OWAtray.Snarl
{
    using System;

    using global::Snarl;

    /// <summary>
    ///     The snarl helper.
    /// </summary>
    public static class SnarlHelper
    {
        #region Constants

        /// <summary>
        ///     The reply msg.
        /// </summary>
        private const int ReplyMsg = 0x400 + 100;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The application.
        /// </summary>
        private static string application;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The pop snarl.
        /// </summary>
        /// <param name="myTitle">
        /// The my title.
        /// </param>
        /// <param name="myMessage">
        /// The my message.
        /// </param>
        /// <param name="iconPath">
        /// The icon path.
        /// </param>
        /// <param name="handle">
        /// The handle.
        /// </param>
        public static void PopSnarl(string myTitle, string myMessage, string iconPath, IntPtr handle)
        {
            SnarlConnector.ShowMessage(myTitle, myMessage, 10, iconPath, handle, (WindowsMessage)ReplyMsg);
        }

        /// <summary>
        /// The register snarl.
        /// </summary>
        /// <param name="thisApplication">
        /// The this application.
        /// </param>
        /// <param name="iconPath">
        /// The icon path.
        /// </param>
        /// <param name="handle">
        /// The handle.
        /// </param>
        public static void RegisterSnarl(string thisApplication, string iconPath, IntPtr handle)
        {
            application = thisApplication;
            SnarlConnector.RegisterConfig(handle, thisApplication, WindowsMessage.WM_MDIMAXIMIZE, iconPath);
        }

        /// <summary>
        /// The revoke.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        public static void Revoke(IntPtr handle)
        {
            SnarlConnector.RevokeConfig(handle);
        }

        #endregion
    }
}
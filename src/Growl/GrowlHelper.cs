// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Growl
// 
//  <copyright file="GrowlHelper.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
// 
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Growl
{
    using global::Growl.Connector;

    /// <summary>
    /// The growl helper.
    /// </summary>
    public static class GrowlHelper
    {
        #region Static Fields

        /// <summary>
        /// The _application.
        /// </summary>
        private static string application;

        /// <summary>
        /// The _notification title.
        /// </summary>
        private static string notificationTitle;

        /// <summary>
        /// The _simple growl.
        /// </summary>
        private static GrowlConnector simpleGrowl;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The pop growl.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void PopGrowl(string title, string message = "")
        {
            var myGrowl = new Notification(application, notificationTitle, title, title, message);
            simpleGrowl.Notify(myGrowl);
        }

        /// <summary>
        /// The register growl.
        /// </summary>
        /// <param name="application">
        /// The application.
        /// </param>
        /// <param name="iconPath">
        /// The icon path.
        /// </param>
        /// <param name="notificationTitle">
        /// The notification title.
        /// </param>
        /// <param name="notificationText">
        /// The notification text.
        /// </param>
        public static void RegisterGrowl(
            string application, string iconPath, string notificationTitle, string notificationText)
        {
            GrowlHelper.application = application;
            GrowlHelper.notificationTitle = notificationTitle;
            simpleGrowl = new GrowlConnector();
            var thisApp = new Application(application) { Icon = iconPath };
            var simpleGrowlType = new NotificationType(notificationTitle, notificationText);
            simpleGrowl.Register(thisApp, new[] { simpleGrowlType });
        }

        #endregion
    }
}
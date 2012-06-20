// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Logging
// 
//  <copyright file="LoggerProxy.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
// 
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Logging
{
    using System;

    using NLog;
    using NLog.Layouts;
    using NLog.Targets;

    /// <summary>
    /// The logger proxy.
    /// </summary>
    public static class LoggerProxy
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constants and Fields

        /// <summary>
        /// The default target name.
        /// </summary>
        private const string DefaultTargetName = "allTarget";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Filename.
        /// </summary>
        public static string Filename
        {
            get
            {
                return GetTargetFilename(DefaultTargetName);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        public static void Log(string message)
        {
            Log(message, true);
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <param name="success">
        /// The success. 
        /// </param>
        public static void Log(string message, bool success)
        {
            logger.Log(success ? LogLevel.Info : LogLevel.Error, message);
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <param name="ex">
        /// The ex. 
        /// </param>
        public static void Log(string message, Exception ex)
        {
            logger.ErrorException(message, ex);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get target.
        /// </summary>
        /// <param name="targetName">
        /// The target name. 
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        private static T GetTarget<T>(string targetName) where T : Target
        {
            if (null == LogManager.Configuration)
            {
                return null;
            }

            var target = LogManager.Configuration.FindTargetByName(targetName) as T;
            return target;
        }

        /// <summary>
        /// The get target filename.
        /// </summary>
        /// <param name="targetName">
        /// The target name. 
        /// </param>
        /// <returns>
        /// The get target filename. 
        /// </returns>
        private static string GetTargetFilename(string targetName)
        {
            var target = GetTarget<FileTarget>(targetName);
            if (null == target)
            {
                return null;
            }

            var layout = target.FileName as SimpleLayout;
            if (null == layout)
            {
                return null;
            }

            string filename = layout.Render(new LogEventInfo()).Replace(@"/", @"\");
            return filename;
        }

        #endregion
    }
}
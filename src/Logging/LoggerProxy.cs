// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.Logging.LoggerProxy.cs
//
//  <copyright file="LoggerProxy.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2013 The Drunken Bakery. All rights reserved.
//  </copyright>
//
//  Author: Iain Smith
// ------------------------------------------------------------------
namespace DrunkenBakery.OWAtray.Logging
{
    using System;

    using NLog;
    using NLog.Layouts;
    using NLog.Targets;

    /// <summary>
    ///     The logger proxy.
    /// </summary>
    public static class LoggerProxy
    {
        #region Constants

        /// <summary>
        ///     The default target name.
        /// </summary>
        private const string DefaultTargetName = "allTarget";

        #endregion

        #region Static Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets Filename.
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
        /// <param name="success">
        /// The success.
        /// </param>
        public static void Log(string message, bool success = true)
        {
            Logger.Log(success ? LogLevel.Info : LogLevel.Error, message);
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
            Logger.ErrorException(message, ex);
        }

        /// <summary>
        /// Writes a deep diagnostic entry to the file log only - never mirrored to the on-screen
        /// connection log, unlike <see cref="Log(string, bool)"/>/<see cref="Log(string, Exception)"/>
        /// (both called from <c>Form1.AddLogEntry</c>, which also appends to the visible ListView).
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Debug(string message)
        {
            Logger.Debug(message);
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
        /// The target type.
        /// </typeparam>
        /// <returns>
        /// The <typeparamref name="T"/>.
        /// </returns>
        private static T GetTarget<T>(string targetName)
            where T : Target
        {
            if (LogManager.Configuration == null)
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
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetTargetFilename(string targetName)
        {
            var target = GetTarget<FileTarget>(targetName);
            if (target == null)
            {
                return null;
            }

            var layout = target.FileName as SimpleLayout;
            if (layout == null)
            {
                return null;
            }

            string filename = layout.Render(new LogEventInfo()).Replace(@"/", @"\");
            return filename;
        }

        #endregion
    }
}
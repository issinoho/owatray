//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// LoggerProxy
//
// <copyright file="LoggerProxy.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Abstracts the logging layer.
//
//------------------------------------------------------------------

using System;
using NLog;
using NLog.Layouts;
using NLog.Targets;

namespace DrunkenBakery.OWAtray.Logging
{
    public static class LoggerProxy
    {
        private const string DefaultTargetName = "allTarget";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Log(string message)
        {
            Log(message, true);
        }

        public static void Log(string message, bool success)
        {
            Logger.Log(success ? LogLevel.Info : LogLevel.Error, message);            
        }

        public static void Log(string message, Exception ex)
        {
            Logger.ErrorException(message, ex);
        }

        private static string GetTargetFilename(string targetName)
        {
            var target = GetTarget<FileTarget>(targetName);
            if (null == target) return null;

            var layout = target.FileName as SimpleLayout;
            if (null == layout) return null;

            var filename = layout.Render(new LogEventInfo()).Replace(@"/", @"\");
            return filename;
        }

        public static string Filename
        {
            get { return GetTargetFilename(DefaultTargetName); }
        }

        private static T GetTarget<T>(string targetName)
            where T : Target
        {
            if (null == LogManager.Configuration) return null;
            var target = LogManager.Configuration.FindTargetByName(targetName) as T;
            return target;
        }
    }
}
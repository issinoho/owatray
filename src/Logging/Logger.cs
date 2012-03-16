//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// Logger
//
// <copyright file="Logger.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Abstracts the logging layer.
//
//------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;

namespace DrunkenBakery.OWAtray.Logging
{
	public static class Logger
	{
		private const string ConfigFileName = "Log4Net.config";
		private static bool _logInitialized;
		private static readonly Dictionary<Type, ILog> Loggers = new Dictionary<Type, ILog>();

		public static Action<object, object> Debug = (source, message) => { };
		public static Action<object, object> Error = (source, message) => { };
		public static Action<object, object> Fatal = (source, message) => { };
		public static Action<object, object> Info = (source, message) => { };
		public static Action<object, object> Warn = (source, message) => { };

		public static string Filename
		{
			get
			{
				var rootAppender = (FileAppender)((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root.Appenders[0];
				return rootAppender.File;
			}
		}

		public static void Execute()
		{
			Debug = (source, message) => GetLogger(GetSourceType(source)).Debug(message);
			Error = (source, message) => GetLogger(GetSourceType(source)).Error(message);
			Fatal = (source, message) => GetLogger(GetSourceType(source)).Fatal(message);
			Info = (source, message) => GetLogger(GetSourceType(source)).Info(message);
			Warn = (source, message) => GetLogger(GetSourceType(source)).Warn(message);
		}

		private static Type GetSourceType(object source)
		{
			var sourceType = source.GetType();
			if (sourceType == typeof(Type))
				return source as Type;
			return sourceType;
		}

		private static void Initialize()
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo(GetConfigFilePath()));
		}

		private static string GetConfigFilePath()
		{
			var basePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			var configPath = Path.Combine(basePath, ConfigFileName);

			if (!File.Exists(configPath))
			{
				configPath = Path.Combine(basePath, "bin");
				configPath = Path.Combine(configPath, ConfigFileName);

				if (!File.Exists(configPath))
					configPath = Path.Combine(basePath, @"..\" + ConfigFileName);
			}

			return configPath;
		}

		private static void EnsureInitialized()
		{
			if (_logInitialized) return;

			Initialize();
			_logInitialized = true;
		}

		private static ILog GetLogger(Type source)
		{
			EnsureInitialized();

			if (!Loggers.ContainsKey(source))
			{
				lock (Loggers)
				{
					if (!Loggers.ContainsKey(source))
						Loggers.Add(source, LogManager.GetLogger(source));
				}
			}

			return Loggers[source];
		}
	}
}
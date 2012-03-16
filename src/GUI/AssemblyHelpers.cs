//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// AssemblyHelpers Class
//
// <copyright file="AssemblyHelpers.cs" company="The Drunken Bakery">
//     Copyright (c) 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Class to supply helper methods related to the Assembly
//
//------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DrunkenBakery.OWAtray.GUI.Properties;

namespace DrunkenBakery.OWAtray.GUI
{
	public static class AssemblyHelpers
	{
		public static string ProductName
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyProductAttribute), false);
				return attributes.Length > 0 ? ((AssemblyProductAttribute) attributes[0]).Product : Resources.AssemblyHelpers_ProductName_Unknown;
			}
		}

		public static string AssemblyCompany
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyCompanyAttribute), false);
				return (attributes.Length > 0 ? ((AssemblyCompanyAttribute) attributes[0]).Company : string.Empty);
			}
		}

		public static string AssemblyCopyright
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
				return (attributes.Length > 0 ? ((AssemblyCopyrightAttribute) attributes[0]).Copyright : string.Empty);
			}
		}

		public static string AssemblyDescription
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyDescriptionAttribute),
				                                                                          false);
				return (attributes.Length > 0 ? ((AssemblyDescriptionAttribute) attributes[0]).Description : string.Empty);
			}
		}

		public static string AssemblyProduct
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyProductAttribute), false);
				return (attributes.Length > 0 ? ((AssemblyProductAttribute) attributes[0]).Product : string.Empty);
			}
		}

		public static string AssemblyTitle
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					var titleAttribute = (AssemblyTitleAttribute) attributes[0];
					if (titleAttribute.Title != string.Empty) return titleAttribute.Title;
				}
				return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public static string AssemblyVersion
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		public static string DotNetRuntimeVersion
		{
			get { return Assembly.GetExecutingAssembly().ImageRuntimeVersion; }
		}

		public static string UpgradeSettings()
		{
			var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
			var appVersionString = appVersion.ToString();

			if (Properties.Settings.Default.ApplicationVersion != appVersion.ToString())
			{
				Properties.Settings.Default.Upgrade();
				Properties.Settings.Default.ApplicationVersion = appVersionString;
			}

			return appVersionString;
		}

		public static IEnumerable<string> DependentAssemblies()
		{
			var refs = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
			return refs.OrderBy(x => x.Name).Select(myRef => string.Format("{0} v{1}", myRef.Name, myRef.Version));
		}
	}
}
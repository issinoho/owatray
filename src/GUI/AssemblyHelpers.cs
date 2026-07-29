// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.GUI
//
//  <copyright file="AssemblyHelpers.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
//
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.GUI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using DrunkenBakery.OWAtray.GUI.Properties;

    /// <summary>
    /// The assembly helpers.
    /// </summary>
    public static class AssemblyHelpers
    {
        #region Public Properties

        /// <summary>
        /// Gets AssemblyCompany.
        /// </summary>
        public static string AssemblyCompany
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return attributes.Length > 0 ? ((AssemblyCompanyAttribute)attributes[0]).Company : string.Empty;
            }
        }

        /// <summary>
        /// Gets AssemblyCopyright.
        /// </summary>
        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return attributes.Length > 0 ? ((AssemblyCopyrightAttribute)attributes[0]).Copyright : string.Empty;
            }
        }

        /// <summary>
        /// Gets AssemblyDescription.
        /// </summary>
        public static string AssemblyDescription
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length > 0 ? ((AssemblyDescriptionAttribute)attributes[0]).Description : string.Empty;
            }
        }

        /// <summary>
        /// Gets AssemblyProduct.
        /// </summary>
        public static string AssemblyProduct
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length > 0 ? ((AssemblyProductAttribute)attributes[0]).Product : string.Empty;
            }
        }

        /// <summary>
        /// Gets AssemblyTitle.
        /// </summary>
        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(
                    typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != string.Empty)
                    {
                        return titleAttribute.Title;
                    }
                }

                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        /// Gets AssemblyVersion.
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                // AssemblyInfo.cs only ever sets three version components (e.g. "3.5.1"), so .NET pads
                // the unused fourth (Revision) with 0. Trim it back to three parts here so the About box
                // matches the version number used everywhere else (release tags, the NSIS installer).
                return Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            }
        }

        /// <summary>
        /// Gets DotNetRuntimeVersion.
        /// </summary>
        public static string DotNetRuntimeVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().ImageRuntimeVersion;
            }
        }

        /// <summary>
        /// Gets ProductName.
        /// </summary>
        public static string ProductName
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length > 0
                           ? ((AssemblyProductAttribute)attributes[0]).Product
                           : Resources.AssemblyHelpers_ProductName_Unknown;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dependent assemblies.
        /// </summary>
        /// <returns>
        /// A list of dependent assemblies.
        /// </returns>
        public static IEnumerable<string> DependentAssemblies()
        {
            AssemblyName[] refs = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            return refs.OrderBy(x => x.Name).Select(myRef => string.Format("{0} v{1}", myRef.Name, myRef.Version));
        }

        /// <summary>
        /// The upgrade settings.
        /// </summary>
        /// <returns>
        /// The new version.
        /// </returns>
        public static string UpgradeSettings()
        {
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            string appVersionString = appVersion.ToString();

            if (Settings.Default.ApplicationVersion != appVersion.ToString())
            {
                Settings.Default.Upgrade();
                Settings.Default.ApplicationVersion = appVersionString;
                Settings.Default.Save();
            }

            return appVersionString;
        }

        #endregion
    }
}
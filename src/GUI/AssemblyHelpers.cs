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
    using System.Runtime.Versioning;

    using DrunkenBakery.OWAtray.GUI.Properties;

    using Microsoft.Win32;

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
        /// Gets the .NET Framework version actually installed and running (e.g. "4.8.1"). .NET 4.5 and
        /// every version after it are in-place updates of .NET 4, so neither <see cref="DotNetRuntimeVersion"/>
        /// (the CLR's own image version, always "v4.0.30319" for anything from 4.0 through 4.8.1) nor
        /// <see cref="Environment.Version"/> (same problem) can tell them apart - both look identical
        /// on a machine running 4.0 and one running 4.8.1. The real version is only readable from the
        /// numeric "Release" value under v4\Full, mapped the same way as the Tools -&gt; .NET Versions
        /// dialog (see <see cref="GetFriendly45PlusVersion"/>). Falls back to <see cref="DotNetRuntimeVersion"/>
        /// if that key/value isn't present (i.e. this is somehow running on .NET 4.0 itself).
        /// </summary>
        public static string InstalledDotNetFrameworkVersion
        {
            get
            {
                RegistryKey ndpKey =
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full", false);
                if (ndpKey != null)
                {
                    object release = ndpKey.GetValue("Release");
                    if (release != null)
                    {
                        return GetFriendly45PlusVersion((int)release);
                    }
                }

                return DotNetRuntimeVersion;
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

        /// <summary>
        /// Gets the .NET Framework version this assembly was built to target (e.g. "4.0"), i.e.
        /// <c>TargetFrameworkVersion</c> from the project file, embedded at build time via
        /// <see cref="TargetFrameworkAttribute"/>. This is what "compiled for" should mean; it's
        /// distinct from - and, unlike <see cref="DotNetRuntimeVersion"/>, actually distinguishes -
        /// which .NET Framework release is installed and running (see
        /// <see cref="InstalledDotNetFrameworkVersion"/>).
        /// </summary>
        public static string TargetDotNetFrameworkVersion
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TargetFrameworkAttribute), false);
                if (attributes.Length == 0)
                {
                    return DotNetRuntimeVersion;
                }

                var frameworkName = new FrameworkName(((TargetFrameworkAttribute)attributes[0]).FrameworkName);
                return frameworkName.Version.ToString();
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
        /// Maps a .NET 4.5+ "Release" registry value to the friendly version it corresponds to, per
        /// https://learn.microsoft.com/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed.
        /// </summary>
        /// <param name="releaseKey">
        /// The release key.
        /// </param>
        /// <returns>
        /// The friendly version string.
        /// </returns>
        public static string GetFriendly45PlusVersion(int releaseKey)
        {
            if (releaseKey >= 533320)
            {
                return "4.8.1";
            }

            if (releaseKey >= 528040)
            {
                return "4.8";
            }

            if (releaseKey >= 461808)
            {
                return "4.7.2";
            }

            if (releaseKey >= 461308)
            {
                return "4.7.1";
            }

            if (releaseKey >= 460798)
            {
                return "4.7";
            }

            if (releaseKey >= 394802)
            {
                return "4.6.2";
            }

            if (releaseKey >= 394254)
            {
                return "4.6.1";
            }

            if (releaseKey >= 393295)
            {
                return "4.6";
            }

            if (releaseKey >= 379893)
            {
                return "4.5.2";
            }

            if (releaseKey >= 378675)
            {
                return "4.5.1";
            }

            if (releaseKey >= 378389)
            {
                return "4.5";
            }

            return "4.0";
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
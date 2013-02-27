// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.GUI.WindowsShortcut.cs
//  
//  <copyright file="WindowsShortcut.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2013 The Drunken Bakery. All rights reserved.
//  </copyright>
//  
//  Author: Iain Smith
// ------------------------------------------------------------------
namespace DrunkenBakery.OWAtray.GUI
{
    using System;
    using System.IO;

    using IWshRuntimeLibrary;

    /// <summary>
    ///     The windows shortcut.
    /// </summary>
    public static class WindowsShortcut
    {
        #region Public Methods and Operators

        /// <summary>
        /// The exists.
        /// </summary>
        /// <param name="folder">
        /// The folder.
        /// </param>
        /// <param name="linkPathName">
        /// The link path name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Exists(Environment.SpecialFolder folder, string linkPathName)
        {
            return Exists(Environment.GetFolderPath(folder), linkPathName);
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="folder">
        /// The folder.
        /// </param>
        /// <param name="targetPathName">
        /// The target path name.
        /// </param>
        /// <param name="linkPathName">
        /// The link path name.
        /// </param>
        /// <param name="install">
        /// The install.
        /// </param>
        public static void Update(
            Environment.SpecialFolder folder, string targetPathName, string linkPathName, bool install)
        {
            Update(Environment.GetFolderPath(folder), targetPathName, linkPathName, install);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The exists.
        /// </summary>
        /// <param name="directoryPath">
        /// The directory path.
        /// </param>
        /// <param name="linkPathName">
        /// The link path name.
        /// </param>
        /// <returns>
        /// True if it exists.
        /// </returns>
        private static bool Exists(string directoryPath, string linkPathName)
        {
            var specialDir = new DirectoryInfo(directoryPath);
            var originalfile = new FileInfo(linkPathName);
            string newFileName = specialDir.FullName + "\\" + originalfile.Name + ".lnk";
            var linkfile = new FileInfo(newFileName);
            return linkfile.Exists;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="directoryPath">
        /// The directory path.
        /// </param>
        /// <param name="targetPathName">
        /// The target path name.
        /// </param>
        /// <param name="linkPathName">
        /// The link path name.
        /// </param>
        /// <param name="create">
        /// The create.
        /// </param>
        private static void Update(string directoryPath, string targetPathName, string linkPathName, bool create)
        {
            var specialDir = new DirectoryInfo(directoryPath);
            var originalFile = new FileInfo(linkPathName);
            string newFileName = specialDir.FullName + "\\" + originalFile.Name + ".lnk";
            var linkFile = new FileInfo(newFileName);

            if (create)
            {
                if (!linkFile.Exists)
                {
                    var shell = new WshShell();
                    var link = (IWshShortcut)shell.CreateShortcut(linkFile.FullName);
                    link.TargetPath = targetPathName;
                    link.WorkingDirectory = Path.GetDirectoryName(targetPathName);
                    link.Save();
                }
            }
            else
            {
                if (linkFile.Exists)
                {
                    linkFile.Delete();
                }
            }
        }

        #endregion
    }
}
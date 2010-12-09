//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// Link class
//
// <copyright file="Link.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Class to add or remove a program link in the Startup folder.
//
//------------------------------------------------------------------
namespace DrunkenBakery.OWAtray
{
    using System;
    using System.IO;

    using IWshRuntimeLibrary;

    /// <summary>
    /// Summary description for Link.
    /// </summary>
    public class Link
    {
        #region Methods

        /// <summary>
        /// Check to see if a shortcut exists in a given directory with a specified file name
        /// </summary>
        /// <param name="DirectoryPath">The directory in which to look</param>
        /// <param name="LinkPathName">Name of the link path.</param>
        /// <returns>Returns true if the link exists</returns>
        public static bool Exists(string DirectoryPath, string LinkPathName)
        {
            // Get some file and directory information
            DirectoryInfo SpecialDir=new DirectoryInfo(DirectoryPath);
            // First get the filename for the original file and create a new file
            // name for a link in the Startup directory
            //
            FileInfo originalfile = new FileInfo(LinkPathName);
            string NewFileName = SpecialDir.FullName+"\\"+originalfile.Name+".lnk";
            FileInfo linkfile = new FileInfo(NewFileName);
            return linkfile.Exists;
        }

        //Check to see if a shell link exists to the given path in the specified special folder
        // return true if it exists
        /// <summary>
        /// Existses the specified folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="LinkPathName">Name of the link path.</param>
        /// <returns></returns>
        public static bool Exists(Environment.SpecialFolder folder, string LinkPathName)
        {
            return Link.Exists(Environment.GetFolderPath(folder), LinkPathName);
        }

        /// <summary>
        /// Update the specified folder by creating or deleting a Shell Link if necessary
        /// </summary>
        /// <param name="folder">A SpecialFolder in which the link will reside</param>
        /// <param name="TargetPathName">The path name of the target file for the link</param>
        /// <param name="LinkPathName">The file name for the link itself or, if a path name the directory information will be ignored.</param>
        /// <param name="install">if set to <c>true</c> [install].</param>
        public static void Update(Environment.SpecialFolder folder, string TargetPathName, string LinkPathName, bool install)
        {
            // Get some file and directory information
            Link.Update(Environment.GetFolderPath(folder), TargetPathName, LinkPathName, install);
        }

        // boolean variable "install" determines whether the link should be there or not.
        // Update the folder by creating or deleting the link as required.
        /// <summary>
        /// Update the specified folder by creating or deleting a Shell Link if necessary
        /// </summary>
        /// <param name="DirectoryPath">The full path of the directory in which the link will reside</param>
        /// <param name="TargetPathName">The path name of the target file for the link</param>
        /// <param name="LinkPathName">The file name for the link itself or, if a path name the directory information will be ignored.</param>
        /// <param name="Create">If true, create the link, otherwise delete it</param>
        public static void Update(string DirectoryPath, string TargetPathName, string LinkPathName, bool Create)
        {
            // Get some file and directory information
            DirectoryInfo SpecialDir=new DirectoryInfo(DirectoryPath);
            // First get the filename for the original file and create a new file
            // name for a link in the Startup directory
            //
            FileInfo OriginalFile = new FileInfo(LinkPathName);
            string NewFileName = SpecialDir.FullName+"\\"+OriginalFile.Name+".lnk";
            FileInfo LinkFile = new FileInfo(NewFileName);

            if(Create) // If the link doesn't exist, create it
            {
                if(LinkFile.Exists)return; // We're all done if it already exists
                //Place a shortcut to the file in the special folder
                try
                {
                    // Create a shortcut in the special folder for the file
                    // Making use of the Windows Scripting Host
                    WshShell shell = new WshShell();
                    IWshShortcut link = (IWshShortcut)shell.CreateShortcut(LinkFile.FullName);
                    link.TargetPath=TargetPathName;
                    link.Save();
                }
                catch
                {
                    throw;
                }
            }
            else // otherwise delete it from the startup directory
            {
                if(!LinkFile.Exists)return; // It doesn't exist so we are done!
                try
                {
                    LinkFile.Delete();
                }
                catch
                {
                    throw;
                }
            }
        }

        #endregion Methods
    }
}
//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// WindowsShortcut
//
// <copyright file="WindowsShortcut.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Class to supply helper methods for managing Windows shortcuts
//
//------------------------------------------------------------------

using System;
using System.IO;
using IWshRuntimeLibrary;

namespace DrunkenBakery.OWAtray
{
	public static class WindowsShortcut
	{
		private static bool Exists(string directoryPath, string linkPathName)
		{
			var specialDir = new DirectoryInfo(directoryPath);
			var originalfile = new FileInfo(linkPathName);
			var newFileName = specialDir.FullName + "\\" + originalfile.Name + ".lnk";
			var linkfile = new FileInfo(newFileName);
			return linkfile.Exists;
		}

		public static bool Exists(Environment.SpecialFolder folder, string linkPathName)
		{
			return Exists(Environment.GetFolderPath(folder), linkPathName);
		}

		private static void Update(string directoryPath, string targetPathName, string linkPathName, bool create)
		{
			var specialDir = new DirectoryInfo(directoryPath);
			var originalFile = new FileInfo(linkPathName);
			var newFileName = specialDir.FullName + "\\" + originalFile.Name + ".lnk";
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
				if (linkFile.Exists) linkFile.Delete();
			}
		}

		public static void Update(Environment.SpecialFolder folder, string targetPathName, string linkPathName, bool install)
		{
			Update(Environment.GetFolderPath(folder), targetPathName, linkPathName, install);
		}
	}
}
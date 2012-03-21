//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// ShellIntegration
//
// <copyright file="Program.cs" company="The Drunken Bakery">
//     Copyright (c) 2009-2011 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Provides Windows shell integration for OWA
//
//------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DrunkenBakery.OWAtray.ShellIntegration.Properties;
using Microsoft.Win32;

namespace DrunkenBakery.OWAtray.ShellIntegration
{
	internal static class Program
	{
		private static readonly byte[] Entropy = Encoding.Unicode.GetBytes("Salt Is Not A Password");

		private static void DoMapi(string target)
		{
			// Loop round each file and add to Clipboard
			var fileEntries = Directory.GetFiles(target, "*");
			var numFiles = fileEntries.Length;
			var files = new string[numFiles];

			var count = 0;
			foreach (var fileName in fileEntries)
			{
				files[count++] = Path.Combine(target, fileName);
			}

			var d = new DataObject();
			d.SetData(DataFormats.FileDrop, files);
			Clipboard.SetDataObject(d, true);

			// Spawn IE
			var myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount + @"/?ae=Item&a=New&t=IPM.Note" +
						   Settings.Default.MimeURL;
			try
			{
				Console.WriteLine("Browsing to " + myUrl);
				Process.Start("IEXPLORE.EXE", myUrl);

				// Wait for it to pop
				Thread.Sleep(Convert.ToInt32(Settings.Default.PopupDelay));

				// Find IE window and send keys to it
				var iHandle = NativeWin32.FindWindow(null, Settings.Default.IETitle);
				Console.WriteLine("Handle1 = " + iHandle);
				if (iHandle == 0)
				{
					iHandle = NativeWin32.FindWindow(null, Settings.Default.IETitle2);
					Console.WriteLine("Handle2 = " + iHandle);
				}
				NativeWin32.SetForegroundWindow(iHandle);

				// Tab stops
				for (var f = 0; f < Convert.ToInt32(Settings.Default.TabStops); ++f)
				{
					SendKeys.SendWait("{TAB}");
					Thread.Sleep(100);
				}

				// Then the paste
				SendKeys.SendWait("^v");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static void InitRegistry()
		{
			var bridge = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
										 Settings.Default.MAPIBridge);
			var shell = Assembly.GetExecutingAssembly().Location;

			try
			{
				// Define a class root for us
				Registry.SetValue(@"HKEY_CLASSES_ROOT\OWA.Url.Mailto", "", "URL:MailTo Protocol");
				Registry.SetValue(@"HKEY_CLASSES_ROOT\OWA.Url.Mailto", "URL Protocol", "");
				Registry.SetValue(@"HKEY_CLASSES_ROOT\OWA.Url.Mailto", "EditFlags", new byte[] { 0x2, 0x0, 0x0, 0x0 });
				Registry.SetValue(@"HKEY_CLASSES_ROOT\OWA.Url.Mailto\DefaultIcon", "", "\"" + shell + "\",0");
				Registry.SetValue(@"HKEY_CLASSES_ROOT\OWA.Url.Mailto\shell\open\command", "", "\"" + shell + "\" mailto %1");

				// Tell windows to use us for mailto links
				Registry.SetValue(@"HKEY_CLASSES_ROOT\mailto\DefaultIcon", "", "\"" + shell + "\",0");
				Registry.SetValue(@"HKEY_CLASSES_ROOT\mailto\shell\open\command", "", "\"" + shell + "\" mailto %1");
				Registry.SetValue(
					@"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice", "Progid",
					Settings.Default.MailtoClass);

				// Set up a mail handler
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi", "", "Outlook Web Access");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi", "DLLPath", bridge);
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi", "EXE", "\"" + shell + "\"");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi", "Parameters", "mapi %1");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Capabilities", "ApplicationDescription",
								  "Integrate Outlook Web Access into the desktop.");
				Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Clients\Mail\OWAMapi\Capabilities\FileAssociations");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Capabilities\Start Menu", "Mail", "OWA");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Capabilities\URLAssociations", "mailto",
								  Settings.Default.MailtoClass);
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto", "", "URL:MailTo Protocol");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto", "EditFlags",
								  new byte[] { 0x2, 0x0, 0x0, 0x0 });
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto", "URL Protocol", "");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto\DefaultIcon", "",
								  "\"" + shell + "\",0");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto\shell\open\command", "",
								  "\"" + shell + "\" mailto %1");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\shell\open\command", "",
								  "\"" + shell + "\" owa");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\DefaultIcon", "", "\"" + shell + "\",0");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", "HideIconsCommand",
								  "\"" + shell + "\" restore");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", "ReinstallCommand",
								  "\"" + shell + "\" registry");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", "ShowIconsCommand",
								  "\"" + shell + "\" registry");
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", "IconsVisible", 1,
								  RegistryValueKind.DWord);

				// Register the application
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\RegisteredApplications", "OWA",
								  @"Software\Clients\Mail\OWAMapi\Capabilities");

				// Set default mail handler
				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", "OWAMapi");
				Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", "OWAMapi");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		[STAThread]
		private static void Main(string[] args)
		{
			// Check for arguments
			if (args.Length < 1)
			{
				Console.WriteLine("Error - minimum of 1 argument required.");
				return;
			}

			// Which command?
			Console.WriteLine("Received command: " + args[0]);
			switch (args[0].ToUpper())
			{
				case "OWA":
					if (args.Length > 1)
					{
						StartOwAinIe(args[1]);
					}
					else
					{
						StartOwAinIe();
					}
					break;

				case "SHELL":
					if (args.Length > 1)
					{
						ShellOwa(args[1]);
					}
					else
					{
						ShellOwa();
					}
					break;

				case "AUTOLOGIN":
					if (args.Length > 1)
					{
						Settings.Default.AutoLogin = args[1];
						Settings.Default.Save();
					}
					break;

				case "BROWSER":
					if (args.Length > 1)
					{
						Settings.Default.Browser = args[1];
						Settings.Default.Save();
					}
					break;

				case "REGISTRY":
					SaveCurrentKey();
					InitRegistry();
					break;

				case "MAILTO":
					if (args.Length > 1)
					{
						SpawnUrl(args[1]);
					}
					break;

				case "MAPI":
					if (args.Length > 1)
					{
						DoMapi(args[1]);
					}
					break;

				case "SAVE":
					SaveCurrentKey();
					break;

				case "RESTORE":
					RestoreKey();
					break;

				case "URL":
					if (args.Length > 1)
					{
						var myPath = args[1];
						myPath = myPath.TrimEnd(new char[] { '\\', '/' });
						Settings.Default.OwaUrl = myPath;
						Settings.Default.Save();
					}
					break;

				case "ACCOUNT":
					Settings.Default.UserAccount = args.Length > 1 ? args[1] : "";
					Settings.Default.Save();
					break;

				case "PASSWORD":
					if (args.Length > 1)
					{
						Settings.Default.Password = (args[1].Length > 0 ? args[1].Encrypt() : "");
						Settings.Default.Save();
					}
					break;

				case "EXCHANGE":
					if (args.Length > 1)
					{
						switch (args[1])
						{
							case "Exchange2010":
								Settings.Default.MimeURL = Settings.Default.URL2010;
								break;
							case "Exchange2010_SP1":
								Settings.Default.MimeURL = Settings.Default.URL2010SP1;
								break;
							case "Exchange2010_SP2":
								Settings.Default.MimeURL = Settings.Default.URL2010SP2;
								break;
							default:
								Settings.Default.MimeURL = "";
								break;
						}
						Settings.Default.Save();
					}
					break;

				default:
					Console.WriteLine("Unknown command");
					break;
			}

			Console.WriteLine("Completed.");
		}

		private static void RestoreKey()
		{
			try
			{
				if (Settings.Default.CurrentKey.Length > 0)
					Registry.SetValue(
						@"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice", "Progid",
						Settings.Default.CurrentKey);
				if (Settings.Default.DefaultMail.Length > 0)
					Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", Settings.Default.DefaultMail);
				if (Settings.Default.DefaultMailUser.Length > 0)
					Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", Settings.Default.DefaultMailUser);
				if (Settings.Default.DefaultIcon.Length > 0)
					Registry.SetValue(@"HKEY_CLASSES_ROOT\mailto\DefaultIcon", "", Settings.Default.DefaultIcon);
				if (Settings.Default.DefaultOpen.Length > 0)
					Registry.SetValue(@"HKEY_CLASSES_ROOT\mailto\shell\open\command", "", Settings.Default.DefaultOpen);

				Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", "IconsVisible", 0,
								  RegistryValueKind.DWord);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static void SaveCurrentKey()
		{
			var bridge = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
										 Settings.Default.MAPIBridge);
			var shell = Assembly.GetExecutingAssembly().Location;

			try
			{
				// Get current mailto and store for use later
				var currentKey =
					Registry.GetValue(
						@"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice", "Progid",
						Settings.Default.MailtoClass).ToString();
				if (currentKey != Settings.Default.MailtoClass)
				{
					Settings.Default.CurrentKey = currentKey;
					Settings.Default.Save();
				}

				// Get current default mail and store for use later
				var mailKey = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", "OWAMapi").ToString();
				if (mailKey != "OWAMapi")
				{
					Settings.Default.DefaultMail = mailKey;
					Settings.Default.Save();
				}

				// Get current default user mail and store for use later
				var userMailKey = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", "OWAMapi").ToString();
				if (userMailKey != "OWAMapi")
				{
					Settings.Default.DefaultMailUser = userMailKey;
					Settings.Default.Save();
				}

				// Get current default icon and store for use later
				var defIconKey = "\"" + shell + "\",0";
				var iconKey = Registry.GetValue(@"HKEY_CLASSES_ROOT\mailto\DefaultIcon", "", defIconKey).ToString();
				if (userMailKey != defIconKey)
				{
					Settings.Default.DefaultIcon = iconKey;
					Settings.Default.Save();
				}

				// Get current default cmd path and store for use later
				var defPathKey = "\"" + shell + "\" mailto %1";
				var pathKey = Registry.GetValue(@"HKEY_CLASSES_ROOT\mailto\shell\open\command", "", defPathKey).ToString();
				if (pathKey != defPathKey)
				{
					Settings.Default.DefaultOpen = pathKey;
					Settings.Default.Save();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static void SpawnUrl(string target)
		{
			if (target.Substring(0, 7) == @"mailto:")
			{
				target = target.Substring(7, target.Length - 7);
			}
			var myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount + "/?ae=Item&a=New&t=IPM.Note" +
						   Settings.Default.MimeURL + "&to=" + target;
			if (Settings.Default.Browser == "Yes")
			{
				Process.Start("IEXPLORE.EXE", myUrl);
			}
			else
			{
				Process.Start(myUrl);
			}

			Console.WriteLine("Browsing to " + myUrl);
		}

		private static void StartOwAinIe(string url = "")
		{
			var myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount + (url.Length > 0 ? "/" + url : "");
			Process.Start("IEXPLORE.EXE", myUrl);
			Console.WriteLine("Browsing to " + myUrl);
			AutoLogin();
		}

		private static void ShellOwa(string url = "")
		{
			var myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount + (url.Length > 0 ? "/" + url : "");
			Process.Start(myUrl);
			Console.WriteLine("Browsing to " + myUrl);
			AutoLogin();
		}

		private static void AutoLogin()
		{
			if (Settings.Default.AutoLogin == "Yes")
			{
				// Wait for it to load the page
				Thread.Sleep(Convert.ToInt32(Settings.Default.PopupDelay));

				// Find IE window and send keys to it
				var iHandle = NativeWin32.FindWindow(null, Settings.Default.LoginTitle);
				NativeWin32.SetForegroundWindow(iHandle);

				// Tab stops
				SendKeys.SendWait(Settings.Default.Password.Decrypt());
				Thread.Sleep(Convert.ToInt32(Settings.Default.SmallWait));

				// Then the paste
				SendKeys.SendWait("{ENTER}");
			}
		}
	}
}
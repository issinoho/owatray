// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.ShellIntegration
// 
//  <copyright file="Program.cs" company="The Drunken Bakery”>
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
// 
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.ShellIntegration
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    using DrunkenBakery.OWAtray.ShellIntegration.Properties;

    using Microsoft.Win32;

    /// <summary>
    /// The program.
    /// </summary>
    internal static class Program
    {
        #region Static Fields

        /// <summary>
        /// The entropy.
        /// </summary>
        private static readonly byte[] entropy = Encoding.Unicode.GetBytes("Salt Is Not A Password");

        #endregion

        #region Methods

        /// <summary>
        /// The auto login.
        /// </summary>
        private static void AutoLogin()
        {
            if (Settings.Default.AutoLogin == "Yes")
            {
                // Wait for it to load the page
                Thread.Sleep(Convert.ToInt32(Settings.Default.PopupDelay));

                // Find IE window and send keys to it
                int handle = NativeWin32.FindWindow(null, Settings.Default.LoginTitle);
                NativeWin32.SetForegroundWindow(handle);

                // Tab stops
                SendKeys.SendWait(Settings.Default.Password.Decrypt());
                Thread.Sleep(Convert.ToInt32(Settings.Default.SmallWait));

                // Then the paste
                SendKeys.SendWait("{ENTER}");
            }
        }

        /// <summary>
        /// The do mapi.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        private static void DoMapi(string target)
        {
            // Loop round each file and add to Clipboard
            string[] fileEntries = Directory.GetFiles(target, "*");
            int numFiles = fileEntries.Length;
            var files = new string[numFiles];

            int count = 0;
            foreach (string fileName in fileEntries)
            {
                files[count++] = Path.Combine(target, fileName);
            }

            var d = new DataObject();
            d.SetData(DataFormats.FileDrop, files);
            Clipboard.SetDataObject(d, true);

            // Spawn IE
            string myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount + @"/?ae=Item&a=New&t=IPM.Note"
                           + Settings.Default.MimeURL;
            try
            {
                Console.WriteLine("Browsing to " + myUrl);
                Process.Start("IEXPLORE.EXE", myUrl);

                // Wait for it to pop
                Thread.Sleep(Convert.ToInt32(Settings.Default.PopupDelay));

                // Find IE window and send keys to it
                int handle = NativeWin32.FindWindow(null, Settings.Default.IETitle);
                Console.WriteLine("Handle1 = " + handle);
                if (handle == 0)
                {
                    handle = NativeWin32.FindWindow(null, Settings.Default.IETitle2);
                    Console.WriteLine("Handle2 = " + handle);
                }

                NativeWin32.SetForegroundWindow(handle);

                // Tab stops
                for (int f = 0; f < Convert.ToInt32(Settings.Default.TabStops); ++f)
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

        /// <summary>
        /// The init registry.
        /// </summary>
        private static void InitRegistry()
        {
            string bridge = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Settings.Default.MAPIBridge);
            string shell = Assembly.GetExecutingAssembly().Location;

            try
            {
                // Define a class root for us
                Registry.SetValue(@"HKEY_CLASSES_ROOT\OWA.Url.Mailto", string.Empty, "URL:MailTo Protocol");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\OWA.Url.Mailto", "URL Protocol", string.Empty);
                Registry.SetValue(@"HKEY_CLASSES_ROOT\OWA.Url.Mailto", "EditFlags", new byte[] { 0x2, 0x0, 0x0, 0x0 });
                Registry.SetValue(@"HKEY_CLASSES_ROOT\OWA.Url.Mailto\DefaultIcon", string.Empty, "\"" + shell + "\",0");
                Registry.SetValue(
                    @"HKEY_CLASSES_ROOT\OWA.Url.Mailto\shell\open\command", string.Empty, "\"" + shell + "\" mailto %1");

                // Tell windows to use us for mailto links
                Registry.SetValue(@"HKEY_CLASSES_ROOT\mailto\DefaultIcon", string.Empty, "\"" + shell + "\",0");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\mailto\shell\open\command", string.Empty, "\"" + shell + "\" mailto %1");
                Registry.SetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice", 
                    "Progid", 
                    Settings.Default.MailtoClass);

                // Set up a mail handler
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi", string.Empty, "Outlook Web Access");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi", "DLLPath", bridge);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi", "EXE", "\"" + shell + "\"");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi", "Parameters", "mapi %1");
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Capabilities", 
                    "ApplicationDescription", 
                    "Integrate Outlook Web Access into the desktop.");
                Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Clients\Mail\OWAMapi\Capabilities\FileAssociations");
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Capabilities\Start Menu", "Mail", "OWA");
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Capabilities\URLAssociations", 
                    "mailto", 
                    Settings.Default.MailtoClass);
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto", string.Empty, "URL:MailTo Protocol");
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto", 
                    "EditFlags", 
                    new byte[] { 0x2, 0x0, 0x0, 0x0 });
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto", "URL Protocol", string.Empty);
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto\DefaultIcon", 
                    string.Empty, 
                    "\"" + shell + "\",0");
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto\shell\open\command", 
                    string.Empty, 
                    "\"" + shell + "\" mailto %1");
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\shell\open\command", string.Empty, "\"" + shell + "\" owa");
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\DefaultIcon", string.Empty, "\"" + shell + "\",0");
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", 
                    "HideIconsCommand", 
                    "\"" + shell + "\" restore");
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", 
                    "ReinstallCommand", 
                    "\"" + shell + "\" registry");
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", 
                    "ShowIconsCommand", 
                    "\"" + shell + "\" registry");
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", 
                    "IconsVisible", 
                    1, 
                    RegistryValueKind.DWord);

                // Register the application
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\RegisteredApplications", 
                    "OWA", 
                    @"Software\Clients\Mail\OWAMapi\Capabilities");

                // Set default mail handler
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", string.Empty, "OWAMapi");
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", string.Empty, "OWAMapi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
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
                        string myPath = args[1];
                        myPath = myPath.TrimEnd(new[] { '\\', '/' });
                        Settings.Default.OwaUrl = myPath;
                        Settings.Default.Save();
                    }

                    break;

                case "ACCOUNT":
                    Settings.Default.UserAccount = args.Length > 1 ? args[1] : string.Empty;
                    Settings.Default.Save();
                    break;

                case "PASSWORD":
                    if (args.Length > 1)
                    {
                        Settings.Default.Password = args[1].Length > 0 ? args[1].Encrypt() : string.Empty;
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
                                Settings.Default.MimeURL = string.Empty;
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

        /// <summary>
        /// The restore key.
        /// </summary>
        private static void RestoreKey()
        {
            try
            {
                if (Settings.Default.CurrentKey.Length > 0)
                {
                    Registry.SetValue(
                        @"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice", 
                        "Progid", 
                        Settings.Default.CurrentKey);
                }

                if (Settings.Default.DefaultMail.Length > 0)
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", string.Empty, Settings.Default.DefaultMail);
                }

                if (Settings.Default.DefaultMailUser.Length > 0)
                {
                    Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", string.Empty, Settings.Default.DefaultMailUser);
                }

                if (Settings.Default.DefaultIcon.Length > 0)
                {
                    Registry.SetValue(@"HKEY_CLASSES_ROOT\mailto\DefaultIcon", string.Empty, Settings.Default.DefaultIcon);
                }

                if (Settings.Default.DefaultOpen.Length > 0)
                {
                    Registry.SetValue(@"HKEY_CLASSES_ROOT\mailto\shell\open\command", string.Empty, Settings.Default.DefaultOpen);
                }

                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", 
                    "IconsVisible", 
                    0, 
                    RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// The save current key.
        /// </summary>
        private static void SaveCurrentKey()
        {
            string bridge = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Settings.Default.MAPIBridge);
            string shell = Assembly.GetExecutingAssembly().Location;

            try
            {
                // Get current mailto and store for use later
                string currentKey =
                    Registry.GetValue(
                        @"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice", 
                        "Progid", 
                        Settings.Default.MailtoClass).ToString();
                if (currentKey != Settings.Default.MailtoClass)
                {
                    Settings.Default.CurrentKey = currentKey;
                    Settings.Default.Save();
                }

                // Get current default mail and store for use later
                string mailKey =
                    Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", string.Empty, "OWAMapi").ToString();
                if (mailKey != "OWAMapi")
                {
                    Settings.Default.DefaultMail = mailKey;
                    Settings.Default.Save();
                }

                // Get current default user mail and store for use later
                string userMailKey =
                    Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", string.Empty, "OWAMapi").ToString();
                if (userMailKey != "OWAMapi")
                {
                    Settings.Default.DefaultMailUser = userMailKey;
                    Settings.Default.Save();
                }

                // Get current default icon and store for use later
                string defIconKey = "\"" + shell + "\",0";
                string iconKey = Registry.GetValue(@"HKEY_CLASSES_ROOT\mailto\DefaultIcon", string.Empty, defIconKey).ToString();
                if (userMailKey != defIconKey)
                {
                    Settings.Default.DefaultIcon = iconKey;
                    Settings.Default.Save();
                }

                // Get current default cmd path and store for use later
                string defPathKey = "\"" + shell + "\" mailto %1";
                string pathKey =
                    Registry.GetValue(@"HKEY_CLASSES_ROOT\mailto\shell\open\command", string.Empty, defPathKey).ToString();
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

        /// <summary>
        /// The shell owa.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        private static void ShellOwa(string url = "")
        {
            string myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount + (url.Length > 0 ? "/" + url : string.Empty);
            Process.Start(myUrl);
            Console.WriteLine("Browsing to " + myUrl);
            AutoLogin();
        }

        /// <summary>
        /// The spawn url.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        private static void SpawnUrl(string target)
        {
            if (target.Substring(0, 7) == @"mailto:")
            {
                target = target.Substring(7, target.Length - 7);
            }

            string myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount + "/?ae=Item&a=New&t=IPM.Note"
                           + Settings.Default.MimeURL + "&to=" + target;
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

        /// <summary>
        /// The start ow ain ie.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        private static void StartOwAinIe(string url = "")
        {
            string myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount + (url.Length > 0 ? "/" + url : string.Empty);
            Process.Start("IEXPLORE.EXE", myUrl);
            Console.WriteLine("Browsing to " + myUrl);
            AutoLogin();
        }

        #endregion
    }
}
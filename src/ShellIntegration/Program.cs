// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.ShellIntegration
// 
//  <copyright file="Program.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
// 
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.ShellIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    using DrunkenBakery.OWAtray.ShellIntegration.Properties;

    using Microsoft.Win32;

    /// <summary>
    /// The program.
    /// </summary>
    internal static class Program
    {
        #region Methods

        /// <summary>
        /// The auto login.
        /// </summary>
        private static void AutoLogin()
        {
            if (Settings.Default.AutoLogin != "Yes")
            {
                return;
            }

            // Wait for it to load the page
            Thread.Sleep(Settings.Default.PopupDelay);

            // Find IE window and send keys to it
            var windowTitle = Settings.Default.Office365 == "Yes"
                                  ? Settings.Default.Office365Title
                                  : Settings.Default.LoginTitle;
            var handle = NativeWin32.FindWindow(null, windowTitle);
            NativeWin32.SetForegroundWindow(handle);

            // If this is Office365 then send extra keys
            if (Settings.Default.Office365 == "Yes")
            {
                SendKeys.SendWait("{ENTER}");
            }

            // Tab stops
            SendKeys.SendWait(Settings.Default.Password.Decrypt());
            Thread.Sleep(Settings.Default.SmallWait);

            // Then the paste
            SendKeys.SendWait("{ENTER}");
        }

        /// <summary>
        /// The do mapi.
        /// </summary>
        /// <param name="target">
        /// The target. 
        /// </param>
        private static void DoMapi(string target)
        {
            // Set up our set of special-case characters
            var specialCharacters = new HashSet<char> {'+', '^', '%', '~', '(', ')'};

            // Which version of Exchange?
            var myUrl = Settings.Default.Version.Contains("Exchange2013") ? Settings.Default.OwaUrl + Settings.Default.UserAccount + Settings.Default.NewMail2013 : Settings.Default.OwaUrl + Settings.Default.UserAccount + Settings.Default.NewMail + Settings.Default.MimeURL;

            try
            {
                // Fire up the browser
                Console.WriteLine("Browsing to " + myUrl);
                if (Settings.Default.Browser == "Yes")
                {
                    Process.Start("IEXPLORE.EXE", myUrl);
                }
                else
                {
                    Process.Start(myUrl);
                }

                // Wait for it to pop
                Thread.Sleep(Settings.Default.Office365 == "Yes" ? Settings.Default.O365PopupDelay : Settings.Default.PopupDelay);

                // Find IE window and send keys to it
                var handle = NativeWin32.FindWindow(null, Settings.Default.IETitle);
                Console.WriteLine("Handle1 = " + handle);
                if (handle == 0)
                {
                    handle = NativeWin32.FindWindow(null, Settings.Default.IETitle2);
                    Console.WriteLine("Handle2 = " + handle);
                }

                // Get focus
                NativeWin32.SetForegroundWindow(handle);

                // Build attachments list
                var files = Directory.GetFiles(target);

                // What we do next depends on the version of Exchange
                if (Settings.Default.Version.Contains("Exchange2013"))
                {
                    // Exchange 2013
                    for (int f = 0; f < (Settings.Default.Office365 == "Yes" ? Settings.Default.O365TabCount : Settings.Default.TabCount); ++f)
                    {
                        SendKeys.SendWait("+{TAB}");
                        Thread.Sleep(100);
                    }

                    SendKeys.SendWait(" ");
                    Thread.Sleep(100);
                    SendKeys.SendWait("{DOWN}");
                    Thread.Sleep(100);
                    SendKeys.SendWait(" ");
                    Thread.Sleep(250);
                    foreach (var c in target)
                    {
                        SendKeys.SendWait(c.ToString(CultureInfo.InvariantCulture));
                    }
                    Thread.Sleep(100);
                    SendKeys.SendWait("{ENTER}");
                    foreach (string file in files)
                    {
                        Thread.Sleep(100);
                        SendKeys.SendWait("\"");
                        Thread.Sleep(100);
                        foreach (var c in Path.GetFileName(file))
                        {
                            if (specialCharacters.Contains(c))
                            {
                                SendKeys.SendWait("{" + c.ToString(CultureInfo.InvariantCulture) + "}");
                            }
                            else
                            {
                                SendKeys.SendWait(c.ToString(CultureInfo.InvariantCulture));
                            }                            
                        }
                        Thread.Sleep(100);
                        SendKeys.SendWait("\"");
                        Thread.Sleep(100);
                        SendKeys.SendWait(" ");
                    }
                    Thread.Sleep(100);
                    SendKeys.SendWait("{ENTER}");
                }
                else
                {
                    // Pre-Exchange 2013
                    var d = new DataObject();
                    d.SetData(DataFormats.FileDrop, files);
                    Clipboard.SetDataObject(d, true);

                    for (int f = 0; f < Settings.Default.TabStops; ++f)
                    {
                        SendKeys.SendWait("{TAB}");
                        Thread.Sleep(100);
                    }

                    // Then the paste
                    SendKeys.SendWait("^v");
                }
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
            var bridge = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Settings.Default.MAPIBridge);
            var shell = Assembly.GetExecutingAssembly().Location;

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
                Registry.SetValue(
                    @"HKEY_CLASSES_ROOT\mailto\shell\open\command", string.Empty, "\"" + shell + "\" mailto %1");
                Registry.SetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice", 
                    "Progid", 
                    Settings.Default.MailtoClass);

                // Set up a mail handler
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi", string.Empty, "Outlook Web App");
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
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto", 
                    string.Empty, 
                    "URL:MailTo Protocol");
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
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\shell\open\command", 
                    string.Empty, 
                    "\"" + shell + "\" owa");
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
                        Settings.Default.Office365 = args[2];
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
                        HandleMailto(args[1]);
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

                        Settings.Default.Version = args[1];
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
                    Registry.SetValue(
                        @"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", string.Empty, Settings.Default.DefaultMail);
                }

                if (Settings.Default.DefaultMailUser.Length > 0)
                {
                    Registry.SetValue(
                        @"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", string.Empty, Settings.Default.DefaultMailUser);
                }

                if (Settings.Default.DefaultIcon.Length > 0)
                {
                    Registry.SetValue(
                        @"HKEY_CLASSES_ROOT\mailto\DefaultIcon", string.Empty, Settings.Default.DefaultIcon);
                }

                if (Settings.Default.DefaultOpen.Length > 0)
                {
                    Registry.SetValue(
                        @"HKEY_CLASSES_ROOT\mailto\shell\open\command", string.Empty, Settings.Default.DefaultOpen);
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
            var bridge = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Settings.Default.MAPIBridge);
            var shell = Assembly.GetExecutingAssembly().Location;

            try
            {
                // Get current mailto and store for use later
                var currentKey =
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
                var mailKey =
                    Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", string.Empty, "OWAMapi").ToString();
                if (mailKey != "OWAMapi")
                {
                    Settings.Default.DefaultMail = mailKey;
                    Settings.Default.Save();
                }

                // Get current default user mail and store for use later
                var userMailKey =
                    Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", string.Empty, "OWAMapi").ToString();
                if (userMailKey != "OWAMapi")
                {
                    Settings.Default.DefaultMailUser = userMailKey;
                    Settings.Default.Save();
                }

                // Get current default icon and store for use later
                var defIconKey = "\"" + shell + "\",0";
                var iconKey =
                    Registry.GetValue(@"HKEY_CLASSES_ROOT\mailto\DefaultIcon", string.Empty, defIconKey).ToString();
                if (userMailKey != defIconKey)
                {
                    Settings.Default.DefaultIcon = iconKey;
                    Settings.Default.Save();
                }

                // Get current default cmd path and store for use later
                var defPathKey = "\"" + shell + "\" mailto %1";
                var pathKey =
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
            string myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount
                           + (url.Length > 0 ? "/" + url : string.Empty);
            Process.Start(myUrl);
            Console.WriteLine("Browsing to " + myUrl);
            AutoLogin();
        }

        /// <summary>
        /// The handle mailto.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        private static void HandleMailto(string target)
        {
            string myUrl;

            // Strip off protocol header
            if (target.Substring(0, 7) == @"mailto:")
            {
                target = target.Substring(7, target.Length - 7);
            }

            // Which version of Exchange?
            if (Settings.Default.Version.Contains("Exchange2013"))
            {
                myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount + Settings.Default.NewMail2013;
            }
            else
            {
                myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount + Settings.Default.NewMail
                        + Settings.Default.MimeURL + "&to=" + target;
            }

            // Fire up the browser
            Console.WriteLine("Browsing to " + myUrl);
            if (Settings.Default.Browser == "Yes")
            {
                Process.Start("IEXPLORE.EXE", myUrl);
            }
            else
            {
                Process.Start(myUrl);
            }

            // If Exchange2013 then paste in address
            if (Settings.Default.Version.Contains("Exchange2013"))
            {
                // Wait for it to pop
                Thread.Sleep(Settings.Default.PopupDelay);

                // Find IE window and send keys to it
                var handle = NativeWin32.FindWindow(null, Settings.Default.IETitle);
                Console.WriteLine("Handle1 = " + handle);
                if (handle == 0)
                {
                    handle = NativeWin32.FindWindow(null, Settings.Default.IETitle2);
                    Console.WriteLine("Handle2 = " + handle);
                }

                // Focus window
                NativeWin32.SetForegroundWindow(handle);

                // Send email address character by character to window
                foreach (var c in target)
                {
                    SendKeys.SendWait(c.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        /// <summary>
        /// The start ow ain ie.
        /// </summary>
        /// <param name="url">
        /// The url. 
        /// </param>
        private static void StartOwAinIe(string url = "")
        {
            string myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount
                           + (url.Length > 0 ? "/" + url : string.Empty);
            Process.Start("IEXPLORE.EXE", myUrl);
            Console.WriteLine("Browsing to " + myUrl);
            AutoLogin();
        }

        #endregion
    }
}
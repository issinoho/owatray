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
    using DrunkenBakery.OWAtray.Logging;
    using DrunkenBakery.OWAtray.ShellIntegration.Properties;
    using Microsoft.Win32;

    /// <summary>
    /// The program.
    /// </summary>
    internal static class Program
    {
        #region Constants and Fields

        /// <summary>
        /// Server versions that use the post-2013 OWA compose UI (and therefore the "NewMail2013"
        /// compose-URL format instead of the legacy MIME-URL one).
        /// </summary>
        private static readonly HashSet<string> ModernComposeUrlVersions = new HashSet<string>
        {
            "Exchange2013", "Exchange2013_SP1", "Exchange2016", "Exchange2019", "ExchangeServerSE",
        };

        #endregion

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

            LoggerProxy.Debug("Starting auto-login (Office365=" + Settings.Default.Office365 + ")");

            // Wait for it to load the page
            Thread.Sleep(Settings.Default.PopupDelay);

            // Find IE window and send keys to it
            var windowTitle = Settings.Default.Office365 == "Yes"
                ? Settings.Default.Office365Title
                : Settings.Default.LoginTitle;
            var handle = NativeWin32.FindWindow(null, windowTitle);
            LoggerProxy.Debug("Auto-login found window \"" + windowTitle + "\", handle=" + handle);
            NativeWin32.SetForegroundWindow(handle);

            // If this is Office365 then send extra keys
            if (Settings.Default.Office365 == "Yes")
            {
                SendKeys.SendWait("{ENTER}");
            }

            // Tab stops - deliberately never logging the decrypted password itself
            SendKeys.SendWait(Settings.Default.Password.Decrypt());
            Thread.Sleep(Settings.Default.SmallWait);

            // Then the paste
            SendKeys.SendWait("{ENTER}");
            LoggerProxy.Debug("Auto-login SendKeys sequence complete");
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
            var specialCharacters = new HashSet<char> { '+', '^', '%', '~', '(', ')' };

            // Which version of Exchange?
            var myUrl = ModernComposeUrlVersions.Contains(Settings.Default.Version)
                ? Settings.Default.OwaUrl + Settings.Default.UserAccount + Settings.Default.NewMail2013
                : Settings.Default.OwaUrl + Settings.Default.UserAccount + Settings.Default.NewMail +
                  Settings.Default.MimeURL;

            try
            {
                // Fire up the browser
                LoggerProxy.Debug("MAPI: browsing to " + myUrl);
                if (Settings.Default.Browser == "Yes")
                {
                    Process.Start("IEXPLORE.EXE", myUrl);
                }
                else
                {
                    Process.Start(myUrl);
                }

                // Wait for it to pop
                Thread.Sleep(Settings.Default.Office365 == "Yes"
                    ? Settings.Default.O365PopupDelay
                    : Settings.Default.PopupDelay);

                // Find IE window and send keys to it
                var handle = NativeWin32.FindWindow(null, Settings.Default.IETitle);
                LoggerProxy.Debug("MAPI: window handle (title 1) = " + handle);
                if (handle == 0)
                {
                    handle = NativeWin32.FindWindow(null, Settings.Default.IETitle2);
                    LoggerProxy.Debug("MAPI: window handle (title 2) = " + handle);
                }

                // Get focus
                NativeWin32.SetForegroundWindow(handle);

                // Build attachments list
                var files = Directory.GetFiles(target);
                LoggerProxy.Debug("MAPI: found " + files.Length + " attachment(s) in " + target);

                // What we do next depends on the version of Exchange
                if (ModernComposeUrlVersions.Contains(Settings.Default.Version))
                {
                    // Exchange 2013
                    for (int f = 0;
                        f <
                        (Settings.Default.Office365 == "Yes" ? Settings.Default.O365TabCount : Settings.Default.TabCount);
                        ++f)
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
                LoggerProxy.Log("MAPI: exception while composing message", ex);
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

            LoggerProxy.Debug("Registering default mail handler: bridge=" + bridge + ", shell=" + shell);

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
                LoggerProxy.Log("Registered OWAtray as the default mail handler", true);
            }
            catch (Exception ex)
            {
                LoggerProxy.Log("Exception while registering as the default mail handler", ex);
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
                LoggerProxy.Log("Error - minimum of 1 argument required.", false);
                return;
            }

            // Which command?
            LoggerProxy.Log("Received command: " + args[0], true);
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
                        LoggerProxy.Debug("AutoLogin=" + args[1] + ", Office365=" + args[2]);
                    }

                    break;

                case "BROWSER":
                    if (args.Length > 1)
                    {
                        Settings.Default.Browser = args[1];
                        Settings.Default.Save();
                        LoggerProxy.Debug("Browser=" + args[1]);
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
                        LoggerProxy.Debug("OwaUrl=" + myPath);
                    }

                    break;

                case "ACCOUNT":
                    Settings.Default.UserAccount = args.Length > 1 ? args[1] : string.Empty;
                    Settings.Default.Save();
                    LoggerProxy.Debug("UserAccount=" + Settings.Default.UserAccount);
                    break;

                case "PASSWORD":
                    if (args.Length > 1)
                    {
                        Settings.Default.Password = args[1].Length > 0 ? args[1].Encrypt() : string.Empty;
                        Settings.Default.Save();

                        // Deliberately never logging the password itself, even encrypted.
                        LoggerProxy.Debug("Password updated");
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
                            case "Exchange2010_SP3":
                                Settings.Default.MimeURL = Settings.Default.URL2010SP3;
                                break;
                            default:
                                Settings.Default.MimeURL = string.Empty;
                                break;
                        }

                        Settings.Default.Version = args[1];
                        Settings.Default.Save();
                        LoggerProxy.Debug("Version=" + args[1]);
                    }

                    break;

                default:
                    LoggerProxy.Log("Unknown command: " + args[0], false);
                    break;
            }

            LoggerProxy.Debug("Completed.");
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
                LoggerProxy.Log("Restored the previous default mail handler", true);
            }
            catch (Exception ex)
            {
                LoggerProxy.Log("Exception while restoring the previous default mail handler", ex);
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
                    Registry.GetValue(@"HKEY_CLASSES_ROOT\mailto\shell\open\command", string.Empty, defPathKey)
                        .ToString();
                if (pathKey != defPathKey)
                {
                    Settings.Default.DefaultOpen = pathKey;
                    Settings.Default.Save();
                }

                LoggerProxy.Debug("Saved the previous default mail handler for later restore");
            }
            catch (Exception ex)
            {
                LoggerProxy.Log("Exception while saving the previous default mail handler", ex);
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
            LoggerProxy.Debug("SHELL: browsing to " + myUrl);
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
            if (ModernComposeUrlVersions.Contains(Settings.Default.Version))
            {
                myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount + Settings.Default.NewMail2013;
            }
            else
            {
                myUrl = Settings.Default.OwaUrl + Settings.Default.UserAccount + Settings.Default.NewMail
                        + Settings.Default.MimeURL + "&to=" + target;
            }

            // Fire up the browser
            LoggerProxy.Debug("MAILTO: browsing to " + myUrl);
            if (Settings.Default.Browser == "Yes")
            {
                Process.Start("IEXPLORE.EXE", myUrl);
            }
            else
            {
                Process.Start(myUrl);
            }

            // If Exchange2013 then paste in address
            if (ModernComposeUrlVersions.Contains(Settings.Default.Version))
            {
                // Wait for it to pop
                Thread.Sleep(Settings.Default.PopupDelay);

                // Find IE window and send keys to it
                var handle = NativeWin32.FindWindow(null, Settings.Default.IETitle);
                LoggerProxy.Debug("MAILTO: window handle (title 1) = " + handle);
                if (handle == 0)
                {
                    handle = NativeWin32.FindWindow(null, Settings.Default.IETitle2);
                    LoggerProxy.Debug("MAILTO: window handle (title 2) = " + handle);
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
            LoggerProxy.Debug("OWA: browsing to " + myUrl);
            AutoLogin();
        }

        #endregion
    }
}
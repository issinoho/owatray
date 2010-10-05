//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// ShellIntegration
//
// <copyright file="Program.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Provides Windows shell integration for OWA
//
//------------------------------------------------------------------
namespace DrunkenBakery.OWAtray
{
    using System;
    using System.IO;
    using System.Timers;
    using System.Windows.Forms;
    using System.Xml;

    using Microsoft.Win32;

    class Program
    {
        #region Methods

        /// <summary>
        /// Does the mapi.
        /// </summary>
        /// <param name="target">The target.</param>
        static void DoMapi(string target)
        {
            // Loop round each file and add to Clipboard
            string[] fileEntries = Directory.GetFiles(target, "*");
            int numFiles = fileEntries.Length;
            string[] files = new string[numFiles];

            int count = 0;
            foreach (string fileName in fileEntries)
            {
                files[count++] = Path.Combine(target, fileName);
            }

            DataObject d = new DataObject();
            d.SetData(DataFormats.FileDrop, files);
            Clipboard.SetDataObject(d, true);

            // Spawn IE
            string myUrl = Properties.Settings.Default.OwaUrl + "/" + Properties.Settings.Default.UserAccount + @"/?ae=Item&a=New&t=IPM.Note"  + Properties.Settings.Default.MimeURL;
            try
            {
                Console.WriteLine("Browsing to " + myUrl);
                System.Diagnostics.Process.Start("IEXPLORE.EXE", myUrl);

                // Wait for it to pop
                System.Threading.Thread.Sleep(Convert.ToInt32(Properties.Settings.Default.PopupDelay));

                // Find IE window and send keys to it
                int iHandle = NativeWin32.FindWindow(null, Properties.Settings.Default.IETitle);
                NativeWin32.SetForegroundWindow(iHandle);

                // Tab stops
                for (int f = 0; f < Convert.ToInt32(Properties.Settings.Default.TabStops); ++f)
                {
                    SendKeys.SendWait("{TAB}");
                    System.Threading.Thread.Sleep(100);
                }

                // Then the paste
                SendKeys.SendWait("^v");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //Console.ReadLine();
        }

        /// <summary>
        /// Inits the registry.
        /// </summary>
        static void InitRegistry()
        {
            string bridge = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.Settings.Default.MAPIBridge);
            string shell = System.Reflection.Assembly.GetExecutingAssembly().Location;

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
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice", "Progid", Properties.Settings.Default.MailtoClass);

                // Set up a mail handler
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi", "", "Outlook Web Access");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi", "DLLPath", bridge);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi", "EXE", "\"" + shell + "\"");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi", "Parameters", "mapi %1");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Capabilities", "ApplicationDescription", "Integrate Outlook Web Access into the desktop.");
                Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Clients\Mail\OWAMapi\Capabilities\FileAssociations");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Capabilities\Start Menu", "Mail", "OWA");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Capabilities\URLAssociations", "mailto", Properties.Settings.Default.MailtoClass);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto", "", "URL:MailTo Protocol");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto", "EditFlags", new byte[] { 0x2, 0x0, 0x0, 0x0 });
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto", "URL Protocol", "");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto\DefaultIcon", "", "\"" + shell + "\",0");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto\shell\open\command", "", "\"" + shell + "\" mailto %1");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\shell\open\command", "", "\"" + shell + "\" owa");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\DefaultIcon", "", "\"" + shell + "\",0");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", "HideIconsCommand", "\"" + shell + "\" restore");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", "ReinstallCommand", "\"" + shell + "\" registry");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", "ShowIconsCommand", "\"" + shell + "\" registry");
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", "IconsVisible", 1, RegistryValueKind.DWord);

                // Register the application
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\RegisteredApplications", "OWA", @"Software\Clients\Mail\OWAMapi\Capabilities");

                // Set default mail handler
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", "OWAMapi");
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", "OWAMapi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        [STAThread]
        static void Main(string[] args)
        {
            // Check for arguments
            if (args.Length < 1)
            {
                System.Console.WriteLine("Error - minimum of 1 argument required.");
                return;
            }

            // Which command?
            Console.WriteLine("Received command: " + args[0]);
            switch (args[0].ToUpper())
            {
                case "OWA":
                    if (args.Length > 1)
                    {
                        StartOWA(args[1]);
                    }
                    else
                    {
                        StartOWA();
                    }
                    break;

                case "SHELL":
                    if (args.Length > 1)
                    {
                        ShellOWA(args[1]);
                    }
                    else
                    {
                        ShellOWA();
                    }
                    break;

                case "REGISTRY":
                    SaveCurrentKey();
                    InitRegistry();
                    break;

                case "MAILTO":
                    if (args.Length > 1)
                    {
                        SpawnURL(args[1]);
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
                        Properties.Settings.Default.OwaUrl = args[1];
                        Properties.Settings.Default.Save();
                    }
                    break;

                case "ACCOUNT":
                    if (args.Length > 1)
                    {
                        Properties.Settings.Default.UserAccount = args[1];
                        Properties.Settings.Default.Save();
                    }
                    break;

                case "EXCHANGE":
                    if (args.Length > 1)
                    {
                        if (args[1].ToUpper() == "2010")
                        {
                            Properties.Settings.Default.MimeURL = Properties.Settings.Default.URL2010;
                        }
                        else if (args[1].ToUpper() == "2010SP1")
                        {
                            Properties.Settings.Default.MimeURL = Properties.Settings.Default.URL2010SP1;
                        }
                        else
                        {
                            Properties.Settings.Default.MimeURL = "";
                        }
                        Properties.Settings.Default.Save();
                    }
                    break;

                default:
                    Console.WriteLine("Unknown command");
                    break;
            }

            Console.WriteLine("Completed.");
        }

        /// <summary>
        /// Restores the key.
        /// </summary>
        static void RestoreKey()
        {
            try
            {
                if (Properties.Settings.Default.CurrentKey.Length > 0)
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice", "Progid", Properties.Settings.Default.CurrentKey);
                if (Properties.Settings.Default.DefaultMail.Length > 0)
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", Properties.Settings.Default.DefaultMail);
                if (Properties.Settings.Default.DefaultMailUser.Length > 0)
                    Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", Properties.Settings.Default.DefaultMailUser);
                if (Properties.Settings.Default.DefaultIcon.Length > 0)
                    Registry.SetValue(@"HKEY_CLASSES_ROOT\mailto\DefaultIcon", "", Properties.Settings.Default.DefaultIcon);
                if (Properties.Settings.Default.DefaultOpen.Length > 0)
                    Registry.SetValue(@"HKEY_CLASSES_ROOT\mailto\shell\open\command", "", Properties.Settings.Default.DefaultOpen);

                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo", "IconsVisible", 0, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Saves the current key.
        /// </summary>
        static void SaveCurrentKey()
        {
            string bridge = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.Settings.Default.MAPIBridge);
            string shell = System.Reflection.Assembly.GetExecutingAssembly().Location;

            try
            {
                // Get current mailto and store for use later
                string currentKey = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice", "Progid", Properties.Settings.Default.MailtoClass).ToString();
                if (currentKey != Properties.Settings.Default.MailtoClass)
                {
                    Properties.Settings.Default.CurrentKey = currentKey;
                    Properties.Settings.Default.Save();
                }

                // Get current default mail and store for use later
                string mailKey = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", "OWAMapi").ToString();
                if (mailKey != "OWAMapi")
                {
                    Properties.Settings.Default.DefaultMail = mailKey;
                    Properties.Settings.Default.Save();
                }

                // Get current default user mail and store for use later
                string userMailKey = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", "OWAMapi").ToString();
                if (userMailKey != "OWAMapi")
                {
                    Properties.Settings.Default.DefaultMailUser = userMailKey;
                    Properties.Settings.Default.Save();
                }

                // Get current default icon and store for use later
                string defIconKey = "\"" + shell + "\",0";
                string iconKey = Registry.GetValue(@"HKEY_CLASSES_ROOT\mailto\DefaultIcon", "", defIconKey).ToString();
                if (userMailKey != defIconKey)
                {
                    Properties.Settings.Default.DefaultIcon = iconKey;
                    Properties.Settings.Default.Save();
                }

                // Get current default cmd path and store for use later
                string defPathKey = "\"" + shell + "\" mailto %1";
                string pathKey = Registry.GetValue(@"HKEY_CLASSES_ROOT\mailto\shell\open\command", "", defPathKey).ToString();
                if (pathKey != defPathKey)
                {
                    Properties.Settings.Default.DefaultOpen = pathKey;
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Spawns the URL.
        /// </summary>
        /// <param name="target">The target.</param>
        static void SpawnURL(string target)
        {
            if (target.Substring(0, 7) == @"mailto:")
            {
                target = target.Substring(7, target.Length - 7);
            }
            string myUrl = Properties.Settings.Default.OwaUrl + "/" + Properties.Settings.Default.UserAccount + "/?ae=Item&a=New&t=IPM.Note" + Properties.Settings.Default.MimeURL + "&to=" + target;
            System.Diagnostics.Process.Start("IEXPLORE.EXE", myUrl);
            Console.WriteLine("Browsing to " + myUrl);
        }

        /// <summary>
        /// Starts the OWA.
        /// </summary>
        static void StartOWA()
        {
            string myUrl = Properties.Settings.Default.OwaUrl + "/" + Properties.Settings.Default.UserAccount;
            System.Diagnostics.Process.Start("IEXPLORE.EXE", myUrl);
            Console.WriteLine("Browsing to " + myUrl);
        }

        /// <summary>
        /// Starts the OWA.
        /// </summary>
        /// <param name="Url">The URL.</param>
        static void StartOWA(string Url)
        {
            string myUrl = Properties.Settings.Default.OwaUrl + "/" + Properties.Settings.Default.UserAccount + "/" + Url;
            System.Diagnostics.Process.Start("IEXPLORE.EXE", myUrl);
            Console.WriteLine("Browsing to " + myUrl);
        }

        /// <summary>
        /// Shells the OWA.
        /// </summary>
        static void ShellOWA()
        {
            string myUrl = Properties.Settings.Default.OwaUrl + "/" + Properties.Settings.Default.UserAccount;
            System.Diagnostics.Process.Start(myUrl);
            Console.WriteLine("Browsing to " + myUrl);
        }

        /// <summary>
        /// Shells the OWA.
        /// </summary>
        /// <param name="Url">The URL.</param>
        static void ShellOWA(string Url)
        {
            string myUrl = Properties.Settings.Default.OwaUrl + "/" + Properties.Settings.Default.UserAccount + "/" + Url;
            System.Diagnostics.Process.Start(myUrl);
            Console.WriteLine("Browsing to " + myUrl);
        }

        #endregion Methods
    }
}
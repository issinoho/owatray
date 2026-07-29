// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.GUI
//
//  <copyright file="Program.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
//
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.GUI
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    using DrunkenBakery.OWAtray.Logging;

    /// <summary>
    /// The program.
    /// </summary>
    internal static class Program
    {
        #region Methods

        /// <summary>
        /// The main.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Catch anything that would otherwise crash silently - previously a startup/shutdown
            // failure left no trace anywhere in the file log.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += ApplicationThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            LoggerProxy.Log("Starting OWAtray v" + AssemblyHelpers.AssemblyVersion, true);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            LoggerProxy.Log("Shutting down", true);
        }

        /// <summary>
        /// Handles an unhandled exception on the main UI thread.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LoggerProxy.Log("Unhandled UI thread exception", e.Exception);
        }

        /// <summary>
        /// Handles an unhandled exception on any other thread, which is otherwise fatal.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                LoggerProxy.Log("Unhandled exception" + (e.IsTerminating ? " (terminating)" : string.Empty), ex);
            }
            else
            {
                LoggerProxy.Log("Unhandled non-CLS-compliant exception: " + e.ExceptionObject, false);
            }
        }

        #endregion
    }
}

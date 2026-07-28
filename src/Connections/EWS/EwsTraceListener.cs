// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Connections.EWS
//
//  <copyright file="EwsTraceListener.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
//
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Connections.EWS
{
    using System.IO;
    using System.Xml;

    using Microsoft.Exchange.WebServices.Data;

    /// <summary>
    /// The ews trace listener.
    /// </summary>
    internal class EwsTraceListener : ITraceListener
    {
        #region Public Methods and Operators

        /// <summary>
        /// The trace.
        /// </summary>
        /// <param name="traceType">
        /// The trace type.
        /// </param>
        /// <param name="traceMessage">
        /// The trace message.
        /// </param>
        public void Trace(string traceType, string traceMessage)
        {
            CreateXmlTextFile(Path.Combine("Logs", traceType), traceMessage);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create xml text file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="traceContent">
        /// The trace content.
        /// </param>
        private static void CreateXmlTextFile(string fileName, string traceContent)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(traceContent);
                xmlDoc.Save(fileName + ".xml");
            }
            catch
            {
                File.WriteAllText(fileName + ".txt", traceContent);
            }
        }

        #endregion
    }
}
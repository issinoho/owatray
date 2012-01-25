//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// EwsTraceListener Class
//
// <copyright file="EwsTraceListener.cs" company="The Drunken Bakery">
//     Copyright (c) 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Implements tracing on an EWS connection
//
//------------------------------------------------------------------

using System.IO;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Exchange.WebServices.Data;

namespace DrunkenBakery.OWAtray.Connections.EWS
{
	internal class EwsTraceListener : ITraceListener
	{
		public void Trace(string traceType, string traceMessage)
		{
			CreateXmlTextFile(Path.Combine(Application.LocalUserAppDataPath, traceType), traceMessage);
		}

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
	}
}
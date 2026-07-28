// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Connections.Abstract
//
//  <copyright file="EmailPayload.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
//
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
    /// <summary>
    /// The email payload.
    /// </summary>
    public class EmailPayload
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets Body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets Recipient.
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// Gets or sets Subject.
        /// </summary>
        public string Subject { get; set; }

        #endregion
    }
}
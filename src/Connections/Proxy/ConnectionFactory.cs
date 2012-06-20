// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Connections.Proxy
// 
//  <copyright file="ConnectionFactory.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
// 
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Connections.Proxy
{
    using DrunkenBakery.OWAtray.Connections.Abstract;
    using DrunkenBakery.OWAtray.Connections.EWS;

    /// <summary>
    /// The connection factory.
    /// </summary>
    public static class ConnectionFactory
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create connection.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// </returns>
        public static IEmailInterface CreateConnection(EmailType provider)
        {
            IEmailInterface connection = null;

            switch (provider)
            {
                case EmailType.Exchange:
                    connection = new EwsConnection();
                    break;
            }

            return connection;
        }

        #endregion
    }
}
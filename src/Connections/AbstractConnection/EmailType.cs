// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Connections.Abstract
//
//  <copyright file="EmailType.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
//
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
    using DrunkenBakery.OWAtray.Connections.Abstract.Properties;

    /// <summary>
    /// The email type.
    /// </summary>
    public enum EmailType
    {
        /// <summary>
        /// The exchange.
        /// </summary>
        Exchange,
    }

    /// <summary>
    /// The enum extensions.
    /// </summary>
    public static partial class EnumExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Return the description.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <returns>
        /// The description.
        /// </returns>
        public static string Description(this EmailType e)
        {
            string state = string.Empty;

            switch (e)
            {
                case EmailType.Exchange:
                    state = Resources.EnumExtensions_Description_Exchange;
                    break;
            }

            return state;
        }

        #endregion
    }
}
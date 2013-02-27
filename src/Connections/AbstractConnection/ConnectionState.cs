// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.Connections.Abstract.ConnectionState.cs
//  
//  <copyright file="ConnectionState.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2013 The Drunken Bakery. All rights reserved.
//  </copyright>
//  
//  Author: Iain Smith
// ------------------------------------------------------------------
namespace DrunkenBakery.OWAtray.Connections.Abstract
{
    using DrunkenBakery.OWAtray.Connections.Abstract.Properties;

    /// <summary>
    ///     The connection state.
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// The disconnected.
        /// </summary>
        Disconnected, 

        /// <summary>
        /// The connecting.
        /// </summary>
        Connecting, 

        /// <summary>
        /// The connected.
        /// </summary>
        Connected, 

        /// <summary>
        /// The disconnecting.
        /// </summary>
        Disconnecting, 

        /// <summary>
        /// The failed.
        /// </summary>
        Failed
    }

    /// <summary>
    /// The enum extensions.
    /// </summary>
    public static partial class EnumExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The description.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Description(this ConnectionState e)
        {
            string state = string.Empty;

            switch (e)
            {
                case ConnectionState.Disconnected:
                    state = Resources.EnumExtensions_Description_Disconnected;
                    break;
                case ConnectionState.Connecting:
                    state = Resources.EnumExtensions_Description_Connecting;
                    break;
                case ConnectionState.Connected:
                    state = Resources.EnumExtensions_Description_Connected;
                    break;
                case ConnectionState.Disconnecting:
                    state = Resources.EnumExtensions_Description_Disconnecting;
                    break;
                case ConnectionState.Failed:
                    state = Resources.EnumExtensions_Description_Failed;
                    break;
            }

            return state;
        }

        #endregion
    }
}
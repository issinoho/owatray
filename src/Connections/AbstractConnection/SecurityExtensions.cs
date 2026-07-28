// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Connections.Abstract
// 
//  <copyright file="SecurityExtensions.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
// 
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// The security extensions.
    /// </summary>
    public static class SecurityExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Decrypt a string.
        /// </summary>
        /// <param name="protectedPassword">
        /// The protected password.
        /// </param>
        /// <returns>
        /// The decrypt.
        /// </returns>
        public static string Decrypt(this string protectedPassword)
        {
            if (protectedPassword.Length == 0)
            {
                return string.Empty;
            }

            byte[] bytes = Convert.FromBase64String(protectedPassword);
            byte[] password = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(password);
        }

        /// <summary>
        /// Encrypt a string.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The encrypt.
        /// </returns>
        public static string Encrypt(this string password)
        {
            if (password.Length == 0)
            {
                return string.Empty;
            }

            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] protectedPassword = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedPassword);
        }

        #endregion
    }
}
//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// SecurityExtensions Class
//
// <copyright file="SecurityExtensions.cs" company="The Drunken Bakery">
//     Copyright (c) 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Provides extension methods that deal with string encryption/decryption
//
//------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
	public static class SecurityExtensions
	{
		public static string Encrypt(this string password)
		{
			var bytes = Encoding.Unicode.GetBytes(password);
			var protectedPassword = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
			return Convert.ToBase64String(protectedPassword);
		}

		public static string Decrypt(this string protectedPassword)
		{
			var bytes = Convert.FromBase64String(protectedPassword);
			var password = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
			return Encoding.Unicode.GetString(password);
		}
	}
}
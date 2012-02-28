//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// AudioHelper
//
// <copyright file="AudioHelper.cs" company="The Drunken Bakery">
//     Copyright (c) 2009 - 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Helper class for Audio
//
//------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DrunkenBakery.OWAtray.Audio
{
	public static class AudioHelper
	{
		#region PlaySoundFlags enum

		[Flags]
		private enum PlaySoundFlags
		{
			SndSync = 0x0000,
			SndAsync = 0x0001,
			SndNodefault = 0x0002,
			SndLoop = 0x0008,
			SndNostop = 0x0010,
			SndNowait = 0x00002000,
			SndFilename = 0x00020000,
			SndResource = 0x00040004
		}

		#endregion PlaySoundFlags enum

		[DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true)]
		private static extern bool PlaySound(string szSound, IntPtr hMod, PlaySoundFlags flags);

		public static void Play(string soundFile)
		{
			if (File.Exists(soundFile))
			{
				PlaySound(soundFile, new IntPtr(), PlaySoundFlags.SndSync);
			}
		}
	}
}
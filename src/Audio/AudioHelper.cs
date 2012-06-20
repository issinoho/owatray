// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Audio
// 
//  <copyright file="AudioHelper.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
// 
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Audio
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The audio helper.
    /// </summary>
    public static class AudioHelper
    {
        #region Enums

        /// <summary>
        /// The play sound flags.
        /// </summary>
        [Flags]
        private enum PlaySoundFlags
        {
            /// <summary>
            /// The snd sync.
            /// </summary>
            SndSync = 0x0000, 

            /// <summary>
            /// The snd async.
            /// </summary>
            SndAsync = 0x0001, 

            /// <summary>
            /// The snd nodefault.
            /// </summary>
            SndNodefault = 0x0002, 

            /// <summary>
            /// The snd loop.
            /// </summary>
            SndLoop = 0x0008, 

            /// <summary>
            /// The snd nostop.
            /// </summary>
            SndNostop = 0x0010, 

            /// <summary>
            /// The snd nowait.
            /// </summary>
            SndNowait = 0x00002000, 

            /// <summary>
            /// The snd filename.
            /// </summary>
            SndFilename = 0x00020000, 

            /// <summary>
            /// The snd resource.
            /// </summary>
            SndResource = 0x00040004
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The play.
        /// </summary>
        /// <param name="soundFile">
        /// The sound file. 
        /// </param>
        public static void Play(string soundFile)
        {
            if (File.Exists(soundFile))
            {
                PlaySound(soundFile, new IntPtr(), PlaySoundFlags.SndSync);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The play sound.
        /// </summary>
        /// <param name="szSound">
        /// The sz sound. 
        /// </param>
        /// <param name="hMod">
        /// The h mod. 
        /// </param>
        /// <param name="flags">
        /// The flags. 
        /// </param>
        /// <returns>
        /// The play sound. 
        /// </returns>
        [DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true)]
        private static extern bool PlaySound(string szSound, IntPtr hMod, PlaySoundFlags flags);

        #endregion
    }
}
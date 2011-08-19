//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// FlatFile class
//
// <copyright file="FlatFile.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Logs to a text file.
//
//------------------------------------------------------------------
namespace DrunkenBakery.OWAtray
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.InteropServices;
    using System.Threading;

    /// <summary>
    /// Class to easily allow arbitrary strings to be written to a flat text file
    /// The file is opened only when writing so doesn't get locked
    /// Various switches define how the logging is formatted
    /// Rollover of logs is supported based in a combination of size and number of files
    /// </summary>
    public class FlatFile
    {
        #region Fields

        private Mutex mInMutex = new Mutex();
        private bool pActive;
        private bool pDateOn;
        private bool pLimitSize;
        private string pLogFile;
        private int pMaxSize;
        private bool pPrecise;
        private bool pScavenge;
        private int pScavengeDays;
        private int pScavengeSize;
        private bool pVerbose;
        private Queue qInQueue = new Queue();
        private Object thisLock = new Object();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlatFile"/> class.
        /// </summary>
        public FlatFile()
        {
            pDateOn = true;
            pLogFile = @"c:\debug.log";
            pLimitSize = true;
            pMaxSize = 1;
            pScavenge = true;
            pScavengeDays = 14;
            pScavengeSize = 10;
            pPrecise = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlatFile"/> class.
        /// </summary>
        /// <param name="logFile">The log file.</param>
        public FlatFile(string logFile)
        {
            pDateOn = true;
            pLogFile = logFile;
            pLimitSize = true;
            pMaxSize = 1;
            pScavenge = true;
            pScavengeDays = 14;
            pScavengeSize = 10;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FlatFile"/> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool Active
        {
            get
            {
                return pActive;
            }
            set
            {
                if (value)
                {
                    pActive = value;

                    if (pVerbose)
                    {
                        AddEntry("");
						AddEntry("--------------------------------------------------------------------------------");
						AddEntry("*** Logging Started");
                    }
                }
                else
                {
                    if (pVerbose)
                    {
						AddEntry("*** Logging Stopped");
						AddEntry("--------------------------------------------------------------------------------");
                        AddEntry("");
                    }

                    pActive = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [date on].
        /// </summary>
        /// <value><c>true</c> if [date on]; otherwise, <c>false</c>.</value>
        public bool DateOn
        {
            get
            {
                return pDateOn;
            }
            set
            {
                pDateOn = value;

                if (pVerbose)
                {
                    if (pDateOn)
                    {
						AddEntry("*** Date stamping switched on");
                    }
                    else
                    {
						AddEntry("*** Date stamping switched off");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [limit size].
        /// </summary>
        /// <value><c>true</c> if [limit size]; otherwise, <c>false</c>.</value>
        public bool LimitSize
        {
            get
            {
                return pLimitSize;
            }
            set
            {
                pLimitSize = value;

                if (pVerbose)
                {
                    if (pLimitSize)
                    {
                        AddEntry("*** Log files are size limited (currently " + pMaxSize + "Mb)");
                    }
                    else
                    {
						AddEntry("*** Log files are NOT size limited");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the log file.
        /// </summary>
        /// <value>The log file.</value>
        public string LogFile
        {
            get
            {
                return pLogFile;
            }
            set
            {
                pLogFile = value;

                if (pVerbose)
                {
					AddEntry("Log file is " + pLogFile);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the max.
        /// </summary>
        /// <value>The size of the max.</value>
        public int MaxSize
        {
            get
            {
                return pMaxSize;
            }
            set
            {
                pMaxSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FlatFile"/> is precise.
        /// </summary>
        /// <value><c>true</c> if precise; otherwise, <c>false</c>.</value>
        public bool Precise
        {
            get
            {
                return pPrecise;
            }
            set
            {
                pPrecise = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FlatFile"/> is scavenge.
        /// </summary>
        /// <value><c>true</c> if scavenge; otherwise, <c>false</c>.</value>
        public bool Scavenge
        {
            get
            {
                return pScavenge;
            }
            set
            {
                pScavenge = value;

                if (pVerbose)
                {
                    if (pScavenge)
                    {
						AddEntry("*** Log files will be scavenged (after " + pScavengeDays + " days or " + pScavengeSize + "Mb total size)");
                    }
                    else
                    {
						AddEntry("*** Log files will NOT be scavenged");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the scavenge days.
        /// </summary>
        /// <value>The scavenge days.</value>
        public int ScavengeDays
        {
            get
            {
                return pScavengeDays;
            }
            set
            {
                pScavengeDays = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the scavenge.
        /// </summary>
        /// <value>The size of the scavenge.</value>
        public int ScavengeSize
        {
            get
            {
                return pScavengeSize;
            }
            set
            {
                pScavengeSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FlatFile"/> is verbose.
        /// </summary>
        /// <value><c>true</c> if verbose; otherwise, <c>false</c>.</value>
        public bool Verbose
        {
            get
            {
                return pVerbose;
            }
            set
            {
                pVerbose = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="newText">The new text.</param>
        public void AddEntry(string newText)
        {
            lock (thisLock)
            {
                string strCopy;

                if (pActive)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(pLogFile))
                        {
                            if (pDateOn)
                            {

                                strCopy = "[" + DateTime.Now.ToString() + (pPrecise ? "." + DateTime.Now.Millisecond.ToString() : "") + "] - " + newText;
                            }
                            else
                            {
                                strCopy = newText;
                            }
                            FileStream file = new FileStream(pLogFile, FileMode.Append);
                            StreamWriter sw = new StreamWriter(file);
                            sw.WriteLine(strCopy);
                            sw.Close();
                            file.Close();
                        }

                        CheckSizeLimit();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="newText">The new text.</param>
        /// <param name="newTime">The new time.</param>
        public void AddEntry(string newText, DateTime newTime)
        {
            lock (thisLock)
            {
                string strCopy;

                if (pActive)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(pLogFile))
                        {
                            if (pDateOn)
                            {

                                strCopy = "[" + newTime.ToString() + (pPrecise ? "." + newTime.Millisecond.ToString() : "") + "] - " + newText;
                            }
                            else
                            {
                                strCopy = newText;
                            }
                            FileStream file = new FileStream(pLogFile, FileMode.Append);
                            StreamWriter sw = new StreamWriter(file);
                            sw.WriteLine(strCopy);
                            sw.Close();
                            file.Close();
                        }

                        CheckSizeLimit();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// Dequeues this instance.
        /// </summary>
        public void Dequeue()
        {
            if (!pActive) return;

            mInMutex.WaitOne();
            if (qInQueue.Count > 0)
            {
                do
                {
                    qEntry _entry = (qEntry)qInQueue.Dequeue();
                    AddEntry(_entry.text, _entry.timestamp);
                } while (qInQueue.Count > 0);
            }
            mInMutex.ReleaseMutex();
        }

        /// <summary>
        /// Enqueues the specified new text.
        /// </summary>
        /// <param name="newText">The new text.</param>
        public void Enqueue(string newText)
        {
            if (!pActive) return;

            mInMutex.WaitOne();
            qEntry _entry = new qEntry();
            _entry.timestamp = DateTime.Now;
            _entry.text = newText;
            qInQueue.Enqueue(_entry);
            mInMutex.ReleaseMutex();
        }

        /// <summary>
        /// Purges this instance.
        /// </summary>
        public void Purge()
        {
            if (!String.IsNullOrEmpty(pLogFile))
            {
                try
                {
                    if (File.Exists(pLogFile))
                    {
                        File.Delete(pLogFile);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Checks the scavenge.
        /// </summary>
        private void CheckScavenge()
        {
            long totalSize = 0;

            try
            {
                if (pScavenge)
                {
                    // Set up file & folder objects
                    FileInfo myFile = new FileInfo(pLogFile);
                    DirectoryInfo myDir = new DirectoryInfo(myFile.DirectoryName);

                    // Scavenge files older than specified days first
                    if (pScavengeDays >= 1 || pScavengeSize >= 1)
                    {
                        FileInfo[] theseFiles = myDir.GetFiles();
                        foreach (FileInfo thisFile in theseFiles)
                        {
                            if (thisFile.Name.Length >= myFile.Name.Length)
                            {
                                if ((thisFile.Name.Substring(0, myFile.Name.Length) == myFile.Name) & (thisFile.Name != myFile.Name))
                                {
                                    TimeSpan ts = DateTime.Now - thisFile.LastWriteTime;
                                    if (ts.TotalDays > pScavengeDays)
                                    {
                                        thisFile.Delete();
                                    }
                                    else
                                    {
                                        totalSize += Convert.ToInt32(thisFile.Length.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture);
                                    }
                                }
                            }
                        }
                    }

                    // If remaining files collectively exceed the specified maximum then delete until target reached
                    FileInfo[] remainingFiles = myDir.GetFiles();
                    foreach (FileInfo lastFile in remainingFiles)
                    {
                        if (totalSize > (pScavengeSize * 1048576))
                        {
                            if (lastFile.Name.Length >= myFile.Name.Length)
                            {
                                if ((lastFile.Name.Substring(0, myFile.Name.Length) == myFile.Name) & (lastFile.Name != myFile.Name))
                                {
                                    int countSize = Convert.ToInt32(lastFile.Length.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture);
                                    lastFile.Delete();
                                    totalSize -= countSize;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Checks the size limit.
        /// </summary>
        private void CheckSizeLimit()
        {
            try
            {
                if (pLimitSize & pMaxSize >= 1)
                {
                    FileInfo myFile = new FileInfo(pLogFile);
                    int fileSize = Convert.ToInt32(myFile.Length.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture);
                    if (fileSize > pMaxSize * 1048576)
                    {
                        CheckScavenge();
                        File.Move(pLogFile, pLogFile + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss", CultureInfo.CurrentCulture) + ".log");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion Methods

        #region Nested Types

        struct qEntry
        {
            #region Fields

            public string text;
            public DateTime timestamp;

            #endregion Fields
        }

        #endregion Nested Types
    }
}
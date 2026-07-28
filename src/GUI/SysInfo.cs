// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.GUI
//
//  <copyright file="SysInfo.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
//
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.GUI
{
    using System;
    using System.Linq;
    using System.Management;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using DrunkenBakery.OWAtray.GUI.Properties;

    /// <summary>
    /// The sys info.
    /// </summary>
    public partial class SysInfo : Form
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SysInfo"/> class.
        /// </summary>
        public SysInfo()
        {
            this.InitializeComponent();

            // Empty trees
            this.tvOptions.Nodes.Clear();
            this.tvCheat.Nodes.Clear();

            // Make the dummy one visible while we build the real tree
            this.tvOptions.Visible = false;
            this.tvCheat.Visible = true;

            // Please wait...
            var newNode = new TreeNode(string.Format("{0}...", Resources.SysInfo_SysInfo_Gathering_data__please_wait))
                {
                   ImageIndex = 23, SelectedImageIndex = 23,
                };
            this.tvCheat.Nodes.Add(newNode);

            // Wait and then gather data
            this.timer1.Enabled = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get computer.
        /// </summary>
        /// <param name="newNode">
        /// The new node.
        /// </param>
        private static void GetComputer(TreeNode newNode)
        {
            try
            {
                // Manufacturer details
                var query1 = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
                ManagementObjectCollection queryCollection1 = query1.Get();
                foreach (ManagementObject mo in queryCollection1)
                {
                    var childNode = new TreeNode(mo["Manufacturer"].ToString())
                        {
                           ImageIndex = 14, SelectedImageIndex = 14,
                        };
                    newNode.Nodes.Add(childNode);
                    childNode = new TreeNode(mo["Model"].ToString()) { ImageIndex = 13, SelectedImageIndex = 13 };
                    newNode.Nodes.Add(childNode);
                }

                // Processor details
                query1 = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                queryCollection1 = query1.Get();
                int count = 1;
                foreach (TreeNode childNode in from ManagementObject mo in queryCollection1
                                               select
                                                   new TreeNode(
                                                   string.Format(
                                                       "{0} {1}: {2} ({3} {4})",
                                                       Resources.SysInfo_GetComputer_CPU,
                                                       count++,
                                                       Regex.Replace(mo["Name"].ToString(), @"^\s+|\s+$", string.Empty),
                                                       mo["AddressWidth"],
                                                       Resources.SysInfo_GetComputer_bit))
                                                       {
                                                          ImageIndex = 17, SelectedImageIndex = 17,
                                                       })
                {
                    newNode.Nodes.Add(childNode);
                }

                // Memory
                query1 = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
                queryCollection1 = query1.Get();
                ulong totalCapacity = queryCollection1.Cast<ManagementObject>().Aggregate<ManagementObject, ulong>(
                    0, (current, mo) => current + Convert.ToUInt64(mo["Capacity"]));
                var memNode =
                    new TreeNode(
                        string.Format(
                            "{0}: {1} {2}",
                            Resources.SysInfo_GetComputer_Memory,
                            totalCapacity / 1073741824,
                            Resources.SysInfo_GetComputer_GB)) { ImageIndex = 19, SelectedImageIndex = 19 };
                newNode.Nodes.Add(memNode);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// The get network.
        /// </summary>
        /// <param name="newNode">
        /// The new node.
        /// </param>
        private static void GetNetwork(TreeNode newNode)
        {
            try
            {
                // Domain stuff
                var query1 = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                ManagementObjectCollection queryCollection1 = query1.Get();
                foreach (
                    TreeNode childNode in
                        from ManagementObject mo in queryCollection1 select new TreeNode(mo["CSName"].ToString()))
                {
                    childNode.ImageIndex = 22;
                    childNode.SelectedImageIndex = 22;
                    newNode.Nodes.Add(childNode);
                }

                // Domain stuff
                query1 = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
                queryCollection1 = query1.Get();
                foreach (ManagementObject mo in queryCollection1)
                {
                    var childNode = new TreeNode(mo["UserName"].ToString()) { ImageIndex = 2, SelectedImageIndex = 2 };
                    newNode.Nodes.Add(childNode);
                    childNode = new TreeNode(mo["Domain"].ToString()) { ImageIndex = 21, SelectedImageIndex = 21 };
                    newNode.Nodes.Add(childNode);
                }

                // IP Address
                string myHost = Dns.GetHostName();
                string myIp = Dns.GetHostEntry(myHost).AddressList[0].ToString();
                var myNode = new TreeNode(myIp) { ImageIndex = 20, SelectedImageIndex = 20 };
                newNode.Nodes.Add(myNode);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// The get os.
        /// </summary>
        /// <param name="newNode">
        /// The new node.
        /// </param>
        private static void GetOs(TreeNode newNode)
        {
            try
            {
                var query1 = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                ManagementObjectCollection queryCollection1 = query1.Get();
                foreach (ManagementObject mo in queryCollection1)
                {
                    var childNode = new TreeNode(mo["Caption"].ToString()) { ImageIndex = 7, SelectedImageIndex = 7 };
                    newNode.Nodes.Add(childNode);
                    childNode = new TreeNode(mo["CSDVersion"].ToString()) { ImageIndex = 8, SelectedImageIndex = 8 };
                    newNode.Nodes.Add(childNode);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// The get owner.
        /// </summary>
        /// <param name="newNode">
        /// The new node.
        /// </param>
        private static void GetOwner(TreeNode newNode)
        {
            try
            {
                var query1 = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                ManagementObjectCollection queryCollection1 = query1.Get();
                foreach (ManagementObject mo in queryCollection1)
                {
                    var childNode = new TreeNode(mo["RegisteredUser"].ToString())
                        {
                           ImageIndex = 3, SelectedImageIndex = 3,
                        };
                    newNode.Nodes.Add(childNode);
                    childNode = new TreeNode(mo["Organization"].ToString()) { ImageIndex = 4, SelectedImageIndex = 4 };
                    newNode.Nodes.Add(childNode);
                    childNode = new TreeNode(mo["SerialNumber"].ToString()) { ImageIndex = 5, SelectedImageIndex = 5 };
                    newNode.Nodes.Add(childNode);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// The get storage.
        /// </summary>
        /// <param name="newNode">
        /// The new node.
        /// </param>
        private static void GetStorage(TreeNode newNode)
        {
            try
            {
                var query1 =
                    new ManagementObjectSearcher("select FreeSpace,Size,Name from Win32_LogicalDisk where DriveType=3");
                ManagementObjectCollection queryCollection1 = query1.Get();
                foreach (TreeNode childNode in from ManagementObject mo in queryCollection1
                                               let freeSpace = Convert.ToUInt64(mo["FreeSpace"])
                                               let size = Convert.ToUInt64(mo["Size"])
                                               select
                                                   new TreeNode(
                                                   string.Format(
                                                       "{0}: {1} {2} ({3} {4} {5})",
                                                       mo["Name"],
                                                       size / 1073741824,
                                                       Resources.SysInfo_GetStorage_GB,
                                                       freeSpace / 1073741824,
                                                       Resources.SysInfo_GetStorage_GB,
                                                       Resources.SysInfo_GetStorage_free))
                                                       {
                                                          ImageIndex = 15, SelectedImageIndex = 15,
                                                       })
                {
                    newNode.Nodes.Add(childNode);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// The build tree.
        /// </summary>
        private void BuildTree()
        {
            // Empty tree
            this.SuspendLayout();
            this.tvOptions.Nodes.Clear();

            // Top level branches
            var newNode = new TreeNode(Resources.SysInfo_BuildTree_Operating_System)
                {
                   ImageIndex = 10, SelectedImageIndex = 10,
                };
            this.tvOptions.Nodes.Add(newNode);
            GetOs(newNode);

            newNode = new TreeNode(Resources.SysInfo_BuildTree_Computer) { ImageIndex = 0, SelectedImageIndex = 0 };
            this.tvOptions.Nodes.Add(newNode);
            GetComputer(newNode);

            newNode = new TreeNode(Resources.SysInfo_BuildTree_Owner) { ImageIndex = 12, SelectedImageIndex = 12 };
            this.tvOptions.Nodes.Add(newNode);
            GetOwner(newNode);

            newNode = new TreeNode(Resources.SysInfo_BuildTree_Network) { ImageIndex = 11, SelectedImageIndex = 11 };
            this.tvOptions.Nodes.Add(newNode);
            GetNetwork(newNode);

            newNode = new TreeNode(Resources.SysInfo_BuildTree_Storage) { ImageIndex = 6, SelectedImageIndex = 6 };
            this.tvOptions.Nodes.Add(newNode);
            GetStorage(newNode);

            this.ResumeLayout();
        }

        /// <summary>
        /// The cmd o k_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CmdOkClick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// The timer 1_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Timer1Tick(object sender, EventArgs e)
        {
            // Stop re-entrancy
            this.timer1.Enabled = false;

            // Tree
            this.BuildTree();
            this.tvOptions.SelectedNode = this.tvOptions.Nodes[0];

            // Now switch the trees
            this.tvCheat.Visible = false;
            this.tvOptions.Visible = true;
        }

        #endregion
    }
}
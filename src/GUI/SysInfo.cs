//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// SysInfo Form
//
// <copyright file="SysInfo.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Form to display basic System Information (OS, CPU, Disk, etc.)
// Note the the WMI is used to retrieve this information.
// Dummy line - ignore
//
//------------------------------------------------------------------

using System.Linq;
using DrunkenBakery.OWAtray.GUI.Properties;

namespace DrunkenBakery.OWAtray.GUI
{
	using System;
	using System.Management;
	using System.Text.RegularExpressions;
	using System.Windows.Forms;

	public partial class SysInfo : Form
	{
		public SysInfo()
		{
			InitializeComponent();

			// Empty trees
			tvOptions.Nodes.Clear();
			tvCheat.Nodes.Clear();

			// Make the dummy one visible while we build the real tree
			tvOptions.Visible = false;
			tvCheat.Visible = true;

			// Please wait...
			var newNode = new TreeNode(String.Format("{0}...", Resources.SysInfo_SysInfo_Gathering_data__please_wait)) {ImageIndex = 23, SelectedImageIndex = 23};
			tvCheat.Nodes.Add(newNode);

			// Wait and then gather data
			timer1.Enabled = true;
		}

		private void BuildTree()
		{
			// Empty tree
			this.SuspendLayout();
			tvOptions.Nodes.Clear();

			// Top level branches
			var newNode = new TreeNode(Resources.SysInfo_BuildTree_Operating_System) {ImageIndex = 10, SelectedImageIndex = 10};
			tvOptions.Nodes.Add(newNode);
			GetOs(newNode);

			newNode = new TreeNode(Resources.SysInfo_BuildTree_Computer) {ImageIndex = 0, SelectedImageIndex = 0};
			tvOptions.Nodes.Add(newNode);
			GetComputer(newNode);

			newNode = new TreeNode(Resources.SysInfo_BuildTree_Owner) {ImageIndex = 12, SelectedImageIndex = 12};
			tvOptions.Nodes.Add(newNode);
			GetOwner(newNode);

			newNode = new TreeNode(Resources.SysInfo_BuildTree_Network) {ImageIndex = 11, SelectedImageIndex = 11};
			tvOptions.Nodes.Add(newNode);
			GetNetwork(newNode);

			newNode = new TreeNode(Resources.SysInfo_BuildTree_Storage) {ImageIndex = 6, SelectedImageIndex = 6};
			tvOptions.Nodes.Add(newNode);
			GetStorage(newNode);

			this.ResumeLayout();
		}

		private void cmdOK_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private static void GetComputer(TreeNode newNode)
		{
			try
			{
				// Manufacturer details
				var query1 = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
				var queryCollection1 = query1.Get();
				foreach (ManagementObject mo in queryCollection1)
				{
				    var childNode = new TreeNode(mo["Manufacturer"].ToString()) {ImageIndex = 14, SelectedImageIndex = 14};
				    newNode.Nodes.Add(childNode);
					childNode = new TreeNode(mo["Model"].ToString()) {ImageIndex = 13, SelectedImageIndex = 13};
				    newNode.Nodes.Add(childNode);
				}

				// Processor details
				query1 = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
				queryCollection1 = query1.Get();
				var count = 1;
                foreach (var childNode in from ManagementObject mo in queryCollection1
                                          select new TreeNode(
                                          String.Format("{0} {1}: {2} ({3} {4})", Resources.SysInfo_GetComputer_CPU, count++, Regex.Replace(mo["Name"].ToString(), @"^\s+|\s+$", ""), mo["AddressWidth"].ToString(), Resources.SysInfo_GetComputer_bit)) { ImageIndex = 17, SelectedImageIndex = 17 })
                    
				{
					newNode.Nodes.Add(childNode);
				}

				// Memory
				query1 = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
				queryCollection1 = query1.Get();
				var totalCapacity = queryCollection1.Cast<ManagementObject>().Aggregate<ManagementObject, ulong>(0, (current, mo) => current + System.Convert.ToUInt64(mo["Capacity"]));
			    var memNode = new TreeNode(
			        String.Format("{0}: {1} {2}", Resources.SysInfo_GetComputer_Memory, (totalCapacity/1073741824), Resources.SysInfo_GetComputer_GB))
			                      {ImageIndex = 19, SelectedImageIndex = 19};
			    newNode.Nodes.Add(memNode);
			}
			catch (Exception)
			{
			}
		}

		private static void GetNetwork(TreeNode newNode)
		{
			try
			{
				// Domain stuff
				var query1 = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
				var queryCollection1 = query1.Get();
				foreach (var childNode in from ManagementObject mo in queryCollection1 select new TreeNode(mo["CSName"].ToString()))
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
					var childNode = new TreeNode(mo["UserName"].ToString()) {ImageIndex = 2, SelectedImageIndex = 2};
					newNode.Nodes.Add(childNode);
					childNode = new TreeNode(mo["Domain"].ToString()) {ImageIndex = 21, SelectedImageIndex = 21};
					newNode.Nodes.Add(childNode);
				}

				// IP Address
				var myHost = System.Net.Dns.GetHostName();
				var myIp = System.Net.Dns.GetHostEntry(myHost).AddressList[0].ToString();
				var ipNode = new TreeNode(myIp) {ImageIndex = 20, SelectedImageIndex = 20};
				newNode.Nodes.Add(ipNode);
			}
			catch (Exception)
			{
			}
		}

		private static void GetOs(TreeNode newNode)
		{
			try
			{
				var query1 = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
				var queryCollection1 = query1.Get();
				foreach( ManagementObject mo in queryCollection1 )
				{
					var childNode = new TreeNode(mo["Caption"].ToString()) {ImageIndex = 7, SelectedImageIndex = 7};
					newNode.Nodes.Add(childNode);
					childNode = new TreeNode(mo["CSDVersion"].ToString()) {ImageIndex = 8, SelectedImageIndex = 8};
					newNode.Nodes.Add(childNode);
				}
			}
			catch(Exception)
			{
			}
		}

		private static void GetOwner(TreeNode newNode)
		{
			try
			{
				var query1 = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
				var queryCollection1 = query1.Get();
				foreach (ManagementObject mo in queryCollection1)
				{
					var childNode = new TreeNode(mo["RegisteredUser"].ToString()) {ImageIndex = 3, SelectedImageIndex = 3};
					newNode.Nodes.Add(childNode);
					childNode = new TreeNode(mo["Organization"].ToString()) {ImageIndex = 4, SelectedImageIndex = 4};
					newNode.Nodes.Add(childNode);
					childNode = new TreeNode(mo["SerialNumber"].ToString()) {ImageIndex = 5, SelectedImageIndex = 5};
					newNode.Nodes.Add(childNode);
				}
			}
			catch (Exception)
			{
			}
		}

		private static void GetStorage(TreeNode newNode)
		{
			try
			{
				var query1 = new ManagementObjectSearcher("select FreeSpace,Size,Name from Win32_LogicalDisk where DriveType=3");
				var queryCollection1 = query1.Get();
                foreach (var childNode in from ManagementObject mo in queryCollection1
                                          let freeSpace = System.Convert.ToUInt64(mo["FreeSpace"])
                                          let size = System.Convert.ToUInt64(mo["Size"])
                                          select new TreeNode(
                                              String.Format("{0}: {1} {2} ({3} {4} {5})", mo["Name"].ToString(), (size / 1073741824), Resources.SysInfo_GetStorage_GB, (freeSpace / 1073741824), Resources.SysInfo_GetStorage_GB, Resources.SysInfo_GetStorage_free)) { ImageIndex = 15, SelectedImageIndex = 15 })
				{
					newNode.Nodes.Add(childNode);
				}
			}
			catch (Exception)
			{
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			// Stop re-entrancy
			timer1.Enabled = false;

			// Tree
			BuildTree();
			tvOptions.SelectedNode = tvOptions.Nodes[0];

			// Now switch the trees
			tvCheat.Visible = false;
			tvOptions.Visible = true;
		}
	}
}
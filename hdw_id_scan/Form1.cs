using System;
using System.Windows.Forms;
using System.Management;
using System.Net.NetworkInformation;

namespace hdw_id_scan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
              ("SELECT Manufacturer, Product, SerialNumber FROM Win32_BaseBoard");

            ManagementObjectCollection information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                    listBox1.Items.Add(string.Format("{0} = {1}", data.Name, data.Value));
            }
            searcher = new ManagementObjectSearcher ("SELECT Name, MaxClockSpeed, ProcessorId FROM Win32_Processor");
            information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                    listBox1.Items.Add(string.Format("{0} = {1}", data.Name, data.Value));
            }
            searcher = new ManagementObjectSearcher("SELECT Caption, AdapterRAM FROM Win32_VideoController");
            information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                    listBox1.Items.Add(string.Format("{0} = {1}", data.Name, data.Value));
            }
            /////////
            searcher = new ManagementObjectSearcher("SELECT MacAddress FROM Win32_NetworkAdapterConfiguration");
            information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                    if (string.Format("{0}", data.Value)!="") listBox1.Items.Add(string.Format("{0} = {1}", data.Name, data.Value.ToString().Replace(":", "")));
            }
          searcher = new ManagementObjectSearcher("SELECT Model, Size, SerialNumber FROM Win32_DiskDrive");
        //  searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_PhysicalMedia");
            information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                {
                    listBox1.Items.Add(string.Format("{0} = {1}", data.Name, data.Value.ToString().Replace(":", "")));
                    textBox1.Text += string.Format("{0} = {1}", data.Name, data.Value.ToString().Replace(":", ""))+Environment.NewLine;
                }

            }



            //////////
            // listBox1.Items.Add(MACAddress.ToString());
        }
    }
}

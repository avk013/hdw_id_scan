using System;
using System.Windows.Forms;
using System.Management;

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
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
              ("SELECT Product, SerialNumber FROM Win32_BaseBoard");

            ManagementObjectCollection information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                    listBox1.Items.Add(string.Format("{0} = {1}", data.Name, data.Value));
            }
            searcher = new ManagementObjectSearcher ("SELECT Name, ProcessorId FROM Win32_Processor");
            information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                    listBox1.Items.Add(string.Format("{0} = {1}", data.Name, data.Value));
            }
            // listBox1.Items.Add(Environment.)
        }
    }
}

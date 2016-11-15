using System;
using System.Windows.Forms;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;

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
  // searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
         //    searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
            information = searcher.Get();
            byte[] data1= FromHex("20");
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                {
                    if (string.Format("{0}", data.Value) != "") listBox1.Items.Add(string.Format("{0} = {1}", data.Name, data.Value.ToString().Replace(":", "")));
                   string aa;
                   if (data.Name == "SerialNumber")  data1 = FromHex(data.Value.ToString());
                    if (data.Name != "SerialNumber") aa = data.Value.ToString().Replace(":", ""); else aa = Encoding.ASCII.GetString(data1);
                    textBox1.Text += data.Name+" = "+aa+Environment.NewLine;
                }

            }
        }
        public static byte[] FromHex(string hex)
        {// переводим строку кодов символов в строку символов, исключая пробелы
           hex = hex.Replace("20", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);}
            for (int i = 0; i < raw.Length; i+=2)
            {   byte a=raw[i+1];
                raw[i + 1] = raw[i];
                raw[i] = a;}
                return raw;
        }
    }
}

using System;
using System.Windows.Forms;
using System.Management;
using System.Text;
using MySql.Data.MySqlClient;
// mysql
//server edis.mysql.ukraine.com.ua
//name edis_gr42
//pass lalala42bombom
//bd edis_gr42
//table 
//42.edis.pp.ua
// view http://www.codeproject.com/Articles/43438/Connect-C-to-MySQL
namespace hdw_id_scan
{
    public partial class Form1 : Form
    {
        const string server="edis.mysql.ukraine.com.ua";
        const string name ="edis_gr42";
        const string pass ="lalala42bombom";
        const string db ="edis_gr42";
        string[] select_from_win32;
        private MySqlConnection connection;

        private void init()
        {
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            db + ";" + "UID=" + name + ";" + "PASSWORD=" + pass + ";";
            connection = new MySqlConnection(connectionString);
        }
        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }}

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
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
                {  if (string.Format("{0}", data.Value) != "") listBox1.Items.Add(string.Format("{0} = {1}", data.Name, data.Value.ToString().Replace(":", "")));
                   string aa;
                   if (data.Name == "SerialNumber")  data1 = FromHex(data.Value.ToString());
                    if (data.Name != "SerialNumber") aa = data.Value.ToString().Replace(":", ""); else aa = Encoding.ASCII.GetString(data1);
                    textBox1.Text += data.Name+" = "+aa+Environment.NewLine;}}}
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

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            init();
            string query = "select id, name, block from 2_hdw_accessories order by block", outex = "";
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    outex += dataReader["id"].ToString() + dataReader["name"].ToString() + dataReader["block"].ToString() + Environment.NewLine;
                    string[] m = { dataReader["id"].ToString(), dataReader["name"].ToString(), dataReader["block"].ToString()};
                    go_info(m);
                 }

                //close Data Reader
                dataReader.Close();
                //close Connection
                this.CloseConnection();
                }
            //
            textBox1.Text = outex;
        }
        private void go_info(string[] massiv)
        {
            string sql = "SELECT " + massiv[1] + " FROM Win32_" + massiv[2];
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
  (sql);

            ManagementObjectCollection information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                    if (string.Format("{0}", data.Value) != "") listBox1.Items.Add(massiv[0]+massiv[1]+massiv[2]+"_"+ string.Format("{0} = {1}", data.Name, data.Value));
            }
        }
    }
}

using System;
using System.Windows.Forms;
using System.Management;
using System.Text;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Net;


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
        string[] data2db = new string[1];
        int i = 0;
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
        // для первода номера винта в нормальный вид
         // пока отключен, оказалось что в некоторых устройствах он не зашифрован      
        public static string FromHex(string hex)
        {// переводим строку кодов символов в строку символов, исключая пробелы
           hex = hex.Replace("20", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);}
            for (int i = 0; i < raw.Length; i+=2)
            {   byte a=raw[i+1];
                raw[i + 1] = raw[i];
                raw[i] = a;}
            return Encoding.ASCII.GetString(raw);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            const int codeLen = 11; // Максимальный размер кода
            const string dictionary = "abcdefghijkmnopqrstvwxyz23456789"; // словарь возможных значений
            Random rnd = new Random(); // генератор псевдослучайных чисел
            StringBuilder code = new StringBuilder(codeLen); // тут собираем код
            while (code.Length < codeLen) // пока не наберём нужное количество символов
                // добавляем в код символы по случайному индексу из словаря
                code.Append(dictionary[rnd.Next(0, dictionary.Length)]);
            string generatedCode = code.ToString(); // сохраняем получившийся код
            string name_komp = generatedCode;
            if (InputBox("имя компьютера", "введите осознанно геграфическое название компьютера:", ref name_komp) == DialogResult.OK)
            {   name_komp = name_komp + "_" + SystemInformation.UserName + "_" + Environment.MachineName;
                label1.Text= name_komp; }
            //   textBox1.Text += "\r\nRAM: " + Convert.ToDouble(ram.GetPropertyValue("Capacity")) / 1073741824 + "GB";
            string inn_komp = "none";
            if (InputBox("инвертарный номер компьютера", "введите ИНН компьютера:", ref inn_komp) == DialogResult.OK)
            { label2.Text = inn_komp; }
            //else
            {
                listBox1.Items.Clear();
            init();
            string query = "select id, name, block from 2_hdw_accessories order by block";
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();
                //Read the data and store them in the list
                while (dataReader.Read())
                {  string[] m = { dataReader["id"].ToString(), dataReader["name"].ToString(), dataReader["block"].ToString()};
                   go_info(m);
                 }
                //close Data Reader
                dataReader.Close();
                //close Connection
                this.CloseConnection();
                }
                Insert(data2db, inn_komp, name_komp);
                MessageBox.Show("OK");
                this.Close();
                // CloseConnection();
                //

            }
        }
        public void Insert(string[] mas, string inn, string name)
      {   //// думаю что данні нужно получать в виде массива
            long id_komp = 0;
            string query = "INSERT INTO 2_hdw_komp (`name`, `inventar`) VALUES('" + name + "', '" + inn+ "');";
            //open connection
            if (this.OpenConnection() == true)
            { //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Execute command
                cmd.ExecuteNonQuery();
                id_komp = cmd.LastInsertedId;
                //close connection
                this.CloseConnection();
            }
          //  MessageBox.Show(id_komp.ToString());
            string[] record;
            for (int i=0;i<mas.Length-1;i++)
            {
                record = mas[i].Split('Ь');
                long id_assesories = Convert.ToInt32(record[0]);
                string values = record[1];
                query = "INSERT INTO 2_hdw_zmist (`komp`, `accessories`, `value`,`dat`) VALUES('" + id_komp + "', '" + id_assesories + "', '" + values+ "', now());";
                //open connection
                if (this.OpenConnection() == true)
                { //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Execute command
                    cmd.ExecuteNonQuery();
                    //close connection
                    this.CloseConnection();
                }}}

        private void go_info(string[] massiv)
        {
            string value;
            
            string sql = "SELECT " + massiv[1] + " FROM Win32_" + massiv[2];
            ManagementObjectSearcher searcher = new ManagementObjectSearcher (sql);

            ManagementObjectCollection information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                    if ((string.Format("{0}", data.Value) != "") && (string.Format("{0}", data.Value) != "20:41:53:59:4E:FF"))
                    {
                   //  if (massiv[0]=="12") value = FromHex(data.Value.ToString()).ToString(); else
                            value = data.Value.ToString();
                        //data2db[i] = massiv[0] + "Ь" + data.Value.ToString()+ "Ь"+"0";                        
                        data2db[i] = massiv[0] + "Ь" + value + "Ь" + "0";
                        listBox1.Items.Add(massiv[0] + ". "+massiv[1] + " "+massiv[2] + "_" + string.Format("{0} = {1}", data.Name, value));
                        Array.Resize(ref data2db, ++i+1);
                    }
            }

            
        }
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
         //   Button buttonCancel = new Button();
            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;
            buttonOk.Text = "OK";
         //   buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
         //   buttonCancel.DialogResult = DialogResult.Cancel;
            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
           // buttonCancel.SetBounds(309, 72, 75, 23);
            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
//            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            //form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
         //   form.CancelButton = buttonCancel;
            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
        public bool ConnectionAvailable(string strServer)
        {
            try
            {
                HttpWebRequest reqFP = (HttpWebRequest)HttpWebRequest.Create(strServer);

                HttpWebResponse rspFP = (HttpWebResponse)reqFP.GetResponse();
                if (HttpStatusCode.OK == rspFP.StatusCode)
                {
                    // HTTP = 200 - Интернет безусловно есть! 
                    rspFP.Close();
                    return true;
                }
                else
                {
                    // сервер вернул отрицательный ответ, возможно что инета нет
                    rspFP.Close();
                    return false;
                }
            }
            catch (WebException)
            {
                // Ошибка, значит интернета у нас нет. Плачем :'(
                return false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(ConnectionAvailable("http://www.google.com").ToString());
            if (ConnectionAvailable("http://www.google.com") == false)
            {
                MessageBox.Show("не вижу интернета :(((((");
                this.Close();
            };
        }
    }
}

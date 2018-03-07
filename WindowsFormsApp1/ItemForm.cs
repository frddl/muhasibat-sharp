using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Globalization;

namespace WindowsFormsApp1
{
    public partial class ItemForm : Form
    {
        private static string taxi, type;
        private static int id;

        public ItemForm(string t2, string t3)
        {
            taxi = t2;
            type = t3;
            InitializeComponent();
            this.CenterToScreen();
            fillComboBox();
        }

        public ItemForm(int sentId)
        {
            id = sentId;

            InitializeComponent();
            this.CenterToScreen();
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + Utility.databaseName + ";Version=3;");
            m_dbConnection.Open();
            string sql = "select rowid, * from data where rowid = '" + sentId + "'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                textBox1.Text = reader["name"].ToString();
                textBox2.Text = reader["amount"].ToString();
                textBox3.Text = reader["purpose"].ToString();
                
                DateTime dateValue = Convert.ToDateTime(reader["time"].ToString());
                dateTimePicker1.Value = dateValue;
                
                button1.Visible = false;
                button2.Visible = true;
                button3.Visible = true;
            }

            fillComboBox();
        }

        private void fillComboBox()
        {
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + Utility.databaseName + ";Version=3;");
            m_dbConnection.Open();
            string sql = "select distinct name from data";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader["name"].ToString());
            }

            sql = "select distinct purpose from data";
            command = new SQLiteCommand(sql, m_dbConnection);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                comboBox2.Items.Add(reader["purpose"].ToString());
            }

            m_dbConnection.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            int amount;
            Int32.TryParse(textBox2.Text, out amount);
            string purpose = textBox3.Text;
            string date = dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "insert into data(name, amount, purpose, time, type, taxi) values('"+name+"', '"+amount+"', '"+purpose+ "', '" + date + "', '" + type+"', '"+taxi+"')";
            Utility.AskConf_Add(sql);
            //Utility.Execute(sql);
            //this.Close();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Utility.EditItem(id, textBox1.Text, textBox2.Text, textBox3.Text, dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            //this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Utility.DeleteItem(id);
            //this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = comboBox1.SelectedItem.ToString();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            textBox3.Text = comboBox2.SelectedItem.ToString();
        }

        private void form_Load(object sender, EventArgs e)
        {
            
        }
    }
}

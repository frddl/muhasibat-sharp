using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace Muhasibat
{
    public partial class StockManager : Form
    {
        string selectedItem = "";

        public StockManager()
        {
            InitializeComponent();
            this.CenterToScreen();
            fillComboBox();
        }

        //anbara medaxil
        private void button2_Click(object sender, EventArgs e)
        {
            handleDatabase(1);
        }

        //anbardan mexaric
        private void button3_Click(object sender, EventArgs e)
        {
            handleDatabase(-1);
        }

        private void handleDatabase(int f)
        {
            int j = 0;
            Int32.TryParse(textBox2.Text, out j);

            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + Utility.databaseName + ";Version=3;");
            m_dbConnection.Open();
            string sql = "";
            if (selectedItem == textBox1.Text)
            {
                sql = "update stock set amount = amount + " + (f * j) +" where name = '"+ textBox1.Text +"';";
            }

            else
            {
                sql = "insert into stock(name, amount) values ('" + textBox1.Text + "', '" + (f * j) + "')";
            }

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader["name"].ToString());
            }

            m_dbConnection.Close();
            this.Close();
        }

        private void fillComboBox()
        {
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + Utility.databaseName + ";Version=3;");
            m_dbConnection.Open();
            string sql = "select distinct name from stock";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader["name"].ToString());
            }

            m_dbConnection.Close();

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            textBox1.Text = comboBox1.SelectedItem.ToString();
            selectedItem = textBox1.Text;
        }
    }
}

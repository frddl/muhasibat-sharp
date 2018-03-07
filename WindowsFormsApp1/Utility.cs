using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;
using Muhasibat;

namespace WindowsFormsApp1
{
    internal class Utility
    {
        private static bool isOpen = false, release = true;
        public static string databaseName = "MyDatabase.sqlite";
        public static string stockItem = "";

        private static SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + databaseName + ";Version=3;");

        public static void InitDatabase()
        {
            isOpen = !isOpen;
            SQLiteConnection.CreateFile(databaseName);
            m_dbConnection.Open();
            string sql = "CREATE TABLE data (name TEXT, amount INT, purpose TEXT, time TIMESTAMP, type TEXT, taxi TEXT)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            if (!release)
            {
                for (int i = 0; i < 50; i++)
                {
                    sql = "insert into data (name, amount, purpose, time, type, taxi) values ('Məmmədov Fərid Elvin', 100, 'Təyinat', '"+ (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "', 'medaxil', 'city')";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();

                    sql = "insert into data (name, amount, purpose, time, type, taxi) values ('Məmmədov Fərid Elvin', 200, 'Təyinat', '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "', 'mexaric', 'city')";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();

                    sql = "insert into data (name, amount, purpose, time, type, taxi) values ('Məmmədov Fərid Elvin', 300, 'Təyinat', '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "', 'medaxil', 'xalq')";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();

                    sql = "insert into data (name, amount, purpose, time, type, taxi) values ('Məmmədov Fərid Elvin', 400, 'Təyinat', '" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "', 'mexaric', 'xalq')";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
            }

            m_dbConnection.Close();
        }

        public static void InitStock()
        {
            m_dbConnection.Open();
            string sql = "CREATE TABLE stock (name TEXT, amount INT)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            if (!release)
            {
                sql = "insert into stock (name, amount) values ('şaşki', 15)";
                command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }

            m_dbConnection.Close();
        }

        public static void Execute(String sql)
        {
            if (!isOpen)
            {
                m_dbConnection.Open();
                isOpen = !isOpen;
            }
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }


        public static void DeleteItem(int id, bool confirmed = false)
        {
            Utility.AskConfirmation(id, "del");
            string sql = "DELETE from data where rowid = " + id;
            if (confirmed) Execute(sql);
        }

        private static string nname, namount, npurpose, ndate;

        public static void EditItem(int id, string name = "", string amount = "", string purpose = "", string date = "", bool confirmed = false)
        {
            if (name != "") nname = name;
            if (amount != "") namount = amount;
            if (purpose != "") npurpose = purpose;
            if (date != "") ndate = date;
            Utility.AskConfirmation(id, "edit");

            int n = 0;
            Int32.TryParse(namount, out n);
            string sql = "UPDATE data set name = '" + nname + "', amount = '" + n + "', purpose = '" + npurpose + "', time = '"+ ndate +"' where rowid = " + id;
            if (confirmed) Execute(sql);
        }

        public static void AskConfirmation(int id, string type)
        {
            Confirmation frm = new Confirmation(id, type);
            frm.FormClosing += new FormClosingEventHandler(Form2_FormClosing);
            frm.Show();
        }

        public static void AskConf_Add(string sql, string type = "add")
        {
            Confirmation frm = new Confirmation(sql, type);
            frm.FormClosing += new FormClosingEventHandler(Form2_FormClosing);
            frm.Show();
        }

        public static void AskConf_Stock(int sql, string type = "delStock")
        {
            Confirmation frm = new Confirmation(sql, type);
            frm.FormClosing += new FormClosingEventHandler(StockConfirmed);
            frm.Show();
        }

        private static void StockConfirmed(object sender, FormClosingEventArgs e)
        {
            //Confirmation.ActiveForm.Dispose();
            //Confirmation.ActiveForm.Close();
            stockItem = "";
        }

        static private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            ItemForm.ActiveForm.Dispose();
            ItemForm.ActiveForm.Close();
        }

        public static void DeleteStockItem(int stock, bool confirmed = false)
        {
            if (!confirmed) Utility.AskConf_Stock(stock);
            string sql = "DELETE from stock where rowid = " + stock;
            if (confirmed) Execute(sql);
        }
    }
}

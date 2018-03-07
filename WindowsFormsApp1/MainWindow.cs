    using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections;
using System.Data.SQLite;
using Muhasibat;

namespace WindowsFormsApp1
{
    public partial class MainWindow : Form
    {
        bool doubleClickFlag = false;
        int balansVeziyyet = 0;

        public MainWindow()
        {
            InitializeComponent();
            this.CenterToScreen();
            //Utility.InitDatabase();
            //Utility.InitStock();
        }

        private void CreateHeaders(ListView lv)
        {
            lv.View = View.Details;
            lv.GridLines = true;
            lv.FullRowSelect = true;

            lv.Columns.Add("ID", 50);
            lv.Columns.Add("Soyad, Ad, Ata adı", 200);
            lv.Columns.Add("Məbləğ", 100);
            lv.Columns.Add("Təyinat", 300);
            lv.Columns.Add("Tarix", 180);
            lv.MouseClick += null;
            lv.Alignment = ListViewAlignment.Default;

            if (!doubleClickFlag)
            {
                MouseEventHandler handler = new MouseEventHandler(this.listView_ItemActivate);
                lv.MouseDoubleClick += handler;
                //doubleClickFlag = !doubleClickFlag;
            }
        }

        private void CreateHeadersForSearch(ListView lv)
        {
            lv.View = View.Details;
            lv.GridLines = true;
            lv.FullRowSelect = true;

            lv.Columns.Add("ID", 50);
            lv.Columns.Add("Soyad, Ad, Ata adı", 200);
            lv.Columns.Add("Məbləğ", 100);
            lv.Columns.Add("Tip", 100);
            lv.Columns.Add("Təyinat", 200);
            lv.Columns.Add("Tarix", 180);
            lv.MouseClick += null;
            lv.Alignment = ListViewAlignment.Default;
        }

        private void CreateHeadersForStock(ListView lv)
        {
            lv.View = View.Details;
            lv.GridLines = true;
            lv.FullRowSelect = true;

            lv.Columns.Add("ID", 45);
            lv.Columns.Add("Adı", 90);
            lv.Columns.Add("Say", 90);
            lv.MouseClick += null;

            lv.Alignment = ListViewAlignment.Default;
        }

        private void FillListView(ListView lv, string vtaxi, string vtype)
        {
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + Utility.databaseName + ";Version=3;");
            m_dbConnection.Open();
            string sql = "select rowid, * from data where taxi = '" + vtaxi + "' and type = '" + vtype + "' order by time desc";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string[] arr = new string[5];
                ListViewItem itm;
                arr[0] = reader["rowid"].ToString();
                arr[1] = reader["name"].ToString();
                arr[2] = reader["amount"].ToString();
                arr[3] = reader["purpose"].ToString();
                arr[4] = reader["time"].ToString();
                itm = new ListViewItem(arr);
                lv.Items.Add(itm);
            }
        }

        private void FillSearch(ListView lv, string vtaxi, string vtype = "")
        {
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + Utility.databaseName + ";Version=3;");
            m_dbConnection.Open();
            string sql = "select rowid, * from data where taxi = '" + vtaxi + "'";
            if (vtype != "") sql += " and type = '" + vtype + "'";
            sql += " order by time desc";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            int t = 0;
            while (reader.Read())
            {
                string[] arr = new string[6];
                ListViewItem itm;
                arr[0] = reader["rowid"].ToString();
                arr[1] = reader["name"].ToString();
                arr[2] = reader["amount"].ToString();
                if (reader["type"].ToString() == "medaxil")
                {
                    arr[3] = "Mədaxil";
                    Int32.TryParse(arr[2], out t);
                    balansVeziyyet += t;
                    arr[2] = " + " + arr[2];
                }
                else
                {
                    arr[3] = "Məxaric";
                    Int32.TryParse(arr[2], out t);
                    balansVeziyyet -= t;
                    arr[2] = "  - " + arr[2];
                }

                arr[4] = reader["purpose"].ToString();
                arr[5] = reader["time"].ToString();
                itm = new ListViewItem(arr);
                lv.Items.Add(itm);
                if (vtaxi == "city") label4.Text = balansVeziyyet.ToString() + " azn";
                if (vtaxi == "xalq") label5.Text = balansVeziyyet.ToString() + " azn";
            }
        }

        private void FillStock(ListView lv)
        {
            SQLiteConnection con = new SQLiteConnection("Data Source=" + Utility.databaseName + ";Version=3;");
            con.Open();
            string md = "select sum(amount) from data where type = 'medaxil'";
            string mx = "select sum(amount) from data where type = 'mexaric'";

            SQLiteCommand command = new SQLiteCommand(md, con);
            SQLiteDataReader read = command.ExecuteReader();

            int medaxil, mexaric;
            Int32.TryParse(read["sum(amount)"].ToString(), out medaxil);

            command = new SQLiteCommand(mx, con);
            read = command.ExecuteReader();
            Int32.TryParse(read["sum(amount)"].ToString(), out mexaric);
            con.Close();

            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + Utility.databaseName + ";Version=3;");
            m_dbConnection.Open();
            string sql = "select rowid, * from stock order by rowid asc";
            command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string[] arr = new string[3];
                ListViewItem itm;
                arr[0] = reader["rowid"].ToString();
                arr[1] = reader["name"].ToString();
                arr[2] = reader["amount"].ToString();
                if (arr[1] == "kassa")
                {
                    int amount = 0;
                    Int32.TryParse(arr[2], out amount);
                    amount += (medaxil - mexaric);
                    arr[2] = amount.ToString();
                }

                itm = new ListViewItem(arr);
                lv.Items.Add(itm);
            }

            m_dbConnection.Close();
        }

        private void listView_ItemActivate(object sender, EventArgs e)
        {
            int rowid = 0;
            Int32.TryParse(((ListView)sender).SelectedItems[0].Text, out rowid);
            Form frm = new ItemForm(rowid);
            frm.Text = "Düzəliş et @ " + ((ListView)sender).SelectedItems[0].SubItems[4].Text;
            frm.FormClosing += new FormClosingEventHandler(this.Form2_FormClosing);
            frm.Show();
        }

        private void FillHesabat(ListView lv, string taxi)
        {
            lv.View = View.Details;
            lv.GridLines = true;
            lv.FullRowSelect = true;

            lv.Columns.Add("Müddət", 300);
            lv.Columns.Add("Tip", 150);
            lv.Columns.Add("Məbləğ", 150);

            string[] muddet = { "Bugün", "Son 1 ay", "Son 6 ay", "İlk hesablama tarixindən" };
            DateTime today = DateTime.Now;
            lv.Items.Add("");
            InsertData(lv, taxi, "medaxil", muddet[0], "Mədaxil", today.ToString("yyyy-MM-dd"));
            InsertData(lv, taxi, "mexaric", muddet[0], "Məxaric", today.ToString("yyyy-MM-dd"));
            InsertData(lv, taxi, "", muddet[0], "Fərq", today.ToString("yyyy-MM-dd"));
            lv.Items.Add("");

            DateTime monthBack = today.AddMonths(-1);
            InsertData(lv, taxi, "medaxil", muddet[1], "Mədaxil", monthBack.ToString("yyyy-MM-dd"));
            InsertData(lv, taxi, "mexaric", muddet[1], "Məxaric", monthBack.ToString("yyyy-MM-dd"));
            InsertData(lv, taxi, "", muddet[1], "Fərq", monthBack.ToString("yyyy-MM-dd"));
            lv.Items.Add("");

            DateTime sixMonthsBack = today.AddMonths(-6);
            InsertData(lv, taxi, "medaxil", muddet[2], "Mədaxil", sixMonthsBack.ToString("yyyy-MM-dd"));
            InsertData(lv, taxi, "mexaric", muddet[2], "Məxaric", sixMonthsBack.ToString("yyyy-MM-dd"));
            InsertData(lv, taxi, "", muddet[2], "Fərq", sixMonthsBack.ToString("yyyy-MM-dd"));
            lv.Items.Add("");

            InsertData(lv, taxi, "medaxil", muddet[3], "Mədaxil", "2010-10-01");
            InsertData(lv, taxi, "mexaric", muddet[3], "Məxaric", "2010-10-01");
            InsertData(lv, taxi, "", muddet[3], "Fərq", "2010-10-01");

        }

        private void InsertData(ListView lv, string taxi, string type, string duration, string title, string time)
        {
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + Utility.databaseName + ";Version=3;");
            m_dbConnection.Open();
            string sql = "select sum(amount) from data where taxi = '" + taxi + "' and type = '" + type + "' and time >= '" + time + "'";


            if (title == "Fərq")
            {
                string sql1 = "select sum(amount) from data where taxi = '" + taxi + "' and type = 'medaxil' and time >= '" + time + "'";
                string sql2 = "select sum(amount) from data where taxi = '" + taxi + "' and type = 'mexaric' and time >= '" + time + "'";

                SQLiteCommand command = new SQLiteCommand(sql1, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();

                int medaxil, mexaric;
                Int32.TryParse(reader["sum(amount)"].ToString(), out medaxil);

                command = new SQLiteCommand(sql2, m_dbConnection);
                reader = command.ExecuteReader();
                Int32.TryParse(reader["sum(amount)"].ToString(), out mexaric);

                string[] arr = new string[3];
                ListViewItem itm;
                arr[0] = duration;
                arr[1] = title;
                arr[2] = (medaxil - mexaric).ToString();
                itm = new ListViewItem(arr);
                lv.Items.Add(itm);

                m_dbConnection.Close();
            }
            else
            {
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string[] arr = new string[3];
                    ListViewItem itm;
                    arr[0] = duration;
                    arr[1] = title;
                    if (reader["sum(amount)"].ToString() == "") arr[2] = "0";
                    else arr[2] = reader["sum(amount)"].ToString();
                    itm = new ListViewItem(arr);
                    lv.Items.Add(itm);
                }
            }
        }

        private void LoadListView()
        {
            listView1.Clear();
            listView2.Clear();
            listView3.Clear();
            listView4.Clear();
            listView5.Clear();
            listView6.Clear();
            listView7.Clear();
            listView8.Clear();
            listView9.Clear();

            CreateHeaders(listView1);
            CreateHeaders(listView2);
            CreateHeaders(listView3);
            CreateHeaders(listView4);
            
            FillListView(listView1, "city", "medaxil");
            FillListView(listView2, "city", "mexaric");
            FillListView(listView3, "xalq", "medaxil");
            FillListView(listView4, "xalq", "mexaric");

            FillHesabat(listView5, "city");
            FillHesabat(listView6, "xalq");

            CreateHeadersForSearch(listView7);
            CreateHeadersForSearch(listView8);

            FillSearch(listView7, "city");
            FillSearch(listView8, "xalq");

            CreateHeadersForStock(listView9);
            FillStock(listView9);

            if (!doubleClickFlag) doubleClickFlag = !doubleClickFlag;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadListView();
            detailedExportToExcel(listView1);
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // CityTaxi, Medaxil, Yeni
        private void button1_Click(object sender, EventArgs e)
        {
            Form frm = new ItemForm("city", "medaxil");
            frm.Text = "CityTaxi Mədaxil";
            frm.FormClosing += new FormClosingEventHandler(this.Form2_FormClosing);
            frm.Show();
        }

        // CityTaxi, Mexaric, Yeni
        private void button4_Click(object sender, EventArgs e)
        {
            Form frm = new ItemForm("city", "mexaric");
            frm.Text = "CityTaxi Məxaric";
            frm.FormClosing += new FormClosingEventHandler(this.Form2_FormClosing);
            frm.Show();
        }

        // XalqTaxi, Medaxil, Yeni
        private void button6_Click(object sender, EventArgs e)
        {
            Form frm = new ItemForm("xalq", "medaxil");
            frm.Text = "XalqTaxi Mədaxil";
            frm.FormClosing += new FormClosingEventHandler(this.Form2_FormClosing);
            frm.Show();
        }

        // XalqTaxi, Mexaric, Yeni
        private void button8_Click(object sender, EventArgs e)
        {
            Form frm = new ItemForm("xalq", "mexaric");
            frm.Text = "XalqTaxi Məxaric";
            frm.FormClosing += new FormClosingEventHandler(this.Form2_FormClosing);
            frm.Show();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoadListView();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form frm = new StockManager();
            frm.FormClosing += new FormClosingEventHandler(this.Form2_FormClosing);
            frm.Show();
        }

        private void exportToExcel(ListView lv, string[] header)
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Add(1);
            Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];
            ws.Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
            ws.Cells.ColumnWidth = 25;
            ws.Columns.AutoFit();
            ws.Rows.AutoFit();

            int i = 1;
            int i2 = 2;
            for (int k = 0; k < header.Length; k++)
            {
                ws.Cells[1, k + 1] = header[k];
            }

            foreach (ListViewItem lvi in lv.Items)
            {
                i = 1;
                foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                {
                    ws.Cells[i2, i] = lvs.Text;
                    i++;
                }
                i2++;
            }
        }

        private void detailedExportToExcel(ListView lv)
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Add(1);
            Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];
            ws.Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
            ws.Cells.ColumnWidth = 25;
            ws.Columns.AutoFit();
            ws.Rows.AutoFit();

            int row = 2, totalrows;
            int column = 1, totalcolumns;

            foreach (ListViewItem lvi in lv.Items)
            {
                if (row > 2 && ws.Cells[row - 1, column].Text == ws.Cells[row, column].Text)
                    row--;

                ws.Cells[row, column] = lvi.SubItems[4].Text.Substring(0, lvi.SubItems[4].Text.Length - 9);
                row++;
                
            }

            totalrows = row;
            row = 1;
            column = 2;

            foreach (ListViewItem lvi in lv.Items)
            {
                ws.Cells[row, column] = lvi.SubItems[1].Text;
                column++;
            }

            totalcolumns = column;
            column = 2;

            for (int i = 2; i < totalrows; i++)
            {
                for (int j = 2; j < totalcolumns; j++)
                {
                    if (ws.Cells[i, 1].Text == lv.Items[i - 2].SubItems[4].Text.Substring(0, lv.Items[i - 2].SubItems[4].Text.Length - 9) && 
                        ws.Cells[1, j].Text == lv.Items[i - 2].SubItems[1].Text) {
                        ws.Cells[i, j] = lv.Items[i - 2].SubItems[2].Text + " (" + lv.Items[i - 2].SubItems[3].Text + ")";
                    }
                }
            }
        }

        // Eksport, CityTaxi, Mədaxil
        private void button2_Click(object sender, EventArgs e)
        {
            string[] headers = { "ID", "Soyad, Ad, Ata adı", "Məbləğ", "Təyinat", "Tarix" };
            //exportToExcel(listView1, headers);
            detailedExportToExcel(listView1);
        }

        // Eksport, CityTaxi, Mexaric
        private void button3_Click(object sender, EventArgs e)
        {
            string[] headers = { "ID", "Soyad, Ad, Ata adı", "Məbləğ", "Təyinat", "Tarix" };
            exportToExcel(listView2, headers);
        }

        // Eksport, XalqTaxi, Medaxil
        private void button5_Click(object sender, EventArgs e)
        {
            string[] headers = { "ID", "Soyad, Ad, Ata adı", "Məbləğ", "Təyinat", "Tarix" };
            exportToExcel(listView3, headers);
        }

        // Eksport, XalqTaxi, Mexaric
        private void button7_Click(object sender, EventArgs e)
        {
            string[] headers = { "ID", "Soyad, Ad, Ata adı", "Məbləğ", "Təyinat", "Tarix" };
            exportToExcel(listView4, headers);
        }

        private void listView7_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        // CityTaxi, Axtarış, eksport
        private void button10_Click(object sender, EventArgs e)
        {
            string[] headers = { "ID", "Soyad, Ad, Ata adı", "Məbləğ", "Tip", "Təyinat", "Tarix" };
            exportToExcel(listView7, headers);
        }

        // CityTaxi, Axtarış, Axtar
        private void button9_Click(object sender, EventArgs e)
        {
            string dateFrom = dateTimePicker1.Value.ToString("yyyy-MM-dd 00:00:00");
            string dateTo = dateTimePicker2.Value.ToString("yyyy-MM-dd 23:59:59");

            string keyword = textBox1.Text;
            bool medaxil = checkBox1.Checked;
            bool mexaric = checkBox2.Checked;

            string sql = "select rowid, * from data where taxi = 'city' and ";

            if (medaxil && !mexaric) sql += "type = 'medaxil' and ";
            else if (!medaxil && mexaric) sql += "type = 'mexaric' and ";

            string[] keywords = keyword.Split(' ');
            for (int i = 0; i < keywords.Length; i++)
            {
                sql += "((lower(name) LIKE lower('%" + keywords[i] + "%') or upper(name) LIKE upper('%" + keywords[i] + "%')) or (lower(purpose) LIKE lower('%" + keywords[i] + "%') or upper(purpose) LIKE upper('%" + keywords[i] + "%'))) and ";
            }

//            sql += "(lower(name) LIKE lower('%" + keyword + "%') or upper(name) LIKE upper('%" + keyword + "%')) and ";

            sql += "time >= '"+ dateFrom + "' and time <= '"+ dateTo +"' ";

            sql += "order by time desc";
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + Utility.databaseName + ";Version=3;");
            m_dbConnection.Open();
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            int t = 0;

            listView7.Clear();
            CreateHeadersForSearch(listView7);
            label4.Text = "0 azn";
            balansVeziyyet = 0;

            while (reader.Read())
            {
                string[] arr = new string[6];
                ListViewItem itm;
                arr[0] = reader["rowid"].ToString();
                arr[1] = reader["name"].ToString();
                arr[2] = reader["amount"].ToString();
                if (reader["type"].ToString() == "medaxil")
                {
                    arr[3] = "Mədaxil";
                    Int32.TryParse(arr[2], out t);
                    balansVeziyyet += t;
                    arr[2] = " + " + arr[2];
                }
                else
                {
                    arr[3] = "Məxaric";
                    Int32.TryParse(arr[2], out t);
                    balansVeziyyet -= t;
                    arr[2] = "  - " + arr[2];
                }

                arr[4] = reader["purpose"].ToString();
                arr[5] = reader["time"].ToString();
                itm = new ListViewItem(arr);
                listView7.Items.Add(itm);
                label4.Text = balansVeziyyet.ToString() + " azn";
            }
        }

        private void tabPage10_Click(object sender, EventArgs e)
        {

        }

        // XalqTaksi, Axtarış, axtar
        private void button12_Click(object sender, EventArgs e)
        {
            string dateFrom1 = dateTimePicker4.Value.ToString("yyyy-MM-dd 00:00:00");
            string dateTo1 = dateTimePicker3.Value.ToString("yyyy-MM-dd 23:59:59");

            string keyword1 = textBox2.Text;
            bool medaxil1 = checkBox3.Checked;
            bool mexaric1 = checkBox4.Checked;

            string sql = "select rowid, * from data where taxi = 'xalq' and ";

            if (medaxil1 && !mexaric1) sql += "type = 'medaxil' and ";
            else if (!medaxil1 && mexaric1) sql += "type = 'mexaric' and ";

            string[] keywords1 = keyword1.Split(' ');
            for (int i = 0; i < keywords1.Length; i++)
            {
                sql += "((lower(name) LIKE lower('%" + keywords1[i] + "%') or upper(name) LIKE upper('%" + keywords1[i] + "%')) or (lower(purpose) LIKE lower('%" + keywords1[i] + "%') or upper(purpose) LIKE upper('%" + keywords1[i] + "%'))) and ";
            }

            //sql += "(lower(name) LIKE lower('%" + keyword + "%') or upper(name) LIKE upper('%" + keyword + "%')) and ";

            sql += "time >= '" + dateFrom1 + "' and time <= '" + dateTo1 + "' ";

            sql += "order by time desc";
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + Utility.databaseName + ";Version=3;");
            m_dbConnection.Open();
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            int t = 0;

            listView8.Clear();
            CreateHeadersForSearch(listView8);
            label5.Text = "0 azn";
            balansVeziyyet = 0;

            while (reader.Read())
            {
                string[] arr = new string[6];
                ListViewItem itm;
                arr[0] = reader["rowid"].ToString();
                arr[1] = reader["name"].ToString();
                arr[2] = reader["amount"].ToString();
                if (reader["type"].ToString() == "medaxil")
                {
                    arr[3] = "Mədaxil";
                    Int32.TryParse(arr[2], out t);
                    balansVeziyyet += t;
                    arr[2] = " + " + arr[2];
                }
                else
                {
                    arr[3] = "Məxaric";
                    Int32.TryParse(arr[2], out t);
                    balansVeziyyet -= t;
                    arr[2] = "  - " + arr[2];
                }

                arr[4] = reader["purpose"].ToString();
                arr[5] = reader["time"].ToString();
                itm = new ListViewItem(arr);
                listView8.Items.Add(itm);
                label5.Text = balansVeziyyet.ToString() + " azn";
            }
        }

        // xalqtaksi, axtarish, eksport
        private void button11_Click(object sender, EventArgs e)
        {
            string[] headers = { "ID", "Soyad, Ad, Ata adı", "Məbləğ", "Tip", "Təyinat", "Tarix" };
            exportToExcel(listView8, headers);
        }

        // anbar, edit
        private void button13_Click(object sender, EventArgs e)
        {
            Confirmation frm = new Confirmation(0, "addStock");
            frm.FormClosing += new FormClosingEventHandler(Form3_FormClosing);
            frm.Show();
        }

        // anbar, eksport
        private void button14_Click(object sender, EventArgs e)
        {
            string[] headers = { "ID", "Ad", "Say" };
            exportToExcel(listView9, headers);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (listView9.SelectedItems.Count == 0) MessageBox.Show("Silmək üçün heç nə seçməmisiniz!");
            else if (listView9.SelectedItems.Count > 1) MessageBox.Show("Eyni anda bir neçə elementi silmək mümkün deyil!");
            else
            {
                string stockItem = listView9.SelectedItems[0].SubItems[0].Text;
                int j = 0;
                Int32.TryParse(stockItem, out j);
                //Utility.DeleteStockItem(j);
                Confirmation frm = new Confirmation(j, "delStock");
                frm.FormClosing += new FormClosingEventHandler(Form2_FormClosing);
                frm.Show();
            }
        }

    }
}

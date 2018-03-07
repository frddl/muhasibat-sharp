using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Confirmation : Form
    {
        static int itemId = 1;
        static string funcType, execSql;

        public Confirmation(int id, string type)
        {
            itemId = id;
            funcType = type;
            InitializeComponent();
            this.CenterToScreen();
            //MessageBox.Show(funcType);
        }

        public Confirmation(string sql, string type)
        {
            execSql = sql;
            funcType = type;
            InitializeComponent();
            this.CenterToScreen();
            //MessageBox.Show(funcType);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == DateTime.Now.ToString("HHmm"))
            {
                if (itemId != 0) MessageBox.Show("Əməliyyat uğurla başa çatdı");
                if (funcType == "del") Utility.DeleteItem(itemId, true);
                else if (funcType == "edit") Utility.EditItem(itemId, "", "", "", "", true);
                else if (funcType == "add") Utility.Execute(execSql);
                else if (funcType == "delStock") Utility.DeleteStockItem(itemId, true);
                else if (funcType == "addStock") ;

                this.Close();
            }

            else if (textBox1.Text == "0013")
            {
                MessageBox.Show("Əməliyyat başa çatdırılmadı.");
                this.Close();
            }

            else
            {
                MessageBox.Show("Şifrə yalnışdır!");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

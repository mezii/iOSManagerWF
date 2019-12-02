using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoManager.Data;

namespace AutoManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine(Client911.getIPAddrees());
            button2.Enabled = false;
            LoadDataHome();
        }

        void addCombobox(int indexColumn,string item)
        {
            DataGridViewComboBoxColumn cbc = (DataGridViewComboBoxColumn)dataGridView1.Columns[indexColumn];
            cbc.Items.Add(item);

        }

        private void button1_Click(object sender, EventArgs e)
        {



            if (txtipaddress.Text != "" && txtipaddress.Text.IndexOf(".") > 0 && txtipaddress.Text.Split('.').Length - 1 > 1)
            {
                DeviceConnect deviceConnect = new DeviceConnect();
                lblSerinumber.Text = deviceConnect.getSeri(txtipaddress.Text);
                button2.Enabled = true;
            }
            else
            {
                MessageBox.Show("IP Nhap sai");
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            DBConnection db = new DBConnection();
            if (!Directory.Exists("DB"))
            {
                Directory.CreateDirectory("DB");

            }
            db.ConnectDB("DB", "ToolManager");

            db.CreateTable("Manager", "ipaddress varchar(30),tile varchar(30),country varchar(30),stt varchar(30)");
            if (txtipaddress.Text != "" && txtipaddress.Text.IndexOf(".") > 0 && txtipaddress.Text.Split('.').Length - 1 > 1 && lblSerinumber.Text != "" )
            {
                List<string> ls = db.RunCommand("select * from Manager where tile='" + lblSerinumber.Text+"'");
                if (ls.Count - 1 >= 0)
                {
                    db.RunCommand("Update Manager set ipaddress='" + txtipaddress.Text + "' where tile='" + lblSerinumber.Text + "'");
                    LoadDataHome();
                    MessageBox.Show("Da Update IPAddress Cua Seri:" + lblSerinumber.Text);
                }
                else
                {
                    db.RunCommand("INSERT INTO Manager VALUES('" + txtipaddress.Text + "','" + lblSerinumber.Text + "'" +",'US','OFF')");
                    LoadDataHome();
                }
            }
           
            button2.Enabled = false;
        }

        public void LoadDataHome()
        {
           
            if (File.Exists("ipaddress.txt"))
            {
                txtipaddress.Text = File.ReadAllText("ipaddress.txt");
            }
            dataGridView1.Rows.Clear();
            DBConnection db = new DBConnection();
            if (!Directory.Exists("DB"))
            {
                Directory.CreateDirectory("DB");

            }
            db.ConnectDB("DB", "ToolManager");
            db.CreateTable("Manager", "ipaddress varchar(30),tile varchar(30),country varchar(30),stt varchar(30)");
            List<string> ls = db.RunCommand("Select * from Manager",3);
            int i = 0;
            foreach(string item in ls)
            {
                string[] arrItem = item.Split('|');
                if(arrItem.Length-1 > 0)
                {
                    dataGridView1.Rows.Add(arrItem[0], arrItem[1]);
                    DataGridViewComboBoxCell c = new DataGridViewComboBoxCell();
                    c.Items.AddRange(Listcounty.arrCountry);
                    dataGridView1.Rows[i].Cells[2] = c;
                    dataGridView1.Rows[i].Cells[2].Value = "US";
                    if (arrItem.Length - 1 > 1)
                    {
                        dataGridView1.Rows[i].Cells[2].Value = arrItem[2];
                    }
                    if (arrItem.Length - 1 > 2)
                    {
                        dataGridView1.Rows[i].Cells[3].Value = arrItem[3];
                    }

                }
                i++;
            }
           
        }

      

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


            bool flag = e.ColumnIndex == this.dataGridView1.Rows[0].Cells["Run"].ColumnIndex;
            if (flag)
            {
                string ipAddress = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                string seri = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string country = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                string stt = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                DBConnection db = new DBConnection();
                if (!Directory.Exists("DB"))
                {
                    Directory.CreateDirectory("DB");

                }
                db.ConnectDB("DB", "ToolManager");
                db.RunCommand("Update Manager set stt= '" + stt + "', country= '" + country + "' where tile='" + seri + "'");
                if (dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString() == "ON")
                {
                    dataGridView1.Rows[e.RowIndex].Cells[3].Value = "OFF";
                    
                    int port = 5000 + e.RowIndex;
                    DeviceConnect dv = new DeviceConnect();
                    dv.SetProxy(ipAddress, ipAddress, port, false);
                }
                else
                {
                    dataGridView1.Rows[e.RowIndex].Cells[3].Value = "ON";
                   
                    int port = 5000 + e.RowIndex;
                    Client911.RunClient(ipAddress, country, port);
                }
               
            }
            

        }

        private void txtipaddress_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText("ipaddress.txt", txtipaddress.Text);
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
           
        }
        int posxyMouseRowHome = 0;
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip ctmenu = new ContextMenuStrip();
                posxyMouseRowHome = dataGridView1.HitTest(e.X, e.Y).RowIndex;

                if (posxyMouseRowHome >= 0)
                {
                    ctmenu.Items.Add("Delete").Name = "Delete";
      
                }
                ctmenu.Show(dataGridView1, new Point(e.X, e.Y));
                ctmenu.ItemClicked += new ToolStripItemClickedEventHandler(deleteHomeClick);
            }
        }
        private void deleteHomeClick(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Name.ToString())
            {
                case "Delete": 
                    deleteIP(dataGridView1.Rows[posxyMouseRowHome].Cells[0].Value.ToString(), dataGridView1.Rows[posxyMouseRowHome].Cells[1].Value.ToString()); LoadDataHome(); break;
                
            }
        }
        public void deleteIP(string ip , string seri)
        {
            DBConnection db = new DBConnection();
            if (!Directory.Exists("DB"))
            {
                Directory.CreateDirectory("DB");

            }
            db.ConnectDB("DB", "ToolManager");
            db.CreateTable("Manager", "ipaddress varchar(30),tile varchar(30),country varchar(30)");
            db.RunCommand("DELETE FROM Manager WHERE ipaddress='"+ip + "' and tile='"+seri +"'", 0);

        }
    }
}

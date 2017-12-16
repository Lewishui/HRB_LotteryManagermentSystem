using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HRB_LotteryManagermentSystem
{
    public partial class frmFindinput : Form
    {
        public string conditions = "";
        public frmFindinput()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var startAt = this.stockOutDateTimePicker.Value.AddDays(0).Date;
            //var endAt = this.stockInDateTimePicker1.Value.AddDays(0).Date;

            //conditions = "where Input_Date BETWEEN #" + startAt + "# AND #" + endAt + "#";//成功
            //conditions = "where  datetime(Input_Date) >=datetime( '" + "" + startAt.ToString("yyyy-MM-dd HH:mm:ss")
            //    + "'" + ")" + "AND datetime(Input_Date)<=datetime('" + "" + endAt.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ")";
            //conditions  = "where [Input_Date] >='2017-12-01 00:00:00" ;//成功
           
            if (textBox8.Text.Length > 0)
            {
                //conditions += " And dangriqihao like '%" + textBox8.Text + "%'";
                conditions += "" + textBox8.Text + "'";
            }
             
            this.Close();

        }
    }
}

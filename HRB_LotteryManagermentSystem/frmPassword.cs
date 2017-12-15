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
    public partial class frmPassword : Form
    {
        public frmPassword()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "2980024943")
            {
                MessageBox.Show("初始化成功!--用户名：Admin 密码：123");

                this.Close();


            }
            else
                MessageBox.Show("信息错误");

        }
    }
}

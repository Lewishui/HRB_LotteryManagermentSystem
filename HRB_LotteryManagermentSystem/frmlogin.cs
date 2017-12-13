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
    public partial class frmlogin : Form
    {
        private frmMain frmMain;

        public frmlogin()
        {
            InitializeComponent();
        }

        private void 导入彩票数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.scrollingText1.Visible = true;
            toolStrip1.Visible = false;


            if (frmMain == null)
            {
                frmMain = new frmMain( );
                frmMain.FormClosed += new FormClosedEventHandler(FrmOMS_FormClosed);
            }
            if (frmMain == null)
            {
                frmMain = new frmMain( );
            }
            frmMain.Show(this.dockPanel2);
            //var form = new frmMain( );

            //if (form.ShowDialog() == DialogResult.OK)
            //{

            //}

        }
        void FrmOMS_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is frmMain)
            {
                frmMain = null;
            }
        }
    }
}

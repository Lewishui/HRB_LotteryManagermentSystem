using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Order.Common;

namespace HRB_LotteryManagermentSystem
{
    public partial class frmlogin : Form
    {
        public log4net.ILog ProcessLogger;
        public log4net.ILog ExceptionLogger;
        private TextBox txtSAPPassword;
        private CheckBox chkSaveInfo;
        Sunisoft.IrisSkin.SkinEngine se = null;
        frmAboutBox aboutbox;
        private frmMain frmMain;
        //存放要显示的信息
        List<string> messages;
        //要显示信息的下标索引
        int index = 0;
        public frmlogin()
        {
            InitializeComponent();
            aboutbox = new frmAboutBox();
            InitialSystemInfo();
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
            se.SkinFile = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""), "PageColor1.ssk");

            InitialPassword();
            ProcessLogger.Fatal("login" + DateTime.Now.ToString());
            messages = new List<string>();
            messages.Add("欢迎使用彩票号码自动推荐管理系统  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            messages.Add("主界面的左侧一排按键是操作历史信息  ");

            timer1.Interval = 12000;
            timer1.Start();
            timer1.Tick += timer1_Tick;
        }
        void timer1_Tick(object sender, EventArgs e)
        {
            //滚动显示
            index = (index + 1) % messages.Count;
            //toolStripLabel9.Text = messages[index];
            this.scrollingText1.ScrollText = messages[index];

        }

        private void 导入彩票数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.scrollingText1.Visible = true;
            toolStrip1.Visible = false;


            if (frmMain == null)
            {
                frmMain = new frmMain();
                frmMain.FormClosed += new FormClosedEventHandler(FrmOMS_FormClosed);
            }
            if (frmMain == null)
            {
                frmMain = new frmMain();
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

        private void InitialSystemInfo()
        {
            #region 初始化配置
            ProcessLogger = log4net.LogManager.GetLogger("ProcessLogger");
            ExceptionLogger = log4net.LogManager.GetLogger("SystemExceptionLogger");
            ProcessLogger.Fatal("System Start " + DateTime.Now.ToString());
            #endregion
        }
        private void InitialPassword()
        {
            try
            {
                txtSAPPassword = new TextBox();
                txtSAPPassword.PasswordChar = '*';
                ToolStripControlHost t = new ToolStripControlHost(txtSAPPassword);
                t.Width = 100;
                t.AutoSize = false;
                t.Alignment = ToolStripItemAlignment.Right;
                this.toolStrip1.Items.Insert(this.toolStrip1.Items.Count - 4, t);

                chkSaveInfo = new CheckBox();
                chkSaveInfo.Text = "";
                chkSaveInfo.Padding = new Padding(5, 2, 0, 0);
                ToolStripControlHost t1 = new ToolStripControlHost(chkSaveInfo);
                t1.AutoSize = true;

                t1.ToolTipText = clsShowMessage.MSG_002;
                t1.Alignment = ToolStripItemAlignment.Right;
                this.toolStrip1.Items.Insert(this.toolStrip1.Items.Count - 5, t1);
                getUserAndPassword();
                chkSaveInfo.Checked = false;

            }
            catch (Exception ex)
            {
                //clsLogPrint.WriteLog("<frmMain> InitialPassword:" + ex.Message);
                throw ex;
            }
        }
        private void getUserAndPassword()
        {
            try
            {
                RegistryKey rkLocalMachine = Registry.LocalMachine;
                RegistryKey rkSoftWare = rkLocalMachine.OpenSubKey(clsConstant.RegEdit_Key_SoftWare);
                RegistryKey rkAmdape2e = rkSoftWare.OpenSubKey(clsConstant.RegEdit_Key_AMDAPE2E);
                if (rkAmdape2e != null)
                {
                    this.txtSAPUserId.Text = clsCommHelp.encryptString(clsCommHelp.NullToString(rkAmdape2e.GetValue(clsConstant.RegEdit_Key_User)));
                    this.txtSAPPassword.Text = clsCommHelp.encryptString(clsCommHelp.NullToString(rkAmdape2e.GetValue(clsConstant.RegEdit_Key_PassWord)));
                    if (clsCommHelp.NullToString(rkAmdape2e.GetValue(clsConstant.RegEdit_Key_Date)) != "")
                    {
                        this.chkSaveInfo.Checked = true;
                    }
                    else
                    {
                        this.chkSaveInfo.Checked = false;
                    }
                    rkAmdape2e.Close();
                }
                rkSoftWare.Close();
                rkLocalMachine.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw ex;
            }
        }
        private void saveUserAndPassword()
        {
            try
            {
                RegistryKey rkLocalMachine = Registry.LocalMachine;
                RegistryKey rkSoftWare = rkLocalMachine.OpenSubKey(clsConstant.RegEdit_Key_SoftWare, true);
                RegistryKey rkAmdape2e = rkSoftWare.CreateSubKey(clsConstant.RegEdit_Key_AMDAPE2E);
                if (rkAmdape2e != null)
                {
                    rkAmdape2e.SetValue(clsConstant.RegEdit_Key_User, clsCommHelp.encryptString(this.txtSAPUserId.Text.Trim()));
                    rkAmdape2e.SetValue(clsConstant.RegEdit_Key_PassWord, clsCommHelp.encryptString(this.txtSAPPassword.Text.Trim()));
                    rkAmdape2e.SetValue(clsConstant.RegEdit_Key_Date, DateTime.Now.ToString("yyyMMdd"));
                }
                rkAmdape2e.Close();
                rkSoftWare.Close();
                rkLocalMachine.Close();

            }
            catch (Exception ex)
            {
                //ClsLogPrint.WriteLog("<frmMain> saveUserAndPassword:" + ex.Message);
                throw ex;
            }
        }

        private void 关于系统ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutbox.ShowDialog();

        }

        private void tsbLogin_Click(object sender, EventArgs e)
        {
            //, 
            if (this.txtSAPUserId.Text == "Admin" || this.txtSAPUserId.Text == "Lewis")
            {
                if (this.txtSAPPassword.Text.Trim() == "123")
                {
                    this.WindowState = FormWindowState.Maximized;
                    tsbLogin.Text = "登录成功";
                    toolStripDropDownButton1.Enabled = true;

                    this.scrollingText1.Visible = true;

                }
                else
                    tsbLogin.Text = "登录失败，密码错误";

            }
            else
            {
                tsbLogin.Text = "登录失败，用户名错误";

            }
        }

        private void eToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new frmPassword();
            if (form.ShowDialog() == DialogResult.OK)
            {

            }
        }



    }
}

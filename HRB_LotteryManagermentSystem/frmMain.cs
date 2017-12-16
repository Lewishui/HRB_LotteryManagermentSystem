using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using DCTS.CustomComponents;
using Lottery.Buiness;
using Lottery.DB;
using Order.Common;
using WeifenLuo.WinFormsUI.Docking;

namespace HRB_LotteryManagermentSystem
{
    public partial class frmMain : DockContent
    {
        // 后台执行控件
        private BackgroundWorker bgWorker;
        // 消息显示窗体
        private frmMessageShow frmMessageShow;
        // 后台操作是否正常完成
        private bool blnBackGroundWorkIsOK = false;
        //后加的后台属性显
        private bool backGroundRunResult;
        List<clTuijianhaomalan_info> NewResult;
        System.Timers.Timer aTimer = new System.Timers.Timer(100);//实例化Timer类，设置间隔时间为10000毫秒； 
        System.Timers.Timer t = new System.Timers.Timer(1000);//实例化Timer类，设置间隔时间为10000毫秒； 
        bool timers_resfresh = false;
        private SortableBindingList<clTuijianhaomalan_info> sortableList;
        List<clsJisuanqi_info> JisuanqiResult;
        private SortableBindingList<clsJisuanqi_info> sortableJisuanqiList;
        List<clTuijianhaomalan_info> zhongjiangxinxi_Result;
        private SortableBindingList<clTuijianhaomalan_info> sortableList_zhongjiangxinxi;
        private ListBox ListBoxInfo;
        private List<string> selectitem = new List<string>();
        List<clTuijianhaomalan_info> Result;
        public frmMain()
        {
            InitializeComponent();
            //this.comboBox1.SelectedIndex = 0;
            timers_resfresh = true;
            this.pbStatus.Visible = false;
            //NewMethod1();
            this.Load += new EventHandler(frmMain_Load);
        }


        #region MyRegion





        #endregion

        private void button1_Click(object sender, EventArgs e)
        {

            clsAllnew BusinessHelp = new clsAllnew();
            DataTable dataTable = BusinessHelp.read();

            dataGridView.DataSource = dataTable;
            //label1.Text = dataTable.Rows.Count.ToString();
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                blnBackGroundWorkIsOK = false;
            }
            else if (e.Cancelled)
            {
                blnBackGroundWorkIsOK = true;
            }
            else
            {
                blnBackGroundWorkIsOK = true;
            }
        }
        private void InitialBackGroundWorker()
        {
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            bgWorker.ProgressChanged +=
                new ProgressChangedEventHandler(bgWorker_ProgressChanged);
        }
        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (frmMessageShow != null && frmMessageShow.Visible == true)
            {
                //设置显示的消息
                frmMessageShow.setMessage(e.UserState.ToString());
                //设置显示的按钮文字
                if (e.ProgressPercentage == clsConstant.Thread_Progress_OK)
                {
                    frmMessageShow.setStatus(clsConstant.Dialog_Status_Enable);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void filterButton_Click(object sender, EventArgs e)
        {

            get_combox();

            NewMethod();
        }

        private void get_combox()
        {
            selectitem = new List<string>();
            int s = this.tabControl1.SelectedIndex;
            if (s == 0)
            {
                string[] tatile1 = System.Text.RegularExpressions.Regex.Split(checkBoxComboBox1.Text, " ");

                for (int i = 0; i < tatile1.Length; i++)
                    selectitem.Add(tatile1[i]);
            }
            s = this.tabControl1.SelectedIndex;
            if (s == 1)
            {
                string[] tatile1 = System.Text.RegularExpressions.Regex.Split(checkBoxComboBox2.Text, " ");

                for (int i = 0; i < tatile1.Length; i++)
                    selectitem.Add(tatile1[i]);
            }
        }
        private void APIREST(object sender, EventArgs e)
        {
            NewMethod();
        }
        private void NewMethod()
        {
            Result = new List<clTuijianhaomalan_info>();

            this.pbStatus.Visible = true;
            clsAllnew BusinessHelp = new clsAllnew();
            BusinessHelp.pbStatus = pbStatus;
            BusinessHelp.tsStatusLabel1 = toolStripLabel1;
            Result = BusinessHelp.ReadWeb_Report(ref this.bgWorker, selectitem);
            zhongjiangxinxi_Result = BusinessHelp.zhongjiangxinxi_ResultAll;


            #region 计算逻辑
            NewResult = new List<clTuijianhaomalan_info>();

            NewResult = Result;
            int i = 0;

            foreach (clTuijianhaomalan_info item in NewResult)
            {
                i++;
                item.dangriqihao = DateTime.Now.ToString("yyyyMMdd").Substring(2, 6) + i.ToString().PadLeft(2, '0');
                item.tuijianhaoma = item.haomaileixing;
                item.nizhuihaoqishu = item.zuidayilou;

                //查找中奖的期数
                string[] tatile1 = System.Text.RegularExpressions.Regex.Split(item.tuijianhaoma, " ");

                foreach (clTuijianhaomalan_info temp in zhongjiangxinxi_Result)
                {
                    int time = 0;
                    //string[] tatile2 = System.Text.RegularExpressions.Regex.Split(temp.kaijianghaoma, " ");
                    for (int iq = 0; iq < tatile1.Length; iq++)
                    {
                        if (temp.kaijianghaoma.Contains(tatile1[iq]))//10 07 02 04 03   01 02
                            time++;

                    }
                    if (time == tatile1.Length)
                    {
                        item.zhongjiangqishu = temp.zhongjiangqishu;
                        break;
                    }
                }
            }
            //中奖期数
            //List<clTuijianhaomalan_info> Result1 = NewResult.Where((x, i1) => NewResult.FindIndex(z => z.tuijianhaoma == x.tuijianhaoma) == i1).ToList();
            //foreach (clTuijianhaomalan_info item in Result1)
            //{
            //    List<clTuijianhaomalan_info> Result2 = NewResult.FindAll(so => so.tuijianhaoma != null && so.tuijianhaoma == item.haomaileixing);
            //    if (Result2.Count > 1)
            //        Result2[0].zhongjiangqishu = Result2[1].dangriqihao;



            //}
            //var list = from x in str1
            //           select new
            //           {
            //               Nchar = x,
            //               Icount = str2.Count(c => c == x)
            //           };
            //foreach (var obj in list.Where(a => a.Icount > 0))
            //{
            //    Console.WriteLine("{0}出现{1}", obj.Nchar, obj.Icount);
            //}

            #endregion
            JisuanqiResult = new List<clsJisuanqi_info>();

            Showdave(Result);
            int s = this.tabControl1.SelectedIndex;

            if (s == 0)
                this.toolStripLabel1.Text = NewResult.Count.ToString() + "  刷新结束，请查看～";

            this.pbStatus.Visible = false;

            //MessageBox.Show(
            //    "Sucessful ,down file completed");
        }

        private void Showdave(List<clTuijianhaomalan_info> Result)
        {



            sortableList = new SortableBindingList<clTuijianhaomalan_info>(NewResult);
            this.bindingSource1.DataSource = this.sortableList;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.DataSource = this.bindingSource1;



            sortableList = new SortableBindingList<clTuijianhaomalan_info>(Result);
            this.bindingSource2.DataSource = this.sortableList;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = this.bindingSource2;

            timers_resfresh = true;
            //开奖信息
            sortableList_zhongjiangxinxi = new SortableBindingList<clTuijianhaomalan_info>(zhongjiangxinxi_Result);
            this.bindingSource4.DataSource = this.sortableList_zhongjiangxinxi;
            dataGridView3.AutoGenerateColumns = false;
            dataGridView3.DataSource = this.bindingSource4;


        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true && timers_resfresh == true)
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                if (backgroundWorker2.IsBusy != true)
                {
                    backgroundWorker2.RunWorkerAsync(new WorkerArgument { OrderCount = 0, CurrentIndex = 0 });


                }


                //System.Threading.ThreadStart start = new System.Threading.ThreadStart(Auto);
                //System.Threading.Thread th = new System.Threading.Thread(start);
                //th.ApartmentState = System.Threading.ApartmentState.STA;//关键


                this.button1.Enabled = false;

                // Auto();
                timers_resfresh = false;

            }
            else
            {
                this.button1.Enabled = true;
                aTimer.Stop();

            }
        }

        private void Auto()
        {
            aTimer.Elapsed += new ElapsedEventHandler(APIREST);
            aTimer.Start();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            bool success = dailysaveList(worker, e);

        }
        private bool dailysaveList(BackgroundWorker worker, DoWorkEventArgs e)
        {
            WorkerArgument arg = e.Argument as WorkerArgument;
            clsAllnew BusinessHelp = new clsAllnew();
            List<string> changeindex = new List<string>();
            changeindex.Add("1");

            bool success = true;
            try
            {
                //BusinessHelp.InputClickStatus_Server("frmDailyReportprocessor 日报保存 Save 34232", "toolStripButton11_Click", username);

                int rowCount = changeindex.Count;
                arg.OrderCount = rowCount;
                int j = 1;
                int progress = 0;

                NewMethod();

                if (arg.CurrentIndex % 5 == 0)
                {
                    backgroundWorker2.ReportProgress(progress, arg);
                }

                backgroundWorker2.ReportProgress(100, arg);
                e.Result = string.Format("{0} Saved ！", changeindex.Count);

            }
            catch (Exception ex)
            {
                if (!e.Cancel)
                {
                    //arg.HasError = true;
                    //arg.ErrorMessage = exception.Message;
                    e.Result = ex.Message + "";
                }
                success = false;
            }

            return success;
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WorkerArgument arg = e.UserState as WorkerArgument;
            if (!arg.HasError)
            {
                this.toolStripLabel1.Text = String.Format("{0}/{1}", arg.CurrentIndex, arg.OrderCount);
                this.ProgressValue = e.ProgressPercentage;
            }
            else
            {
                this.toolStripLabel1.Text = arg.ErrorMessage;
            }
        }
        public int ProgressValue
        {
            get { return this.pbStatus.Value; }
            set { pbStatus.Value = value; }
        }
        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                MessageBox.Show(string.Format("It is cancelled!"));
            }
            else
            {
                //toolStripLabel8.Text = string.Format("{0}", e.Result);
                toolStripLabel1.Text = "" + "(" + e.Result + ")" + "--刷新成功";

                toolStripLabel1.BackColor = Color.LimeGreen;


            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            get_combox();

            NewMethod();
            List<clTuijianhaomalan_info> Find_JisuanqiResult2 = NewResult.FindAll(so => so.zhongjiangqishu != null && so.zhongjiangqishu != "");
            if (Find_JisuanqiResult2.Count == 0)
                return;
            //
            JisuanqiResult = new List<clsJisuanqi_info>();


            this.pbStatus.Visible = true;
            clsAllnew BusinessHelp = new clsAllnew();
            BusinessHelp.pbStatus = pbStatus;
            BusinessHelp.tsStatusLabel1 = toolStripLabel1;
            JisuanqiResult = BusinessHelp.ReadHistroy(ref this.bgWorker, selectitem, NewResult);
            Showdave2(JisuanqiResult);

        }

        private void Showdave2(List<clsJisuanqi_info> Result)
        {
            sortableJisuanqiList = new SortableBindingList<clsJisuanqi_info>(JisuanqiResult);
            this.bindingSource3.DataSource = this.sortableJisuanqiList;
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.DataSource = this.bindingSource3;
            this.pbStatus.Visible = false;

            this.toolStripLabel1.Text = sortableJisuanqiList.Count + " -刷新结束，请查看～";

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            string sql = "";

            var form = new frmFindinput();
            if (form.ShowDialog() == DialogResult.OK)
            {
                sql = form.conditions;

            }
            //else
            //    return;

            clsAllnew BusinessHelp = new clsAllnew();

            int s = this.tabControl1.SelectedIndex;
            string conditions = "";
            if (s == 3)
            {


                if (sql == null || sql == "")
                    conditions = "select * from KaijiangInfo";//成功
                else
                    conditions = "select * from KaijiangInfo where zhongjiangqishu like '" + sql;//成功
                //                conditions = "select * from KaijiangInfo where Input_Date Between '2017-06-10' and  '2017-12-30' ";
                //                conditions = "select * from KaijiangInfo where datetime(Input_Date) between datetime('"
                //+ "2017-06-10" + "') and datetime('" + "2019-12-30" + "')";
                DataTable dataTable = BusinessHelp.readKaijiang(conditions);
                //dataGridView3.AutoGenerateColumns = true;
                dataGridView3.DataSource = dataTable;
                label1.Text = dataTable.Rows.Count.ToString();
                this.toolStripLabel1.Text = dataTable.Rows.Count + " -刷新结束，请查看～";

            }
            if (s == 2)
            {
                if (sql == null || sql == "")
                    conditions = "select * from yuanshizoushixinxi";//成功
                else
                    conditions = "select * from yuanshizoushixinxi where haomaileixing like '" + sql;//成功

                DataTable dataTable = BusinessHelp.read_yuanshizoushitu(conditions);
                //dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = dataTable;
                label1.Text = dataTable.Rows.Count.ToString();
                this.toolStripLabel1.Text = dataTable.Rows.Count + " -刷新结束，请查看～";

            }
            if (s == 1)
            {
                if (sql == null || sql == "")
                    conditions = "select * from lishizongjiang";//成功
                else
                    conditions = "select * from lishizongjiang where dangriqihao like '" + sql;//成功

                DataTable dataTable = BusinessHelp.read_lishizhongjiang(conditions);
                //dataGridView1.AutoGenerateColumns = true;
                dataGridView2.DataSource = dataTable;
                label1.Text = dataTable.Rows.Count.ToString();
                this.toolStripLabel1.Text = dataTable.Rows.Count + " -刷新结束，请查看～";

            }
            if (s == 0)
            {
                if (sql == null || sql == "")
                    conditions = "select * from tuijanhaoma";//成功
                else
                    conditions = "select * from tuijanhaoma where dangriqihao like '" + sql;//成功

                DataTable dataTable = BusinessHelp.read_tuixuanhaomalan(conditions);

                dataGridView.DataSource = dataTable;
                label1.Text = dataTable.Rows.Count.ToString();
                this.toolStripLabel1.Text = dataTable.Rows.Count + " -刷新结束，请查看～";


            }
            //DataTable dataTable = BusinessHelp.read();
            //dataGridView.AutoGenerateColumns = true;
            //dataGridView.DataSource = dataTable;
            //label1.Text = dataTable.Rows.Count.ToString();


        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            int s = this.tabControl1.SelectedIndex;
            if (s == 0)
            {
                downEXCEL(dataGridView);
            }
            else if (s == 1)
            {
                downEXCEL(dataGridView2);
            }
            else if (s == 2)
            {
                downEXCEL(dataGridView1);
            }
            else if (s == 3)
            {
                downEXCEL(dataGridView3);
            }
        }
        public void downEXCEL(DataGridView dgv)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("Sorry , No Data Output !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "csv|*.csv";
            string strFileName = "System  Info" + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            saveFileDialog.FileName = strFileName;
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                strFileName = saveFileDialog.FileName.ToString();
            }
            else
            {
                return;
            }
            FileStream fa = new FileStream(strFileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fa, Encoding.Unicode);
            string delimiter = "\t";
            string strHeader = "";
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                strHeader += dgv.Columns[i].HeaderText + delimiter;
            }
            sw.WriteLine(strHeader);

            //output rows data
            for (int j = 0; j < dgv.Rows.Count; j++)
            {
                string strRowValue = "";

                for (int k = 0; k < dgv.Columns.Count; k++)
                {
                    if (dgv.Rows[j].Cells[k].Value != null)
                    {
                        strRowValue += dgv.Rows[j].Cells[k].Value.ToString().Replace("\r\n", " ").Replace("\n", "") + delimiter;
                        if (dgv.Rows[j].Cells[k].Value.ToString() == "LIP201507-35")
                        {

                        }

                    }
                    else
                    {
                        strRowValue += dgv.Rows[j].Cells[k].Value + delimiter;
                    }
                }
                sw.WriteLine(strRowValue);
            }
            sw.Close();
            fa.Close();
            MessageBox.Show("下载完成 ！", "System", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            clsAllnew BusinessHelp = new clsAllnew();
            int s = this.tabControl1.SelectedIndex;
            if (s == 3)
            {
                BusinessHelp.inster(zhongjiangxinxi_Result);
                MessageBox.Show("保存开奖信息完成", "保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (s == 2)
            {
                BusinessHelp.inster_yuanshizoushixinxi(Result);
                MessageBox.Show("保存走势图-数据-完成", "保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (s == 1)
            {
                BusinessHelp.inster_lishizongjianglan(JisuanqiResult);
                MessageBox.Show("保存-历史中奖-完成", "保存", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            if (s == 0)
            {
                BusinessHelp.inster_tuijanhaoma(NewResult);
                MessageBox.Show("保存-推选号码栏-完成", "保存", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int s = this.tabControl1.SelectedIndex;
            if (s == 0)
            {

                this.toolStripLabel1.Text = dataGridView.RowCount + " -条";
            }
            else if (s == 1)
            {

                this.toolStripLabel1.Text = dataGridView2.RowCount + " -条";
            }
            else if (s == 2)
            {

                this.toolStripLabel1.Text = dataGridView1.RowCount + " -条";
            }
            else if (s == 3)
            {

                this.toolStripLabel1.Text = dataGridView3.RowCount + " -条";
            }
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            Graphics g = e.Graphics;
            Pen p = new Pen(Color.Blue, 1);
            g.DrawRectangle(p, e.Bounds.X + 1, e.Bounds.Y + 1, 12, 12);
            if (e.Index == cb.SelectedIndex)
                g.DrawString("√", new Font(FontFamily.GenericSerif, 10), Brushes.Red,
         e.Bounds.Location, StringFormat.GenericDefault);
            g.DrawString(cb.GetItemText(cb.Items[e.Index]), new Font(FontFamily.GenericSerif, 9),
                Brushes.Black, 15, e.Bounds.Y + 1, StringFormat.GenericDefault);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
            //comboBox1.DrawItem += new DrawItemEventHandler(comboBox1_DrawItem);
            //comboBox1.Items.Add("sd1");
            //comboBox1.Items.Add("sd2");
            //comboBox1.Items.Add("sd3");

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            

            try
            {
                string sql = "";

                int s = this.tabControl1.SelectedIndex;
                if (s == 3)
                {
                    if (MessageBox.Show("确认要删除全部开奖信息, 继续 ?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                    }
                    else
                        return;
                    sql = "delete FROM KaijiangInfo";
                }
                if (s == 2)
                {
                    if (MessageBox.Show("确认要删除全部走势数据信息, 继续 ?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                    }
                    else
                        return;
                    sql = "delete FROM yuanshizoushixinxi";
                }
                if (s == 1)
                {
                    if (MessageBox.Show("确认要删除全部历史中奖信息, 继续 ?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                    }
                    else
                        return;
                    sql = "delete FROM lishizongjiang";
                }
                if (s == 0)
                {
                    if (MessageBox.Show("确认要删除全部推荐号码信息, 继续 ?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                    }
                    else
                        return;
                    sql = "delete FROM tuijanhaoma";
                }

                clsAllnew BusinessHelp = new clsAllnew();

                BusinessHelp.delete(sql);
                this.toolStripLabel1.Text = " -删除成功！";

            }
            catch (Exception ex)
            {
                MessageBox.Show("错误" + ex);

                return;

                throw;
            }

        }

        private void toolStripButton4_Click_1(object sender, EventArgs e)
        {
            Application.Exit();


        }
    }
}

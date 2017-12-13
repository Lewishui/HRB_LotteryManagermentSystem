using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        public frmMain()
        {
            InitializeComponent();
            this.comboBox1.SelectedIndex = 0;
            timers_resfresh = true;

        }
        #region MyRegion



        //clsAllnew BusinessHelp = new clsAllnew();

        //BusinessHelp.delete();

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

            NewMethod();
        }
        private void APIREST(object sender, EventArgs e)
        {
            NewMethod();
        }
        private void NewMethod()
        {
            this.pbStatus.Visible = true;
            clsAllnew BusinessHelp = new clsAllnew();
            BusinessHelp.pbStatus = pbStatus;
            BusinessHelp.tsStatusLabel1 = toolStripLabel1;
            List<clTuijianhaomalan_info> Result = BusinessHelp.ReadWeb_Report(ref this.bgWorker, comboBox1.Text);
            zhongjiangxinxi_Result = BusinessHelp.zhongjiangxinxi_Result;


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

            this.toolStripLabel1.Text = "刷新结束，请查看～";
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
            NewMethod();
            List<clTuijianhaomalan_info> Find_JisuanqiResult2 = NewResult.FindAll(so => so.zhongjiangqishu != null && so.zhongjiangqishu != "");
            if (Find_JisuanqiResult2.Count == 0)
                return  ;
            //
            JisuanqiResult = new List<clsJisuanqi_info>();


            this.pbStatus.Visible = true;
            clsAllnew BusinessHelp = new clsAllnew();
            BusinessHelp.pbStatus = pbStatus;
            BusinessHelp.tsStatusLabel1 = toolStripLabel1;
            JisuanqiResult = BusinessHelp.ReadHistroy(ref this.bgWorker, comboBox1.Text, NewResult);
            Showdave2(JisuanqiResult);

        }

        private void Showdave2(List<clsJisuanqi_info> Result)
        {
            sortableJisuanqiList = new SortableBindingList<clsJisuanqi_info>(JisuanqiResult);
            this.bindingSource3.DataSource = this.sortableJisuanqiList;
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.DataSource = this.bindingSource3;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            clsAllnew BusinessHelp = new clsAllnew();
            DataTable dataTable = BusinessHelp.read();
            dataGridView.AutoGenerateColumns = true;
            dataGridView.DataSource = dataTable;
            label1.Text = dataTable.Rows.Count.ToString();

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            clsAllnew BusinessHelp = new clsAllnew();

            BusinessHelp.inster();
        }
    }
}

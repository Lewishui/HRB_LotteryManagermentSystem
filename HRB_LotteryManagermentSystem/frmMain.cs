﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        public Order.Common.ScrollingText scrollingText1;
        //多线程访问 davgiridview 报错

        private delegate void InvokeHandler();
        public log4net.ILog ProcessLogger;
        public log4net.ILog ExceptionLogger;
        // 后台执行控件
        private BackgroundWorker bgWorker;
        // 消息显示窗体
        private frmMessageShow frmMessageShow;
        // 后台操作是否正常完成
        private bool blnBackGroundWorkIsOK = false;
        //后加的后台属性显
        private bool backGroundRunResult;
        List<clTuijianhaomalan_info> NewResult;
        List<clTuijianhaomalan_info> Zhongjiangle_Result;
        System.Timers.Timer aTimer = new System.Timers.Timer(100);//实例化Timer类，设置间隔时间为10000毫秒； 
        System.Timers.Timer t = new System.Timers.Timer(1000);//实例化Timer类，设置间隔时间为10000毫秒； 
        bool timers_resfresh = false;
        private SortableBindingList<clTuijianhaomalan_info> sortableList;
        private SortableBindingList<clTuijianhaomalan_info> sortableList_dav4;
        List<clsJisuanqi_info> JisuanqiResult;
        private SortableBindingList<clsJisuanqi_info> sortableJisuanqiList;
        private SortableBindingList<clsJisuanqi_info> sortableJisuanqiList_dav5;
        List<clTuijianhaomalan_info> zhongjiangxinxi_Result;
        private SortableBindingList<clTuijianhaomalan_info> sortableList_zhongjiangxinxi;
        private ListBox ListBoxInfo;
        private List<string> selectitem = new List<string>();
        List<clTuijianhaomalan_info> Result;
        private List<clszhongleiDuiyingQishu_info> mapping_Result;
        private bool IsRun = false;
        private DateTime StopTime;
        private string linkID;

        private Thread GetDataforRawDataThread;
        int rowcount = 0;
        int RowRemark = 0;
        int cloumn = 0;
        public frmMain()
        {
            InitializeComponent();

            clsAllnew BusinessHelp = new clsAllnew();

            string tx = BusinessHelp.getUserAndPassword();
            if (tx != null && tx != "")
                toolStripComboBox1.Text = tx;

            //this.comboBox1.SelectedIndex = 0;
            InitialSystemInfo();

            timers_resfresh = true;
            // this.pbStatus.Visible = false;
            mapping_Result = new List<clszhongleiDuiyingQishu_info>();
            huoquduiyingqishu();
            //NewMethod1();
            this.Load += new EventHandler(frmMain_Load);


        }
        private void InitialSystemInfo()
        {
            #region 初始化配置
            ProcessLogger = log4net.LogManager.GetLogger("ProcessLogger");
            ExceptionLogger = log4net.LogManager.GetLogger("SystemExceptionLogger");
            ProcessLogger.Fatal("System Start " + DateTime.Now.ToString());
            #endregion
        }
        private void huoquduiyingqishu()
        {
            clszhongleiDuiyingQishu_info item = new clszhongleiDuiyingQishu_info();
            item.wanfazhonglei = "任选二";
            mapping_Result.Add(item);
            item.wanfazhonglei = "任选三";
            mapping_Result.Add(item);
            item.wanfazhonglei = "任选四";
            mapping_Result.Add(item);
            item.wanfazhonglei = "任选五";
            mapping_Result.Add(item);
            item.wanfazhonglei = "任选六";
            mapping_Result.Add(item);
            item.wanfazhonglei = "任选七";
            mapping_Result.Add(item);
            item.wanfazhonglei = "任选八";
            mapping_Result.Add(item);
            item.wanfazhonglei = "前一直";
            mapping_Result.Add(item);
            item.wanfazhonglei = "前二直";
            mapping_Result.Add(item);
            item.wanfazhonglei = "前三直";
            mapping_Result.Add(item);
            item.wanfazhonglei = "前二组";
            mapping_Result.Add(item);
            item.wanfazhonglei = "前三组";
            mapping_Result.Add(item);
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
            if (toolStripComboBox1.Text == "网址ID")
            {
                MessageBox.Show("请选择网址ID,然后重试");
                return;


            }
            Main();

        }

        private void Main()
        {
            string path1 = AppDomain.CurrentDomain.BaseDirectory + "System\\confing.txt";

            string[] fileText1 = File.ReadAllLines(path1);
            mapping_Result = new List<clszhongleiDuiyingQishu_info>();
            for (int i = 0; i < fileText1.Length; i++)
            {
                string[] tatile1 = System.Text.RegularExpressions.Regex.Split(fileText1[i], " ");
                clszhongleiDuiyingQishu_info item = new clszhongleiDuiyingQishu_info();
                item.wanfazhonglei = tatile1[0];
                item.qishu = tatile1[1];
                mapping_Result.Add(item);


            }
            get_combox();
            if (selectitem.Count == 0 || selectitem[0] == "")
            {
                MessageBox.Show("请选择玩法,再次尝试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            NewMethod();
            //判断是否 执行上次timer 工作
            IsRun = false;
            StopTime = DateTime.Now;//
            toolStripLabel2.Text = "上次刷新时间:  " + DateTime.Now.ToString("HH:mm:ss");
        }

        private void get_combox()
        {
            selectitem = new List<string>();
            int s = this.tabControl1.SelectedIndex;
            //if (s == 0)//在哪个界面都要查找这个页面的 Checkbox
            {
                string[] tatile1 = System.Text.RegularExpressions.Regex.Split(checkBoxComboBox1.Text, " ");

                for (int i = 0; i < tatile1.Length; i++)
                    selectitem.Add(tatile1[i]);
            }
            s = this.tabControl1.SelectedIndex;
            if (s == 1)
            {
                //string[] tatile1 = System.Text.RegularExpressions.Regex.Split(checkBoxComboBox2.Text, " ");

                //for (int i = 0; i < tatile1.Length; i++)
                //    selectitem.Add(tatile1[i]);
            }
            //LINK 
            linkID = toolStripComboBox1.Text;




        }
        private void APIREST(object sender, EventArgs e)
        {
            Main();

            //NewMethod();
        }
        private void NewMethod()
        {

            try
            {
                Result = new List<clTuijianhaomalan_info>();

                pbStatus.Visible = true;
                clsAllnew BusinessHelp = new clsAllnew();
                BusinessHelp.ProcessLogger = ProcessLogger;
                BusinessHelp.ExceptionLogger = ExceptionLogger;
                BusinessHelp.pbStatus = pbStatus;
                BusinessHelp.tsStatusLabel1 = toolStripLabel1;
                ProcessLogger.Fatal("读取NewMethod" + DateTime.Now.ToString());
                pbStatus.Maximum = 5;
                pbStatus.Value = 1;

                //BusinessHelp.linkid(Convert.ToInt32(toolStripComboBox1.Text));
                BusinessHelp.linkid(Convert.ToInt32(linkID));

                

                Result = BusinessHelp.ReadWeb_Report(ref this.bgWorker, selectitem, mapping_Result);
                //if (Result != null && Result.Count > 0)
                //{
                //    List<clTuijianhaomalan_info> quchong_Result = Result.Where((x, ii) => Result.FindIndex(z => z.wanfazhonglei == x.wanfazhonglei && z.haomaileixing == x.haomaileixing) == ii).ToList();//Lambda表达式去重  
                //    List<clTuijianhaomalan_info> quchong_Result1 = (from v in Result select v).Distinct().ToList();
                //}
                zhongjiangxinxi_Result = BusinessHelp.zhongjiangxinxi_ResultAll;
                if (zhongjiangxinxi_Result == null || zhongjiangxinxi_Result.Count == 0 || Result == null || Result.Count == 0)
                {
                    //  pbStatus.Visible = false;
                    pbStatus.Value = 0;
                    //this.toolStripLabel1.Text = "信息获取失败，请确认网站访问正常后重新尝试 ！";

                    return;
                }
                pbStatus.Value = 2;


                this.toolStripLabel1.Text = "整理数据中....";

                #region 推荐号码 造的 假数据
                //clTuijianhaomalan_info iii = new clTuijianhaomalan_info();
                //iii.wanfazhonglei = "乐四";
                //iii.haomaileixing = "04 06 08 09";
                //iii.chuxiancishu = "9";
                //iii.zuidayilou = "1";
                //Result.Add(iii);
                //iii = new clTuijianhaomalan_info();
                //iii.wanfazhonglei = "乐五";
                //iii.chuxiancishu = "9";
                //iii.zuidayilou = "1";
                //iii.haomaileixing = "09 08 10 02 12";
                //Result.Add(iii); 
                #endregion


                #region 计算逻辑

                #region 如果新推选号码与已经中奖但还未转到历史中奖栏中的原推选号码一致，新推选号码显示在推荐号码栏中。

                string conditions = "select * from tuijanhaoma where Input_Date like '" + DateTime.Now.ToString("yyyy/MM/dd") + "'";//成功
                this.toolStripLabel1.Text = "查询当日推荐号....";

                List<clTuijianhaomalan_info> ClaimReport_Server = BusinessHelp.ReadServer_tuijanhaoma(conditions);
                ProcessLogger.Fatal("ReadServer_lishizhongjian 1" + DateTime.Now.ToString());


                conditions = "select * from lishizongjiang where Input_Date like '" + DateTime.Now.ToString("yyyy/MM/dd") + "'";//成功

                this.toolStripLabel1.Text = "查询当日历史中奖....";

                List<clsJisuanqi_info> Jisuanqi_Server = BusinessHelp.ReadServer_lishizhongjiang(conditions);

                #endregion
                Zhongjiangle_Result = new List<clTuijianhaomalan_info>();

                NewResult = new List<clTuijianhaomalan_info>();
                List<clTuijianhaomalan_info> hebing_NewResult = new List<clTuijianhaomalan_info>();


                List<string> quchong_wanfazhonglei = (from v in Result select v.wanfazhonglei).Distinct().ToList();
                for (int index = 0; index < quchong_wanfazhonglei.Count; index++)
                {
                    NewResult = Result;

                    NewResult = NewResult.FindAll(s => s.wanfazhonglei != null && s.wanfazhonglei == quchong_wanfazhonglei[index]);

                    int i = 0;
                    var q = (from o in NewResult
                             orderby o.chuxiancishu descending
                             select o);
                    NewResult = q.ToList();
                    NewResult = NewResult.FindAll(s => s.chuxiancishu != null && s.chuxiancishu == NewResult[0].chuxiancishu);
                    NewResult = NewResult.Where((x, ii) => NewResult.FindIndex(z => z.haomaileixing == x.haomaileixing) == ii).ToList();//Lambda表达式去重  


                    //List<clTuijianhaomalan_info> NewResultNew = q.ToList().FindAll(s => s.chuxiancishu != null && s.chuxiancishu == NewResult[0].chuxiancishu);
                    //dataGridView4.DataSource = NewResultNew;

                    //descending ascending

                    q = (from o in NewResult
                         orderby Convert.ToInt32(o.zuidayilou) descending
                         select o);

                    var qq = q.Last();
                    List<clTuijianhaomalan_info> zuidayilou_same = NewResult.FindAll(s => s.zuidayilou != null && s.zuidayilou == qq.zuidayilou);


                    NewResult = new List<clTuijianhaomalan_info>();
                    if (zuidayilou_same != null && zuidayilou_same.Count == 1)
                        NewResult.Add(qq);
                    else if (zuidayilou_same != null && zuidayilou_same.Count >= 1)
                    {
                        foreach (clTuijianhaomalan_info itemadd in zuidayilou_same)
                            NewResult.Add(itemadd);
                    }
                    foreach (clTuijianhaomalan_info item in NewResult)
                    {
                        //如果以前已存在此推荐号则不追加
                        clTuijianhaomalan_info temp1 = ClaimReport_Server.Find(s => s.tuijianhaoma != null && s.tuijianhaoma == item.haomaileixing && s.wanfazhonglei != null && s.wanfazhonglei == item.wanfazhonglei);
                        if (temp1 != null)
                            break;
                        i++;
                        //item.dangriqihao = DateTime.Now.ToString("yyyyMMdd").Substring(2, 6) + i.ToString().PadLeft(2, '0');
                        //
                        if (zhongjiangxinxi_Result != null && zhongjiangxinxi_Result.Count > 0)
                            item.dangriqihao = zhongjiangxinxi_Result[zhongjiangxinxi_Result.Count - 1].zhongjiangqishu;

                        item.tuijianhaoma = item.haomaileixing;
                        item.nizhuihaoqishu = item.zuidayilou;
                        item.Message = "New";

                        hebing_NewResult.Add(item);
                    }
                }
                NewResult = new List<clTuijianhaomalan_info>();
                NewResult = hebing_NewResult.Concat(ClaimReport_Server).ToList();
                #region  //开奖号码 的 假数据
                //clTuijianhaomalan_info iit = new clTuijianhaomalan_info();
                //iit.kaijianghaoma = "11 08 10 06 09";
                //iit.zhongjiangqishu = "180129111";

                //zhongjiangxinxi_Result.Add(iit);
                #endregion

                //计算 中奖信息
                #region MyRegion
                Is_Zhongjiang(BusinessHelp);
                try
                {
                    pbStatus.Value = 3;
                }
                catch
                {

                }

                //保存新的推荐号
                int ss1 = this.tabControl1.SelectedIndex;
                //if (ss1 == 0)
                {
                    if (NewResult.Count > 0)
                        BusinessHelp.inster_tuijanhaoma(NewResult);
                }

                #endregion

                Zhongjiangle_Result = NewResult.FindAll(so => so.zhongjiangqishu != null && so.zhongjiangqishu != "");
                //去除已中奖的条目
                NewResult = NewResult.Except(Zhongjiangle_Result).ToList();
                try
                {
                    pbStatus.Value = 4;
                }
                catch
                {

                }
                #endregion
                JisuanqiResult = new List<clsJisuanqi_info>();
                #region 查询金额
                this.toolStripLabel1.Text = "计算金额....";

                ProcessLogger.Fatal("ReadHistroy 9912" + DateTime.Now.ToString());
                //网页查找的方法 由于新改逻辑 取消网页方法
                //JisuanqiResult = BusinessHelp.ReadHistroy(ref this.bgWorker, selectitem, Zhongjiangle_Result);

                #region    //二期后 改成直接固定值

                foreach (clTuijianhaomalan_info ITEM in Zhongjiangle_Result)
                {
                    clsJisuanqi_info item = new clsJisuanqi_info();
                    var qushutxt = Convert.ToDouble(ITEM.zhongjiangqishu.Substring(6, ITEM.zhongjiangqishu.Length - 6)) - Convert.ToDouble(ITEM.dangriqihao.Substring(6, ITEM.dangriqihao.Length - 6)) + 1;
                    qushutxt = Math.Abs(Convert.ToDouble(qushutxt));
                    string chaoqishu = "";
                    BusinessHelp.ITEM = ITEM;
                    string fa = Convert.ToInt32(qushutxt.ToString()).ToString().Replace("-", "").ToString();
                    chaoqishu = BusinessHelp.chaxun_chaoqishu(chaoqishu);

                    if (chaoqishu != "" && Convert.ToInt32(chaoqishu) < Convert.ToInt32(fa.ToString()))
                    {
                        //  continue;
                    }
                    item.qishu = fa;
                    ITEM.fanganqishu = item.qishu;

                    item.tourubeishu = "1";
                    //item.benqitouru = pingjunyilouLocation.innerText;
                    //item.leijitouru = zuidayilouLocation.innerText;
                    item.benqishouyi = ITEM.zhongjiangjine;
                    //item.yilishouyi = disiyilouLocation.innerText;

                    //item.shouyilv = disanyilouLocation.innerText;

                    item.dangriqihao = ITEM.dangriqihao;
                    item.tuijianhaoma = ITEM.tuijianhaoma;
                    item.wanfazhonglei = ITEM.wanfazhonglei;
                    item.zhongjiangqishu = ITEM.zhongjiangqishu;
                    item.fanganqishu = ITEM.fanganqishu;
                    item.Input_Date = DateTime.Now.ToString("yyyy/MM/dd");
                    JisuanqiResult.Add(item);

                }
                #endregion

                if (JisuanqiResult != null && JisuanqiResult.Count > 0)
                    BusinessHelp.inster_lishizongjianglan(JisuanqiResult);
                //JisuanqiResult = JisuanqiResult.Concat(Jisuanqi_Server).ToList();
                //刷新读取数据库数据
                JisuanqiResult = BusinessHelp.ReadServer_lishizhongjiang(conditions);
                JisuanqiResult = JisuanqiResult.Where((x, ii) => JisuanqiResult.FindIndex(z => z.wanfazhonglei == x.wanfazhonglei && z.tuijianhaoma == x.tuijianhaoma) == ii).ToList();//Lambda表达式去重  

                #endregion
                #region 二次确认是否将中奖划拨过去

                #endregion
                Showdave2(JisuanqiResult);
                Showdave(Result);
                Showdave3(JisuanqiResult, Result);

                int ss = this.tabControl1.SelectedIndex;

                if (ss == 0 && NewResult != null)
                    this.toolStripLabel1.Text = NewResult.Count.ToString() + "  刷新结束，请查看～";
                try
                {
                    pbStatus.Value = 5;
                }
                catch
                {

                }
                //pbStatus.Visible = false;

            }
            catch (Exception ex)
            {
                ProcessLogger.Fatal("EX987" + ex + DateTime.Now.ToString());

                // MessageBox.Show("错误：201" + ex);
                return;

                throw;
            }
        }

        private void Is_Zhongjiang(clsAllnew BusinessHelp)
        {
            foreach (clTuijianhaomalan_info item in NewResult)
            {
                if (item.tuijianhaoma.Contains("04 06 08 09"))
                {


                }
                //查找中奖的期数
                string[] tatile1 = System.Text.RegularExpressions.Regex.Split(item.tuijianhaoma, " ");
                if (zhongjiangxinxi_Result != null)
                {
                    List<clTuijianhaomalan_info> filter_zhongjiangxinxi = zhongjiangxinxi_Result.FindAll(so => so.zhongjiangqishu != null && Convert.ToDouble(so.zhongjiangqishu) > Convert.ToDouble(item.dangriqihao));
                    filter_zhongjiangxinxi = filter_zhongjiangxinxi.OrderBy(o => o.zhongjiangqishu).ToList();

                    bool qianis_ture = false;
                    if (filter_zhongjiangxinxi != null && filter_zhongjiangxinxi.Count > 0)
                    {
                        foreach (clTuijianhaomalan_info temp in filter_zhongjiangxinxi)
                        {
                            //刨除  不是前的 玩法  只算包含的的次数
                            int time = 0;
                            if (!item.wanfazhonglei.Contains("前") && !item.wanfazhonglei.Contains("乐"))
                            {
                                for (int iq = 0; iq < tatile1.Length; iq++)
                                {
                                    if (temp.kaijianghaoma.Contains(tatile1[iq]))//10 07 02 04 03   01 02
                                        time++;
                                }
                                //判断是否达到中奖次数
                                if (item.wanfazhonglei == "任八遗漏" && time == 5)
                                    qianis_ture = true;
                                else if (item.wanfazhonglei == "任七遗漏" && time == 5)
                                    qianis_ture = true;
                                else if (item.wanfazhonglei == "任六遗漏" && time == 5)
                                    qianis_ture = true;
                                else if (item.wanfazhonglei == "任五遗漏" && time == 5)
                                    qianis_ture = true;
                                else if (item.wanfazhonglei == "任四遗漏" && time == 4)
                                    qianis_ture = true;
                                else if (item.wanfazhonglei == "任三遗漏" && time == 3)
                                    qianis_ture = true;
                                else if (item.wanfazhonglei == "任二遗漏" && time == 2)
                                    qianis_ture = true;
                                else if (item.wanfazhonglei == "任一遗漏" && time == 1)
                                    qianis_ture = true;
                            }

                            #region   //计算前一直 和前一组的 算法
                            else if (item.wanfazhonglei.Contains("前"))
                            {
                                //前一直
                                if (item.wanfazhonglei.Contains("直") || item.wanfazhonglei.Contains("遗漏")) //前一遗漏
                                {
                                    //前一直
                                    if (item.wanfazhonglei.Contains("一"))
                                    {
                                        if (temp.kaijianghaoma != null && temp.kaijianghaoma != "" && item.wanfazhonglei != null && item.wanfazhonglei != "" && temp.kaijianghaoma.Substring(0, 2) == item.tuijianhaoma.Substring(0, 2))
                                        {
                                            time++;
                                            qianis_ture = true;

                                        }
                                    }
                                    //前二直
                                    else if (item.wanfazhonglei.Contains("二"))
                                    {
                                        if (temp.kaijianghaoma != null && temp.kaijianghaoma != "" && item.wanfazhonglei != null && item.wanfazhonglei != "" && temp.kaijianghaoma.Substring(0, 5) == item.tuijianhaoma.Substring(0, 5))
                                        {
                                            time++;
                                            qianis_ture = true;

                                        }
                                    }
                                    //前三直
                                    else if (item.wanfazhonglei.Contains("三"))
                                    {
                                        if (temp.kaijianghaoma != null && temp.kaijianghaoma != "" && item.wanfazhonglei != null && item.wanfazhonglei != "" && temp.kaijianghaoma.Substring(0, 8) == item.tuijianhaoma.Substring(0, 8))
                                        {
                                            time++;
                                            qianis_ture = true;

                                        }
                                    }

                                }
                                //前一组 只要 前几位对应有此数即可
                                else if (item.wanfazhonglei.Contains("组"))
                                {
                                    //前一组
                                    if (item.wanfazhonglei.Contains("一"))
                                    {
                                        if (temp.kaijianghaoma != null && temp.kaijianghaoma != "" && item.wanfazhonglei != null && item.wanfazhonglei != "" && temp.kaijianghaoma.Substring(0, 2) == item.tuijianhaoma.Substring(0, 2))
                                        {
                                            // && temp.wanfazhonglei.Substring(0, 5) == item.wanfazhonglei.Substring(0, 5)

                                            time++;
                                            qianis_ture = true;

                                        }
                                    }
                                    else if (item.wanfazhonglei.Contains("二"))
                                    {
                                        if (temp.kaijianghaoma != null && temp.kaijianghaoma != "" && item.wanfazhonglei != null && item.wanfazhonglei != "")
                                        {

                                            string[] splittemp = System.Text.RegularExpressions.Regex.Split(temp.kaijianghaoma, " ");
                                            for (int iq = 0; iq < tatile1.Length; iq++)
                                            {
                                                for (int iq1 = 0; iq1 < 2; iq1++)
                                                {
                                                    if (splittemp[iq1].Contains(tatile1[iq]))
                                                        time++;
                                                }
                                            }

                                            if (time == 2)
                                                qianis_ture = true;

                                        }
                                    }
                                    else if (item.wanfazhonglei.Contains("三"))
                                    {
                                        if (temp.kaijianghaoma != null && temp.kaijianghaoma != "" && item.wanfazhonglei != null && item.wanfazhonglei != "")
                                        {

                                            string[] splittemp = System.Text.RegularExpressions.Regex.Split(temp.kaijianghaoma, " ");
                                            for (int iq = 0; iq < tatile1.Length; iq++)
                                            {
                                                for (int iq1 = 0; iq1 < 3; iq1++)
                                                {
                                                    if (splittemp[iq1].Contains(tatile1[iq]))
                                                        time++;
                                                }
                                            }

                                            if (time == 3)
                                                qianis_ture = true;

                                        }
                                    }
                                }

                            }
                            #endregion

                            #region  //乐四 乐五 玩法的 中奖金额计算


                            //乐选四中奖条件：5个中奖号码中，乐选四号码中包含3个中19元，包含4个中154元。乐选五中奖条件：
                            //5个中奖号码中，乐选五号码中包含4个中90元，包含5个中1080元。中奖金额用红色标记。   
                            else if (item.wanfazhonglei.Contains("乐"))
                            {

                                if (item.wanfazhonglei.Contains("乐四"))
                                {
                                    if (temp.kaijianghaoma != null && temp.kaijianghaoma != "" && item.wanfazhonglei != null && item.wanfazhonglei != "")
                                    {
                                        string[] splittemp = System.Text.RegularExpressions.Regex.Split(temp.kaijianghaoma.Trim(), " ");
                                        for (int iq = 0; iq < tatile1.Length; iq++)
                                        {
                                            for (int iq1 = 0; iq1 < 5; iq1++)
                                            {
                                                if (splittemp[iq1].Contains(tatile1[iq]))
                                                    time++;
                                            }
                                        }

                                        if (time >= 3)
                                        {
                                            qianis_ture = true;
                                        }
                                    }
                                }
                                else if (item.wanfazhonglei.Contains("乐五"))
                                {
                                    if (temp.kaijianghaoma != null && temp.kaijianghaoma != "" && item.wanfazhonglei != null && item.wanfazhonglei != "")
                                    {
                                        string[] splittemp = System.Text.RegularExpressions.Regex.Split(temp.kaijianghaoma.Trim(), " ");
                                        for (int iq = 0; iq < tatile1.Length; iq++)
                                        {
                                            for (int iq1 = 0; iq1 < 5; iq1++)
                                            {
                                                if (splittemp[iq1].Contains(tatile1[iq]))
                                                    time++;
                                            }
                                        }

                                        if (time >= 4)
                                        {
                                            qianis_ture = true;
                                        }

                                    }
                                }
                            }
                            #endregion

                            //如果中奖标记期数
                            if (time == tatile1.Length || qianis_ture == true)
                            {
                                item.zhongjiangqishu = temp.zhongjiangqishu;
                                string inputJiangjin = "";
                                BusinessHelp.ITEM = item;
                                inputJiangjin = BusinessHelp.chaxunjiangjin(inputJiangjin, time);
                                item.zhongjiangjine = inputJiangjin;

                                break;
                            }
                        }
                    }
                }
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            get_combox();
            if (selectitem[0] == "")
            {
                MessageBox.Show("请选择玩法,再次尝试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            if (checkBox1.Checked == true && timers_resfresh == true)
            {

                this.toolStripLabel1.Text = "自动获取中,无需任何操作...";

                if (toolStripButton5.Text == "全自动")
                {
                    toolStripButton5.Text = "已自动";
                    this.filterButton.Enabled = false;

                    Control.CheckForIllegalCrossThreadCalls = false;
                    int jiange_time = selectitem.Count * 60000;

                    aTimer = new System.Timers.Timer(60000);//300000
                    aTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimeControl);
                    aTimer.AutoReset = true;
                    aTimer.Start();

                    toolStripButton5.BackColor = Color.Red;
                }
                else
                {
                    toolStripButton5.Text = "全自动";
                    toolStripButton5.BackColor = Color.Green;

                    aTimer.Stop();

                }


            }
            else
            {
                this.filterButton.Enabled = true;
                toolStripButton5.Text = "全自动";
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

                //NewMethod();
                Main();
                if (arg.CurrentIndex % 5 == 0)
                {
                    backgroundWorker2.ReportProgress(progress, arg);
                }

                backgroundWorker2.ReportProgress(100, arg);
                e.Result = string.Format("{0} " + DateTime.Now.ToString("HH:mm:ss"), changeindex.Count);

            }
            catch (Exception ex)
            {
                if (!e.Cancel)
                {
                    //arg.HasError = true;
                    //arg.ErrorMessage = exception.Message;
                    ProcessLogger.Fatal("EX89891" + ex + DateTime.Now.ToString());

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
            if (selectitem[0] == "")
            {
                MessageBox.Show("请选择玩法,再次尝试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            NewMethod();
            //   List<clTuijianhaomalan_info> Find_JisuanqiResult2 = NewResult.FindAll(so => so.zhongjiangqishu != null && so.zhongjiangqishu != "");
            if (Zhongjiangle_Result == null && Zhongjiangle_Result.Count == 0)
                return;
            //
            JisuanqiResult = new List<clsJisuanqi_info>();


            this.pbStatus.Visible = true;
            clsAllnew BusinessHelp = new clsAllnew();
            BusinessHelp.pbStatus = pbStatus;
            BusinessHelp.tsStatusLabel1 = toolStripLabel1;
            JisuanqiResult = BusinessHelp.ReadHistroy(ref this.bgWorker, selectitem, Zhongjiangle_Result);
            Showdave2(JisuanqiResult);

        }

        private void Showdave(List<clTuijianhaomalan_info> Result)
        {
            try
            {
                //50 = 60 - 10;

                //  NewResult = NewResult.Skip(NewResult.Count-50).ToList();

                sortableList = new SortableBindingList<clTuijianhaomalan_info>(NewResult);
                this.bindingSource1.DataSource = this.sortableList;
                bindingSource1.Sort = "wanfazhonglei ASC";
                dataGridView.AutoGenerateColumns = false;
                dataGridView.DataSource = this.bindingSource1;

                //new  e二期
                //bindingSource1.Sort = "wanfazhonglei ASC";

                //dataGridView4.AutoGenerateColumns = false;
                //dataGridView4.DataSource = this.bindingSource1;


                //Result = Result.Skip(Result.Count - 50).ToList();

                sortableList = new SortableBindingList<clTuijianhaomalan_info>(Result);
                this.bindingSource2.DataSource = this.sortableList;
                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.DataSource = this.bindingSource2;
                List<string> quchongnashuidanwei = (from v in Result select v.wanfazhonglei).Distinct().ToList();
                if (quchongnashuidanwei.Count > 0)
                {
                    comboBox2.DataSource = quchongnashuidanwei;
                    comboBox2.SelectedIndex = 0;
                }

                timers_resfresh = true;
                //开奖信息
                if (zhongjiangxinxi_Result != null)
                {
                    //  zhongjiangxinxi_Result = zhongjiangxinxi_Result.Skip(zhongjiangxinxi_Result.Count - 50).ToList();

                    sortableList_zhongjiangxinxi = new SortableBindingList<clTuijianhaomalan_info>(zhongjiangxinxi_Result);
                    this.bindingSource4.DataSource = this.sortableList_zhongjiangxinxi;
                    dataGridView3.AutoGenerateColumns = false;
                    dataGridView3.DataSource = this.bindingSource4;

                }
            }
            catch (Exception ex)
            {
                ProcessLogger.Fatal("Ex6688" + ex + DateTime.Now.ToString());
                return;

                throw;
            }
        }


        private void Showdave2(List<clsJisuanqi_info> Result)
        {
            try
            {
                //取出最后50数据

                //JisuanqiResult = JisuanqiResult.Skip(JisuanqiResult.Count- 50).ToList();
                if (JisuanqiResult != null && JisuanqiResult.Count > 0)
                {
                    sortableJisuanqiList = new SortableBindingList<clsJisuanqi_info>(JisuanqiResult);
                    this.bindingSource3.DataSource = this.sortableJisuanqiList;
                    dataGridView2.AutoGenerateColumns = false;

                    bindingSource3.Sort = "wanfazhonglei ASC";

                    dataGridView2.DataSource = this.bindingSource3;

                    //new 
                    //子线程中

                    //this.Invoke(new InvokeHandler(delegate()
                    //{

                    //    dataGridView5.DataSource = null;
                    //    dataGridView5.AutoGenerateColumns = false;

                    //    dataGridView5.DataSource = bindingSource3;

                    //}));


                    // this.pbStatus.Visible = false;

                    List<clsJisuanqi_info> JisuanqiResultcombox = JisuanqiResult.FindAll(s => s.wanfazhonglei != null && s.wanfazhonglei != "");


                    List<string> quchongnashuidanwei = (from v in JisuanqiResultcombox select v.wanfazhonglei).Distinct().ToList();
                    if (quchongnashuidanwei != null && quchongnashuidanwei.Count >= 0)
                    {

                        quchongnashuidanwei.Add("所有");
                        comboBox1.DataSource = quchongnashuidanwei;
                        comboBox1.SelectedIndex = quchongnashuidanwei.Count - 1;
                    }
                    if (sortableJisuanqiList != null)
                        this.toolStripLabel1.Text = sortableJisuanqiList.Count + " -刷新结束，请查看～";
                }
            }
            catch (Exception ex)
            {
                ProcessLogger.Fatal("Ex7799" + ex + DateTime.Now.ToString());

                return;

                throw;
            }
        }


        private void Showdave3(List<clsJisuanqi_info> Result, List<clTuijianhaomalan_info> Dav4Result)
        {
            try
            {
                if (JisuanqiResult != null && JisuanqiResult.Count > 0)
                {
                    sortableJisuanqiList_dav5 = new SortableBindingList<clsJisuanqi_info>(JisuanqiResult);
                    this.bindingSource5.DataSource = this.sortableJisuanqiList_dav5;

                    bindingSource5.Sort = "wanfazhonglei ASC";
                    //new 
                    //子线程中
                    //this.Invoke(new InvokeHandler(delegate()
                    //{

                    dataGridView5.DataSource = null;
                    dataGridView5.AutoGenerateColumns = false;
                    dataGridView5.DataSource = bindingSource5;

                    //}));

                }
                if (NewResult != null && NewResult.Count > 0)
                {
                    sortableList_dav4 = new SortableBindingList<clTuijianhaomalan_info>(NewResult);
                    this.bindingSource6.DataSource = this.sortableList_dav4;
                    //new  e二期
                    bindingSource6.Sort = "wanfazhonglei ASC";
                    //this.Invoke(new InvokeHandler(delegate()
                    //{
                    dataGridView4.DataSource = null;
                    dataGridView4.AutoGenerateColumns = false;
                    dataGridView4.DataSource = bindingSource6;

                    //}));

                }
            }
            catch (Exception ex)
            {
                ProcessLogger.Fatal("Ex7799" + ex + DateTime.Now.ToString());

                return;

                throw;
            }
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
                if (dataTable != null)
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
                if (dataTable != null)

                    this.toolStripLabel1.Text = dataTable.Rows.Count + " -刷新结束，请查看～";

            }
            if (s == 1)
            {
                if (sql == null || sql == "")
                    conditions = "select * from lishizongjiang";//成功
                else
                    conditions = "select * from lishizongjiang where dangriqihao like '" + sql;//成功
                JisuanqiResult = new List<clsJisuanqi_info>();


                JisuanqiResult = BusinessHelp.ReadServer_lishizhongjiang(conditions);
                Showdave2(JisuanqiResult);

                //DataTable dataTable = BusinessHelp.read_lishizhongjiang(conditions);
                ////dataGridView1.AutoGenerateColumns = true;
                //dataGridView2.DataSource = dataTable;
                //label1.Text = dataTable.Rows.Count.ToString();
                //this.toolStripLabel1.Text = dataTable.Rows.Count + " -刷新结束，请查看～";

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
                if (dataTable != null)

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
            else if (s == 4)
            {
                downEXCEL(dataGridView4);
                downEXCEL(dataGridView5);
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
                if (dataGridView != null)
                    this.toolStripLabel1.Text = dataGridView.RowCount + " -条";
            }
            else if (s == 1)
            {
                if (dataGridView2 != null)

                    this.toolStripLabel1.Text = dataGridView2.RowCount + " -条";
            }
            else if (s == 2)
            {
                if (dataGridView1 != null)

                    this.toolStripLabel1.Text = dataGridView1.RowCount + " -条";
            }
            else if (s == 3)
            {
                if (dataGridView3 != null)

                    this.toolStripLabel1.Text = dataGridView3.RowCount + " -条";
            }
            else if (s == 4)
            {

            }

        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            //   ComboBox cb = sender as ComboBox;
            //   Graphics g = e.Graphics;
            //   Pen p = new Pen(Color.Blue, 1);
            //   g.DrawRectangle(p, e.Bounds.X + 1, e.Bounds.Y + 1, 12, 12);
            //   if (e.Index == cb.SelectedIndex)
            //       g.DrawString("√", new Font(FontFamily.GenericSerif, 10), Brushes.Red,
            //e.Bounds.Location, StringFormat.GenericDefault);
            //   g.DrawString(cb.GetItemText(cb.Items[e.Index]), new Font(FontFamily.GenericSerif, 9),
            //       Brushes.Black, 15, e.Bounds.Y + 1, StringFormat.GenericDefault);
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
                ProcessLogger.Fatal("Ex9989" + ex + DateTime.Now.ToString());

                MessageBox.Show("错误" + ex);

                return;

                throw;
            }

        }

        private void toolStripButton4_Click_1(object sender, EventArgs e)
        {
            Application.Exit();


        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            mapping_Result = new List<clszhongleiDuiyingQishu_info>();
            string path1 = AppDomain.CurrentDomain.BaseDirectory + "System\\confing.txt";

            //string[] fileText1 = File.ReadAllLines(path1);
            //string ZFCEPath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System"), "");
            System.Diagnostics.Process.Start("explorer.exe", path1);

        }

        private void checkBoxComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = checkBoxComboBox1.SelectedIndex;

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {

            if (toolStripButton5.Text == "全自动")
            {
                Control.CheckForIllegalCrossThreadCalls = false;

                aTimer = new System.Timers.Timer(30000);
                aTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimeControl);
                aTimer.AutoReset = true;
                aTimer.Start();
                toolStripButton5.Text = "已自动";

            }
            else
            {
                toolStripButton5.Text = "全自动";
                aTimer.Stop();

            }

        }
        private void TimeControl(object sender, EventArgs e)
        {
            DateTime rq2 = DateTime.Now;  //结束时间

            TimeSpan ts = rq2 - StopTime;
            int timeTotal = ts.Minutes;

            if (timeTotal >= 3)
            {
            }
            if (!IsRun && timeTotal >= 3)
            {
                IsRun = true;
                GetDataforRawDataThread = new Thread(Main);
                GetDataforRawDataThread.SetApartmentState(ApartmentState.STA);

                GetDataforRawDataThread.Start();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<clTuijianhaomalan_info> FilterResult = Result.FindAll(s => s.wanfazhonglei != null && s.wanfazhonglei.Contains(comboBox2.Text));

            //bindingSource2.Filter = "wanfazhonglei = '" + comboBox2.Text + "'";

            sortableList = new SortableBindingList<clTuijianhaomalan_info>(FilterResult);
            this.bindingSource2.DataSource = this.sortableList;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = this.bindingSource2;
            if (bindingSource2 != null)
                this.toolStripLabel1.Text = bindingSource2.Count + " -刷新结束，请查看～";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (JisuanqiResult != null)
            {
                List<clsJisuanqi_info> FilterResult = JisuanqiResult.FindAll(s => s.wanfazhonglei != null && s.wanfazhonglei.Contains(comboBox1.Text));
                if (comboBox1.Text == "所有")
                {
                    FilterResult = JisuanqiResult;

                }
                sortableJisuanqiList = new SortableBindingList<clsJisuanqi_info>(FilterResult);
                this.bindingSource3.DataSource = this.sortableJisuanqiList;
                dataGridView2.AutoGenerateColumns = false;
                dataGridView2.DataSource = this.bindingSource3;
                if (sortableJisuanqiList != null)
                    this.toolStripLabel1.Text = sortableJisuanqiList.Count + " -刷新结束，请查看～";
            }
        }
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clsAllnew BusinessHelp = new clsAllnew();
            //var oids = GetOrderIdsBySelectedGridCell();
            string dangriqihao = Convert.ToString(dataGridView.Rows[RowRemark].Cells["dangriqihao"].Value);
            string wanfazhonglei = Convert.ToString(dataGridView.Rows[RowRemark].Cells["wanfazhonglei"].Value);

            string dele_sql = "delete FROM tuijanhaoma  WHERE dangriqihao ='" + dangriqihao + "'And wanfazhonglei='" + wanfazhonglei + "'";

            BusinessHelp.delete(dele_sql);
            this.toolStripLabel1.Text = " -删除成功！";
        }
        private List<long> GetOrderIdsBySelectedGridCell()
        {

            List<long> order_ids = new List<long>();
            var rows = GetSelectedRowsBySelectedCells(dataGridView1);
            foreach (DataGridViewRow row in rows)
            {
                var Diningorder = row.DataBoundItem as clTuijianhaomalan_info;
                //order_ids.Add((long)Diningorder.dangriqihao);
            }

            return order_ids;
        }
        private IEnumerable<DataGridViewRow> GetSelectedRowsBySelectedCells(DataGridView dgv)
        {
            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            foreach (DataGridViewCell cell in dgv.SelectedCells)
            {
                rows.Add(cell.OwningRow);
                clsAllnew BusinessHelp = new clsAllnew();


            }
            rowcount = dgv.SelectedCells.Count;

            return rows.Distinct();
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            RowRemark = e.RowIndex;
            cloumn = e.ColumnIndex;
        }

        private void dataGridView4_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //SolidBrush b = new SolidBrush(this.dataGridView4.RowHeadersDefaultCellStyle.ForeColor);
            //e.Graphics.DrawString((e.RowIndex + 1).ToString(System.Globalization.CultureInfo.CurrentUICulture), this.dataGridView4.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);

            if (dataGridView4.RowCount > 0)
                foreach (DataGridViewRow row in dataGridView4.Rows)
                {
                    row.Cells[0].Value = row.Index + 1;
                }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsAllnew BusinessHelp = new clsAllnew();

            BusinessHelp.saveUserAndPassword(toolStripComboBox1.Text);

        }

        private void button1_Click_2(object sender, EventArgs e)
        {

            //任选二, 任选三, 任选四, 任选五, 任选六, 任选七, 任选八, 前一直, 前二直, 前三直, 前二组, 前三组, 乐选四, 乐选五

            checkBoxComboBox1.SelectAll();


            //.CheckBoxProperties.AutoCheck=true;

            if (button1.Text == "全选")
            {
                for (int i = 0; i < checkBoxComboBox1.Items.Count; i++)
                {
                    if (i == 0)
                        checkBoxComboBox1.Text = checkBoxComboBox1.Items[i].ToString();
                    else
                        checkBoxComboBox1.Text = checkBoxComboBox1.Text + ", " + checkBoxComboBox1.Items[i].ToString();

                }
                button1.Text = "清空";
            }
            else
            {
                checkBoxComboBox1.Text = "";
                button1.Text = "全选";
            }
        }

        private void dataGridView4_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            bool handle;
            if (dataGridView4.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.Equals(DBNull.Value))
            {
                handle = true;
            }
            else
                handle = false;
            e.Cancel = handle;

        }

        private void dataGridView5_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            bool handle;
            if (dataGridView5.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.Equals(DBNull.Value))
            {
                handle = true;
            }
            else
                handle = false;
            e.Cancel = handle;

        }

        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            bool handle;
            if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.Equals(DBNull.Value))
            {
                handle = true;
            }
            else
                handle = false;
            e.Cancel = handle;
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            bool handle;
            if (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.Equals(DBNull.Value))
            {
                handle = true;
            }
            else
                handle = false;
            e.Cancel = handle;
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            bool handle;
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.Equals(DBNull.Value))
            {
                handle = true;
            }
            else
                handle = false;
            e.Cancel = handle;
        }

        private void dataGridView3_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            bool handle;
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.Equals(DBNull.Value))
            {
                handle = true;
            }
            else
                handle = false;
            e.Cancel = handle;
        }

        private void dataGridView5_Paint(object sender, PaintEventArgs e)
        {
            //DataGridViewForWs.OnPaint(e);
            OnPaint(e);
        }

        private void dataGridView4_Paint(object sender, PaintEventArgs e)
        {
            OnPaint(e);

        }

        private void dataGridView_Paint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
        }

        private void dataGridView2_Paint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
        }

        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
        }

        private void dataGridView3_Paint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
        }
    }

    public class DataGridViewForWs : DataGridView
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
            }
            catch
            {

                Invalidate();
            }
        }
    }

}

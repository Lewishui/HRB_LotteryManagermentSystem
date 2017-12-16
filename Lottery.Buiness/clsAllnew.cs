using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using ISR_System;
using Lottery.DB;
using mshtml;
using Order.Common;

namespace Lottery.Buiness
{
    public enum ProcessStatus
    {
        初始化,
        登录界面,
        确认YES,
        第一页面,
        第二页面,
        Filter下拉,
        关闭页面,
        结束页面

    }
    public enum EapprovalProcessStatus
    {
        初始化,
        Current_Task,
        Task_Queue,
        Search,
        Process
    }
    public class clsAllnew
    {
        private BackgroundWorker bgWorker1;
        //private object missing = System.Reflection.Missing.Value;
        public ToolStripProgressBar pbStatus { get; set; }
        public ToolStripStatusLabel tsStatusLabel1 { get; set; }
        public log4net.ILog ProcessLogger { get; set; }
        public log4net.ILog ExceptionLogger { get; set; }
        private ProcessStatus isrun = ProcessStatus.初始化;
        private EapprovalProcessStatus isrun1 = EapprovalProcessStatus.初始化;
        private bool isOneFinished = false;
        private Form viewForm;
        private WbBlockNewUrl MyWebBrower;
        System.Timers.Timer aTimer = new System.Timers.Timer(100);//实例化Timer类，设置间隔时间为10000毫秒； 
        System.Timers.Timer t = new System.Timers.Timer(1000);//实例化Timer类，设置间隔时间为10000毫秒； 
        private DateTime StopTime;
        WbBlockNewUrl myDoc = null;
        private int login;
        private string dataSource = "Lottery.sqlite";
        string newsth = @"\\9.112.114.167\db\copy\Split\";
        private int aog = 0;
        List<clTuijianhaomalan_info> Tuijianhaomalan_Result;
        List<clTuijianhaomalan_info> Tuijianhaomalan_ResultAll;
        List<clsJisuanqi_info> jisuanqi_Result;
        public List<clTuijianhaomalan_info> zhongjiangxinxi_Result;
        public List<clTuijianhaomalan_info> zhongjiangxinxi_ResultAll;
        string caizhong;
        bool loading;
        clTuijianhaomalan_info ITEM;
        List<clTuijianhaomalan_info> Find_JisuanqiResult2;
        int runtime = 0;

        public clsAllnew()
        {

            newsth = AppDomain.CurrentDomain.BaseDirectory + "" + dataSource;


        }
        public bool read_sqlitefile()
        { 
            bool ishave=false;

            if (File.Exists(newsth))
            {
                ishave = true;

            }
            return ishave;
        }
        public DataTable read()
        {
            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);

            dbConn.Open();

            SQLiteCommand dbCmd = dbConn.CreateCommand();
            DataTable dataTable = Read(dbCmd);
            dbConn.Close();
            return dataTable;

        }
        public DataTable readKaijiang(string sql)
        {
            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);

            dbConn.Open();

            SQLiteCommand dbCmd = dbConn.CreateCommand();
            DataTable dataTable = ReadKaijiang(dbCmd, sql);
            dbConn.Close();
            return dataTable;

        }
        public DataTable read_yuanshizoushitu(string sql)
        {
            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);

            dbConn.Open();

            SQLiteCommand dbCmd = dbConn.CreateCommand();
            DataTable dataTable = Read_yuanshizoushitu(dbCmd,  sql);
            dbConn.Close();
            return dataTable;

        }
        public DataTable read_lishizhongjiang(string sql)
        {
            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);

            dbConn.Open();

            SQLiteCommand dbCmd = dbConn.CreateCommand();
            DataTable dataTable = Read_lishizhongjiang(dbCmd, sql);
            dbConn.Close();
            return dataTable;

        }
        public DataTable read_tuixuanhaomalan(string sql)
        {
            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);

            dbConn.Open();

            SQLiteCommand dbCmd = dbConn.CreateCommand();
            DataTable dataTable = Read_tuijanhaoma(dbCmd, sql);
            dbConn.Close();
            return dataTable;

        }

        private DataTable ReadKaijiang(SQLiteCommand dbCmd, string sql)
        {

            dbCmd.CommandText = sql;// "SELECT * FROM KaijiangInfo";
            //服务器端的
            //string newsth = @"\\9.112.114.167\db\copy\Split\" + dataSource;

            DbDataReader reader = SQLiteHelper.ExecuteReader("Data Source=" + newsth, dbCmd);

            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);
            //SQLiteDataReader dataReader = dbCmd.ExecuteReader();

            DataTable dataTable = new DataTable();
            if (reader.HasRows)
            {

                dataTable.Load(reader);
            }
            return dataTable;

            //dataReader.Close();
        }
        private DataTable Read_yuanshizoushitu(SQLiteCommand dbCmd, string sql)
        {

            //dbCmd.CommandText = "SELECT * FROM yuanshizoushixinxi";
            dbCmd.CommandText = sql;
            DbDataReader reader = SQLiteHelper.ExecuteReader("Data Source=" + newsth, dbCmd);

            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);

            DataTable dataTable = new DataTable();
            if (reader.HasRows)
            {

                dataTable.Load(reader);
            }
            return dataTable;


        }
        private DataTable Read_lishizhongjiang(SQLiteCommand dbCmd, string sql)
        {

            //dbCmd.CommandText = "SELECT * FROM lishizongjiang";
            dbCmd.CommandText = sql;
           
            DbDataReader reader = SQLiteHelper.ExecuteReader("Data Source=" + newsth, dbCmd);

            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);

            DataTable dataTable = new DataTable();
            if (reader.HasRows)
            {

                dataTable.Load(reader);
            }
            return dataTable;


        }
        private DataTable Read_tuijanhaoma(SQLiteCommand dbCmd, string sql)
        {

            //dbCmd.CommandText = "SELECT * FROM tuijanhaoma";
            dbCmd.CommandText = sql;
           
            DbDataReader reader = SQLiteHelper.ExecuteReader("Data Source=" + newsth, dbCmd);

            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);

            DataTable dataTable = new DataTable();
            if (reader.HasRows)
            {

                dataTable.Load(reader);
            }
            return dataTable;




        }
        private DataTable Read(SQLiteCommand dbCmd)
        {

            dbCmd.CommandText = "SELECT * FROM TelephoneBook";
            //服务器端的
            //string newsth = @"\\9.112.114.167\db\copy\Split\" + dataSource;

            DbDataReader reader = SQLiteHelper.ExecuteReader("Data Source=" + newsth, dbCmd);

            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);
            //SQLiteDataReader dataReader = dbCmd.ExecuteReader();

            DataTable dataTable = new DataTable();
            if (reader.HasRows)
            {

                dataTable.Load(reader);
            }
            return dataTable;

            //dataReader.Close();
        }
        public void delete(string sql)
        {
            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);

            SQLiteCommand dbCmd = dbConn.CreateCommand();
            dbCmd.CommandText = "delete FROM TelephoneBook  WHERE telephone ='159'";
            //服务器端的
            dbCmd.CommandText = sql;

            var var = SQLiteHelper.ExecuteNonQuery("Data Source=" + newsth, dbCmd);

           // Read(dbCmd);
        }
        public void inster(List<clTuijianhaomalan_info> zhongjiangxinxi_Result)
        {

            foreach (clTuijianhaomalan_info item in zhongjiangxinxi_Result)
            {

                //const string SQL_UPDATE = @"UPDATE T_LD_CASE_LIST SET ACTION_STATUS = @ACTION_STATUS WHERE ORDER_ID=@ID";

                //dbCmd.CommandText = "UPDATE TelephoneBook SET personID=@personID,telephone=@telephone WHERE telephone='159'";
                //string sql = "INSERT INTO TelephoneBook VALUES('MTB','1589','not mobile')";
                //dbCmd.Parameters.Add("personID", DbType.String).Value = s;
                //dbCmd.Parameters.Add("telephone", DbType.Int32).Value = n;
                string sql = "INSERT INTO KaijiangInfo ( zhongjiangqishu, kaijianghaoma,Input_Date ) " +

                "VALUES (\"" + item.zhongjiangqishu + "\"" +

                       ",\"" + item.kaijianghaoma + "\"" +

                       ",\"" + DateTime.Now + "\")";

               // sql = "CREATE TABLE KaijiangInfo(zhongjiangqishu varchar(20),kaijianghaoma varchar(30),Input_Date varchar(20))";


                int result = SQLiteHelper.ExecuteNonQuery(SQLiteHelper.CONNECTION_STRING_BASE, sql, CommandType.Text, null);
                {

                }
            }
            return;


            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + newsth);
            dbConn.Open();
            instder(dbConn);



            //SQLiteCommand dbCmd = dbConn.CreateCommand();

            //dbCmd.CommandText = "INSERT INTO TelephoneBook VALUES('MTB','159','not mobile')";

            //dbCmd.ExecuteNonQuery();

            dbConn.Close();
            dbConn.Dispose();
        }
        public void inster_yuanshizoushixinxi(List<clTuijianhaomalan_info> zhongjiangxinxi_Result)
        {

            foreach (clTuijianhaomalan_info item in zhongjiangxinxi_Result)
            {

                string sql = "INSERT INTO yuanshizoushixinxi ( haomaileixing, chuxiancishu, pingjunyilou, zuidayilou, diwuyilou, disiyilou, disanyilou, dieryilou, shangciyilou, dangqianyilou, yuchujilv,Input_Date ) " +

                "VALUES (\"" + item.haomaileixing + "\"" +

                       ",\"" + item.chuxiancishu + "\"" +
                                ",\"" + item.pingjunyilou + "\"" +
                                         ",\"" + item.zuidayilou + "\"" +
                                                  ",\"" + item.diwuyilou + "\"" +
                                                           ",\"" + item.disiyilou + "\"" +
                                                                    ",\"" + item.disanyilou + "\"" +
                                                                             ",\"" + item.dieryilou + "\"" +
                                                                                      ",\"" + item.shangciyilou + "\"" +
                                                                                               ",\"" + item.dangqianyilou + "\"" +
                                                                                                   ",\"" + item.yuchujilv + "\"" +

                       ",\"" + DateTime.Now.ToString("yyyy/MM/dd") + "\")";


                // sql = "CREATE TABLE yuanshizoushixinxi(haomaileixing varchar(20),chuxiancishu varchar(30),pingjunyilou varchar(30),zuidayilou varchar(30),diwuyilou varchar(30),disiyilou varchar(30),disanyilou varchar(30),dieryilou varchar(30),shangciyilou varchar(30),dangqianyilou varchar(30),yuchujilv varchar(30),Input_Date varchar(20))";

                int result = SQLiteHelper.ExecuteNonQuery(SQLiteHelper.CONNECTION_STRING_BASE, sql, CommandType.Text, null);
                {

                }
            }
            return;
        }
        public void inster_lishizongjianglan(List<clsJisuanqi_info> zhongjiangxinxi_Result)
        {

            foreach (clsJisuanqi_info item in zhongjiangxinxi_Result)
            {

                string sql = "INSERT INTO lishizongjiang ( wanfazhonglei, tuijianhaoma, dangriqihao, zhongjiangqishu, leijitouru, benqishouyi, yilishouyi,Input_Date ) " +

                "VALUES (\"" + item.wanfazhonglei + "\"" +

                       ",\"" + item.tuijianhaoma + "\"" +
                                ",\"" + item.dangriqihao + "\"" +
                                         ",\"" + item.zhongjiangqishu + "\"" +
                                                  ",\"" + item.leijitouru + "\"" +
                                                           ",\"" + item.benqishouyi + "\"" +
                                                                    ",\"" + item.yilishouyi + "\"" +


                       ",\"" + DateTime.Now.ToString("yyyy/MM/dd") + "\")";


                //sql = "CREATE TABLE lishizongjiang(wanfazhonglei varchar(20),tuijianhaoma varchar(30),dangriqihao varchar(30),zhongjiangqishu varchar(30),leijitouru varchar(30),benqishouyi varchar(30),yilishouyi varchar(30),Input_Date varchar(20))";

                int result = SQLiteHelper.ExecuteNonQuery(SQLiteHelper.CONNECTION_STRING_BASE, sql, CommandType.Text, null);
                {

                }
            }
            return;
        }

        public void inster_tuijanhaoma(List<clTuijianhaomalan_info> zhongjiangxinxi_Result)
        {

            foreach (clTuijianhaomalan_info item in zhongjiangxinxi_Result)
            {

                string sql = "INSERT INTO tuijanhaoma ( wanfazhonglei, tuijianhaoma, nizhuihaoqishu, dangriqihao, zhongjiangqishu,Input_Date ) " +

                "VALUES (\"" + item.wanfazhonglei + "\"" +

                       ",\"" + item.tuijianhaoma + "\"" +
                                ",\"" + item.nizhuihaoqishu + "\"" +
                                         ",\"" + item.dangriqihao + "\"" +
                                                  ",\"" + item.zhongjiangqishu + "\"" +
                       ",\"" + DateTime.Now.ToString("yyyy/MM/dd") + "\")";


                //sql = "CREATE TABLE tuijanhaoma(wanfazhonglei varchar(20),tuijianhaoma varchar(30),nizhuihaoqishu varchar(30),dangriqihao varchar(30),zhongjiangqishu varchar(30),Input_Date varchar(20))";

                int result = SQLiteHelper.ExecuteNonQuery(SQLiteHelper.CONNECTION_STRING_BASE, sql, CommandType.Text, null);
                {

                }
            }
            return;
        }

        private bool instder(SQLiteConnection conn)
        {
            using (DbTransaction dbTrans = conn.BeginTransaction())
            {
                using (DbCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = "INSERT INTO TelephoneBook VALUES('MTB','159','not mobile')";
                    DbParameter Field1 = cmd.CreateParameter();
                    cmd.Parameters.Add(Field1);
                    for (int n = 0; n < 10; n++)
                    {
                        Field1.Value = n + 1000000;
                        cmd.ExecuteNonQuery();
                    }
                }
                dbTrans.Commit();
            }
            return true;

        }

        public List<clTuijianhaomalan_info> ReadWeb_Report(ref BackgroundWorker bgWorker, List<string> selectitem)
        {

            //caizhong = wanfa;

            bgWorker1 = bgWorker;
            try
            {
                Tuijianhaomalan_ResultAll = new List<clTuijianhaomalan_info>();
                zhongjiangxinxi_ResultAll = new List<clTuijianhaomalan_info>();


                isrun = ProcessStatus.初始化;

                #region IE
                tsStatusLabel1.Text = "玩命加载中....";

                Tuijianhaomalan_Result = new List<clTuijianhaomalan_info>();
                {
                    for (int i = 0; i < selectitem.Count; i++)
                    {
                        caizhong = selectitem[i].Replace(",", "");

                        aog = 0;
                        isrun = ProcessStatus.初始化;

                        tsStatusLabel1.Text = "读取中....";
                        ReadWEBAquila();
                        //if (DCN_downloadpath != "")
                        {
                            isrun = ProcessStatus.关闭页面;
                            if (viewForm != null)
                            {
                                MyWebBrower = null;
                                viewForm.Close();
                                // aTimer.Stop();
                            }
                        }
                        Tuijianhaomalan_ResultAll = Tuijianhaomalan_ResultAll.Concat(Tuijianhaomalan_Result).ToList();
                        zhongjiangxinxi_ResultAll = zhongjiangxinxi_ResultAll.Concat(zhongjiangxinxi_Result).ToList();

                    }
                }
                //导入数据库
                tsStatusLabel1.Text = "结束";

                return Tuijianhaomalan_ResultAll;
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                return null;
                throw;
            }
            return null;
        }
        public List<clTuijianhaomalan_info> ReadWEBAquila()
        {
            login = 0;
            try
            {
                tsStatusLabel1.Text = "玩命获取中....";
                isOneFinished = false;
                StopTime = DateTime.Now;
                InitialWebbroswerIE2();

                //var t = new Thread(InitialWebbroswerIE2);
                //t.SetApartmentState(ApartmentState.STA);
                //t.Start();

                //System.Threading.ThreadStart start = new System.Threading.ThreadStart(InitialWebbroswerIE2);
                //System.Threading.Thread th = new System.Threading.Thread(start);
                //th.ApartmentState = System.Threading.ApartmentState.STA;//关键

                while (!isOneFinished)
                {

                    System.Windows.Forms.Application.DoEvents();
                    DateTime rq2 = DateTime.Now;  //结束时间
                    int a = rq2.Second - StopTime.Second;
                    if (a == 30 && isrun == ProcessStatus.Filter下拉)
                    {
                        MyWebBrower = null;
                        viewForm.Close();
                        //MyWebBrower.Refresh();
                        StopTime = DateTime.Now;
                    }
                }
                if (viewForm != null)
                {
                    MyWebBrower = null;
                    viewForm.Close();
                    //aTimer.Stop();
                }
                isOneFinished = false;


                // System.Windows.Forms.Application.DoEvents();
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                throw;
            }
        }
        public void InitialWebbroswerIE2()
        {
            try
            {

                MyWebBrower = new WbBlockNewUrl();
                //不显示弹出错误继续运行框（HP方可）
                MyWebBrower.ScriptErrorsSuppressed = true;
                MyWebBrower.BeforeNewWindow += new EventHandler<WebBrowserExtendedNavigatingEventArgs>(MyWebBrower_BeforeNewWindow2);
                MyWebBrower.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(AnalysisWebInfo2);
                MyWebBrower.Dock = DockStyle.Fill;
                MyWebBrower.IsWebBrowserContextMenuEnabled = true;
                //显示用的窗体
                viewForm = new Form();
                //viewForm.Icon=
                viewForm.ClientSize = new System.Drawing.Size(550, 600);
                viewForm.StartPosition = FormStartPosition.CenterScreen;
                viewForm.Controls.Clear();
                viewForm.Controls.Add(MyWebBrower);
                viewForm.FormClosing += new FormClosingEventHandler(viewForm_FormClosing);
              //  viewForm.Show();
                MyWebBrower.Url = new Uri("http://chart.icaile.com/hlj11x5.php?op=yl3m&num=15");
                if (caizhong.Contains("前一直"))
                {
                    MyWebBrower.Url = new Uri("http://chart.icaile.com/hlj11x5.php?op=q11m");

                }
                else if (caizhong.Contains("前二直"))
                {
                    MyWebBrower.Url = new Uri("http://chart.icaile.com/hlj11x5.php?op=q2zhix");

                }
                else if (caizhong.Contains("前三直"))
                {
                    MyWebBrower.Url = new Uri("http://chart.icaile.com/hlj11x5.php?op=q3zhix");

                }
                else if (caizhong.Contains("前二组"))
                {
                    MyWebBrower.Url = new Uri("http://chart.icaile.com/hlj11x5.php?op=q2zux");

                }
                else if (caizhong.Contains("前三组"))
                {
                    MyWebBrower.Url = new Uri("http://chart.icaile.com/hlj11x5.php?op=q3zux");

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        void MyWebBrower_BeforeNewWindow2(object sender, WebBrowserExtendedNavigatingEventArgs e)
        {
            #region 在原有窗口导航出新页
            //e.Cancel = true;//http://pro.wwpack-crest.hp.com/wwpak.online/regResults.aspx
            //MyWebBrower.Navigate(e.Url);
            #endregion
        }
        private void viewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isrun != ProcessStatus.关闭页面)
            {
                if (MessageBox.Show("正在进行，是否中止?", "CCW", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (MyWebBrower != null)
                    {
                        if (MyWebBrower.IsBusy)
                        {
                            MyWebBrower.Stop();
                        }
                        MyWebBrower.Dispose();
                        MyWebBrower = null;
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        protected void AnalysisWebInfo2(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //  WbBlockNewUrl myDoc = sender as WbBlockNewUrl;

            myDoc = sender as WbBlockNewUrl;
            #region 登录页面
            //http://chart.icaile.com/hlj11x5.php?op=yl3m&num=15
            //点击彩种
            if (myDoc.Url.ToString().IndexOf("http://chart.icaile.com/hlj11x5.php?op") >= 0 && login == 0)//http://chart.icaile.com/hlj11x5.php?op=yl3
            {
                //
                string clickid = "";

                #region MyRegion
                // 任选二
                //任选三
                //任选四
                //任选五
                //任选六
                //任选七
                //任选八
                //前一直
                //前二直
                //前三直
                //前二组
                //前三组
                #endregion
                if (caizhong.Contains("任选二"))
                {
                    clickid = "yl2m";


                }
                else if (caizhong.Contains("任选三"))
                {
                    clickid = "yl3m";
                }
                else if (caizhong.Contains("任选四"))
                {
                    clickid = "yl4m";
                }
                else if (caizhong.Contains("任选五"))
                {
                    clickid = "yl5m";
                }
                else if (caizhong.Contains("任选六"))
                {
                    clickid = "yl6m";
                }
                else if (caizhong.Contains("任选七"))
                {
                    clickid = "yl7m";
                }

                else if (caizhong.Contains("任选八"))
                {
                    clickid = "yl8m";
                }
                else if (caizhong.Contains("任一遗漏"))
                {
                    clickid = "hmyl";
                    //
                }
                else if (caizhong.Contains("前一直") || caizhong.Contains("前二直") || caizhong.Contains("前三直") || caizhong.Contains("前二组") || caizhong.Contains("前三组"))
                {
                    //clickid = "q11m";
                    //MyWebBrower.Navigate("http://chart.icaile.com/hlj11x5.php?op=q11m");
                    isrun = ProcessStatus.登录界面;
                    login++;
                    return;

                }


                HtmlElementCollection atab = myDoc.Document.GetElementsByTagName("a");
                foreach (HtmlElement item in atab)
                {
                    if (item.OuterHtml.Contains(clickid) && item.OuterHtml.Contains("<A href=\"?op=") && item.OuterHtml.Contains("</B></A>"))
                    {
                        item.InvokeMember("Click");
                    }
                }

                isrun = ProcessStatus.登录界面;
                login++;

            }
            else if (myDoc.Url.ToString().IndexOf("http://chart.icaile.com/hlj11x5.php?op") >= 0 && isrun == ProcessStatus.登录界面)//http://chart.icaile.com/hlj11x5.php?op=yl3
            {
                if (login < 2)
                {
                    login++;
                    return;

                }
          
                //  return;

                Tuijianhaomalan_Result = new List<clTuijianhaomalan_info>();

                HtmlElement userName = null;
                HtmlElement password = null;
                HtmlElement submit = null;
                HtmlElement lan = null;
                HtmlElementCollection body_tdlist;
                HtmlElementCollection body_trlist;
                HtmlElement userNames = myDoc.Document.GetElementById("tablesort");
                HtmlElementCollection tab = myDoc.Document.GetElementsByTagName("table");

                if (userNames.OuterText != null && userNames.OuterText.Contains("号码类型"))
                {
                    #region 直接循环取值 比较耗时
                    //body_tdlist = userNames.Document.GetElementsByTagName("tr");
                    ////foreach (HtmlElement temp in body_tdlist)
                    //{

                    //    HtmlElementCollection trlist = userNames.Document.GetElementsByTagName("tr");//获取本层TR
                    //    foreach (HtmlElement item1 in trlist)
                    //    {
                    //        bool ischina = clsCommHelp.HasChineseTest(item1.OuterText.ToString());
                    //        if (ischina == true)
                    //            continue;

                    //        clTuijianhaomalan_info item = new clTuijianhaomalan_info();

                    //        //if (item1.InnerHtml.Contains("号码类型"))
                    //        {
                    //            //string[] tatile = System.Text.RegularExpressions.Regex.Split(item1.InnerHtml, "</TH>");


                    //        }
                    //        HtmlElementCollection tdlist = item1.Document.GetElementsByTagName("td");//循环本tr 下的所有TD取得 号码
                    //        int i = 0;
                    //        foreach (HtmlElement item2 in tdlist)
                    //        {
                    //            {
                    //                ischina = clsCommHelp.HasChineseTest(item2.OuterText.ToString());
                    //                if (ischina == false)
                    //                {
                    //                    ischina = clsCommHelp.HasChineseTest(item2.OuterText.ToString());
                    //                    if (ischina == false)
                    //                    {
                    //                        i++;
                    //                        if (i == 1)
                    //                            item.haomaileixing = item2.OuterText;
                    //                        else if (i == 2)
                    //                            item.chuxiancishu = item2.OuterText;
                    //                        else if (i == 3)
                    //                            item.pingjunyilou = item2.OuterText;
                    //                        else if (i == 4)
                    //                            item.zuidayilou = item2.OuterText;
                    //                        else if (i == 5)
                    //                            item.diwuyilou = item2.OuterText;
                    //                        else if (i == 6)
                    //                            item.disiyilou = item2.OuterText;
                    //                        else if (i == 7)
                    //                            item.disanyilou = item2.OuterText;
                    //                        else if (i == 8)
                    //                            item.dieryilou = item2.OuterText;
                    //                        else if (i == 9)
                    //                            item.shangciyilou = item2.OuterText;
                    //                        else if (i == 10)
                    //                            item.dangqianyilou = item2.OuterText;
                    //                        else if (i == 11)
                    //                        {
                    //                            item.yuchujilv = item2.OuterText;
                    //                            break;

                    //                        }
                    //                        userName = item1;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        Tuijianhaomalan_Result.Add(item);
                    //    }


                    //} 
                    #endregion

                    //获取玩法的种类

                    string wanfazhonglei = "";
                    string[] tatile = System.Text.RegularExpressions.Regex.Split(myDoc.Url.ToString(), "=");


                    HtmlElementCollection activetxt = myDoc.Document.GetElementsByTagName("li");
                    if (tatile[1].Contains("&"))
                    {
                        tatile = System.Text.RegularExpressions.Regex.Split(tatile[1].ToString(), "&");

                    }
                    HtmlElement parent_li = myDoc.Document.GetElementById(tatile[1]);//.Substring(0, 4)
                    if (parent_li != null && parent_li.InnerText != null)
                        wanfazhonglei = parent_li.InnerText.Replace("[多码遗漏]", "").Trim();
                    if (wanfazhonglei == "[常规遗漏] 直选遗漏" || wanfazhonglei == "组选遗漏")
                        wanfazhonglei = caizhong;
          
                    //利用 HTMLTable 抓取信息
                    try
                    {

                        IHTMLDocument2 doc = (IHTMLDocument2)MyWebBrower.Document.DomDocument;
                        HTMLDocument myDoc1 = doc as HTMLDocument;
                        //IHTMLElement doc1 = (IHTMLElement)MyWebBrower.Document.DomDocument;
                        IHTMLElementCollection Tablelin = myDoc1.getElementsByTagName("table");
                        foreach (IHTMLElement items in Tablelin)
                        {

                            if (items.outerText != null && items.outerText.Contains("号码类型"))
                            {

                                HTMLTable materialTable = items as HTMLTable;
                                IHTMLElementCollection rows = materialTable.rows as IHTMLElementCollection;
                                int KeyInfoRowIndex = 1;
                                int KeyInfoCellCIDIndex = 1, KeyInfoCellPN = 2, KeyInfoCellLocation = 3, KeyInfoCellDataSource = 4, KeyInfoCellOrder = 5, KeyInfoCell_haomaileixing = 0, KeyInfoCell_disanyilou = 6, KeyInfoCell_dieryilou = 7, KeyInfoCell_shangciyilou = 8, KeyInfoCell_dangqianyilou = 9, KeyInfoCell_yuchujilv = 10;

                                for (int i = 0; i < rows.length - 1; i++)
                                {
                                    clTuijianhaomalan_info item = new clTuijianhaomalan_info();
                                    #region MyRegion

                                    HTMLTableRowClass KeyRow = rows.item(KeyInfoRowIndex, null) as HTMLTableRowClass;
                                    HTMLTableCell shiming = KeyRow.cells.item(KeyInfoCell_haomaileixing, null) as HTMLTableCell;
                                    item.haomaileixing = shiming.innerText;
                                    //HTMLTableRowClass KeyRowLocation = rows.item(KeyInfoRowIndex, null) as HTMLTableRowClass;
                                    HTMLTableCell shimingLocation = KeyRow.cells.item(KeyInfoCellCIDIndex, null) as HTMLTableCell;
                                    item.chuxiancishu = shimingLocation.innerText;

                                    HTMLTableCell pingjunyilouLocation = KeyRow.cells.item(KeyInfoCellPN, null) as HTMLTableCell;
                                    item.pingjunyilou = pingjunyilouLocation.innerText;

                                    HTMLTableCell zuidayilouLocation = KeyRow.cells.item(KeyInfoCellLocation, null) as HTMLTableCell;
                                    item.zuidayilou = zuidayilouLocation.innerText;

                                    HTMLTableCell diwuyilouLocation = KeyRow.cells.item(KeyInfoCellDataSource, null) as HTMLTableCell;
                                    item.diwuyilou = diwuyilouLocation.innerText;

                                    HTMLTableCell disiyilouLocation = KeyRow.cells.item(KeyInfoCellOrder, null) as HTMLTableCell;
                                    item.disiyilou = disiyilouLocation.innerText;

                                    HTMLTableCell disanyilouLocation = KeyRow.cells.item(KeyInfoCell_disanyilou, null) as HTMLTableCell;
                                    item.disanyilou = disanyilouLocation.innerText;

                                    HTMLTableCell dieryilouLocation = KeyRow.cells.item(KeyInfoCell_dieryilou, null) as HTMLTableCell;
                                    item.dieryilou = dieryilouLocation.innerText;

                                    HTMLTableCell shangciyilouLocation = KeyRow.cells.item(KeyInfoCell_shangciyilou, null) as HTMLTableCell;
                                    item.shangciyilou = shangciyilouLocation.innerText;

                                    HTMLTableCell dangqianyilouLocation = KeyRow.cells.item(KeyInfoCell_dangqianyilou, null) as HTMLTableCell;
                                    item.dangqianyilou = dangqianyilouLocation.innerText;

                                    HTMLTableCell yuchujilvLocation = KeyRow.cells.item(KeyInfoCell_yuchujilv, null) as HTMLTableCell;
                                    item.yuchujilv = yuchujilvLocation.innerText;
                                    #endregion

                                    item.wanfazhonglei = wanfazhonglei;
                                    Tuijianhaomalan_Result.Add(item);
                                    KeyInfoRowIndex++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }


                if (userName == null)
                {
                    //isOneFinished = true;
                    //转到中奖信息页面
                    MyWebBrower.Navigate("http://chart.icaile.com/hlj11x5.php?op=dcjb");

                    isrun = ProcessStatus.确认YES;
                    tsStatusLabel1.Text = "加载中奖信息....";
                    login++;
                }
            }
            //获取中奖信息
            else if (myDoc.Url.ToString().IndexOf("http://chart.icaile.com/hlj11x5.php?op=dcjb") >= 0 && isrun == ProcessStatus.确认YES)
            {
                if (login < 5)
                {
                    login++;
                    return;

                }
           
                loading = true;
                while (loading == true)
                {
                    Application.DoEvents();
                    Get_Kaijiang();
                }

                //if (userName == null)
                {
                    isOneFinished = true;
                    //   myDoc.Refresh();
                    //   return;
                    isrun = ProcessStatus.关闭页面;
                    tsStatusLabel1.Text = "分析....";
                    login++;
                }
            }
            #endregion

        }

        private void Get_Kaijiang()
        {
            try
            {
                zhongjiangxinxi_Result = new List<clTuijianhaomalan_info>();

                IHTMLDocument2 doc = (IHTMLDocument2)MyWebBrower.Document.DomDocument;
                HTMLDocument myDoc1 = doc as HTMLDocument;

                IHTMLElementCollection Tablelin = myDoc1.getElementsByTagName("table");
                foreach (IHTMLElement items in Tablelin)
                {

                    if (items.outerText != null && items.outerText.Contains("开奖号码") && !items.outerText.Contains("**"))
                    {

                        HTMLTable materialTable = items as HTMLTable;
                        IHTMLElementCollection rows = materialTable.rows as IHTMLElementCollection;
                        int KeyInfoRowIndex = 2;
                        int KeyInfoCellCIDIndex = 1, KeyInfoCellPN = 2, KeyInfoCellLocation = 3, KeyInfoCellDataSource = 4, KeyInfoCellOrder = 5, KeyInfoCell_haomaileixing = 0, KeyInfoCell_disanyilou = 6, KeyInfoCell_dieryilou = 7, KeyInfoCell_shangciyilou = 8, KeyInfoCell_dangqianyilou = 9, KeyInfoCell_yuchujilv = 10;

                        for (int i = 0; i < rows.length - 1; i++)
                        {
                            clTuijianhaomalan_info item = new clTuijianhaomalan_info();
                            #region MyRegion

                            HTMLTableRowClass KeyRow = rows.item(KeyInfoRowIndex, null) as HTMLTableRowClass;
                            HTMLTableCell shiming = KeyRow.cells.item(KeyInfoCell_haomaileixing, null) as HTMLTableCell;
                            item.zhongjiangqishu = shiming.innerText;
                            bool ischina = clsCommHelp.HasChineseTest(item.zhongjiangqishu.ToString());
                            if (ischina == true)
                                break;

                            //HTMLTableRowClass KeyRowLocation = rows.item(KeyInfoRowIndex, null) as HTMLTableRowClass;
                            HTMLTableCell shimingLocation = KeyRow.cells.item(KeyInfoCellCIDIndex, null) as HTMLTableCell;
                            item.kaijianghaoma = shimingLocation.innerText;



                            HTMLTableCell zuidayilouLocation = KeyRow.cells.item(KeyInfoCellLocation, null) as HTMLTableCell;
                            item.kaijianghaoma = item.kaijianghaoma + " " + zuidayilouLocation.innerText;



                            HTMLTableCell disiyilouLocation = KeyRow.cells.item(KeyInfoCellOrder, null) as HTMLTableCell;
                            item.kaijianghaoma = item.kaijianghaoma + " " + disiyilouLocation.innerText;



                            HTMLTableCell dieryilouLocation = KeyRow.cells.item(KeyInfoCell_dieryilou, null) as HTMLTableCell;
                            item.kaijianghaoma = item.kaijianghaoma + " " + dieryilouLocation.innerText;



                            HTMLTableCell dangqianyilouLocation = KeyRow.cells.item(KeyInfoCell_dangqianyilou, null) as HTMLTableCell;
                            item.kaijianghaoma = item.kaijianghaoma + " " + dangqianyilouLocation.innerText;

                            #endregion

                            loading = false;
                            zhongjiangxinxi_Result.Add(item);
                            KeyInfoRowIndex++;
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region 历史中奖烂
        public List<clsJisuanqi_info> ReadHistroy(ref BackgroundWorker bgWorker, List<string> selectitem, List<clTuijianhaomalan_info> NewResult)
        {
            login = 0;
            try
            {
                Find_JisuanqiResult2 = new List<clTuijianhaomalan_info>();
                runtime = 0;

                Find_JisuanqiResult2 = NewResult.FindAll(so => so.zhongjiangqishu != null && so.zhongjiangqishu != "");
                if (Find_JisuanqiResult2.Count == 0)
                    return null;

                ITEM = Find_JisuanqiResult2[0];

                tsStatusLabel1.Text = "玩命获取中....";
                isOneFinished = false;
                StopTime = DateTime.Now;
                InitialWebbroswerIE();


                while (!isOneFinished)
                {
                    tsStatusLabel1.Text = "玩命获取中...." + runtime + "/" + Find_JisuanqiResult2.Count.ToString();

                    System.Windows.Forms.Application.DoEvents();
                    DateTime rq2 = DateTime.Now;  //结束时间
                    int a = rq2.Second - StopTime.Second;
                    if (a == 30 && isrun == ProcessStatus.Filter下拉)
                    {
                        MyWebBrower = null;
                        viewForm.Close();

                        StopTime = DateTime.Now;
                    }
                    //
                    //myDoc = sender as WbBlockNewUrl;
                }
                if (viewForm != null)
                {
                    MyWebBrower = null;
                    viewForm.Close();

                }
                isOneFinished = false;
                return jisuanqi_Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                throw;
            }
        }
        public void InitialWebbroswerIE()
        {
            try
            {

                MyWebBrower = new WbBlockNewUrl();
                //不显示弹出错误继续运行框（HP方可）
                MyWebBrower.ScriptErrorsSuppressed = true;
                MyWebBrower.BeforeNewWindow += new EventHandler<WebBrowserExtendedNavigatingEventArgs>(MyWebBrower_BeforeNewWindow2);
                MyWebBrower.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(AnalysisWebInfo);
                MyWebBrower.Dock = DockStyle.Fill;
                MyWebBrower.IsWebBrowserContextMenuEnabled = true;
                //显示用的窗体
                viewForm = new Form();
                //viewForm.Icon=
                viewForm.ClientSize = new System.Drawing.Size(550, 600);
                viewForm.StartPosition = FormStartPosition.CenterScreen;
                viewForm.Controls.Clear();
                viewForm.Controls.Add(MyWebBrower);
                viewForm.FormClosing += new FormClosingEventHandler(viewForm_FormClosing);
                viewForm.Show();
                MyWebBrower.Url = new Uri("http://zx.dahecp.com/tool/beitou.aspx");


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        protected void AnalysisWebInfo(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //  WbBlockNewUrl myDoc = sender as WbBlockNewUrl;

            myDoc = sender as WbBlockNewUrl;
            #region

            if (myDoc.Url.ToString().IndexOf("http://zx.dahecp.com/tool/beitou.aspx") >= 0 && login == 0)
            {
                // 方案期数
                string fa = "";
                //17121319
                //171213129
                var qushutxt = Convert.ToDouble(ITEM.zhongjiangqishu.Substring(6, ITEM.zhongjiangqishu.Length - 6)) - Convert.ToDouble(ITEM.dangriqihao.Substring(6, ITEM.dangriqihao.Length - 6)) + 1;
                qushutxt = Math.Abs(Convert.ToDouble(qushutxt));

                if (qushutxt < 337 && qushutxt > -337)
                    fa = Convert.ToInt32(qushutxt.ToString()).ToString().Replace("-", "").ToString();
                else
                    fa = "336";
                ITEM.fanganqishu = fa;

                HtmlElement qishu = myDoc.Document.GetElementById("sq");
                if (qishu != null)
                    qishu.SetAttribute("Value", fa);
                //投入注数： 
                HtmlElement zhushu = myDoc.Document.GetElementById("sz");
                if (zhushu != null)
                    zhushu.SetAttribute("Value", "1");

                //起始倍数：： 
                HtmlElement qishibeishu = myDoc.Document.GetElementById("sb");
                if (qishibeishu != null)
                    qishibeishu.SetAttribute("Value", "1");

                // 单倍奖金：： 
                HtmlElement danbeijiangjin = myDoc.Document.GetElementById("sb");
                if (danbeijiangjin != null)
                {
                    //查询值
                    string inputJiangjin = "";

                    inputJiangjin = chaxunjiangjin(inputJiangjin);

                    if (inputJiangjin != "")
                        danbeijiangjin.SetAttribute("Value", inputJiangjin);
                }
                //      全程收益率：：： 
                HtmlElement quanchengshouyilv = myDoc.Document.GetElementById("sy1");
                if (quanchengshouyilv != null)
                    quanchengshouyilv.SetAttribute("Value", "1");

                //      最低收益：：：： 
                HtmlElement zuidi = myDoc.Document.GetElementById("syRMB");
                if (zuidi != null)
                    zuidi.SetAttribute("Value", "100");
                HtmlElement submit = myDoc.Document.GetElementById("submit");
                if (submit != null)
                    submit.InvokeMember("Click");


                isrun = ProcessStatus.登录界面;
                login++;
                //点击按钮后直接显示了 不会刷新页面 
                loading = true;
                while (loading == true)
                {
                    Application.DoEvents();
                    NewMethod();
                }
                if (loading == false)
                {
                    runtime++;
                    if (Find_JisuanqiResult2.Count > runtime)
                    {

                        MyWebBrower.Navigate("http://zx.dahecp.com/tool/beitou.aspx");
                        MyWebBrower.Refresh();
                        ITEM = Find_JisuanqiResult2[runtime];
                        login = 0;
                        return;

                    }

                    isOneFinished = true;
                    isrun = ProcessStatus.关闭页面;
                    tsStatusLabel1.Text = "login....";
                    login++;
                }

            }


            #endregion

        }

        private string chaxunjiangjin(string inputJiangjin)
        {
            if (ITEM.wanfazhonglei.Contains("任选一"))
                inputJiangjin = "13";
            else if (ITEM.wanfazhonglei.Contains("任二"))
                inputJiangjin = "6";
            else if (ITEM.wanfazhonglei.Contains("任三"))
                inputJiangjin = "19";
            else if (ITEM.wanfazhonglei.Contains("任四 "))
                inputJiangjin = "78";
            else if (ITEM.wanfazhonglei.Contains("任五"))
                inputJiangjin = "540";
            else if (ITEM.wanfazhonglei.Contains("任六"))
                inputJiangjin = "90";
            else if (ITEM.wanfazhonglei.Contains("任七"))
                inputJiangjin = "26";
            else if (ITEM.wanfazhonglei.Contains("任五"))
                inputJiangjin = "540";
            else if (ITEM.wanfazhonglei.Contains("任八"))
                inputJiangjin = "9";
            else if (ITEM.wanfazhonglei.Contains("二直"))
                inputJiangjin = "130";
            else if (ITEM.wanfazhonglei.Contains("二组"))
                inputJiangjin = "65";
            else if (ITEM.wanfazhonglei.Contains("三直"))
                inputJiangjin = "1170";
            else if (ITEM.wanfazhonglei.Contains("三组"))
                inputJiangjin = "195";
            return inputJiangjin;
        }

        private void NewMethod()
        {
            try
            {
                if (runtime == 0)
                    jisuanqi_Result = new List<clsJisuanqi_info>();

                IHTMLDocument2 doc = (IHTMLDocument2)MyWebBrower.Document.DomDocument;
                HTMLDocument myDoc1 = doc as HTMLDocument;
                IHTMLElementCollection Tablelin = myDoc1.getElementsByTagName("table");
                foreach (IHTMLElement items in Tablelin)
                {
                    int yue_valoume = System.Text.RegularExpressions.Regex.Matches(items.outerText, "\r\n").Count;

                    if (items.outerText != null && items.outerText.Contains("投入倍数") && yue_valoume >= 1)
                    {

                        HTMLTable materialTable = items as HTMLTable;
                        IHTMLElementCollection rows = materialTable.rows as IHTMLElementCollection;
                        int KeyInfoRowIndex = 1;
                        int KeyInfoCellCIDIndex = 1, KeyInfoCellPN = 2, KeyInfoCellLocation = 3, KeyInfoCellDataSource = 4, KeyInfoCellOrder = 5, KeyInfoCell_haomaileixing = 0, KeyInfoCell_disanyilou = 6, KeyInfoCell_dieryilou = 7, KeyInfoCell_shangciyilou = 8, KeyInfoCell_dangqianyilou = 9, KeyInfoCell_yuchujilv = 10;

                        for (int i = 0; i < rows.length - 1; i++)
                        {
                            clsJisuanqi_info item = new clsJisuanqi_info();
                            #region MyRegion

                            HTMLTableRowClass KeyRow = rows.item(KeyInfoRowIndex, null) as HTMLTableRowClass;
                            HTMLTableCell shiming = KeyRow.cells.item(KeyInfoCell_haomaileixing, null) as HTMLTableCell;
                            item.qishu = shiming.innerText;
                            //HTMLTableRowClass KeyRowLocation = rows.item(KeyInfoRowIndex, null) as HTMLTableRowClass;
                            HTMLTableCell shimingLocation = KeyRow.cells.item(KeyInfoCellCIDIndex, null) as HTMLTableCell;
                            item.tourubeishu = shimingLocation.innerText;

                            HTMLTableCell pingjunyilouLocation = KeyRow.cells.item(KeyInfoCellPN, null) as HTMLTableCell;
                            item.benqitouru = pingjunyilouLocation.innerText;

                            HTMLTableCell zuidayilouLocation = KeyRow.cells.item(KeyInfoCellLocation, null) as HTMLTableCell;
                            item.leijitouru = zuidayilouLocation.innerText;

                            HTMLTableCell diwuyilouLocation = KeyRow.cells.item(KeyInfoCellDataSource, null) as HTMLTableCell;
                            item.benqishouyi = diwuyilouLocation.innerText;

                            HTMLTableCell disiyilouLocation = KeyRow.cells.item(KeyInfoCellOrder, null) as HTMLTableCell;
                            item.yilishouyi = disiyilouLocation.innerText;

                            HTMLTableCell disanyilouLocation = KeyRow.cells.item(KeyInfoCell_disanyilou, null) as HTMLTableCell;
                            item.shouyilv = disanyilouLocation.innerText;


                            #endregion

                            loading = false;
                            item.dangriqihao = ITEM.dangriqihao;
                            item.tuijianhaoma = ITEM.tuijianhaoma;
                            item.wanfazhonglei = ITEM.wanfazhonglei;
                            item.zhongjiangqishu = ITEM.zhongjiangqishu;
                            item.fanganqishu = ITEM.fanganqishu;
                            jisuanqi_Result.Add(item);
                            KeyInfoRowIndex++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion
    }
}

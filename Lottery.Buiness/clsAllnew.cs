using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        string filter_qishu;
        bool has_alter = false;

        bool loading;
        clTuijianhaomalan_info ITEM;
        List<clTuijianhaomalan_info> Find_JisuanqiResult2;
        int runtime = 0;
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, uint hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, uint wMsg, IntPtr wParam, int lParam);

        public clsAllnew()
        {

            newsth = AppDomain.CurrentDomain.BaseDirectory + "" + dataSource;
            //  InitialSystemInfo();


        }
        private void InitialSystemInfo()
        {
            #region 初始化配置
            ProcessLogger = log4net.LogManager.GetLogger("ProcessLogger");
            ExceptionLogger = log4net.LogManager.GetLogger("SystemExceptionLogger");
            ProcessLogger.Fatal("System Start " + DateTime.Now.ToString());
            #endregion
        }


        public bool read_sqlitefile()
        {
            bool ishave = false;

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
            DataTable dataTable = Read_yuanshizoushitu(dbCmd, sql);
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

        public List<clsJisuanqi_info> ReadServer_lishizhongjiang(string sql)
        {
            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);

            dbConn.Open();
            SQLiteCommand dbCmd = dbConn.CreateCommand();
            //dbCmd.CommandText = "SELECT * FROM tuijanhaoma";
            dbCmd.CommandText = sql;

            DbDataReader reader = SQLiteHelper.ExecuteReader("Data Source=" + newsth, dbCmd);
            List<clsJisuanqi_info> ClaimReport_Server = new List<clsJisuanqi_info>();


            while (reader.Read())
            {
                clsJisuanqi_info item = new clsJisuanqi_info();

                if (reader.GetValue(0) != null && Convert.ToString(reader.GetValue(0)) != "")
                    item.wanfazhonglei = reader.GetString(0);
                if (reader.GetValue(1) != null && Convert.ToString(reader.GetValue(1)) != "")
                    item.tuijianhaoma = reader.GetString(1);
                if (reader.GetValue(2) != null && Convert.ToString(reader.GetValue(2)) != "")
                    item.dangriqihao = reader.GetString(2);
                if (reader.GetValue(3) != null && Convert.ToString(reader.GetValue(3)) != "")
                    item.zhongjiangqishu = reader.GetString(3);
                if (reader.GetValue(4) != null && Convert.ToString(reader.GetValue(4)) != "")
                    item.leijitouru = reader.GetString(4);
                if (reader.GetValue(5) != null && Convert.ToString(reader.GetValue(5)) != "")
                    item.benqishouyi = reader.GetString(5);

                if (reader.GetValue(6) != null && Convert.ToString(reader.GetValue(6)) != "")
                    item.yilishouyi = reader.GetString(6);
                if (reader.GetValue(7) != null && Convert.ToString(reader.GetValue(7)) != "")
                    item.fanganqishu = reader.GetString(7);

                if (reader.GetValue(8) != null && Convert.ToString(reader.GetValue(8)) != "")
                    item.Input_Date = reader.GetString(8);
                ClaimReport_Server.Add(item);

            }
            return ClaimReport_Server;




        }
        public List<clTuijianhaomalan_info> ReadServer_tuijanhaoma(string sql)
        {

            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);
            ProcessLogger.Fatal("ReadServer_tuijanhaoma open" + DateTime.Now.ToString());
            dbConn.Open();
            ProcessLogger.Fatal("ReadServer_tuijanhaoma open 1" + DateTime.Now.ToString());

            SQLiteCommand dbCmd = dbConn.CreateCommand();
            //dbCmd.CommandText = "SELECT * FROM tuijanhaoma";
            dbCmd.CommandText = sql;
            ProcessLogger.Fatal("ReadServer_tuijanhaoma open 2" + DateTime.Now.ToString());

            DbDataReader reader = SQLiteHelper.ExecuteReader("Data Source=" + newsth, dbCmd);
            ProcessLogger.Fatal("ReadServer_tuijanhaoma open 3" + DateTime.Now.ToString());

            List<clTuijianhaomalan_info> ClaimReport_Server = new List<clTuijianhaomalan_info>();


            while (reader.Read())
            {
                clTuijianhaomalan_info item = new clTuijianhaomalan_info();

                if (reader.GetValue(0) != null && Convert.ToString(reader.GetValue(0)) != "")
                    item.wanfazhonglei = reader.GetString(0);
                if (reader.GetValue(1) != null && Convert.ToString(reader.GetValue(1)) != "")
                    item.tuijianhaoma = reader.GetString(1);
                if (reader.GetValue(2) != null && Convert.ToString(reader.GetValue(2)) != "")
                    item.nizhuihaoqishu = reader.GetString(2);
                if (reader.GetValue(3) != null && Convert.ToString(reader.GetValue(3)) != "")
                    item.dangriqihao = reader.GetString(3);
                if (reader.GetValue(4) != null && Convert.ToString(reader.GetValue(4)) != "")
                    item.zhongjiangqishu = reader.GetString(4);
                if (reader.GetValue(5) != null && Convert.ToString(reader.GetValue(5)) != "")
                    item.Input_Date = reader.GetString(5);


                ClaimReport_Server.Add(item);

            }
            return ClaimReport_Server;




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
                string dele_sql = "delete FROM lishizongjiang  WHERE wanfazhonglei ='" + item.wanfazhonglei + "'And tuijianhaoma='" + item.tuijianhaoma + "'And dangriqihao='" + item.dangriqihao + "'And zhongjiangqishu='" + item.zhongjiangqishu + "'And leijitouru='" + item.leijitouru + "'And benqishouyi='" + item.benqishouyi + "'And yilishouyi='" + item.yilishouyi + "'And Input_Date='" + item.Input_Date + "'";
                delete_step(dele_sql);

                string sql = "INSERT INTO lishizongjiang ( wanfazhonglei, tuijianhaoma, dangriqihao, zhongjiangqishu, leijitouru, benqishouyi, yilishouyi,fanganqishu,Input_Date ) " +

                "VALUES (\"" + item.wanfazhonglei + "\"" +

                       ",\"" + item.tuijianhaoma + "\"" +
                                ",\"" + item.dangriqihao + "\"" +
                                         ",\"" + item.zhongjiangqishu + "\"" +
                                                  ",\"" + item.leijitouru + "\"" +
                                                           ",\"" + item.benqishouyi + "\"" +
                                                                    ",\"" + item.yilishouyi + "\"" +
                                                                       ",\"" + item.fanganqishu + "\"" +

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
                if (item.wanfazhonglei == null || item.wanfazhonglei == "")
                    continue;

                string dele_sql = "delete FROM tuijanhaoma  WHERE dangriqihao ='" + item.dangriqihao + "'And wanfazhonglei='" + item.wanfazhonglei + "'";
                delete_step(dele_sql);


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
        public void delete_step(string sql)
        {
            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dataSource);

            SQLiteCommand dbCmd = dbConn.CreateCommand();
            dbCmd.CommandText = sql;
            //服务器端的
            dbCmd.CommandText = sql;

            var var = SQLiteHelper.ExecuteNonQuery("Data Source=" + newsth, dbCmd);

            // Read(dbCmd);
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

        public List<clTuijianhaomalan_info> ReadWeb_Report(ref BackgroundWorker bgWorker, List<string> selectitem, List<clszhongleiDuiyingQishu_info> mapping_Result)
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

                        clszhongleiDuiyingQishu_info qishuitem = mapping_Result.Find(so => so.wanfazhonglei != null && so.wanfazhonglei == caizhong);
                        if (qishuitem != null)
                        {
                            filter_qishu = qishuitem.qishu;
                        }
                        if (filter_qishu == null || filter_qishu == "")
                        {
                            MessageBox.Show(caizhong + "期数填写有误");
                            continue;

                        }
                        aog = 0;
                        isrun = ProcessStatus.初始化;
                        ProcessLogger.Fatal("读取中 0901" + DateTime.Now.ToString());

                        tsStatusLabel1.Text = caizhong + "读取中....";
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
                        //zhongjiangxinxi_ResultAll = zhongjiangxinxi_ResultAll.Concat(zhongjiangxinxi_Result).ToList();
                        //中奖信息都是一样的
                        zhongjiangxinxi_ResultAll = zhongjiangxinxi_Result;
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
                tsStatusLabel1.Text = caizhong + "玩命获取中....";
                isOneFinished = false;
                StopTime = DateTime.Now;
                InitialWebbroswerIE2();
                tsStatusLabel1.Text = "玩命获取中 012901....";


                int time = 0;
                while (!isOneFinished)
                {
                    time++;
                    tsStatusLabel1.Text = caizhong + "刷新中  " + time.ToString() + "....";

                    System.Windows.Forms.Application.DoEvents();
                    DateTime rq2 = DateTime.Now;  //结束时间
                    int a = rq2.Second - StopTime.Second;
                    TimeSpan ts = rq2 - StopTime;
                    int timeTotal = ts.Minutes;

                    if (timeTotal >= 1)
                    {
                        tsStatusLabel1.Text = caizhong + "超出时间 正在退出....";
                        ProcessLogger.Fatal("超出时间 89011" + DateTime.Now.ToString());
                        isOneFinished = true;

                        //MyWebBrower = null;
                        //viewForm.Close();
                        //MyWebBrower.Refresh();
                        StopTime = DateTime.Now;
                    }
                }
                tsStatusLabel1.Text = caizhong + "关闭1  ....";

                if (viewForm != null)
                {
                    MyWebBrower = null;
                    viewForm.Close();
                    //aTimer.Stop();
                }
                tsStatusLabel1.Text = caizhong + "关闭2  ....";

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
                viewForm.Show();
                ProcessLogger.Fatal("读取中 09010 " + DateTime.Now.ToString());
                //
                //MyWebBrower.Url = new Uri("http://chart.icaile.com/hlj11x5.php?op=yl3m");//&num=15

                //new  二期
                MyWebBrower.Url = new Uri("http://hlj11x5.icaile.com/?op=yl3m"); 
               // MyWebBrower.Url = new Uri("https://www.baidu.com");//&num=15

                if (caizhong != null && caizhong != "")
                {
                    tsStatusLabel1.Text = caizhong + "接入 前 ...." + MyWebBrower.Url;
                    ProcessLogger.Fatal("接入 前 091101 " + DateTime.Now.ToString());


                    if (caizhong.Contains("前一直"))
                    {
                        //MyWebBrower.Url = new Uri("http://chart.icaile.com/hlj11x5.php?op=q11m");
                        //new  二期
                        MyWebBrower.Url = new Uri("http://hlj11x5.icaile.com/?op=q11m");

                    }
                    else if (caizhong.Contains("前二直"))
                    {
                        //MyWebBrower.Url = new Uri("http://chart.icaile.com/hlj11x5.php?op=q2zhix");
                        //new  二期
                        MyWebBrower.Url = new Uri("http://hlj11x5.icaile.com/?op=q2zhix");

                    }
                    else if (caizhong.Contains("前三直"))
                    {
                        //MyWebBrower.Url = new Uri("http://chart.icaile.com/hlj11x5.php?op=q3zhix");
                        //new  二期
                        MyWebBrower.Url = new Uri("http://hlj11x5.icaile.com/?op=q3zhix");

                    }
                    else if (caizhong.Contains("前二组"))
                    {
                        //MyWebBrower.Url = new Uri("http://chart.icaile.com/hlj11x5.php?op=q2zux");
                        //new  二期
                        MyWebBrower.Url = new Uri("http://hlj11x5.icaile.com/?op=q2zux");

                    }
                    else if (caizhong.Contains("前三组"))
                    {
                        //MyWebBrower.Url = new Uri("http://chart.icaile.com/hlj11x5.php?op=q3zux");
                        //new  二期
                        MyWebBrower.Url = new Uri("http://hlj11x5.icaile.com/?op=q3zux");

                    }
                    //
                    else if (caizhong.Contains("乐选四"))
                    {
                        //new  二期
                        MyWebBrower.Url = new Uri("http://hlj11x5.icaile.com/?op=r34m");

                    }
                    else if (caizhong.Contains("乐选五"))
                    {
                        //new  二期
                        MyWebBrower.Url = new Uri("http://hlj11x5.icaile.com/?op=r45m");

                    }
                    ProcessLogger.Fatal("接入 前 091102 " + DateTime.Now.ToString());

                }
                else
                {
                    MessageBox.Show("彩票信息维护错误 !");
                }
                tsStatusLabel1.Text = "接入 ...." + MyWebBrower.Url;

            }
            catch (Exception ex)
            {
                ProcessLogger.Fatal("接入 前 09110 " + ex + DateTime.Now.ToString());

                MessageBox.Show("错误：0001" + ex);
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
                if (MessageBox.Show("正在进行，是否中止?", "关闭", MessageBoxButtons.OKCancel) == DialogResult.OK)
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
            try
            {
                //  WbBlockNewUrl myDoc = sender as WbBlockNewUrl;
                tsStatusLabel1.Text = caizhong + "接入 89902 ....";
                ProcessLogger.Fatal("AnalysisWebInfo2接入 89902" + DateTime.Now.ToString());
                //  MessageBox.Show("" + "接入");

                myDoc = sender as WbBlockNewUrl;
                #region 登录页面
                //http://chart.icaile.com/hlj11x5.php?op=yl3m&num=15
                //点击彩种
                if (myDoc.Url.ToString().IndexOf("http://hlj11x5.icaile.com/?op") >= 0 && login == 0)//http://chart.icaile.com/hlj11x5.php?op=yl3
                {

                    tsStatusLabel1.Text = caizhong + "接入界面 ....";
                    ProcessLogger.Fatal("接入界面" + DateTime.Now.ToString());

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
                    else if (caizhong.Contains("前一直") || caizhong.Contains("前二直") || caizhong.Contains("前三直") || caizhong.Contains("前二组") || caizhong.Contains("前三组") || caizhong.Contains("乐选四") || caizhong.Contains("乐选五"))
                    {
                        ProcessLogger.Fatal("接入界面 001" + DateTime.Now.ToString());

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
                    tsStatusLabel1.Text = caizhong + "界面1 ....";
                    ProcessLogger.Fatal("接入界面 002" + DateTime.Now.ToString());

                    isrun = ProcessStatus.登录界面;
                    login++;

                }
                //
                else if (myDoc.Url.ToString().IndexOf("http://hlj11x5.icaile.com/?op") >= 0 && isrun == ProcessStatus.登录界面)//http://chart.icaile.com/hlj11x5.php?op=yl3
                {
                    ProcessLogger.Fatal("界面2  1901" + DateTime.Now.ToString());

                    tsStatusLabel1.Text = caizhong + "界面2 ....";
                    if (login < 2)
                    {
                        login++;
                        return;

                    }
                    int ii = 0;
                    MyWebBrower.Navigate(myDoc.Url.ToString() + "&num=" + filter_qishu.ToString());
                    login++;
                    isrun = ProcessStatus.第一页面;
                    return;
                }
                else if (myDoc.Url.ToString().IndexOf("http://hlj11x5.icaile.com/?op") >= 0 && myDoc.Url.ToString().IndexOf("&num=" + filter_qishu.ToString()) >= 0 && isrun == ProcessStatus.第一页面)//http://chart.icaile.com/hlj11x5.php?op=yl3
                {
                    ProcessLogger.Fatal("界面3  1902" + DateTime.Now.ToString());

                    // return;
                    tsStatusLabel1.Text = caizhong + "界面3 ....";
                    //刷新没完成 返回继续刷新
                    if (login < 4)
                    {
                        login++;
                        // return;

                    }

                    ProcessLogger.Fatal("界面3  1903" + DateTime.Now.ToString());

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
                        HtmlElement parent_li = myDoc.Document.GetElementById(tatile[0]);//.Substring(0, 4)
                        if (parent_li != null && parent_li.InnerText != null)
                            wanfazhonglei = parent_li.InnerText.Replace("[多码遗漏]", "").Trim();
                        if (wanfazhonglei == "[常规遗漏] 直选遗漏" || wanfazhonglei == "组选遗漏")
                            wanfazhonglei = caizhong;

                        //利用 HTMLTable 抓取信息
                        try
                        {
                            WaitWebPageLoad();
                            tsStatusLabel1.Text = caizhong + "获取[走势数据]信息中 ....";
                            ProcessLogger.Fatal("界面3  190012" + DateTime.Now.ToString());

                            loading = true;
                            while (loading == true)
                            {
                                Application.DoEvents();
                                getwanfa_table(wanfazhonglei);
                            }


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("错误：" + ex);
                            return;

                            throw;
                        }
                    }


                    //if (userName == null)
                    {
                        tsStatusLabel1.Text = caizhong + "加载中奖信息 ....";
                        ProcessLogger.Fatal("读取中奖信息  192305" + DateTime.Now.ToString());

                        //isOneFinished = true;
                        //转到中奖信息页面
                        MyWebBrower.Navigate("http://hlj11x5.icaile.com/?op=dcjb");

                        isrun = ProcessStatus.确认YES;
                        tsStatusLabel1.Text = caizhong + "加载中奖信息....";
                        login++;
                    }
                }
                //获取中奖信息
                else if (myDoc.Url.ToString().IndexOf("http://hlj11x5.icaile.com/?op=dcjb") >= 0 && isrun == ProcessStatus.确认YES)
                {
                    tsStatusLabel1.Text = caizhong + "获取中奖信息  ....";
                    ProcessLogger.Fatal("获取中奖信息  1906" + DateTime.Now.ToString());

                    if (login < 7)
                    {
                        login++;
                        return;

                    }
                    ProcessLogger.Fatal("获取中奖信息  1907" + DateTime.Now.ToString());

                    tsStatusLabel1.Text = caizhong + "获取中奖信息....";

                    loading = true;
                    while (loading == true)
                    {
                        Application.DoEvents();
                        Get_Kaijiang();
                    }
                    ProcessLogger.Fatal("获取中奖信息结束  1909" + DateTime.Now.ToString());

                    //if (userName == null)
                    {
                        isOneFinished = true;
                        //   myDoc.Refresh();
                        //   return;
                        isrun = ProcessStatus.关闭页面;
                        tsStatusLabel1.Text = caizhong + "查询结束....";
                        login++;
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show("ex" + ex, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

                throw ex;
            }
        }

        private void getwanfa_table(string wanfazhonglei)
        {
            IHTMLDocument2 doc = (IHTMLDocument2)MyWebBrower.Document.DomDocument;
            HTMLDocument myDoc1 = doc as HTMLDocument;
            //IHTMLElement doc1 = (IHTMLElement)MyWebBrower.Document.DomDocument;
            IHTMLElementCollection Tablelin = myDoc1.getElementsByTagName("table");
            foreach (IHTMLElement items in Tablelin)
            {
                string aresult = System.Text.RegularExpressions.Regex.Replace(items.outerText, @"[^0-9]+", "");
                //if (items.outerText != null && items.outerText.Contains("号码类型") && items.outerText.Contains("----"))//没有加二期前的 （ 乐选四 乐选五）
                if (items.outerText != null && items.outerText.Contains("号码类型") && aresult.Length > 50)
                {

                    HTMLTable materialTable = items as HTMLTable;
                    IHTMLElementCollection rows = materialTable.rows as IHTMLElementCollection;
                    int KeyInfoRowIndex = 1;
                    int KeyInfoCellCIDIndex = 1, KeyInfoCellPN = 2, KeyInfoCellLocation = 3, KeyInfoCellDataSource = 4, KeyInfoCellOrder = 5, KeyInfoCell_haomaileixing = 0, KeyInfoCell_disanyilou = 6, KeyInfoCell_dieryilou = 7, KeyInfoCell_shangciyilou = 8, KeyInfoCell_dangqianyilou = 9, KeyInfoCell_yuchujilv = 10;
                    ProcessLogger.Fatal("界面3  1904-" + rows.length.ToString() + "   " + DateTime.Now.ToString());

                    for (int i = 0; i < rows.length - 1; i++)
                    {
                        clTuijianhaomalan_info item = new clTuijianhaomalan_info();
                        #region MyRegion

                        HTMLTableRowClass KeyRow = rows.item(KeyInfoRowIndex, null) as HTMLTableRowClass;
                        ProcessLogger.Fatal("界面3  190432-" + rows.length.ToString() + "   " + DateTime.Now.ToString());

                        if (KeyRow != null)
                        {
                            ProcessLogger.Fatal("界面3  190321-" + rows.length.ToString() + "   " + DateTime.Now.ToString());

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
                            loading = false;
                            item.wanfazhonglei = wanfazhonglei;
                            Tuijianhaomalan_Result.Add(item);
                            KeyInfoRowIndex++;
                        }
                    }
                }
            }
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
                        ProcessLogger.Fatal("获取中奖信息  1910" + DateTime.Now.ToString());

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

                aTimer.Elapsed += new ElapsedEventHandler(APIREST);
                aTimer.Start();
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
                //viewForm.Show();
                MyWebBrower.Url = new Uri("http://zx.dahecp.com/tool/beitou.aspx");


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void APIREST(object sender, EventArgs e)
        {

            CloseWin("来自网页的消息", "确定");
            CloseWin("Message from webpage", "OK");

        }
        private void CloseWin(string winTitle, string buttonTitle)
        {
            IntPtr hwnd = FindWindow(null, winTitle);
            if (hwnd != IntPtr.Zero)
            {
                IntPtr hwndText = FindWindowEx(hwnd, 0, null, "按照当前设置");
                //   if (hwndText != IntPtr.Zero)
                {
                    IntPtr hwndSure = FindWindowEx(hwnd, 0, "Button", buttonTitle);
                    if (hwnd != IntPtr.Zero)
                    {
                        SendMessage(hwndSure, 0xF5, (IntPtr)0, 0);  //按她
                        has_alter = true;

                    }
                }
            }
        }
        protected void AnalysisWebInfo(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //  WbBlockNewUrl myDoc = sender as WbBlockNewUrl;
            ProcessLogger.Fatal("ReadHistroy 99712" + DateTime.Now.ToString());

            myDoc = sender as WbBlockNewUrl;
            #region

            if (myDoc.Url.ToString().IndexOf("http://zx.dahecp.com/tool/beitou.aspx") >= 0 && login == 0)
            {
                ProcessLogger.Fatal("ReadHistroy 9712" + DateTime.Now.ToString());

                has_alter = false;
                //查询值
                string chaoqishu = "";

                chaoqishu = chaxun_chaoqishu(chaoqishu);

                // 方案期数
                string fa = "";
                //17121319
                //171213129
                var qushutxt = Convert.ToDouble(ITEM.zhongjiangqishu.Substring(6, ITEM.zhongjiangqishu.Length - 6)) - Convert.ToDouble(ITEM.dangriqihao.Substring(6, ITEM.dangriqihao.Length - 6)) + 1;
                qushutxt = Math.Abs(Convert.ToDouble(qushutxt));

                //if (qushutxt < 67 && qushutxt > -67)
                fa = Convert.ToInt32(qushutxt.ToString()).ToString().Replace("-", "").ToString();
                //else
                // fa = "7766";
                //判断是否超期
                if (chaoqishu != "" && Convert.ToInt32(chaoqishu) < Convert.ToInt32(fa.ToString()))
                {
                    has_alter = true;
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
                    tsStatusLabel1.Text = "退出....";
                    login++;
                    return;
                }
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
                HtmlElement danbeijiangjin = myDoc.Document.GetElementById("dj");
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
                    zuidi.SetAttribute("Value", "1");
                HtmlElement submit = myDoc.Document.GetElementById("submit");
                if (submit != null)
                    submit.InvokeMember("Click");

                ProcessLogger.Fatal("ReadHistroy 99711" + DateTime.Now.ToString());


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
                    tsStatusLabel1.Text = "退出....";
                    login++;
                }

            }


            #endregion

        }

        private string chaxunjiangjin(string inputJiangjin)
        {
            if (ITEM.wanfazhonglei != null)
            {
                if (ITEM.wanfazhonglei.Contains("任选一"))
                    inputJiangjin = "13";
                else if (ITEM.wanfazhonglei.Contains("任二"))
                    inputJiangjin = "6";
                else if (ITEM.wanfazhonglei.Contains("任三"))
                    inputJiangjin = "19";
                else if (ITEM.wanfazhonglei.Contains("任四"))
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
                if (ITEM.wanfazhonglei.Contains("前一"))
                    inputJiangjin = "13";
            }
            else
                inputJiangjin = "1";
            return inputJiangjin;
        }
        private string chaxun_chaoqishu(string inputJiangjin)
        {
            if (ITEM.wanfazhonglei != null)
            {
                if (ITEM.wanfazhonglei.Contains("任选一"))
                    inputJiangjin = "13";
                else if (ITEM.wanfazhonglei.Contains("任二"))
                    inputJiangjin = "15";
                else if (ITEM.wanfazhonglei.Contains("任三"))
                    inputJiangjin = "50";
                else if (ITEM.wanfazhonglei.Contains("任四 "))
                    inputJiangjin = "200";
                else if (ITEM.wanfazhonglei.Contains("任五"))
                    inputJiangjin = "500";
                else if (ITEM.wanfazhonglei.Contains("任六"))
                    inputJiangjin = "170";
                else if (ITEM.wanfazhonglei.Contains("任七"))
                    inputJiangjin = "60";
                else if (ITEM.wanfazhonglei.Contains("任八"))
                    inputJiangjin = "20";
                else if (ITEM.wanfazhonglei.Contains("二直"))
                    inputJiangjin = "200";
                else if (ITEM.wanfazhonglei.Contains("二组"))
                    inputJiangjin = "200";
                else if (ITEM.wanfazhonglei.Contains("三直"))
                    inputJiangjin = "1000";
                else if (ITEM.wanfazhonglei.Contains("三组"))
                    inputJiangjin = "250";
            }
            else
                inputJiangjin = "0";
            return inputJiangjin;
        }
        private void NewMethod()
        {
            try
            {
                if (runtime == 0 || jisuanqi_Result == null)
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

                        //for (int i = 0; i < rows.length - 1; i++)
                        {
                            //修改成只取得最后一行即可
                            KeyInfoRowIndex = rows.length - 1;

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
                            item.Input_Date = DateTime.Now.ToString("yyyy/MM/dd");
                            if (has_alter == false)
                                jisuanqi_Result.Add(item);
                            KeyInfoRowIndex++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("系统异常：" + ex);
                return;
                throw;
            }
        }

        #endregion


        #region 网页延迟50 s
        private bool WaitWebPageLoad()
        {
            int i = 0;
            string sUrl;
            while (true)
            {
                Delay(50);  //系统延迟50毫秒，够少了吧！             
                if (MyWebBrower.ReadyState == WebBrowserReadyState.Interactive) //先判断是否发生完成事件。
                {
                    if (!MyWebBrower.IsBusy) //再判断是浏览器是否繁忙                  
                    {
                        i = i + 1;
                        if (i == 2)
                        {
                            sUrl = MyWebBrower.Url.ToString();
                            if (sUrl.Contains("res")) //这是判断没有网络的情况下                           
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        continue;
                    }
                    i = 0;
                }
            }
        }


        private void Delay(int Millisecond) //延迟系统时间，但系统又能同时能执行其它任务；
        {
            DateTime current = DateTime.Now;
            while (current.AddMilliseconds(Millisecond) > DateTime.Now)
            {
                Application.DoEvents();//转让控制权            
            }
            return;
        }
        #endregion
    }
}

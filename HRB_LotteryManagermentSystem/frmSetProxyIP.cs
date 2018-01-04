using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
 

namespace HRB_LotteryManagermentSystem
{
    public partial class frmSetProxyIP : Form
    {
       

        public frmSetProxyIP()
        {
            InitializeComponent();
            SameTime();

        }


        #region http://blog.csdn.net/wuma0q1an/article/details/51312983

        public Boolean setip(string ip)
        {
            RefreshIESettings(ip);
            IEProxy ie = new IEProxy(ip);
            return ie.RefreshIESettings();
        }
        public struct Struct_INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        };
        private void RefreshIESettings(string strProxy)
        {
            const int INTERNET_OPTION_PROXY = 38;
            const int INTERNET_OPEN_TYPE_PROXY = 3;
            const int INTERNET_OPEN_TYPE_DIRECT = 1;

            Struct_INTERNET_PROXY_INFO struct_IPI;
            // Filling in structure  
            struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
            struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy);
            struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");

            // Allocating memory  
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(Marshal.SizeOf(struct_IPI));
            if (string.IsNullOrEmpty(strProxy) || strProxy.Trim().Length == 0)
            {
                strProxy = string.Empty;
                struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_DIRECT;

            }
            // Converting structure to IntPtr  
            Marshal.StructureToPtr(struct_IPI, intptrStruct, true);

            bool iReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, Marshal.SizeOf(struct_IPI));
        }
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        public class IEProxy
        {
            private const int INTERNET_OPTION_PROXY = 38;
            private const int INTERNET_OPEN_TYPE_PROXY = 3;
            private const int INTERNET_OPEN_TYPE_DIRECT = 1;

            private string ProxyStr;


            [DllImport("wininet.dll", SetLastError = true)]

            private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

            public struct Struct_INTERNET_PROXY_INFO
            {
                public int dwAccessType;
                public IntPtr proxy;
                public IntPtr proxyBypass;
            }

            private bool InternetSetOption(string strProxy)
            {
                int bufferLength;
                IntPtr intptrStruct;
                Struct_INTERNET_PROXY_INFO struct_IPI;

                if (string.IsNullOrEmpty(strProxy) || strProxy.Trim().Length == 0)
                {
                    strProxy = string.Empty;
                    struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
                }
                else
                {
                    struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
                }
                struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy);
                struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");
                bufferLength = Marshal.SizeOf(struct_IPI);
                intptrStruct = Marshal.AllocCoTaskMem(bufferLength);
                Marshal.StructureToPtr(struct_IPI, intptrStruct, true);
                return InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, bufferLength);

            }
            public IEProxy(string strProxy)
            {
                this.ProxyStr = strProxy;
            }
            //设置代理  
            public bool RefreshIESettings()
            {
                return InternetSetOption(this.ProxyStr);
            }
            //取消代理  
            public bool DisableIEProxy()
            {
                return InternetSetOption(string.Empty);
            }
        }
        #endregion

    
       
   

        private static void SameTime()
        {
            //多线程同时抓15页
            for (int i = 1; i <= 15; i++)
            {


            //    ThreadPool.QueueUserWorkItem(getProxyList, i);
            }
            Console.Read();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string proxy = this.textBox1.Text;
            RefreshIESettings(proxy);
            IEProxy ie = new IEProxy(proxy);
            MessageBox.Show(ie.RefreshIESettings().ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IEProxy ie = new IEProxy(null);
            ie.DisableIEProxy();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.webBrowser1.Navigate("http://www.ip138.com/", null, null, null);
      
        }

        #region C# 验证过滤代理IP是否有效
        /// <summary>
        /// 读取txt代理ip
        /// </summary>
        /// <param name="filename"></param>
        private void ReadFileIP(string filename)
        {
            //txtmsg.BeginInvoke(new Action(() =>
            //{
            //    txtmsg.AppendText("开始导入IP代理!".SetLog());
            //}));
            var file = File.Open(filename, FileMode.Open);
            int num = 0;
            int goods = 0;
            int repeat = 0;
            using (var stream = new StreamReader(file))
            {
                while (!stream.EndOfStream)
                {
                    num++;
                    string linetemp = stream.ReadLine().Trim().ToLower();
                    string[] iptxt = linetemp.Split(':');
                    if (iptxt.Count() == 2)
                    {
                        lock (Config.lock_prxoy)
                        {
                            var data = Config._prxoyList.Where(m => m.ip == iptxt[0]).FirstOrDefault();
                            if (data != null)
                            {
                                repeat++;
                                continue;
                            }
                        }
                        goods++;
                        Model.ProxyIP _proxyip = new Model.ProxyIP();
                        _proxyip.ip = iptxt[0];
                        _proxyip.prot = int.Parse(iptxt[1]);
                        ListViewItem item = new ListViewItem(_proxyip.ip);
                        item.SubItems.Add(_proxyip.prot.ToString());
                        item.SubItems.Add("");
                        listViewIP.Invoke(new Action(() =>
                        {
                            ListViewItem itemresult = listViewIP.Items.Add(item);
                            _dic.Add(_proxyip.ip, itemresult);
                            //dic.Add(_send.Tel, backitem);
                        }));
                        lock (Config.lock_prxoy)
                        {
                            Config._prxoyList.Add(_proxyip);
                        }
                    }
                }
            }
            //txtmsg.Invoke(new Action(() =>
            //{
            //    string log = string.Format("添加代理IP完成!有效数据为:{0},过滤重复数据:{1},总数据:{2}", goods.ToString(), repeat.ToString(), num.ToString());
            //    txtmsg.AppendText(log.SetLog());
            //}));
            Thread filter = new Thread(new ThreadStart(filterIP)) { IsBackground = true };
            filter.Start();
        }
        private void filterIP()
        {
            //txtmsg.Invoke(new Action(() =>
            //{
            //    txtmsg.AppendText("正在过滤IP数据！".SetLog());
            //}));
            List<System.Threading.Tasks.Task> TaskList = new List<System.Threading.Tasks.Task>();
            lock (Config.lock_prxoy)
            {
                foreach (Model.ProxyIP _model in Config._prxoyList)
                {
                    var task = System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        bool reslut = VerIP(_model.ip, _model.prot);
                        if (reslut)
                        {
                            _model.filter = Model.filterIP.有效;
                            this.Invoke(new Action(() =>
                            {
                                _dic[_model.ip].SubItems[2].Text = "有效";
                            }));
                        }
                        else
                        {
                            _model.filter = Model.filterIP.无效;
                            this.Invoke(new Action(() =>
                            {
                                _dic[_model.ip].SubItems[2].Text = "无效";
                            }));
                        }
                    });
                    TaskList.Add(task);
                }
            }
            System.Threading.Tasks.Task.WaitAll(TaskList.ToArray());
            //txtmsg.Invoke(new Action(() =>
            //{
            //    txtmsg.AppendText(Config._prxoyList[0].filter.ToString() + "过滤IP数据完成!".SetLog());
            //}));
        }

        private bool VerIP(string ip, int port)
        {
            try
            {
                HttpWebRequest Req;
                HttpWebResponse Resp;
                WebProxy proxyObject = new WebProxy(ip, port);// port为端口号 整数型
                Req = WebRequest.Create("https://www.baidu.com") as HttpWebRequest;
                Req.Proxy = proxyObject; //设置代理
                Req.Timeout = 1000;   //超时
                Resp = (HttpWebResponse)Req.GetResponse();
                Encoding bin = Encoding.GetEncoding("UTF-8");
                using (StreamReader sr = new StreamReader(Resp.GetResponseStream(), bin))
                {
                    string str = sr.ReadToEnd();
                    if (str.Contains("百度"))
                    {
                        Resp.Close();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}

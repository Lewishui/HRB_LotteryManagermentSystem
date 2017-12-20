using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HRB_LotteryManagermentSystem
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            if (IsAlreadyRunning())
            {
                MessageBox.Show("The tool is running!");
                return;
            }

            FileInfo Log4NetFile = new FileInfo("./Log4Net.Config");
            log4net.Config.XmlConfigurator.Configure(Log4NetFile);

            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Control.CheckForIllegalCrossThreadCalls = false;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmlogin());


            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new frmlogin());
        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            log4net.ILog objLogger = log4net.LogManager.GetLogger("SystemExceptionLogger");
            objLogger.Fatal("System Error " + e.Exception.Message.ToString() + ", Exception Detail Info :" + e.Exception.StackTrace + "Time" + DateTime.Now.ToString());

            MessageBox.Show("系统异常：00000，请关闭并重新启动如继续异常请检查数据信息是否有误!", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static bool IsAlreadyRunning()
        {
            Process processCurrent = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (processCurrent.Id != process.Id)
                {
                    if (processCurrent.ProcessName == process.ProcessName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

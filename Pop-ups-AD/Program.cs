using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pop_ups_AD
{
    class Program
    {
        public static bool _IsExit = false, _IsSearch = true;
        public static string input, title;
        public static Thread threadMonitorInput,thrSearch;
        public static long count = 0;
        public static ManualResetEvent sea = new ManualResetEvent(true);
        static void Main(string[] args)
        {
            title = "广告窗口拦截_启动时间_" + DateTime.Now.ToString();
            Console.Title = title;
            threadMonitorInput = new Thread(new ThreadStart(MonitorInput));
            thrSearch = new Thread(new ThreadStart(Search));
            new MyNotifyIcon(MyWindow.FindWindow(null, title));
            threadMonitorInput.Start();
            thrSearch.Start();
            while (true)
            {
                Application.DoEvents();
                if (_IsExit)
                {
                    break;
                }
            }
        }
        
        static void MonitorInput()
        {
            while (true)
            {
                input = Console.ReadLine();
                if ("|exit|esc|".Contains($"|{input}|"))
                {
                    _IsExit = true;
                    Thread.CurrentThread.Abort();
                }
            }
        }

        static void Search()
        {
            MyWindow.CallBack callBack = new MyWindow.CallBack(Program.Report);
            while (!_IsExit)
            {
                sea.WaitOne();
                MyWindow.EnumWindows(callBack, 0);
                Thread.Sleep(1000);
                Console.Title = $"{title}_检测次数_{++count}";
                //Console.WriteLine(DateTime.Now.ToString() + "检测线程暂停!");
                //Thread.CurrentThread.Interrupt();
            }
        }

        //private class My {
        //    int i;
        //    public void Start(int myHw)
        //    {
        //        MyWindow.CallBack callBack = new MyWindow.CallBack(Program.Report);
        //        MyWindow.EnumWindows(callBack, 0);
        //        Console.ReadKey();
        //    }
        //}

        const int BM_CLICK = 0xF5;
        private static bool Report(int hwnd, int lParam)
        {
            WindowInfo info = MyWindow.GetInfo( new WindowInfo { hWnd = (IntPtr)hwnd });
            //窗口黑名单
            if ("|MSCTFIME UI|Default|Default IME|Mode Indicator|".Contains($"|{info.szWindowName}|")) return true;
            if ("|aaa|".Contains($"|{info.szClassName}|")) return true;
            if (info.szWindowName != "" && info.szClassName != "")
            {
                int i = 0;
                //Console.WriteLine("{0},n=[{1}]c=[{2}]", hwnd, info.szWindowName, info.szClassName);
                //窗口白名单
                if (info.szWindowName == "发起会话" && info.szClassName== "#32770") {
                    IntPtr childHwnd = MyWindow.FindWindowEx(info.hWnd, IntPtr.Zero, null, "确定");
                    MyWindow.SendMessage(childHwnd, BM_CLICK, 0, 0);
                    MyWindow.SendMessage(childHwnd, BM_CLICK, 0, 0);
                    Console.Write(DateTime.Now.ToString() + $"第{count}次检测时 成功关闭{++i}=>");
                    Console.Write("teamviewer窗口[{0}][{1}][{2}]",
                        info.hWnd, info.szWindowName, info.szClassName);
                }
                if (i > 0) Console.Write("\n");
                //Console.Read();
            }
            return true;
        }
    }
}
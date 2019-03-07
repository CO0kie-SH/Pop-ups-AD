using System;
using System.Threading;
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
                Thread.Sleep(222);
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
                Thread.Sleep(2000);
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
                if(info.szWindowName== "错误报告" && info.szClassName== "#32770")
                {
                    if( MyWindow.FindWindowEx(info.hWnd, IntPtr.Zero, null, "微信遇到错误，给您带来不便，我们深表歉意。") != IntPtr.Zero)
                    {
                        Console.Write($"检测到微信\t次数={++i}\t");
                        IntPtr childHwnd = MyWindow.FindWindowEx(info.hWnd, IntPtr.Zero, null, "发送错误报告(&S)"),
                            childHwnd2 = MyWindow.FindWindowEx(info.hWnd, IntPtr.Zero, "Button", "确定(&O)");
                        Console.Write($"{childHwnd}\t{childHwnd2}\n");
                        
                        MyWindow.PostMessage(childHwnd, MSG.WM_IME_KEYUP, (int)Keys.Space, 0);
                        MyWindow.PostMessage(childHwnd, MSG.WM_IME_KEYDOWN, (int)Keys.Space, 0);
                        MyWindow.PostMessage(childHwnd, MSG.WM_IME_KEYUP, (int)Keys.Space, 0);
                        Thread.Sleep(100);
                        MyWindow.PostMessage(childHwnd2, MSG.WM_IME_KEYUP, (int)Keys.Space, 0);
                        MyWindow.PostMessage(childHwnd2, MSG.WM_IME_KEYDOWN, (int)Keys.Space, 0);
                        MyWindow.PostMessage(childHwnd2, MSG.WM_IME_KEYUP, (int)Keys.Space, 0);
                    }
                }
                if(info.szWindowName== "登录" && info.szClassName== "WeChatLoginWndForPC")
                {
                    Console.Write($"检测到微信登录\t次数={++i}\t");
                    //MyWindow.SendMessage(info.hWnd, BM_CLICK, 135, 279);
                    //MyWindow.SendMessage(info.hWnd, BM_CLICK, 135, 279);
                    //mouse_event(MOUSEEVENTF_MOVE, 0, 0, 0, 0);
                    //mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 135, 279, 0, 0);
                    MouseLefClickEvent(1324, 616, 0);
                }
                if (i > 0) Console.Write("\n");
                //Console.Read();
            }
            return true;
        }
        public static void MouseLefClickEvent(int dx, int dy, uint data)
        {
            SetCursorPos(dx, dy);
            mouse_event(MouseEventFlag.LeftDown, dx, dy, data, UIntPtr.Zero);
            mouse_event(MouseEventFlag.LeftUp, dx, dy, data, UIntPtr.Zero);
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);
        enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }


        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        //移动鼠标 
        const int MOUSEEVENTF_MOVE = 0x0001;
        //模拟鼠标左键按下 
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //模拟鼠标左键抬起 
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        //模拟鼠标右键抬起 
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        //模拟鼠标中键抬起 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        //标示是否采用绝对坐标 
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;

    }
}
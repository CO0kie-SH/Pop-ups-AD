using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Pop_ups_AD
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            MyWindow.CallBack callBack = new MyWindow.CallBack(Program.Report);
            MyWindow.EnumWindows(callBack, 0);
            Console.ReadKey();
        }

        const int BM_CLICK = 0xF5;
        private static bool Report(int hwnd, int lParam)
        {
            WindowInfo info = GetInfo( new WindowInfo { hWnd = (IntPtr)hwnd });
            if ("|MSCTFIME UI|Default|Default IME|Mode Indicator|".Contains($"|{info.szWindowName}|")) return true;
            if ("||".Contains($"|{info.szClassName}|")) return true;
            if (info.szWindowName != "" && info.szClassName != "")
            {
                //Console.WriteLine("{0},n=[{1}]c=[{2}]", hwnd, info.szWindowName, info.szClassName);
                if (info.szWindowName == "发起会话") {
                    IntPtr childHwnd = MyWindow.FindWindowEx(info.hWnd, IntPtr.Zero, null, "确定");
                    MyWindow.SendMessage(childHwnd, BM_CLICK, 0, 0);
                    MyWindow.SendMessage(childHwnd, BM_CLICK, 0, 0);
                    Console.WriteLine(DateTime.Now.ToString() + " 成功关闭teamviewer窗口[{0}][{1}][{2}]",
                        info.hWnd, info.szWindowName, info.szClassName);
                }
                //Console.Read();
            }
            return true;
        }
        

        private static WindowInfo GetInfo(WindowInfo info)
        {
            StringBuilder sb = new StringBuilder(256);
            MyWindow.GetWindowText(info.hWnd, sb, sb.Capacity);
            info.szWindowName = sb.ToString();
            MyWindow.GetClassNameW(info.hWnd, sb, sb.Capacity);
            info.szClassName = sb.ToString();
            return info;
        }
    }
}

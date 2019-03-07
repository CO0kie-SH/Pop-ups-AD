using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Pop_ups_AD
{
    public struct WindowInfo
    {
        public IntPtr hWnd;
        public string szWindowName;
        public string szClassName;
    }
    public class MSG
    {
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_IME_KEYDOWN = 0x0290;
        public const int WM_IME_KEYUP = 0x0291;
        public const int WM_SETTEXT = 0x000C;
        public const int WM_QUIT = 0x0012;
        public const int WM_CLOSE = 0x0010;
        public const int SC_CLOSE = 0xF060;

        //MyWindow.SendMessage(_hwnd, MSG.WM_SETTEXT, null, "exit");
        //Program.threadMonitorInput.Abort();
        //MyWindow.PostMessage(_hwnd, MSG.WM_IME_KEYDOWN, (int)Keys.Enter, 0);
        //MyWindow.PostMessage(_hwnd, MSG.WM_IME_KEYUP, (int)Keys.Enter, 0);
    }
    class MyWindow
    {
        public delegate bool CallBack(int hwnd, int lParam);
        //枚举窗口
        [DllImport("user32")]
        public static extern int EnumWindows(CallBack x, int y);

        //取出标题
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        ////取出类名
        //[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        //public static extern int GetClassNameA(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        //取出窗口
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        //取出子窗口
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        //发送消息
        [DllImport("User32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, int WM_CHAR, int wParam, int lParam);
        [DllImport("User32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, int WM_CHAR, string wParam, string lParam);

        //post消息
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private const int WM_SETTEXT = 0x000C;

        /// <summary>  
        /// 设置窗体的显示与隐藏  
        /// </summary>  
        /// <param name="hWnd"></param>  
        /// <param name="nCmdShow"></param>  
        /// <returns></returns>  
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        #region 禁用关闭按钮
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        public static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);
        #endregion


        public static WindowInfo GetInfo(WindowInfo info)
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

using System;
using System.Threading;
using System.Windows.Forms;

namespace Pop_ups_AD
{
    class MyNotifyIcon
    {
        private NotifyIcon _NotifyIcon = new NotifyIcon();
        private static IntPtr _hwnd;
        private static bool _isShow = true;
        public MyNotifyIcon(IntPtr hwnd)
        {
            _hwnd = hwnd;
            _NotifyIcon.Icon = new System.Drawing.Icon(@"D:\aa\6981\b5.ico");
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.RenderMode = ToolStripRenderMode.System;
            ToolStripMenuItem item = new ToolStripMenuItem { Name = "Search", Text = "暂停检测" };
            menu.Items.Add(item);
            item = new ToolStripMenuItem { Name="exit",Text="退出"};
            menu.Items.Add(item);
            menu.ItemClicked += new ToolStripItemClickedEventHandler(menu_ItemClicked);
            _NotifyIcon.ContextMenuStrip = menu;
            _NotifyIcon.MouseClick += new MouseEventHandler(NotifyIcon_MouseClick);
            _NotifyIcon.Visible = true;
            DisableCloseButton();
            _NotifyIcon.ShowBalloonTip(5000, "欢迎您", "检测开启\n"+DateTime.Now.ToString(), ToolTipIcon.None);
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            //Console.WriteLine(e.Clicks + "");
            //Console.WriteLine(e.Button + "");
            if (e.Button.ToString() == "Left")
            {
                _isShow = !_isShow;
                MyWindow.ShowWindow(_hwnd, _isShow ? 1 : 0);
            }
        }

        private void menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Name)
            {
                case "exit":
                    _NotifyIcon.Dispose();
                    MyWindow.PostMessage(_hwnd, MSG.WM_CLOSE, 0, 0);
                    break;
                case "Search":
                    Program._IsSearch = !Program._IsSearch;
                    if (Program._IsSearch)
                    {
                        Program.sea.Set();
                        Console.WriteLine(DateTime.Now.ToString() + "检测线程已恢复！");
                        e.ClickedItem.Text = "暂停检测";

                    }
                    else
                    {
                        Program.sea.Reset();
                        Console.WriteLine(DateTime.Now.ToString() + "检测线程已暂停！");
                        e.ClickedItem.Text = "恢复检测";
                    }
                    break;
                default:
                    break;
            }
        }

        #region 禁用关闭按钮
        /// <summary>
        /// 禁用关闭按钮
        /// </summary>
        /// <param name="consoleName">控制台名字</param>
        public static void DisableCloseButton()
        {
            Thread.Sleep(100);
            //线程睡眠，确保closebtn中能够正常FindWindow，否则有时会Find失败。。
            IntPtr closeMenu = MyWindow.GetSystemMenu(_hwnd, IntPtr.Zero);
            MyWindow.RemoveMenu(closeMenu, (uint)MSG.SC_CLOSE, 0x0);
        }
        #endregion
    }//class end
}//空间end

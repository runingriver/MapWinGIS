using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
namespace MapWinGIS.MainProgram
{
    public sealed class WinApi32
    {
        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        public static extern int GetWindowRect(int hwnd, ref RECT lpRect);

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        public static extern int GetCursorPos(ref POINTAPI lpPoint);

        public struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(int vKey); //获取键盘按键状态

    }
}

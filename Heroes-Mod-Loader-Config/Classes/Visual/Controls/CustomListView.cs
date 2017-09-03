using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeroesModLoaderConfigurator.Classes.Visual.Controls
{
    public class CustomListView : ListView
    {
        /* Override the WndProc function for when a message is sent to a window */
        protected override void WndProc(ref Message Window_Message)
        {
            switch (Window_Message.Msg)
            {
                case 0x83: // WM_NCCALCSIZE | Calculates size
                    int Initial_Style = (int)GetWindowLong(this.Handle, GWL_STYLE); // Obtain Initial Style
                    // If the initial style for the Window contains the vertical scrollbar, remove it from the window style.
                    if ((Initial_Style & WS_VSCROLL) == WS_VSCROLL) { Initial_Style = SetWindowLong(this.Handle, GWL_STYLE, Initial_Style & ~WS_VSCROLL); }
                    // Repeat for horizontal scrollbar.
                    if ((Initial_Style & WS_HSCROLL) == WS_HSCROLL) { Initial_Style = SetWindowLong(this.Handle, GWL_STYLE, Initial_Style & ~WS_HSCROLL); }
                    // Paint the rest of the Window.
                    base.WndProc(ref Window_Message);
                    break;

                default:
                    base.WndProc(ref Window_Message); // No modification.
                    break;
            }
        }

        const int GWL_STYLE = -16;
        const int WS_VSCROLL = 0x00200000;
        const int WS_HSCROLL = 0x00100000;

        public int SelectedIndex { get; internal set; }

        public static int GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
                return (int)GetWindowLong32(hWnd, nIndex);
            else
                return (int)(long)GetWindowLongPtr64(hWnd, nIndex);
        }

        public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
        {
            if (IntPtr.Size == 4)
                return (int)SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            else
                return (int)(long)SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
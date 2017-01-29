using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace WhiteStructs.Interop
{
    /// <summary>
    /// This class allows sending text to Windows Notepad.
    /// </summary>
    public static class NotepadSender
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        private static uint WM_SYSCOMMAND = 0x0112;
        private static int SC_MAXIMIZE = 0xF030;

        /// <summary>
        /// Sends a text to a new Windows Notepad instance.
        /// </summary>
        /// <param name="text">The text to send.</param>
        public static void SendTextToNewNotepadInstance(string text)
        {
            Process app = Process.Start("notepad.exe");

            app.WaitForInputIdle();

            SetForegroundWindow(app.MainWindowHandle);
            SendMessage(app.MainWindowHandle.ToInt32(), WM_SYSCOMMAND, SC_MAXIMIZE, 0);

            Clipboard.SetText(text);
            SendKeys.SendWait(Clipboard.GetText());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace InterceptorWindow1C
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        const int TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        private int numberMonitor;

        private ManagementEventWatcher watcher;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonStartSubscription_Click(object sender, RoutedEventArgs e)
        {
            if (watcher == null)
            {
                try
                {
                    watcher = new ManagementEventWatcher("Select * From Win32_ProcessStartTrace");
                    watcher.EventArrived += Watcher_EventArrived;
                    watcher.Start();
                    CheckBoxEnable.IsChecked = true;
                }
                catch (Exception ex)
                {
                    CheckBoxEnable.IsChecked = false;
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

        private void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if ((string)e.NewEvent["ProcessName"] == "1cv8c.exe")
            {
                ChangeMonitor((uint)e.NewEvent["ProcessID"]);
            }
        }

        private void ChangeMonitor(uint processID)
        {

            Process process1C = Process.GetProcessById((int)processID);
            IntPtr ApplicationHandle = process1C.MainWindowHandle;

            if (ApplicationHandle != IntPtr.Zero)
            {
                Screen[] screens = Screen.AllScreens;

                System.Drawing.Rectangle monitor2 = screens[numberMonitor].WorkingArea;
                MoveWindow(ApplicationHandle, monitor2.Left, monitor2.Top, monitor2.Width, monitor2.Height, true);
            }
        }

        private void ButtonStopSubscription_Click(object sender, RoutedEventArgs e)
        {
            CheckBoxEnable.IsChecked = false;
            if (watcher != null)
            {
                watcher.Stop();
                watcher = null;
            }
        }

        private void ButtonChangeMonitor_Click(object sender, RoutedEventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("1cv8c");
            if (processes.Count() > 0)
            {
                ChangeMonitor((uint)processes.First().Id);
            }
        }


        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        private void TextBoxMonitor_TextChanged(object sender, TextChangedEventArgs e)
        {
            numberMonitor = int.Parse(TextBoxMonitor.Text);
        }
    }
}

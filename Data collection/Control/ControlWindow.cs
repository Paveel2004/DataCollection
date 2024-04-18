using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Control
{
    internal static class ControlWindow
    {
        static public List<string> processList = new List<string>();
        private static CancellationTokenSource cts = new CancellationTokenSource();

        public static void Start()
        {
            cts = new CancellationTokenSource();
            Task.Run(() => BlockProcess(cts.Token), cts.Token);
        }
        public static void Stop()
        {
            cts.Cancel();
        }


        private static void CloseProcess(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
                process.CloseMainWindow();
            }
        }
        public static void CloseProcess()
        {
            foreach (string processName in processList)
            {
                CloseProcess(processName);
            }
        }
        public static void BlockedProcess(string processName)
        {
            processList.Add(processName);
        }
        public static void BlockedProcess(params string[] processName)
        {
            foreach (string process in processName)
            {
                processList.Add(process);
            }
        }
        private static void BlockProcess(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                CloseProcess();
            }
        }
    }
}

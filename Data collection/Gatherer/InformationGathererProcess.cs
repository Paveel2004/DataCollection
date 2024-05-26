using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;

namespace Data_collection.Gatherer
{
    internal class InformationGathererProcess
    {
        public class ProvessInfo
        {
            string ProcessName { get; set; }
            string ProcessID { get; set; }
            DateTime datatime { get; set; }
            public ProvessInfo(string ProcessName, string ProcessID, DateTime dateTime)
            {
                this.ProcessName = ProcessName;
                this.ProcessID = ProcessID;
                datatime = dateTime;
            }

        }

        public List<ProvessInfo> closeProcess = new List<ProvessInfo>();

        public List<Process> GetPrcessList()
        {
            List<Process> list = new List<Process>();
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (process.MainWindowTitle != "")
                    {
                        list.Add(process);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Process: {process.ProcessName} ID: {process.Id} Start time: N/A");
                }
            }
            return list;
        }

        async Task CloseProcess()
        {
            ManagementEventWatcher watcher = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStopTrace");
            watcher.EventArrived += (sender, e) =>
            {

                /*          Console.WriteLine("Процесс был закрыт: {0}, ID: {1}, Время: {2}",
                              e.NewEvent.Properties["ProcessName"].Value,
                              e.NewEvent.Properties["ProcessID"].Value,
                              DateTime.Now);*/
                closeProcess.Add(new ProvessInfo(e.NewEvent.Properties["ProcessName"].Value.ToString(), e.NewEvent.Properties["ProcessID"].Value.ToString(), DateTime.Now));
            };
            watcher.Start();
            while (true) { }
        }
    }
}

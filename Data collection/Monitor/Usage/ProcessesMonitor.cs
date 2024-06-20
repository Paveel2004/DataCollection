using Data_collection.Gatherer;
using GlobalClass;
using Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Monitor.Usage
{
    public class ProcessesMonitor
    {
        private static string SID = InformationGathererUser.GetUserSID();
        public static void StartOpenProcessMonitor()
        {
            // Set up the event query to listen for process creation events
            string query = "SELECT * FROM Win32_ProcessStartTrace";
            ManagementEventWatcher watcher = new ManagementEventWatcher(new WqlEventQuery(query));
            watcher.EventArrived += new EventArrivedEventHandler(ProcessStarted);
            watcher.Start();


        }
        public  static void ProcessStarted(object sender, EventArrivedEventArgs e)
        {
            try
            {
                // Extract the process name and ID from the event data
                string processName = (string)e.NewEvent.Properties["ProcessName"].Value;
                uint processId = (uint)e.NewEvent.Properties["ProcessID"].Value;

                DataBaseHelper.Query($"INSERT INTO Процессы(Пользователь, Процесс, [Дата/Время]) VALUES ('{SID}', '{processName}', '{DateTime.Now}')");



            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving process information: {ex.Message}");
            }
        }
        public static List<ProcessInfo> GetProcessInfo()
        {
            List<ProcessInfo> processInfoList = new List<ProcessInfo>();

            // Получаем все процессы
            Process[] processlist = Process.GetProcesses();

            // Выводим информацию о каждом процессе
            foreach (Process theprocess in processlist)
            {
                string title = theprocess.MainWindowTitle != "" ? theprocess.MainWindowTitle : "—";
                double memoryUsageMB = theprocess.WorkingSet64 / Math.Pow(1024, 2);
                ProcessInfo processInfo = new ProcessInfo(theprocess.ProcessName, title, memoryUsageMB);

                processInfoList.Add(processInfo);
            }

            return processInfoList;
        }
    }
}

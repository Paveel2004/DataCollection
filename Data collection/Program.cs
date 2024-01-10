using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Data_collection
{
    class DiskInfo
    {
        public string DeviceID { get; set; }
        public ulong Size { get; set; }
        public ulong FreeSpace { get; set; }
        public string VolumeName { get; set; }
    }
    internal class Program
    {
        static void WriteLineRed(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n" + text);
            Console.ForegroundColor = ConsoleColor.Green;
        }
        static void ExeCopyng(string destinationDirectory)
        {
            string currentDirrectory = AppDomain.CurrentDomain.BaseDirectory,
                executablePath = Path.Combine(currentDirrectory, "Data collection.exe");

        }
        static DiskInfo[] GetDiskInformation()
        {
            string query = "SELECT * FROM Win32_LogicalDisk";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection = searcher.Get();

            DiskInfo[] diskInfo = new DiskInfo[queryCollection.Count];
            int index = 0;

            foreach (ManagementObject m in queryCollection)
            {
                diskInfo[index] = new DiskInfo
                {
                    DeviceID = m["DeviceID"].ToString(),
                    Size = Convert.ToUInt64(m["Size"]),
                    FreeSpace = Convert.ToUInt64(m["FreeSpace"]),
                    VolumeName = m["VolumeName"]?.ToString() ?? "N/A"
                };
                index++;
            }

            return diskInfo;
        }
       
        static void Main(string[] args)
        {

            Console.WriteLine(DataNetwork.EthernetSpeed());
            while (true)
            {
                var message = new
                {
                    CPU = new
                    {
                        Architecture = DataCPU.GetProcessorArchitecture(),
                        Name = DataCPU.GetProcessorName(),
                        CoreCount = DataCPU.GetProcessorCoreCount(),
                        Temperature = DataCPU.GetProcessorTemperature(),
                        SerialNumber = DataCPU.GetProcessorNum(),
                        CpuUsage = DataCPU.GetCpuUsage()
                    },
                    OS = new
                    {
                        OS = DataOS.GetOperatingSystem(),
                        Architecture = DataOS.GetSystemBitArchitecture(),
                        SerialNumber = DataOS.GetOperatingSystemSerialNumber(),
                        NumberOfUsers = DataOS.GetNumberOfUsers(),
                        SystemState = DataOS.GetSystemState(),
                        VersionOS = DataOS.GetOperatingSystemVersion(),

                    },
                    BIOS = new
                    {
                        SerialNumber = DataBIOS.GetBiosSerialNumber(),
                        BiosVeesion = DataBIOS.GetBiosVersion()
                    },
                    USER = new
                    {
                        UserName = DataUser.GetUserName(),
                        UserSID = DataUser.GetUserSID(),
                        UserState = DataUser.GetUserStatus()
                    },
                    NETWORK = new
                    {
                        IP = DataNetwork.GetIPAddress(),
                        MAC = DataNetwork.GetPhysicalMacAddress(),
                        EthernetSpeed = DataNetwork.EthernetSpeed(),
                    },
                    RAM = new
                    {
                        RamType = DataRam.RamType,
                        RamUsage = DataRam.GetMemoryUsage(),
                        TotalPhisicalMemory = DataRam.GetTotalPhysicalMemory(),
                    }

                };
                string messageData = JsonConvert.SerializeObject(message, Formatting.Indented);

                Console.WriteLine(messageData);


                try
                {

                    string serverAddress = "127.0.0.1";
                    int port = 1111;

                    // Создаем TcpClient и подключаемся к серверу
                    using TcpClient client = new TcpClient(serverAddress, port);
                    Console.WriteLine("Подключено к серверу...");

                    // Получаем поток для обмена данными с сервером
                    using NetworkStream stream = client.GetStream();

                    // Отправляем сообщение серверу

                    byte[] data = Encoding.UTF8.GetBytes(messageData);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine($"Отправлено сообщение: {messageData}");
                    Thread.Sleep(5000);

                    // Читаем ответ от сервера
                    data = new byte[5000];
                    int bytesRead = stream.Read(data, 0, data.Length);
                    string response = Encoding.UTF8.GetString(data, 0, bytesRead);
                    Console.WriteLine($"Ответ от сервера: {response}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.ReadLine();
            }






    /* WriteLineRed("Процессор: ");*/


    /*  Console.WriteLine($"Архитектура: {DataCPU.GetProcessorArchitecture()}");
      Console.WriteLine($"Имя: {DataCPU.GetProcessorName()}");
      Console.WriteLine($"Количество ядер: {DataCPU.GetProcessorCoreCount()}");
      Console.WriteLine($"Температура процессора: {DataCPU.GetProcessorTemperature()} *C {DataCPU.GetProcessorTemperature()}");
      Console.WriteLine($"Серийный номер процессора: {DataCPU.GetProcessorNum()}");
      Console.WriteLine($"Нагруженность: {DataCPU.GetCpuUsage()}");

      // Информация об операционной системе
      WriteLineRed("Операционная система:");
      Console.WriteLine($"Версия ОС: {Environment.OSVersion}");

      //Console.WriteLine($"Имя компьютера: {Environment.MachineName}");
      Console.WriteLine($"Имя пользователя: {Environment.UserName}");
      //Console.WriteLine($"Версия .NET Framework: {Environment.Version}");
      //ОС
      ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2");
      ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
      ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

      ManagementObjectCollection queryCollection = searcher.Get();

      foreach (ManagementObject m in queryCollection)
      {
          Console.WriteLine($"Операционная система: {m["Caption"]}");
          Console.WriteLine($"Версия: {m["OSArchitecture"]}");
          Console.WriteLine($"Серийный номер: {m["SerialNumber"]}");
          Console.WriteLine($"Имя компьютера: {m["CSName"]}");
          Console.WriteLine($"Количество пользователей: {m["NumberOfUsers"]}");
          Console.WriteLine($"Состояние: {m["Status"]}");

      }

      // Пути к системным каталогам
      WriteLineRed("Системные каталоги:");
      Console.WriteLine($"Автозапуск: {DataOS.GetStartupFolderPath()}");
      Console.WriteLine($"Каталог системы: {Environment.SystemDirectory}");
      Console.WriteLine($"Каталог временных файлов: {Environment.GetEnvironmentVariable("TEMP")}");// Информация об операционной системе


      //Пользователи 
      WriteLineRed("Пользователи: ");
      ObjectQuery userQuery = new ObjectQuery("SELECT * FROM Win32_UserAccount");
      ManagementObjectSearcher userSearcher = new ManagementObjectSearcher(scope, userQuery);

      ManagementObjectCollection userCollection = userSearcher.Get();

      foreach (ManagementObject user in userCollection)
      {
          Console.WriteLine($"Имя пользователя: {user["Name"]}");
          Console.WriteLine($"Полное имя пользователя: {user["FullName"]}");
          Console.WriteLine($"SID: {user["SID"]}" + " | " + user["SID"].ToString().Length);
          Console.WriteLine($"Статус: {user["Status"]}");
          Console.WriteLine();
      }

      WriteLineRed("BIOS");
      ObjectQuery biosQuery = new ObjectQuery("SELECT * FROM Win32_BIOS");
      ManagementObjectSearcher biosSearcher = new ManagementObjectSearcher(scope, biosQuery);

      ManagementObjectCollection biosCollection = biosSearcher.Get();

      foreach (ManagementObject bios in biosCollection)
      {
          Console.WriteLine($"BIOS Version: {bios["Version"]}");
          Console.WriteLine($"Manufacturer: {bios["Manufacturer"]}");
          Console.WriteLine($"Description: {bios["Description"]}");
          Console.WriteLine($"Серийный номер: {bios["SerialNumber"].ToString()}");

      }


      WriteLineRed("Оперативная память");
      ObjectQuery memoryQuery = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
      ManagementObjectSearcher menorySearcher = new ManagementObjectSearcher(scope, memoryQuery);

      ManagementObjectCollection memoryCollection = menorySearcher.Get();

      foreach (ManagementObject memory in memoryCollection)
      {
          ulong totalPhysicalMemory = Convert.ToUInt64(memory["TotalPhysicalMemory"]);
          Console.WriteLine($"Объем оперативной памяти: {totalPhysicalMemory / (1024 * 1024)} MB");
      }

      Console.WriteLine(RamInfo.RamType);



      ///////////
      ///
      try
      {
          Console.WriteLine("Информация о дисках:");
          var diskInfo = GetDiskInformation();
          foreach (var disk in diskInfo)
          {
              Console.WriteLine($"Диск {disk.DeviceID}:");
              Console.WriteLine($"   Объем диска: {disk.Size} байт");
              Console.WriteLine($"   Свободное пространство: {disk.FreeSpace} байт");
              Console.WriteLine($"   Метка тома: {disk.VolumeName}");
              Console.WriteLine();
          }
      }
      catch (ManagementException e)
      {
          Console.WriteLine("Ошибка WMI: " + e.Message);
      }
      WriteLineRed("Сеть");

      //EthernetSpeed();
*/

}



       
    }
}
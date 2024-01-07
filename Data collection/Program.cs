using System.Management;

namespace Data_collection
{
    internal class Program
    {
        static void WriteLineRed(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n"+text);
            Console.ForegroundColor = ConsoleColor.Green;
        }
        static void ExeCopyng(string destinationDirectory)
        {
            string currentDirrectory = AppDomain.CurrentDomain.BaseDirectory,
                executablePath = Path.Combine(currentDirrectory, "Data collection.exe");

        }
        static void Main(string[] args)
        {

            WriteLineRed("Процессор: ");


            Console.WriteLine($"Архитектура: {DataCPU.GetProcessorArchitecture()}");
            Console.WriteLine($"Имя: {DataCPU.GetProcessorName()}");
            Console.WriteLine($"Количество ядер: {DataCPU.GetProcessorCoreCount()}");
            Console.WriteLine($"Температура процессора: {DataCPU.GetProcessorTemperature()} *C {DataCPU.GetProcessorTemperature()}");


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

            foreach(ManagementObject m in queryCollection)
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
                Console.WriteLine($"SID: {user["SID"]}" +" | "+ user["SID"].ToString().Length) ;
                Console.WriteLine($"Статус: {user["Status"]}");
                Console.WriteLine();
            }

            WriteLineRed("BIOS");
            ObjectQuery biosQuery = new ObjectQuery("SELECT * FROM Win32_BIOS");
            ManagementObjectSearcher biosSearcher = new ManagementObjectSearcher(scope, biosQuery);

            ManagementObjectCollection biosCollection = biosSearcher.Get();

            foreach(ManagementObject bios in biosCollection)
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

            foreach(ManagementObject memory in memoryCollection)
            {
                ulong totalPhysicalMemory = Convert.ToUInt64(memory["TotalPhysicalMemory"]);
                Console.WriteLine($"Объем оперативной памяти: {totalPhysicalMemory / (1024 * 1024)} MB");
            }
            ///////////
            Console.ReadLine();





        }
    }
}
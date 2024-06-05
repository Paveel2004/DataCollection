using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Data_collection.Gatherer;
using Server;

namespace Data_collection.Monitor.Usage
{
    public class ProcessorUsageMonitor
    {
        private static System.Timers.Timer _timer; // Таймер для выполнения мониторинга
        private static string _cpuSerialNumber; // Серийный номер процессора
        static string SID = InformationGathererUser.GetUserSID();
        // Метод для запуска мониторинга
        public static void StartMonitoring()
        {
       
            _timer = new System.Timers.Timer(5000); // Создание таймера с интервалом 1 минута
            _timer.Elapsed += OnTimedEvent; // Подписываемся на событие Elapsed, которое вызывается по истечении интервала
            _timer.AutoReset = true; // Устанавливаем автоповторение таймера
            _timer.Enabled = true; // Включаем таймер
          
        }

        // Обработчик события Elapsed таймера
        // Обработчик события Elapsed таймера
        // Обработчик события Elapsed таймера
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            string powerShellScript = @"
$cpuSerialNumber = (Get-WmiObject -Class Win32_Processor).ProcessorId
$processorInfo = Get-WmiObject -Query 'SELECT Name, PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name != ''_Total'''
$result = @()
foreach ($item in $processorInfo) {
    $result += [PSCustomObject]@{
        ProcessorSerialNumber = $cpuSerialNumber
        Name = $item.Name
        PercentProcessorTime = $item.PercentProcessorTime
    }
}
$result | Format-Table -AutoSize";
            string powerShellOutput = RunPowerShellScript(powerShellScript);



            // Обработка вывода скрипта PowerShell и передача данных в базу данных
            string[] lines = powerShellOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(3)) // Пропускаем первые три строки с заголовками
            {
                var values = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string processorSerialNumber = values[0];
                string name = values[1];
                float percentProcessorTime = float.Parse(values[2]);

                // Вставляем данные в базу данных
                DataBaseHelper.Query($"INSERT INTO [Логические процессы] ([Номер ядра], [Серийный номер ЦП], Нагрузка, [Дата/Время],Пользователь) VALUES ({name}, '{processorSerialNumber}', {percentProcessorTime}, '{DateTime.Now}','{SID}');");
            }
        }



        // Метод для выполнения PowerShell скрипта
        private static string RunPowerShellScript(string script)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return result;
            }
        }
    }
}

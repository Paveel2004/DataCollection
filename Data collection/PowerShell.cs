using PcapDotNet.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection
{
    public class PowerShell
    {
        protected static List<string> GetPowershellValueListClass(string className, string column)
        {
            // Команда PowerShell
            string powerShellCommand = $"Get-CimInstance -ClassName {className} | Select-Object {column}";

            // Выполняем команду PowerShell и получаем результат
            string powerShellOutput = RunPowerShellCommand(powerShellCommand);

            // Разделяем результат на строки по разделителю новой строки
            string[] lines = powerShellOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Пропускаем возможные пустые строки и возвращаем список значений
            List<string> values = new List<string>();
            for (int i = 2; i < lines.Length; i++)
            {
                values.Add(lines[i]);
            }

            return values;
        }
        protected static List<string> GetValueWMI(string className, string column)
        {
            // Команда PowerShell
            string powerShellCommand = $"Get-WmiObject -Class {className} | Select-Object -Property {column} | Format-Table -AutoSize";

            // Выполняем команду PowerShell и получаем результат
            string powerShellOutput = RunPowerShellCommand(powerShellCommand);

            // Разделяем результат на строки по разделителю новой строки
            string[] lines = powerShellOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Пропускаем возможные пустые строки и возвращаем список значений
            List<string> values = new List<string>();
            for (int i = 2; i < lines.Length; i++)
            {
                values.Add(lines[i]);
            }

            return values;
        }
        protected static List<string> GetPowershellValueList(string table, string column)
        {
            // Команда PowerShell
            string powerShellCommand = $"Get-{table} | Format-Table {column}";

            // Выполняем команду PowerShell и получаем результат
            string powerShellOutput = RunPowerShellCommand(powerShellCommand);

            // Разделяем результат на строки по разделителю новой строки
            string[] lines = powerShellOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Пропускаем первые две строки, содержащие разделители и заголовки
            List<string> values = new List<string>();
            for (int i = 2; i < lines.Length; i++)
            {
                values.Add(lines[i]);
            }
            // Возвращаем список строк с серийными номерами
            return values;
        }
        protected static string RunPowerShellCommand(string command)
        {
            // Создаем новый процесс для выполнения команды PowerShell
            Process process = new Process();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = command;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            // Считываем результат выполнения команды
            string output = process.StandardOutput.ReadToEnd();

            // Ждем, пока процесс завершится
            process.WaitForExit();

            // Возвращаем результат
            return output;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Monitor.Static
{
    public static class JsonReader
    {
        public static string ReadJsonFile(string filePath)
        {
            try
            {
                // Чтение содержимого файла в строку
                string jsonString = File.ReadAllText(filePath);
                return jsonString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
                return null;
            }
        }
    }
}

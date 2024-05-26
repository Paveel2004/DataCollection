using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Data_collection.Monitor.Static
{
    internal class JsonWriter
    {
        public static void WriteToJsonFile(object obj, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault // Игнорировать значения по умолчанию
            };

            try
            {
                // Сериализация объекта в JSON строку
                string jsonString = JsonSerializer.Serialize(obj, options);

                // Запись JSON строки в файл
                File.WriteAllText(filePath, jsonString);

                Console.WriteLine($"Данные успешно записаны в файл: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи в файл: {ex.Message}");
            }
        }
    }
}

using GlobalClass.Dynamic_data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Connection
{
    internal static class ServerMessageSender
    {
        public static void SendMessageUsage<T>(string serverAddress, int port, string message)
        {
            T obj = JsonConvert.DeserializeObject<T>(message);
            var type = typeof(T);
            PropertyInfo property;

            if (obj is UsageRAM)
            {
                property = type.GetProperty("Workload");
                string currentWorkload = property.GetValue(obj)?.ToString();
                if (lastUsageRam != currentWorkload)
                {
                    SendMessage(serverAddress, port, message);
                    lastUsageRam = currentWorkload;
                }
            }
            if (obj is UsageOS)
            {
                property = type.GetProperty("Status");
                string currentStatus = property.GetValue(obj)?.ToString();
                if (lastUsageOS != currentStatus)
                {
                    SendMessage(serverAddress, port, message);
                    lastUsageOS = currentStatus;
                }
            }
            if (obj is UsageCPU)
            {
                var property2 = type.GetProperty("Temperature");
                property = type.GetProperty("Workload");

                string currentTemperature = property2.GetValue(obj)?.ToString();

                string currentWorkload = property.GetValue(obj)?.ToString();

                string currentStatus = currentWorkload + '|' + currentTemperature;
                if (lastUsageCPU != currentStatus)
                {
                    SendMessage(serverAddress, port, message);
                    lastUsageCPU = currentStatus;
                }
            }
            if (obj is UsageEthernet)
            {
                property = type.GetProperty("Speed");
                string cerrentSpeed = property.GetValue(obj)?.ToString();
                if (lastEthernetSpeed != cerrentSpeed)
                {
                    SendMessage(serverAddress, port, message);
                    lastEthernetSpeed = cerrentSpeed;
                }
            }
            if (obj is UsageDisk)
            {
                property = type.GetProperty("FreeSpace");
                string cerrentFreeSpace = property.GetValue(obj)?.ToString();
                if (lastUsageDisk != cerrentFreeSpace)
                {
                    SendMessage(serverAddress, port, message);
                    lastUsageDisk = cerrentFreeSpace;
                }
            }

        }
        private static string lastUsageDisk = null;
        private static string lastEthernetSpeed = null;
        private static string lastUsageRam = null;
        private static string lastUsageOS = null;
        private static string lastUsageCPU = null;
        public static void SendMessage(string serverAddress, int port, string message)
        {
            try
            {
                // Создаем TcpClient и подключаемся к серверу
                using TcpClient client = new TcpClient(serverAddress, port);
                Console.WriteLine($"Подключено к серверу на порту {port}...");

                // Получаем поток для обмена данными с сервером
                using NetworkStream stream = client.GetStream();

                // Отправляем сообщение серверу
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Console.WriteLine($"Отправлено сообщение: {message}");

                // Читаем ответ от сервера
                data = new byte[256];
                int bytesRead = stream.Read(data, 0, data.Length);
                string response = Encoding.UTF8.GetString(data, 0, bytesRead);
                Console.WriteLine($"Ответ от сервера: {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

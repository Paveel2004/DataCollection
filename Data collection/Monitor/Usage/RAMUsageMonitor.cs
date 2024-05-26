using Data_collection.Gatherer;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Data_collection.Monitor.Usage
{
    public static class RAMUsageMonitor
    {
        private static System.Timers.Timer _timer; // Таймер для выполнения мониторинга
        private static double _totalRAMUsage = 0; // Общее использование оперативной памяти
        private static int _numSamples = 0; // Количество собранных образцов
        private static object _lock = new object(); // Объект блокировки для потокобезопасности
        public static int monitoringInterval { get; set; } = 60000; // Интервал мониторинга по умолчанию (1 минута)

        // Метод для запуска мониторинга
        public static void StartMonitoring()
        {
            _timer = new System.Timers.Timer(monitoringInterval); // Создание таймера с указанным интервалом
            _timer.Elapsed += OnTimedEvent; // Подписываемся на событие Elapsed, которое вызывается по истечении интервала
            _timer.AutoReset = true; // Устанавливаем автоповторение таймера
            _timer.Enabled = true; // Включаем таймер
        }

        // Обработчик события Elapsed таймера
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            double ramUsage = InformationGathererRAM.GetUsageRam(); // Получаем текущее использование оперативной памяти

            lock (_lock) // Блокируем доступ к общим данным из разных потоков
            {
                _totalRAMUsage += ramUsage; // Добавляем текущее использование к общей сумме
                _numSamples++; // Увеличиваем счетчик собранных образцов
            }

            if (_numSamples == 1) // Если прошла одна минута
            {
                double averageRAMUsage;

                lock (_lock) // Блокируем доступ к общим данным
                {
                    averageRAMUsage = _totalRAMUsage / _numSamples; // Вычисляем среднее использование памяти
                    _totalRAMUsage = 0; // Обнуляем сумму использования
                    _numSamples = 0; // Сбрасываем счетчик образцов
                }
                ExecuteDatabaseQuery(averageRAMUsage); // Выполняем запрос к базе данных с полученным средним значением использования
            }
        }

        // Метод для выполнения запроса к базе данных
        private static void ExecuteDatabaseQuery(double averageRAMUsage)
        {
            // Выполняем запрос к базе данных для записи данных об использовании памяти
            string dateTimeString = DateTime.Now.ToString("s");
            DataBaseHelper.Query($"EXECUTE ДобавитьИспользование @СерийныйНомерBIOS='{InformationGathererBIOS.GetBiosSerialNumber()}', @ТипХарактеристики = 'ОЗУ', @Характеристика = 'Загруженность', @Значение = '{averageRAMUsage}', @ДатаВремя = '{dateTimeString}'");

        }

    }
}

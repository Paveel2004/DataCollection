using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Control
{
    internal static class WebControl
    {
        private static string path = @"C:\Windows\System32\drivers\etc\hosts";
        public static void BlockedWebsite(StringBuilder address)
        {
            StringBuilder record = new("127.0.0.1 ");
            record.Append(address.ToString());

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(record);
            }
        }
        public static void UnBlockedWebSite(string address)
        {
            var lines = File.ReadAllLines(path).ToList();
            lines.RemoveAll(line => line.Contains(address));
            File.WriteAllLines(path, lines);
        }
    }
}

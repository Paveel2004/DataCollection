using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PcapDotNet.Core;
using PcapDotNet.Core.Extensions;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.Transport;
namespace Data_collection
{
    static public class Sniffer
    {
        public class DeviceInfo
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string MacAddress { get; set; }
        }
        private static IList<LivePacketDevice> devices;
        private static Regex name = new Regex("'(.*?)'");
        public static List<string> GetNameNetworkInterfase()
        {
            var matchedNames = new List<string>();
            foreach (var device in devices)
            {
                Match match = name.Match(device.Description);
                if (match.Success)
                {
                    matchedNames.Add(match.Groups[1].Value);
                }
            }
            return matchedNames;
        }
        public static List<DeviceInfo> DevicesInfo()
        {
            var deviceInfoList = new List<DeviceInfo>();
            try
            {
                foreach (LivePacketDevice i in devices)
                {
                    DeviceInfo deviceInfo = new()
                    {
                        Name = i.Name,
                        Description = i.Description,
                        MacAddress = i.GetMacAddress().ToString()
                    };
                    deviceInfoList.Add(deviceInfo);
                }
                return deviceInfoList;
            }
            catch { return deviceInfoList; }
        }


        static Sniffer()
        {
            devices = LivePacketDevice.AllLocalMachine;
            if (devices.Count == 0)
            {
                throw new Exception("Устройства не найдены! Убедитесь, что вы работаете от имени администратора.");
            }
        }
        static void PrintPacketDetails(Packet packet)
        {
            IpV4Datagram ip = packet.Ethernet.IpV4;
            UdpDatagram udp = ip.Udp;

            if (udp != null && udp.DestinationPort == 53)
            {
                var dns = udp.Dns;

                foreach (var query in dns.Queries)
                {
                    Console.WriteLine($"Domain: {query.DomainName} | Time: {packet.Timestamp}");
                }
            }
        }
        public static void Start(string deviceName)
        {
            var device = devices.FirstOrDefault(d => name.Match(d.Description).Groups[1].Value == deviceName);
            if (device != null)
            {
                using (var communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    // Захватываем пакеты
                    communicator.ReceivePackets(0, PrintPacketDetails);
                }
            }
            else
            {
                throw new Exception("Устройство не найдено");
            }
        }
    }
}

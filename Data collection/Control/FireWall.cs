using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetFwTypeLib;
namespace Data_collection.Control
{
    internal class FireWall
    {
        public  static void BlockDomain(string domain)
        {
            // Получение всех IP-адресов для домена
            IPAddress[] addresses = Dns.GetHostAddresses(domain);

            // Создание экземпляра для управления правилами брандмауэра
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            foreach (var address in addresses)
            {
                // Создание и настройка правила для входящего трафика
                INetFwRule inboundRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                inboundRule.Name = $"Block {domain} (IN - {address})";
                inboundRule.Description = $"Block inbound traffic for {address}";
                inboundRule.RemoteAddresses = address.ToString();
                inboundRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                inboundRule.Enabled = true;

                // Добавление правила в брандмауэр
                firewallPolicy.Rules.Add(inboundRule);

                // Создание и настройка правила для исходящего трафика
                INetFwRule outboundRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                outboundRule.Name = $"Block {domain} (OUT - {address})";
                outboundRule.Description = $"Block outbound traffic for {address}";
                outboundRule.RemoteAddresses = address.ToString();
                outboundRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
                outboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                outboundRule.Enabled = true;

                // Добавление правила в брандмауэр
                firewallPolicy.Rules.Add(outboundRule);
            }
        }
        public static void UnblockDomain(string domain)
        {
            // Получение всех IP-адресов для домена
            IPAddress[] addresses = Dns.GetHostAddresses(domain);

            // Создание экземпляра для управления правилами брандмауэра
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            foreach (var address in addresses)
            {
                // Удаление правила для входящего трафика
                firewallPolicy.Rules.Remove($"Block {domain} (IN - {address})");

                // Удаление правила для исходящего трафика
                firewallPolicy.Rules.Remove($"Block {domain} (OUT - {address})");

                Console.WriteLine($"Unblocked domain {domain} (IP: {address})");
            }
        }
    }
}

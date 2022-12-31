using System.Diagnostics;
using Microsoft.Win32;
using System.Net.NetworkInformation;

namespace NICSpeedChanger
{
    public class Program
    {
        private static readonly int Value = int.Parse(Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Class\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\0001").GetValue("*SpeedDuplex").ToString());
        
        private static void Main()
        {
            switch (Value)
            {
                case 0:
                    ChangeSpeed(1);
                    Console.WriteLine("Speed changed to 10Mbps");
                    break;
                case 1:
                    ChangeSpeed(0);
                    Console.WriteLine("Speed changed to auto negotiate");
                    break;
                default:
                    ChangeSpeed(1);
                    Console.WriteLine("Speed changed to 10Mbps");
                    break;
            }
            
            ResetInterface();
        }
        
        private static void ChangeSpeed(int speed)
        {
            Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Control\\Class\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\0001").SetValue("*SpeedDuplex", speed, RegistryValueKind.String);
        }
        
        private static void ResetInterface()
        {
            Process.Start("netsh", "interface set interface \"Ethernet\" disable");
            Thread.Sleep(1000);
            
            while (IsEthernetUp())
            {
                Thread.Sleep(1000);
                Console.WriteLine("Waiting for interface to go down...");
            }
            
            Thread.Sleep(1000);
            Process.Start("netsh", "interface set interface \"Ethernet\" enable");
        }
        
        private static bool IsEthernetUp()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.Name == "Ethernet")
                {
                    if (ni.OperationalStatus == OperationalStatus.Up)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
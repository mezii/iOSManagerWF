using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoIt;
namespace AutoManager
{
    public class Client911
    {


        public static string CmdProcess(string PathFileOpen, string code)
        {
            string result;
            try
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo = new ProcessStartInfo("cmd.exe")
                    {
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    StringBuilder output = new StringBuilder();
                    StringBuilder error = new StringBuilder();
                    proc.OutputDataReceived += delegate (object o, DataReceivedEventArgs ef)
                    {
                        output.Append(ef.Data + "\n");
                    };
                    proc.ErrorDataReceived += delegate (object o, DataReceivedEventArgs ef)
                    {
                        error.Append(ef.Data + "\n");
                    };
                    proc.Start();
                    proc.StandardInput.WriteLine(PathFileOpen.Substring(0, 2));
                    proc.StandardInput.WriteLine("cd " + PathFileOpen);
                    proc.StandardInput.WriteLine(code);
                    proc.StandardInput.WriteLine("exit");
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();
                    //    proc.WaitForExit();
                    proc.Dispose();
                    proc.Close();
                    result = output.ToString();
                }
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }
        public static void Change(string country,int port)
        {
           CmdProcess("./ChangeProxy/ProxyTool/", "ProxyAPI.exe -changeproxy/" + country + " -proxyport=" + port);
        }



        public static void RunClient(string ipadress,string country, int port)
        {
            int icheck = 0;
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {
                string tile = process.MainWindowTitle;

                if (!String.IsNullOrEmpty(tile))
                {

                    if (tile.ToLower().Contains("911 S5 3.26"))
                    {
                        icheck = 1;
                       
                    }
                }
            }
           
                Task t = new Task(() =>
                {
                    if (icheck == 0)
                    {
                        CmdProcess("./ChangeProxy/", "Client.exe");
                        Thread.Sleep(7000);
                        AutoItX.WinActivate("911 S5 3.26", "");
                        AutoItX.ControlClick("911 S5 3.26", "", "[CLASS:ThunderRT6CommandButton; INSTANCE:4]");
                        Thread.Sleep(7000);
                        AutoItX.WinClose("Annoucement");
                        Thread.Sleep(3000);
                        Change(country, port);
                        DeviceConnect dv = new DeviceConnect();
                        string ipPC = getIPAddrees();
                        dv.SetProxy(ipadress, ipPC, port, true);
                    }
                    else
                    {
                        Change(country, port);
                    }
                });
                t.Start();
           
            
        }


        public static string getIPAddrees()
        {
            
                UnicastIPAddressInformation mostSuitableIp = null;

                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var network in networkInterfaces)
                {
                    if (network.OperationalStatus != OperationalStatus.Up)
                        continue;

                    var properties = network.GetIPProperties();

                    if (properties.GatewayAddresses.Count == 0)
                        continue;

                    foreach (var address in properties.UnicastAddresses)
                    {
                        if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;

                        if (IPAddress.IsLoopback(address.Address))
                            continue;

                        if (!address.IsDnsEligible)
                        {
                            if (mostSuitableIp == null)
                                mostSuitableIp = address;
                            continue;
                        }

                        // The best IP is the IP got from DHCP server
                        if (address.PrefixOrigin != PrefixOrigin.Dhcp)
                        {
                            if (mostSuitableIp == null || !mostSuitableIp.IsDnsEligible)
                                mostSuitableIp = address;
                            continue;
                        }

                        return address.Address.ToString();
                    }
                }

                return mostSuitableIp != null
                    ? mostSuitableIp.Address.ToString()
                    : "";
            
           
        }
    }
}

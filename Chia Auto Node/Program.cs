using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chia_Auto_Node
{
    class Program
    {
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public static string Get(string uri)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                string current_time = DateTime.Now.ToString("h:mm:ss tt");
                Console.WriteLine("[" + current_time + "][Error] Fetch data error.");
                return "";
            }
        }

        public static bool PingHost(string hostUri, int portNumber)
        {
            try
            {
                TcpClient tcpClient = new TcpClient();
                if (tcpClient.ConnectAsync(hostUri, portNumber).Wait(TimeSpan.FromMilliseconds(400)))
                    return true;
                else
                    return false;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        static void Main(string[] args)
        {
            Console.Title = "iDisk Thailand - Auto Add Full Node";
            Console.WriteLine("------------------------------------------------------------------------------");
            Console.WriteLine("iDisk Thailand - Auto Add Full Node");
            Console.WriteLine("Fanpage - https://www.facebook.com/IDISKThailand");
            Console.WriteLine("------------------------------------------------------------------------------");

            var FileExecute = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToString();
            if (Directory.Exists(FileExecute + "\\chia-blockchain"))
            {
                string[] DirSeacrhAppChia = Directory.GetDirectories(FileExecute + "\\chia-blockchain", "app*", SearchOption.TopDirectoryOnly);
                if (DirSeacrhAppChia.Length != 1)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("h:mm:ss tt") + "][System]Please re-install Chia-Blockchian because program broken!");
                }
                else
                {
                    string AppChia = "";
                    foreach (string DirAppChia in DirSeacrhAppChia)
                    {
                        AppChia = DirAppChia;
                        break;
                    }
                    AppChia = AppChia + "\\resources\\app.asar.unpacked\\daemon";

                    while (true)
                    {
                        string current_time = DateTime.Now.ToString("h:mm:ss tt");
                        Console.WriteLine("[" + current_time + "][System] Fetch new node from iDisk Thailand");
                        string RespNode = Get("https://chia-thailand.com/nodes/api.php?t=" + GetTimestamp(new DateTime()));
                        dynamic JsonResp = JsonConvert.DeserializeObject(RespNode);
                        foreach (dynamic Data in JsonResp)
                        {
                            Console.WriteLine("[" + current_time + "][System] Node - " + Data.IP + ", pending");

                            string[] SplitIPData = ((string)Data.IP).Split(':');
                            if (PingHost(SplitIPData[0], Int32.Parse(SplitIPData[1])))
                            {
                                Console.WriteLine("[" + current_time + "][System] Node - " + Data.IP + ", connecting");

                                Process p = new Process();
                                p.StartInfo = new ProcessStartInfo("chia.exe");
                                p.StartInfo.WorkingDirectory = AppChia;
                                p.StartInfo.Arguments = "show -a " + Data.IP;
                                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                p.Start();
                            }
                            else
                            {
                                Console.WriteLine("[" + current_time + "][System] Node - " + Data.IP + ", dead node");
                            }
                            Thread.Sleep(2000);
                        }
                        Thread.Sleep(30000);
                    }
                }
            }
            else
            {
                Console.WriteLine("[" + DateTime.Now.ToString("h:mm:ss tt") + "][System]Please install Chia-Blockchian on you computer");
            }
        }
    }
}

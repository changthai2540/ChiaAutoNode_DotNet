using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

        static void Main(string[] args)
        {
            Console.Title = "iDisk Thailand - Auto Add Full Node";
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("iDisk Thailand - Auto Add Full Node");
            Console.WriteLine("-----------------------------------");

            string chia_exec_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\chia-blockchain\\app-1.1.5\\resources\\app.asar.unpacked\\daemon";
            
            while(true)
            {
                string current_time = DateTime.Now.ToString("h:mm:ss tt");
                Console.WriteLine("[" + current_time + "][System] Fetch new node from iDisk");
                string RespNode = Get("https://chia-thailand.com/nodes/api.php?t=" + GetTimestamp(new DateTime()));
                dynamic JsonResp = JsonConvert.DeserializeObject(RespNode);
                foreach(dynamic Data in JsonResp)
                {
                    Console.WriteLine("[" + current_time + "][System] Try to add node - " + Data.IP);

                    Process p = new Process();
                    p.StartInfo = new ProcessStartInfo("chia.exe");
                    p.StartInfo.WorkingDirectory = chia_exec_path;
                    p.StartInfo.Arguments = "show -a " + Data.IP;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.Start();
                }

                Thread.Sleep(30000);
            }
        }
    }
}

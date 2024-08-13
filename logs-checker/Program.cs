using System;
 using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;

namespace logs_checker
{

    internal class Program
    
    {

        static string EncodeRsa(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            int[] publicKey = { 923, 367 };
            List<string> encodedData = new List<string>();
            foreach (byte b in bytes)
            {
                //Console.Write(charInt + ", ");
                BigInteger encoded = BigInteger.ModPow((BigInteger)b, publicKey[1], publicKey[0]);
                encodedData.Add(encoded.ToString());

            }
            string output = "";

            for (int i = 0; i < encodedData.Count; i++)
            {
                if (i != encodedData.Count - 1)
                {
                    output += encodedData[i] + ",";
                } else
                {
                    output += encodedData[i];
                }
            }

            return output;

        }
        static string DecodeRsa(int[] data)
        {
            int[] private_key = { 923, 103 };
            List<byte> bytes = new List<byte>();
            foreach (int i in data) 
            {
                BigInteger temp =  BigInteger.ModPow((BigInteger)i,private_key[1], private_key[0]);
                bytes.Add((byte)temp);
       
            }
   
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        static string GetCommandOutput(string command)
        {

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c " + command;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = false;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string result = output;
            process.WaitForExit();

            return result;

        }


        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Default;
            string command1 = "wevtutil qe system /q:*[System[(EventID=6008)]] /rd:true /f:text /c:5";
            string command2 = "wevtutil qe system /q:*[System[(EventID=41)]] /rd:true /f:text /c:5";

            string result = GetCommandOutput(command1) + "\n" + GetCommandOutput(command2);
            var p = EventLog.GetEventLogs();
            string logName = "System";


            string outputToBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(result));



            File.WriteAllText("logs.ds", outputToBase64);

        }
    }
}

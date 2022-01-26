using System;
using System.Collections.Generic;
using System.Linq;
using MemoryHacking;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SpeedByte
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("SpeedByte by TobyAdd");
            Thread.Sleep(1000);
            if (args.Length < 4)
            {
                Console.WriteLine($"Usage: {new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Name} <process> <module> <offset> <data[0]> <data[1]> <data[...]>");
                if (args.Length < 1) Console.WriteLine("No process name specified");
                if (args.Length < 2) Console.WriteLine("No process module name specified");
                if (args.Length < 3) Console.WriteLine("No offset address specified");
                Console.WriteLine("No data specified");
                return;
            }

            string processName = args[0];
            string moduleName = args[1];
            IntPtr address;

            try // даунский поступок
            {
                address = new IntPtr(int.Parse(args[2].Replace("0x", ""), NumberStyles.HexNumber));
            }
            catch
            {
                Console.WriteLine("Address isn't correct");
                return;
            }

            List<byte> bytes2write = new List<byte>();
            for (int dataByteIndex = 3; dataByteIndex < args.Length; dataByteIndex++)
                bytes2write.Add(byte.Parse(args[dataByteIndex].Replace("0x", ""), NumberStyles.HexNumber));

            Process[] processes = Process.GetProcessesByName(processName);
            
            if (!processes.Any())
            {
                Console.WriteLine("Process not found");
                return;
            }

            Memory memory = new Memory(processes[0]);
            var memoryRegion = memory.GetModuleByName(moduleName);

            if (memoryRegion == null)
            {
                Console.WriteLine("Module not found");
                return;
            }

            try
            {
                memory.Write(memoryRegion.BaseAddress + address.ToInt32(), bytes2write.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to write bytes: " + ex.Message);
                return;
            }


            Console.WriteLine("Success");
            Thread.Sleep(1000);

        }
    }
}

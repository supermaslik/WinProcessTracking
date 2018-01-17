using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinProcessTracking;

namespace WinProcessTracking
{
    class Program
    {
        static void Main(string[] args)
        {
            TimerCallback tm = new TimerCallback(MethodCall);
            Timer timer = new Timer(tm, null, 0, 20000);
            Console.ReadKey();
        }


        static public void MethodCall(object obj)
        {
                Console.Clear();
                Process[] processes = Process.GetProcesses();
                var counters = new List<PerformanceCounter>();
                DeclaringProcessesCounters(counters, processes);
                TypingResultToConsole(counters, processes);

        }







        private static void TypingResultToConsole(List<PerformanceCounter> counters, Process[] processes)
        {
            //Thread.Sleep(1000);
            int i = 0;
            string wholeMessage = "";
            float prsentOfCpuUsage = 0;
            foreach (var counter in counters)
            {
                var currentCpuUsage = counter.NextValue();
                prsentOfCpuUsage += currentCpuUsage;
                   wholeMessage += string.Format("ID: {0,3}  Name: {1,64}, RamUsage(MB): {2,4} |CPU% = {3}", processes[i].Id, processes[i].ProcessName, RamUsage(processes[i]), (Math.Round(currentCpuUsage, 3)));
                wholeMessage += "\n";
                ++i;

            }
            Console.WriteLine(wholeMessage);
            Console.WriteLine(prsentOfCpuUsage);
            Console.WriteLine(Environment.ProcessorCount);
        }

        private static void DeclaringProcessesCounters(List<PerformanceCounter> counters, Process[] processes)
        {
            foreach (Process process in processes)
            {
                var counter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName);
                counter.NextValue();
                counters.Add(counter);
            }
        }

        static public int RamUsage(Process proc)
        {
            int size = 0; // memsize in Kbyte
            PerformanceCounter PC = new PerformanceCounter();
            PC.CategoryName = "Process";
            PC.CounterName = "Working Set - Private";
            PC.InstanceName = proc.ProcessName;
            size = Convert.ToInt32(PC.NextValue()) / (int)(1024);
            PC.Close();
            PC.Dispose();
            return size / 1000; //Mbyte is retur value
        }

    }
}

using System.Diagnostics;
using System.Text;

namespace 记录资源消耗
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("开始记录资源消耗");
            string processName = "test";
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length == 0)
            {
                Console.WriteLine($"未找到名为 {processName} 的进程。");
                return;
            }

            Process currentProcess = processes[0];

            while (!currentProcess.HasExited)
            {
                TimeSpan startCpuUsage = currentProcess.TotalProcessorTime;
                DateTime startTime = DateTime.Now;

                // 等待一段时间以计算 CPU 使用率
                Thread.Sleep(100);

                // 获取当前 CPU 时间
                TimeSpan endCpuUsage = currentProcess.TotalProcessorTime;
                DateTime endTime = DateTime.Now;

                // 计算 CPU 使用率
                double cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                double totalMsPassed = (endTime - startTime).TotalMilliseconds;
                double cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

                // 获取内存使用情况
                long memoryUsage = currentProcess.WorkingSet64;

                WriteStringToCsv(cpuUsageTotal * 100, memoryUsage / 1024 / 1024);
            }
            // 获取初始 CPU 时间


        }
        //记录csv信息到文件
        public static void WriteStringToCsv(double cpu, double mem)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}   {cpu:f2}   {mem:f2}");
            WriteLog($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")},{cpu:f2},{mem:f2}");

        }
        //记录信息到某个文件夹
        public static void WriteLog(string log)
        {
            string path = @".\data.csv";
            //文件不存在则创建
            if (!File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    //添加csv的头信息 时间,cpu,内存
                    sw.WriteLine("时间,cpu(%),内存(MB)");
                }
            }

            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
            {
                sw.WriteLine(log);
            }
        }
    }

}

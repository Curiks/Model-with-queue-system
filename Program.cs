using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MS_LW1
{
    class Program
    {
        private double Tf = 500;
        private double tm = 0;
        private double L1 = 0;
        private double L2 = 0;
        private double h = 501;
        private double nextL1, nextL2;
        private int reqL1, reqL2;
        private int queueL1, queueL2, queueLeftL1, queueLeftL2;
        private int serverL1, serverL2, queueEmptyL1, queueEmptyL2;
        private Server server = new Server();
        private Queue queue = new Queue();

        static void Main(string[] args)
        {
            Console.WriteLine("Distributions: Gamma, Poisson, Normal, Exponential\n");
            Console.WriteLine("Event" + "\t\t" + "| Model time" + ", " + "L1" + ", " +
                "L2" + ", " + "Processing time" + ", " + "Server status" + ", " + "Queue content");
            Console.WriteLine("------------------------------------------------------------------" +
                "-----------------");

            Program program = new Program();
            program.WriteToFile();
            program.Init();
            program.Proceed();

            Console.ReadLine();
        }

        private void Proceed()
        {
            if (server.Available)
            {
                CountServerSleep();
            }

            tm = Min(L1, L2, h, Tf);

            if (tm == L1 || tm == L2)
            {
                if (tm == L1)
                {
                    nextL1 = tm + GetL1();

                    if (server.Available)
                    {
                        server.Available = false;
                        h = tm + SleepL1();
                        serverL1++;
                        queueEmptyL1++;

                        Console.WriteLine("L1 on server" + "\t| " + Math.Round(tm, 3) + ", " + Math.Round(nextL1, 3) +
                            ", - , " + Math.Round(h, 3) + ", " + server.Available + ", Q:" + queue.Size + " L1:" +
                            queue.CountObj(typeof(L1)) + " L2:" + queue.CountObj(typeof(L2)));
                    }
                    else
                    {
                        queue.Put(new L1());
                        queueL1++;

                        Console.WriteLine("L1 in queue" + "\t| " + Math.Round(tm, 3) + ", " + Math.Round(nextL1, 3) +
                            ", - , " + Math.Round(h, 3) + ", " + server.Available + ", Q:" + queue.Size + " L1:" +
                            queue.CountObj(typeof(L1)) + " L2:" + queue.CountObj(typeof(L2)));
                    }

                    L1 = nextL1;
                    reqL1++;
                }
                else
                {
                    nextL2 = tm + GetL2();

                    if (server.Available)
                    {
                        server.Available = false;
                        h = tm + SleepL2();
                        serverL2++;
                        queueEmptyL2++;

                        Console.WriteLine("L2 on server" + "\t| " + Math.Round(tm, 3) + ", - , " + Math.Round(nextL2, 3) +
                            ", " + Math.Round(h, 3) + ", " + server.Available + ", Q:" + queue.Size + " L1:" +
                            queue.CountObj(typeof(L1)) + " L2:" + queue.CountObj(typeof(L2)));
                    }
                    else
                    {
                        queue.Put(new L2());
                        queueL2++;

                        Console.WriteLine("L2 in queue" + "\t| " + Math.Round(tm, 3) + ", - , " + Math.Round(nextL2, 3) +
                            ", " + Math.Round(h, 3) + ", " + server.Available + ", Q:" + queue.Size + " L1:" +
                            queue.CountObj(typeof(L1)) + " L2:" + queue.CountObj(typeof(L2)));
                    }

                    L2 = nextL2;
                    reqL2++;
                }

                Proceed();
            }
            else if (tm == h)
            {
                if (queue.Empty)
                {
                    h = 501;
                    server.Available = true;
                }
                else
                {
                    QueueObject obj = queue.Pop();

                    if (obj.GetType() == typeof(L1))
                    {
                        h = tm + SleepL1();
                        queueLeftL1++;
                        serverL1++;

                        Console.WriteLine("L1 from queue" + "\t| " + Math.Round(tm, 3) + ", - , - , " +
                            Math.Round(h, 3) + ", " + server.Available + ", Q:" + queue.Size + " L1:" +
                            queue.CountObj(typeof(L1)) + " L2:" + queue.CountObj(typeof(L2)));
                    }
                    else if (obj.GetType() == typeof(L2))
                    {
                        h = tm + SleepL2();
                        queueLeftL2++;
                        serverL2++;

                        Console.WriteLine("L2 from queue" + "\t| " + Math.Round(tm, 3) + ", - , - , " +
                            Math.Round(h, 3) + ", " + server.Available + ", Q:" + queue.Size + " L1:" +
                            queue.CountObj(typeof(L1)) + " L2:" + queue.CountObj(typeof(L2)));
                    }
                }

                Proceed();
            }
            else
            {
                SummaryStats();
            }
        }

        private void Init()
        {
            L1 = GetL1();
            L2 = GetL2();

            Console.WriteLine("Start" + "\t\t| " + Math.Round(tm, 3) + ", " + Math.Round(L1, 3) + ", " +
                Math.Round(L2, 3) + ", " + h + ", " + server.Available + ", " + "Q:" + queue.Size +
                " L1:" + queue.CountObj(L1.GetType()) + " L2:" + queue.CountObj(L2.GetType()));
        }

        private void SummaryStats()
        {
            Console.WriteLine("------------------------------------------------------------------" +
                "-----------------");
            Console.WriteLine("\n\tИТОГОВАЯ СТАТИСТИКА");
            Console.WriteLine("L1: " + reqL1);
            Console.WriteLine("L2: " + reqL2);
            Console.WriteLine("Всего заявок: " + (reqL1 + reqL2));
            Console.WriteLine("---------Очередь---------------");
            Console.WriteLine("Вхождений L1 в очередь: " + queueL1);
            Console.WriteLine("Вхождений L2 в очередь: " + queueL2);
            Console.WriteLine("Всего вхождений в очередь: " + (queueL1 + queueL2) + "\n");
            Console.WriteLine("Осталось в очереди L1: " + (queueL1 - queueLeftL1));
            Console.WriteLine("Осталось в очереди L2: " + (queueL2 - queueLeftL2));
            Console.WriteLine("Всего осталось в очереди: " + ((queueL1 - queueLeftL1) + (queueL2 - queueLeftL2)));
            Console.WriteLine("---------Сервер---------------");
            Console.WriteLine("Вхождений L1 без очереди: " + (queueEmptyL1));
            Console.WriteLine("Вхождений L2 без очереди: " + (queueEmptyL2));
            Console.WriteLine("Всего вхождений без очереди: " + (queueEmptyL1 + queueEmptyL2) + "\n");
            Console.WriteLine("Всего вхождений L1: " + serverL1);
            Console.WriteLine("Всего вхождений L2: " + serverL2);
            Console.WriteLine("Всего вхождений на сервер: " + (serverL1 + serverL2));
            Console.WriteLine("\nКоэффициэнт простоя сервера: " + Math.Round(server.SleepTime / 501, 3));
            Console.WriteLine("Среднее число заявок, стоявших в очереди: " + queue.AvgInQueue);
        }

        private static double Min(double a, double b, double c, double d)
        {
            return Math.Min(Math.Min(a, b), Math.Min(c, d));
        }

        private void CountServerSleep()
        {
            server.SleepTime += Min(L1, L2, h, Tf) - tm;
        }

        private static double GetL1()
        {
            return Distribution.Gamma(3, 0.25);
        }

        private static double GetL2()
        {
            return Distribution.Poisson(0.5);
        }

        private static double SleepL1()
        {
            return Distribution.Normal(12, 2);
        }

        private static double SleepL2()
        {
            return Distribution.Exponential(2);
        }

        private void WriteToFile()
        {
            string[] file_paths = new string[]
            {
                @"C:\Users\Curiks\Documents\Visual Studio 2013\Projects\MS_LW1_solution\MS_LW1\Exponential.txt",
                @"C:\Users\Curiks\Documents\Visual Studio 2013\Projects\MS_LW1_solution\MS_LW1\Normal.txt",
                @"C:\Users\Curiks\Documents\Visual Studio 2013\Projects\MS_LW1_solution\MS_LW1\Gamma.txt"
            };

            TextWriter writerExp = File.CreateText(file_paths[0]);
            TextWriter writerNorm = File.CreateText(file_paths[1]);
            TextWriter writerGamma = File.CreateText(file_paths[2]);

            for (int i = 0; i < 500; i++)
            {
                writerExp.WriteLine(Math.Round(Distribution.Exponential(2), 3));
                writerNorm.WriteLine(Math.Round(Distribution.Normal(12, 2), 3));
                writerGamma.WriteLine(Math.Round(Distribution.Gamma(3, 0.25), 3));
            }

            writerExp.Close();
            writerNorm.Close();
            writerGamma.Close();
        }
    }
}

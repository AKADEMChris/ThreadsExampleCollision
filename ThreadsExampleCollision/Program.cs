using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadsExampleCollision
{
    internal class Program
    {
        // ф-ия, генерирующая массив 
        static int[] GenerateRandomArr(int n, int min, int max, Random r)
        {
            int[] arr = new int[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = r.Next(min, max + 1);
            }
            return arr;
        }

        // однопоточная сумма
        static int SumArr(int[] arr)
        {
            return arr.Sum();
        }
        // многопоточная сумма, n - кол-во потоков
        static int SumArr(int[] arr, int n)//n-колво потоков
        {
            object obj = new object();
            int sum = 0;

            // 1. ПОДГОТОВИТЬ ПОТОКИ И СРАЗУ ЗАПУСТИТЬ
            int step = arr.Length / n;  // сколько элементов суммируется одним потоком
            List<Thread> threads = new List<Thread>();
            for (int k = 0; k < n; k++)
            {
                int start = k * step;
                int end = start + step;
                if (k == n - 1)
                {
                    end = arr.Length;
                }
                Thread thread = new Thread(() =>
                {
                    lock (obj)
                    {
                        int localSum = SumArr(arr, start, end);//??                      
                        Interlocked.Add(ref sum, localSum);
                    }
                });
                thread.Start();
                threads.Add(thread);
            }
            // 2. ДОЖДАТЬСЯ ПОТОКОВ
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            return sum;
        }
        static int SumArr(int[] arr, int start, int end)
        {
            int sum = 0;
            for (int i = start; i < end; i++)
            {
                sum += arr[i];
            }
            return sum;
            //return arr.Skip(start).Take(end).Sum();//неправильно отрабатывала 
        }
        static void Main(string[] args)
        {
            Random r = new Random();
            int n = 500, min = 1, max = 9;
            int[] arr = GenerateRandomArr(n, min, max, r);
            //-------------------------------------------
            int sum = SumArr(arr);//однопоточная
            int sumN = SumArr(arr, 8);//многопоточная
            if (sum == sumN)
            {
                Console.WriteLine($"ok: {sum}");
            }
            else
            {
                Console.WriteLine($"{sum} != {sumN}");
            }
        }
    }
}

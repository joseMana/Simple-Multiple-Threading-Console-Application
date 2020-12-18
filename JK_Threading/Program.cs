using System;
using System.Collections.Generic;
using System.Threading;

namespace JK_Threading
{
    class Program
    {
        static Queue<int> numbers = new Queue<int>();
        static Random rand = new Random(987);
        const int numThreads = 3;
        static int[] sums = new int[numThreads];
        static void Main(string[] args)
        {
            var producingThread = new Thread(ProduceNumbers);
            producingThread.Start();
            Thread[] threads = new Thread[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                threads[i] = new Thread(SumNumbers);
                threads[i].Start(i);
            }

            for (int i = 0; i < numThreads; i++)
                threads[i].Join();

            int totalSum = 0;
            for (int i = 0; i < numThreads; i++)
                totalSum += sums[i];

            Console.WriteLine("Done adding. Total is " + totalSum);
        }
        static void ProduceNumbers()
        {
            for (int i = 0; i < 10; i++)
            {
                int numToEnqueue = rand.Next(10);
                Console.WriteLine("Producing thread adding " + numToEnqueue + "to the queue!");

                lock(numbers)
                    numbers.Enqueue(numToEnqueue);

                Thread.Sleep(rand.Next(1000));
            }
        }
        static void SumNumbers(object threadNumber)
        {
            DateTime startTime = DateTime.Now;
            int mySum = 0;
            while ((DateTime.Now - startTime).Seconds < 11)
            {
                int numToSum = -1;
                lock (numbers)
                {
                    if (numbers.Count != 0)
                    {
                        numToSum = numbers.Dequeue();
                    }
                }

                if(numToSum != -1)
                {
                    mySum += numToSum;
                    Console.WriteLine("Consuming thread # " + threadNumber + " adding " + numToSum + " to its total sum making " + mySum + " for the thread total.");
                }
            }
            sums[(int)threadNumber] = mySum;
        }
    }
}

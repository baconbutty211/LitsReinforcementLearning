using System;
using System.Threading;

namespace SimpleLitsMadeSimpler
{
    class Program
    {
        static void Main(string[] args)
        {
            Agent bond = new Agent();
            bond.Explore();
            foreach (string stateStr in bond.DisplayOptimumPath())
            {
                Console.Clear();
                Console.WriteLine(stateStr);
                Thread.Sleep(1000);
            }
        }
    }
}

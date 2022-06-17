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
            bond.Exploit();

            string prevStr = Environment.ToString(new Environment().Reset().state);
            foreach (string stateStr in bond.DisplayOptimumPath())
            {
                Console.Clear();
                for (int i = 0; i < prevStr.Length; i++)
                {
                    if (stateStr[i] != prevStr[i])
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(stateStr[i]);
                        Console.ResetColor();
                    }
                    else
                        Console.Write(stateStr[i]);
                }
                prevStr = stateStr;
                Thread.Sleep(2000);
            }
        }
    }
}

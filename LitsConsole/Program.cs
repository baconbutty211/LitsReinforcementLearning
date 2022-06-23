using System;
using System.IO;
using System.Threading;

namespace SimpleLitsMadeSimpler
{
    class Program
    {
        static void Main(string[] args)
        {
            Agent bond = new Agent();
            bond.Load("Bond");
            bond.Explore();
            bond.Save("Bond");
            bond.Exploit();

            //DisplayOptimumPath(bond);
        }

        static void DisplayOptimumPath(Agent agent) 
        {
            string prevStr = Environment.ToString(new Environment().Reset().state);
            foreach (string stateStr in agent.DisplayOptimumPath())
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

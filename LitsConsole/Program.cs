using System;
using System.IO;
using System.Threading;

namespace LitsReinforcementLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!int.TryParse(args[0], out int episodes))
                return;

            Log.Clear();
            
            Agent bond = new Agent();
            
            //Log.Write("Loading Agent Bond...");
            bond.Load("Bond");
            //Log.Write("...Loaded Agent Bond");
            
            //Log.Write("Exploring for 1 episode...");
            bond.Explore(episodes);
            //Log.Write("...Explored 1 episode");
            
            //Log.Write("Saving Agent Bond...");
            bond.Save("Bond");
            //Log.Write("...Saved Agent Bond");

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

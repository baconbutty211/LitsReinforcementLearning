using System;
using System.IO;
using System.Threading;

namespace LitsReinforcementLearning
{
    class Program
    {
        /// <summary>
        /// Args: [0 = Agent type], [1 = MC -> Episodes]
        /// </summary>
        static void Main(string[] args)
        {
            if (args[0] == "MC")
            {
                if (!int.TryParse(args[1], out int episodes))
                    return;
                if (episodes == -1)
                    episodes = int.MaxValue;

                Log.Clear();

                MonteCarloAgent bond = new MonteCarloAgent();

                //Log.Write("Loading Agent Bond...");
                bond.Load("Bond");
                //Log.Write("...Loaded Agent Bond");

                //Log.Write("Exploring for 1 episode...");
                bond.Explore(episodes);
                //Log.Write("...Explored 1 episode");

                //Log.Write("Saving Agent Bond...");
                bond.Save("Bond");
                //Log.Write("...Saved Agent Bond");

                int[] optimumPath = bond.Exploit();
                DisplayOptimumPath(optimumPath);
            }
            else if(args[0] == "DP") 
            {
                Log.Clear();
                DynamicProgrammingAgent powers = new DynamicProgrammingAgent();

                powers.Load("Powers");

                powers.Explore();
                
                powers.Save("Powers");

                int[] optimumPath = powers.Exploit();
                DisplayOptimumPath(optimumPath);
            }
        }

        static void DisplayOptimumPath(int[] optimumPath) 
        {
            Environment environment = new Environment();
            string prevStr = environment.ToString();
            string route = "Route: ";
            foreach (int action in optimumPath)
            {
                environment.Step(Action.GetAction(action));
                string stateStr = environment.ToString();
                
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
                route += $"{action}, ";
                Console.WriteLine(route);
                prevStr = stateStr;
                Thread.Sleep(2000);
            }
        }
    }
}

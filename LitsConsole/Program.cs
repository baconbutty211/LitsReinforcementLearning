using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace LitsReinforcementLearning
{
    public enum Verbosity { None, Low, Mid, High }
    class Program
    {
        static void Main(string[] args)
        {
            string command = args[0].ToLower();

            if (command == "create")
            {
                string agentName = args[1];
                bool isFirstPlayer = bool.Parse(args[2]);

                Console.Title = $"Creating {agentName}...";
                Trainer.CreateNewAgent(agentName, isFirstPlayer);
            }
            else if (command == "test")
            {
                string agentName1 = args[1];
                string agentName2 = args[2];

                Agent agent1 = new DynamicProgrammingAgent(agentName1);
                Agent agent2 = new DynamicProgrammingAgent(agentName2);

                Console.Title = $"Testing {agentName1} vs {agentName2}...";
                Tester.PlayGame(agent1, agent2);
            }
            else if (command == "play")
            {
                string agentName = args[1];
                Agent agent = new DynamicProgrammingAgent(agentName);

                Console.Title = $"Playing {agentName}";
                Playground.PlayGame(agent);
            }
            else if (command == "train")
            {
                string agentName1 = args[1];
                Agent agent1 = new DynamicProgrammingAgent(agentName1);

                if (!int.TryParse(args[2], out int episodes))
                {
                    Console.WriteLine("No integer value for episodes given.");
                    return;
                }

                Console.Title = $"Training {agentName1}...";
                Trainer.Train(agent1, episodes, Verbosity.Low);
                agent1.Save(agentName1);
            }
            else
            {
                Console.WriteLine("Not acceptable, try again.");
            }
        }
    }
}
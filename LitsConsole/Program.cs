﻿using System;
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
            System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
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
                string agentName = args[1];
                Agent agent = new DynamicProgrammingAgent(agentName);

                if (!int.TryParse(args[3], out int episodes))
                {
                    Console.WriteLine("No integer value for episodes given.");
                    return;
                }

                string trainTypeStr = args[2].ToLower();
                Console.Title = $"Training {agentName} {trainTypeStr}...";
                switch (trainTypeStr)
                {
                    case "async":
                        Trainer.Train(agent, episodes, Trainer.Type.Async, Verbosity.Low);
                        break;
                    case "background":
                        Trainer.Train(agent, episodes, Trainer.Type.Background, Verbosity.Low);
                        break;
                    default:
                        Trainer.Train(agent, episodes, Trainer.Type.Sync, Verbosity.Low);
                        break;
                }
                agent.Save(agentName);
            }
            else
            {
                Console.WriteLine("Not acceptable, try again.");
            }
        }
    }
}
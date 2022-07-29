using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace LitsReinforcementLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            string trainORplayORcreate = args[0].ToLower();
            string aiOrsoloORuser = args[1].ToLower();
            string agentName = args[2];
            
            Trainer.Player player = Trainer.Player.None;
            if (aiOrsoloORuser == "ai")
                player = Trainer.Player.AI;
            else if (aiOrsoloORuser == "solo")
                player = Trainer.Player.Solo;
            else if (aiOrsoloORuser == "user")
                player = Trainer.Player.User;
            else
                Console.WriteLine("Not acceptable, try again.");

            if (trainORplayORcreate == "train")
            {
                Agent agent = new Agent(AgentType.DynamicProgramming, agentName);

                if (!int.TryParse(args[3], out int episodes))
                {
                    Console.WriteLine("No integer value for episodes given.");
                    return;
                }

                Console.Title = $"Training {player}...";
                Trainer.Train(agent, player, episodes);
                agent.Save("Powers");
            }
            else if(trainORplayORcreate == "play")
            {
                Agent agent = new Agent(AgentType.DynamicProgramming, agentName);

                Console.Title = $"Playing {player}...";
                Trainer.PlayGame(agent, player);
            }
            else if(trainORplayORcreate == "create")
            {
                Console.Title = $"Creating {player}...";
                Trainer.CreateNewAgent(agentName);
            }
            else
            {
                Console.WriteLine("Not acceptable, try again.");
            }
        }
    }
}

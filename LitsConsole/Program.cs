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
            string trainORplay = args[0].ToLower();
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

            //Trainer.CreateNewAgent("Fresh");
            Agent agent = new Agent(AgentType.DynamicProgramming, agentName);

            if (trainORplay == "train")
            {
                if (int.TryParse(args[3], out int episodes))
                {
                    Console.WriteLine("No integer value for episodes given.");
                    return;
                }

                Console.Title = $"Training {player}...";
                Trainer.Train(agent, player, episodes);
                agent.Save("Powers");
            }
            else if(trainORplay == "play")
            {
                Console.Title = $"Playing {player}...";
                Trainer.PlayGame(agent, player);
            }
            else
            {
                Console.WriteLine("Not acceptable, try again.");
            }
        }
    }
}

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
            string agentName = args[2];

            //Trainer.CreateNewAgent("Fresh");

            Agent agent1 = new Agent(AgentType.DynamicProgramming, agentName, true);
            Agent agent2 = new Agent(AgentType.DynamicProgramming, agentName, false);


            string trainORplay = args[0].ToLower();
            string aiOrsoloORuser = args[1].ToLower();
            if (trainORplay == "train")
            {
                if (!int.TryParse(args[3], out int episodes))
                {
                    Console.WriteLine("No integer value for episodes given.");
                    return;
                }

                if (aiOrsoloORuser == "ai")
                {
                    Console.Title = "Training AI...";
                    Trainer.TrainAI(agent1, agent2, episodes);
                    agent1.Save("Powers");
                }
                else if(aiOrsoloORuser == "solo") 
                {
                    Console.Title = "Training solo...";
                    Trainer.TrainSolo(agent1, episodes);
                    agent1.Save("Powers");
                }
                else 
                {
                    Console.WriteLine("Not acceptable, try again.");
                }
            }
            else if(trainORplay == "play")
            {
                if (aiOrsoloORuser == "ai")
                {
                    Console.Title = "Playing AI...";
                    Trainer.PlayAI(agent1, agent2);
                }
                else if (aiOrsoloORuser == "solo")
                {
                    Console.Title = "Playing solo...";
                    Trainer.PlaySolo(agent1);
                }
                else if (aiOrsoloORuser == "user")
                {
                    Console.Title = "Playing user...";
                    Trainer.PlayUser(agent1);
                }
                else
                {
                    Console.WriteLine("Not acceptable, try again.");
                }
            }
            else
            {
                Console.WriteLine("Not acceptable, try again.");
            }
        }
    }
}

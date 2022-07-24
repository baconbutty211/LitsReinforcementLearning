﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace LitsReinforcementLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            //Trainer.CreateNewAgent("Fresh");

            Agent powers = new Agent(AgentType.DynamicProgramming, "Powers", true);
            Agent drEvil = new Agent(AgentType.DynamicProgramming, "Powers", false);

            string trainORplay = args[0].ToLower();
            string aiOrsoloORuser = args[1].ToLower();
            if (trainORplay == "train")
            {
                if (!int.TryParse(args[2], out int episodes))
                {
                    Console.WriteLine("No integer value for episodes given.");
                    return;
                }

                if (aiOrsoloORuser == "ai")
                {
                    Console.Title = "Training AI...";
                    Trainer.TrainAI(powers, drEvil, episodes);
                    powers.Save("Powers");
                }
                else if(aiOrsoloORuser == "solo") 
                {
                    Console.Title = "Training solo...";
                    Trainer.TrainSolo(powers, episodes);
                    powers.Save("Powers");
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
                    Trainer.PlayAI(powers, drEvil);
                }
                else if (aiOrsoloORuser == "solo")
                {
                    Console.Title = "Playing solo...";
                    Trainer.PlaySolo(powers);
                }
                else if (aiOrsoloORuser == "user")
                {
                    Console.Title = "Playing user...";
                    Trainer.PlaySolo(powers);
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

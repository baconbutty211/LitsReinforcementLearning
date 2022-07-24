using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace LitsReinforcementLearning
{
    class Program
    {
        static List<int> optimumPath = new List<int>();

        static void Main(string[] args)
        {
            bool isUserPlaying = bool.Parse(args[0]);

            Agent powers = new Agent(AgentType.DynamicProgramming, "Powers", true);
            Agent drEvil = new Agent(AgentType.DynamicProgramming, "Powers", false);
            Trainer.TrainAI(powers, drEvil, 10, Trainer.Verbosity.Low);
        }
    }
}

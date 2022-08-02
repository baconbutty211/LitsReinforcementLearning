using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LitsReinforcementLearning
{
    
    public static class Trainer
    {
        static Environment environment = new Environment();

        public static Agent CreateNewAgent(string name, bool isFirstPlayer) 
        {
            Agent newAgent = new Agent(AgentType.DynamicProgramming, environment.features.len, isFirstPlayer);
            newAgent.Save(name);
            return newAgent;
        }

        private static void PlayGame(Agent agent, Verbosity verbosity = Verbosity.High)
        {
            while (!environment.isDone)
            {
                agent.Explore(environment, verbosity);   // Trains on best next move
                Action action = agent.Exploit(environment);     // Evaluates the next best move

                Log.Write($"Applying action {action}...");
                environment.Step(action);   
            } // Play Game

            if (verbosity >= Verbosity.Mid)
                Console.WriteLine(environment.GetResult());

            environment.Reset();
        }
        public static void Train(Agent agent1, int episodes, Verbosity verbosity = Verbosity.Low) 
        {
            for (int i = 0; i < episodes; i++)
            {
                PlayGame(agent1, verbosity);

                if (verbosity == Verbosity.Low)
                    Console.Write($"\rGames of training completed: {i+1}");
                else if(verbosity > Verbosity.Low)
                    Console.WriteLine($"Games of training completed: {i+1}");
            }
        }
    }
}

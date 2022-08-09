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
        public enum Type { Sync, Async, Background }
        static Environment environment = new Environment();

        public static Agent CreateNewAgent(string name, bool isFirstPlayer) 
        {
            Agent newAgent = new DynamicProgrammingAgent(environment.features.Length, isFirstPlayer);
            newAgent.Save(name);
            return newAgent;
        }

        private static async void PlayGame(Agent agent, Type trainType, Verbosity verbosity = Verbosity.High)
        {
            while (!environment.isDone)
            {
                Action action = null;
                switch (trainType)
                {
                    case Type.Sync:
                        agent.Explore(environment, verbosity);   // Trains on best next move
                        action = agent.Exploit(environment);     // Evaluates the next best move
                        break;
                    case Type.Async:
                        agent.ExploreAsync(environment, verbosity);   // Trains on best next move
                        action = await agent.ExploitAsync(environment);     // Evaluates the next best move
                        break;
                    case Type.Background:
                        agent.ExploreBackground(environment, verbosity);   // Trains on best next move
                        action = agent.ExploitBackground(environment);     // Evaluates the next best move
                        break;
                    default:
                        throw new NotImplementedException($"No case block for training type {trainType}");
                }
                

                environment.Step(action);
            } // Play Game

            if (verbosity >= Verbosity.Mid)
                Console.WriteLine(environment.GetResult());

            environment.Reset();
        }
        public static void Train(Agent agent, int episodes, Type trainType, Verbosity verbosity = Verbosity.Low) 
        {
            Console.Clear();
            for (int i = 0; i < episodes; i++)
            {
                PlayGame(agent, trainType, verbosity);

                if (verbosity == Verbosity.Low)
                    Console.Write($"\rGames of training completed: {i + 1}");
                else if (verbosity > Verbosity.Low)
                    Console.WriteLine($"Games of training completed: {i + 1}");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LitsReinforcementLearning
{
    /// <summary>
    /// DEPRECATED
    /// </summary>

    public static class Trainer
    {
        static Environment environment = new Environment();

        private static void PlayGame(Agent agent, Verbosity verbosity = Verbosity.High)
        {
            while (!environment.isDone)
            {
                Action action = null;
                        agent.Explore(environment, verbosity);   // Trains on best next move
                        action = agent.Exploit(environment);     // Evaluates the next best move
                

                environment.Step(action);
            } // Play Game

            if (verbosity >= Verbosity.Mid)
                Console.WriteLine(environment.GetResult());

            environment.Reset();
        }
        public static void Train(Agent agent, int episodes, Verbosity verbosity = Verbosity.Low) 
        {
            Console.Clear();
            for (int i = 0; i < episodes; i++)
            {
                PlayGame(agent, verbosity);

                if (verbosity == Verbosity.Low)
                    Console.Write($"\rGames of training completed: {i + 1}");
                else if (verbosity > Verbosity.Low)
                    Console.WriteLine($"Games of training completed: {i + 1}");
            }
        }
    }
}

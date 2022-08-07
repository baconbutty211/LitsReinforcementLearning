using System;
using System.Threading;
using System.Diagnostics;
using LitsReinforcementLearning;
using Action = LitsReinforcementLearning.Action;
using Environment = LitsReinforcementLearning.Environment;

namespace LitsConsoleDebug
{
    class Program
    {
        static Stopwatch timer = new Stopwatch();

        static void Main(string[] args)
        {
            string command = args[0].ToLower();
            if (command == "speedtest")
            {
                string agentName = args[1];
                Agent agent = new DynamicProgrammingAgent(agentName);
                Console.Clear();

                int games = int.Parse(args[2]);
                Console.WriteLine("Initiating speedtest with random actions...");
                long randomtime = SpeedTest(null, games);
                Console.WriteLine($"Speed test took {randomtime}ms to play {games} games (with random actions).");

                Console.WriteLine("\nInitiating speedtest with agent actions...");
                long agenttime = SpeedTest(agent, games);
                Console.WriteLine($"Speed test took {agenttime}ms to play {games} games (with agent actions).");
            }
            else
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                Console.WriteLine(timer.ElapsedMilliseconds);
                Thread.Sleep(100);
                Console.WriteLine(timer.ElapsedMilliseconds);
                Console.WriteLine(timer.ElapsedMilliseconds);
            }
        }

        static long SpeedTest(Agent agent = null, int iterations = 100)
        {
            Environment environment = new Environment();

            long totalTime = 0;
            timer.Start();
            for (int i = 0; i < iterations; i++)
            {
                while (!environment.isDone)
                {
                    if(agent == null)
                        environment.Step(environment.GetRandomAction());
                    else
                    {
                        Action action = agent.Exploit(environment);
                        environment.Step(action);
                    }
                    WriteElapsedTime(ref totalTime, $"Step {environment.stepCount}");
                }
                //WriteElapsedTime(ref totalTime, $"Game {i + 1}");
                environment.Reset();
            }
            timer.Stop();
            return totalTime;
        }

        static void WriteElapsedTime(ref long totalTime, string header)
        {
            long time = timer.ElapsedMilliseconds;
            totalTime += time;
            Console.WriteLine($"{header} took {time}ms.");
            timer.Restart();
        }
    }
}

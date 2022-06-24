using System;
using LitsReinforcementLearning;
using Action = LitsReinforcementLearning.Action;
using Environment = LitsReinforcementLearning.Environment;

namespace LitsConsoleDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment environment = new Environment();
            foreach (string action in args) 
                environment.Step(Action.GetAction(int.Parse(action)));

            Console.WriteLine("Welcome to the manual game of LiTS");
            Console.WriteLine(environment);

            while (!environment.isDone) 
            {
                Console.WriteLine($"({environment.validActions.Length - 1} valid actions available). Enter an action you to apply (-1 for a random action):");
                if (!int.TryParse(Console.ReadLine(), out int action))
                    continue;
                if(action == -1)
                    environment.Step(environment.GetRandomAction());
                else
                    environment.Step(Action.GetAction(action));
                Console.Clear();
                Console.WriteLine(environment.ToString());
            }
        }
    }
}

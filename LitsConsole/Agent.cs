using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LitsReinforcementLearning
{
    public abstract class Agent
    {
        private static string savesPath = $"{Path.directory}{Path.Slash}Agents";

        protected Environment environment = new Environment();
        protected Tree litsTree; //Tree trunk
        protected Tree cwt; //Current working tree
        protected List<Tree> optimumPath = new List<Tree>();

        private string name;

        public Agent()
        {
            Observation initial = environment.Reset();
            litsTree = new Tree(initial);
            cwt = litsTree;
        }

        public abstract void Explore();
        /// <summary>
        /// Finds the optimum child/path. Sets the current working tree to the optimum child.
        /// </summary>
        public int[] Exploit() 
        {
            cwt = litsTree;
            while (!(cwt.Leaf || cwt.Empty))
            {
                Tree favChild = cwt.FavouriteChild;
                if (favChild != null)
                {
                    cwt = favChild;
                    optimumPath.Add(favChild);
                }
                else
                    throw new NullReferenceException();
            }
            return GetOptimumPath();
        }

        /// <summary>
        /// Saves the agents state tree to file.
        /// If the directory already exists it will be overwritten.
        /// Otherwise, it will be created.
        /// </summary>
        public void Save(string agentName)
        {
            Tree.Save(litsTree, $"{savesPath}{Path.Slash}{agentName}");
        }
        /// <summary>
        /// Loads a the agents state tree from file
        /// </summary>
        public void Load(string agentName)
        {
            try
            {
                litsTree = Tree.Load($"{savesPath}{Path.Slash}{agentName}");
                name = agentName;
            }
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException) { }
        }

        protected int[] GetOptimumPath() 
        {
            int[] optPath = new int[optimumPath.Count];
            for (int i = 0; i < optimumPath.Count; i++)
                optPath[i] = optimumPath[i].PreviousAction.Id;
            return optPath;
        }
    }

    public class MonteCarloAgent : Agent
    {
        private Random rnd = new Random();
        
        private const float Exploration = 0.2f;

        public MonteCarloAgent() : base()
        {
        }

        public void Explore(int episodes = 1)
        {
            for (int iters = 0; iters < episodes; iters++)
            {
                if (iters % 100 == 0)
                    Console.WriteLine($"Currently explored {iters} episodes.");
                Explore();
            }
        }
        public override void Explore()
        {
            cwt = litsTree;
            environment.Reset();
            List<Tree> route = new List<Tree>() { litsTree };
            List<float> rewards = new List<float>() { 0 };
            while (!environment.isDone)
            {
                Action action;
                if (rnd.NextDouble() < Exploration) // Chance of exploring a random branch
                    action = environment.GetRandomAction();
                else                                // Otherwise, take the best possible route
                {
                    Tree favChild = cwt.FavouriteChild;
                    if (favChild == null)
                        action = environment.GetRandomAction();
                    else
                        action = cwt.FavouriteChild.PreviousAction;
                }
                Observation obs = environment.Step(action);
                cwt = cwt.Branch(obs, action);
                //if (cwt == null) //Only happens when environment and tree disagree on isDone. Remove, if that bug has been fixed.
                //    break;
                route.Add(cwt);
                rewards.Add(obs.reward);
            } // Feed Forward
            for (float i = route.Count - 1, totalReward = 0; i >= 0; i--)
            {
                totalReward += rewards[(int)i];
                route[(int)i].ErrorCorrect(totalReward);
            } // Back Propagate

            Log.Rotate();
            Log.Clear();
        }
    }

    public class DynamicProgrammingAgent : Agent
    {
        public DynamicProgrammingAgent() : base()
        {
        }

        public override void Explore()
        {
            while (!environment.isDone) 
            {
                Explore(environment, cwt, 1);
                Tree favChild = cwt.FavouriteChild;
                Action bestAction = favChild.PreviousAction;
                Observation obs = environment.Step(bestAction);
                cwt = cwt.Branch(obs, bestAction);
            }
        }
        private void Explore(Environment env, Tree cwt, int depth = 1)
        {
            if (depth == 0)
                return;

            Action[] validActions = env.validActions;
            //if (validActions.Length == 0) // Environment is done.
            //    return;

            foreach (Action action in validActions)
            {
                Environment future = env.Clone();
                Observation obs = future.Step(action);
                Tree child = cwt.Branch(obs, action);
                Explore(future, child, depth-1);
            }
        }
    }
}

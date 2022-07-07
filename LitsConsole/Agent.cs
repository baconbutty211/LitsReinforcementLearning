using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LitsReinforcementLearning
{
    public abstract class Agent
    {
        protected static string savesPath = $"{Path.directory}{Path.Slash}Agents";

        protected Tree litsTree; //Tree trunk
        protected Tree cwt; //Current working tree

        //protected string name;
        protected bool isStartPlayer;

        public Agent(bool isStartPlayer = true) 
        {
            this.isStartPlayer = isStartPlayer;
        } //Hoping to phase out?
        public Agent(bool isStartPlayer, Observation initial)
        {
            this.isStartPlayer = isStartPlayer;

            litsTree = new Tree(initial);
            cwt = litsTree;
        }
    }
    public class MonteCarloAgent : Agent
    {

        private Random rnd = new Random();
        private const float Exploration = 0.2f;

        private Environment environment = new Environment();
        private new MonteCarloTree litsTree;
        private new MonteCarloTree cwt;

        public MonteCarloAgent() : base() 
        {
            Observation initial = environment.Reset();
            litsTree = new MonteCarloTree(initial);
            cwt = litsTree;
        }

        public void Explore(int episodes)
        {
            for (int iters = 0; iters < episodes; iters++)
            {
                if (iters % 100 == 0)
                    Console.WriteLine($"Currently explored {iters} episodes.");
                Explore();
            }
        }
        public void Explore()
        {
            cwt = litsTree;
            environment.Reset();
            List<MonteCarloTree> route = new List<MonteCarloTree>() { litsTree };
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
        /// <summary>
        /// Finds the optimum child/path. Sets the current working tree to the optimum child.
        /// </summary>
        public int[] Exploit()
        {
            List<MonteCarloTree> optimumPath = new List<MonteCarloTree>();

            cwt = litsTree;
            while (!(cwt.Leaf || cwt.Empty))
            {
                MonteCarloTree favChild = cwt.FavouriteChild;
                if (favChild != null)
                {
                    cwt = favChild;
                    optimumPath.Add(favChild);
                }
                else
                    throw new NullReferenceException();
            }
            int[] optPath = new int[optimumPath.Count];
            for (int i = 0; i < optimumPath.Count; i++)
                optPath[i] = optimumPath[i].PreviousAction.Id;
            return optPath;
        }

        #region Save/Load
        /// <summary>
        /// Saves the agents state tree to file.
        /// If the directory already exists it will be overwritten.
        /// Otherwise, it will be created.
        /// </summary>
        public void Save(string agentName)
        {
            MonteCarloTree.Save(litsTree, $"{savesPath}{Path.Slash}{agentName}");
        }
        /// <summary>
        /// Loads a the agents state tree from file
        /// </summary>
        public void Load(string agentName)
        {
            try
            {
                litsTree = MonteCarloTree.Load($"{savesPath}{Path.Slash}{agentName}");
            }
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException) { }
        }
        #endregion
    }

    public class DynamicProgrammingAgent : Agent
    {
        private new DynamicProgrammingTree litsTree;
        private new DynamicProgrammingTree cwt;

        public DynamicProgrammingAgent(bool isStartPlayer, Observation initial) : base(isStartPlayer, initial) 
        {
            litsTree = new DynamicProgrammingTree(initial);
            Reset();
        }
        public DynamicProgrammingAgent(bool isStartPlayer, string name) : base(isStartPlayer)
        {
            litsTree = Load(name);
            Reset();
        }

        public void Reset() 
        {
            cwt = litsTree;
        }
        /// <summary>
        /// Steps through every valid action (depth = 1).
        /// </summary>
        /// <param name="environment">The current state of the environment.</param>
        /// <returns>the best action found</returns>
        public Action Exploit(Environment environment) 
        {
            Action[] validActions = environment.validActions;
            foreach (Action action in validActions)
            {
                Environment future = environment.Clone();
                Observation obs = future.Step(action);
                cwt.Branch(obs, action);
            }

            DynamicProgrammingTree favChild = isStartPlayer ? cwt.FavouriteChild : cwt.ProblemChild;
            Action bestAction = favChild.PreviousAction;
            favChild.UpdateExpectedValue();
            cwt = favChild;
            return bestAction;
        }

        #region Save/Load
        /// <summary>
        /// Saves the agents state tree to file.
        /// If the directory already exists it will be overwritten.
        /// Otherwise, it will be created.
        /// </summary>
        public void Save(string agentName)
        {
            DynamicProgrammingTree.Save(litsTree, $"{savesPath}{Path.Slash}{agentName}");
        }
        /// <summary>
        /// Loads a the agents state tree from file
        /// </summary>
        public DynamicProgrammingTree Load(string agentName)
        {
            try
            {
                return DynamicProgrammingTree.Load($"{savesPath}{Path.Slash}{agentName}");
            }
            catch (FileNotFoundException) { return null; }
            catch (DirectoryNotFoundException) { return null; }
        }
        #endregion
    }

    /// <summary>
    /// Should be able to explore the entire state space. NEVER run it (its just nice to have).
    /// </summary>
    public class ExhaustiveSearchAgent : Agent
    {
        private Environment environment = new Environment();

        public ExhaustiveSearchAgent() : base()
        {
            Observation initial = environment.Reset();
            litsTree = new Tree(initial);
            cwt = litsTree;
        }

        public void Explore()
        {
            while (!environment.isDone)
            {
                Explore(environment, cwt);
                Tree favChild = cwt.FavouriteChild;
                Action bestAction = favChild.PreviousAction;
                Observation obs = environment.Step(bestAction);
                cwt = cwt.Branch(obs, bestAction);
            }
        }
        private void Explore(Environment env, Tree cwt)
        {
            if (environment.isDone) //Not strictly necessary, but useful to see where the recursion stops.
                return;

            Action[] validActions = env.validActions;

            foreach (Action action in validActions)
            {
                Environment future = env.Clone();
                Observation obs = future.Step(action);
                Tree child = cwt.Branch(obs, action);
                Explore(future, child);
            }
        }
    }
}

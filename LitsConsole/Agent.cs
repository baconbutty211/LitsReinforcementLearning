using System;
using System.Collections.Generic;

namespace LitsReinforcementLearning
{
    public enum AgentType { Abstract, MonteCarlo, DynamicProgramming, ExhaustiveSearch }
    public partial class Agent
    {
        protected static string savesPath = $"{Path.directory}{Path.Slash}Agents";

        private AgentType type;

        protected Tree litsTree; //Tree trunk
        protected Tree cwt; //Current working tree

        protected bool isStartPlayer;

        private Vector weights; // This vector must be the same size as the Environment's features vector.

        public Agent(AgentType type, Observation initial, Vector initialFeatures, bool isStartPlayer = true) 
        {
            this.type = type;
            this.isStartPlayer = isStartPlayer;

            // Sets random weights
            weights = Vector.InitializeRandom(initialFeatures.Count);

            litsTree = new Tree(initial);
            Reset();
        } // Creates a new agent.
        public Agent(AgentType type, string agentName, bool isStartPlayer = true) 
        {
            this.type = type;
            this.isStartPlayer = isStartPlayer;

            litsTree = Tree.LoadJson(savesPath, agentName);
            // Weights = LoadJson(path, agentName);
            Reset();
        } // Loads an agent from a save file.
       
        public virtual void Reset()
        {
            cwt = litsTree;
        }

        public void Save(string agentName) 
        {
            Tree.SaveJson(litsTree, savesPath, agentName);
        }

        public void Explore() 
        {
            
        }
        public Action Exploit(Environment env) 
        {
            switch (type)
            {
                case AgentType.Abstract:
                    break;

                case AgentType.MonteCarlo:
                    throw new NotImplementedException();
                
                case AgentType.DynamicProgramming:
                    return ExploitDynamicProgramming(env);
                
                case AgentType.ExhaustiveSearch:
                    break;
                
                default:
                    break;
            }
            return null;
        }
        private float Evaluate(Vector features)
        {
            return Vector.Dot(features, weights);
        }
    }

    /// <summary>
    /// All the dynamic programming stuff.
    /// </summary>
    public partial class Agent
    {
        public static float discount = 0.95f;
        private static float learningRate = 0.1f;

        private Action ExploitDynamicProgramming(Environment env)
        {
            Tree favChild = null;
            float bestChildVal = isStartPlayer ? float.MinValue : float.MaxValue;
            float currentValue = Evaluate(env.features);
            float bestChildReward = 0;

            Action[] validActions = env.validActions;
            foreach (Action action in validActions)
            {
                Environment future = env.Clone();
                Observation obs = future.Step(action);
                Tree child = cwt.Branch(obs);

                float futureValue = Evaluate(future.features);
                
                if (isStartPlayer)
                {
                    if (futureValue >= bestChildVal)
                    {
                        favChild = child;
                        bestChildVal = futureValue;
                        bestChildReward = obs.reward;
                    }
                }
                else
                {
                    if (futureValue <= bestChildVal)
                    {
                        favChild = child;
                        bestChildVal = futureValue;
                        bestChildReward = obs.reward;
                    }
                }
            }

            Vector deltaWeights = learningRate * ((bestChildReward + (discount * bestChildVal)) - currentValue) * env.features;
            weights -= deltaWeights; //Shift weights in the optimal direction

            cwt = favChild;
            return favChild.PreviousAction;
        }
    }
}

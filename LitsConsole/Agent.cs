using System;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;

namespace LitsReinforcementLearning
{
    public enum AgentType { Abstract, MonteCarlo, DynamicProgramming, ExhaustiveSearch }
    public partial class Agent
    {
        protected static string savesPath = $"{Path.directory}{Path.Slash}Agents";
        public const float discount = 0.95f;

        private AgentType type;
        NeuralNet model;

        protected Tree litsTree; //Tree trunk
        protected Tree cwt; //Current working tree

        protected bool isStartPlayer;


        public Agent(AgentType type, Observation initial, Vector<float> initialFeatures, bool isStartPlayer = true) 
        {
            this.type = type;
            this.isStartPlayer = isStartPlayer;

            model = new NeuralNet(initialFeatures.Count);
            litsTree = new Tree(initial);                               // Initializes new tree

            Reset();
        } // Creates a new agent.
        public Agent(AgentType type, string agentName, bool isStartPlayer = true) 
        {
            this.type = type;
            this.isStartPlayer = isStartPlayer;

            Load(agentName);
            Reset();
        } // Loads an agent from a save file.
        
        public virtual void Reset()
        {
            cwt = litsTree;
        }

        #region Save/Load
        private void Load(string agentName)
        {
            string path = $"{savesPath}{Path.Slash}{agentName}";
            litsTree = Tree.LoadJson(path);
        }
        public void Save(string agentName) 
        {
            string path = $"{savesPath}{Path.Slash}{agentName}";
            Tree.SaveJson(litsTree, path);
        }
        #endregion

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
    }

    

    /// <summary>
    /// All the dynamic programming stuff.
    /// </summary>
    public partial class Agent
    {
        private Action ExploitDynamicProgramming(Environment env)
        {
            Tree favChild = null;
            Vector<float> currentValues = model.Evaluate(env.features);

            float bestChildVal = isStartPlayer ? float.MinValue : float.MaxValue;
            Vector<float> bestChildsVals = null;
            float bestChildReward = 0;
            foreach (Action action in env.validActions)
            {
                Environment future = env.Clone();
                Observation obs = future.Step(action);
                Tree child = cwt.Branch(obs);

                Vector<float> futureValues = model.Evaluate(future.features);
                float futureValue = MaxValidActionValue(env, futureValues);

                if (isStartPlayer)
                {
                    if (futureValue >= bestChildVal)
                    {
                        favChild = child;
                        bestChildVal = futureValue;
                        bestChildsVals = futureValues;
                        bestChildReward = obs.reward;
                    }
                }
                else
                {
                    if (futureValue <= bestChildVal)
                    {
                        favChild = child;
                        bestChildVal = futureValue;
                        bestChildsVals = futureValues;
                        bestChildReward = obs.reward;
                    }
                }
            }

            model.Update(bestChildReward, bestChildsVals, currentValues, env.features);

            cwt = favChild;
            return favChild.PreviousAction;
        }
        private float MaxValidActionValue(Environment env, Vector<float> values) 
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }

            float maxVal = float.MinValue;
            foreach(Action action in env.validActions)
                if (values[action.Id] > maxVal)
                    maxVal = values[action.Id];
            return maxVal;
        }
    }
}

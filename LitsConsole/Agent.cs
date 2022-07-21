using System;
using System.Collections.Generic;
using System.Numerics;

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

        private Vector<float> weights; // This vector must be the same size as the Environment's features vector.

        public Agent(AgentType type, Observation initial, Vector<float> initialFeatures, bool isStartPlayer = true) 
        {
            this.type = type;
            this.isStartPlayer = isStartPlayer;

            // Sets random weights
            Random rnd = new Random();
            Span<float> featsLst = new Span<float>();
            initialFeatures.CopyTo(featsLst);
            for (int i = 0; i < featsLst.Length; i++)
                featsLst[i] = Convert.ToSingle(rnd.NextDouble());
            weights = new Vector<float>(featsLst);

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
        private float Evaluate(Vector<float> features)
        {
            return Vector.Dot(features, weights);
        }
    }

    /// <summary>
    /// All the dynamic programming stuff.
    /// </summary>
    public partial class Agent
    {

        private Action ExploitDynamicProgramming(Environment env)
        {
            Action[] validActions = env.validActions;
            foreach (Action action in validActions)
            {
                Environment future = env.Clone();
                Observation obs = future.Step(action);

                // Custom dynamic programming value
                float futureValue = Evaluate(future.features);
                obs.SetCustomReward(futureValue);
                
                cwt.Branch(obs);
            }

            Tree favChild = isStartPlayer ? cwt.FavouriteChild : cwt.ProblemChild;
            Action bestAction = favChild.PreviousAction;
            cwt = favChild;
            return bestAction;
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LitsReinforcementLearning
{
    public enum AgentType { Abstract, MonteCarlo, DynamicProgramming, ExhaustiveSearch }
    public class Agent
    {
        protected static string savesPath = $"{Path.directory}{Path.Slash}Agents";

        private AgentType type;

        protected Tree litsTree; //Tree trunk
        protected Tree cwt; //Current working tree

        protected bool isStartPlayer;
        
        public Agent(AgentType type, Observation initial, bool isStartPlayer = true)
        {
            this.type = type;
            this.isStartPlayer = isStartPlayer;

            litsTree = new Tree(initial);
            Reset();
        }
        public Agent(AgentType type, string agentName, bool isStartPlayer = true) 
        {
            this.type = type;
            this.isStartPlayer = isStartPlayer;

            litsTree = Tree.LoadJson(savesPath, agentName);
            Reset();
        }
       
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
        private Action ExploitDynamicProgramming(Environment env) 
        {
            Action[] validActions = env.validActions;
            foreach (Action action in validActions)
            {
                Environment future = env.Clone();
                Observation obs = future.Step(action);
                cwt.Branch(obs);
            }

            Tree favChild = isStartPlayer ? cwt.FavouriteChild : cwt.ProblemChild;
            Action bestAction = favChild.PreviousAction;
            cwt = favChild;
            return bestAction;
        }
    }
}

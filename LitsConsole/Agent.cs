using System;
using Numpy;

namespace LitsReinforcementLearning
{
    public enum AgentType { Abstract, MonteCarlo, DynamicProgramming, ExhaustiveSearch }
    public partial class Agent
    {
        protected static string savesPath = $"{Path.directory}{Path.Slash}Agents";
        public const float discount = 0.95f;

        private AgentType type;
        KerasNet model;

        protected Tree litsTree; //Tree trunk
        protected Tree cwt; //Current working tree

        protected bool isStartPlayer;


        public Agent(AgentType type, Observation initial, NDarray initialFeatures, bool isStartPlayer = true) 
        {
            this.type = type;
            this.isStartPlayer = isStartPlayer;

            model = new KerasNet(initialFeatures.len, Action.actionSpaceSize);  // Initializes new neural network
            litsTree = new Tree(initial);                                       // Initializes new tree

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
            model = KerasNet.Load(path);
        }
        public void Save(string agentName) 
        {
            string path = $"{savesPath}{Path.Slash}{agentName}";
            Tree.SaveJson(litsTree, path);
            model.Save(path);
        }
        #endregion

        // Trains the neural network model on the current state and best future state.
        public void Explore(Environment env) 
        {
            switch (type)
            {
                case AgentType.Abstract:
                    break;

                case AgentType.MonteCarlo:
                    throw new NotImplementedException();

                case AgentType.DynamicProgramming:
                    ExploreDynamicProgramming(env);
                    break;

                case AgentType.ExhaustiveSearch:
                    break;

                default:
                    break;
            }
        }
        public Action Exploit(Environment env, bool isTrain=false) 
        {
            if (isTrain)
                Explore(env);

            NDarray values = model.Predict(env.features);
            int actionId = MaxValidActionId(env, values.GetData<float>());
            return Action.GetAction(actionId);
        }
    }

    

    /// <summary>
    /// All the dynamic programming stuff.
    /// </summary>
    public partial class Agent
    {
        private Action ExploreDynamicProgramming(Environment env)
        {
            Tree favChild = null;
            NDarray currentValues = model.Predict(env.features);

            float bestChildVal = isStartPlayer ? float.MinValue : float.MaxValue;
            NDarray bestChildVals = null;
            float bestChildReward = 0;
            foreach (Action action in env.validActions)
            {
                Environment future = env.Clone();
                Observation obs = future.Step(action);
                Tree child = cwt.Branch(obs);

                NDarray futureValues = model.Predict(future.features);
                float futureValue = MaxValidActionValue(env, futureValues.GetData<float>());

                if (isStartPlayer)
                {
                    if (futureValue >= bestChildVal)
                    {
                        favChild = child;
                        bestChildVal = futureValue;
                        bestChildVals = futureValues;
                        bestChildReward = obs.reward;
                    }
                }
                else
                {
                    if (futureValue <= bestChildVal)
                    {
                        favChild = child;
                        bestChildVal = futureValue;
                        bestChildVals = futureValues;
                        bestChildReward = obs.reward;
                    }
                }
            }

            float[] rewards = new float[bestChildVals.size];
            for(int i = 0; i < bestChildVals.size; i++)
                rewards[i] = bestChildReward;
            NDarray bestChildRewards = np.array(rewards);
            model.Train(env.features, bestChildRewards + (discount * bestChildVals));

            cwt = favChild;
            return favChild.PreviousAction;
        }
        private float MaxValidActionValue(Environment env, float[] values) 
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
        private int MaxValidActionId(Environment env, float[] values) 
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }
            int maxId = -1;
            float maxVal = float.MinValue;
            foreach (Action action in env.validActions)
                if (values[action.Id] > maxVal)
                {
                    maxVal = values[action.Id];
                    maxId = action.Id;
                }
            return maxId;
        }
    }
}

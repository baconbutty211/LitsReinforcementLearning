﻿using System;
using System.IO;
using Numpy;

namespace LitsReinforcementLearning
{
    //public enum AgentType { Abstract, MonteCarlo, DynamicProgramming, ExhaustiveSearch }
    public abstract class Agent
    {
        protected static string savesPath = $"{Path.directory}{Path.Slash}Agents";
        public const float discount = 0.95f;

        public bool isFirstPlayer;
        protected KerasNet model;

        public Agent(bool isFirstPlayer, int inputSize, int outputSize, params int[] hiddenSizes) 
        {
            this.isFirstPlayer = isFirstPlayer;
            model = new KerasNet(inputSize, outputSize, hiddenSizes);  // Initializes new neural network
        } // Creates a new agent.
        public Agent(string agentName) 
        {
            Load(agentName);
        } // Loads an agent from a save file.
        
        #region Save/Load
        protected virtual void Load(string agentName)
        {
            string path = $"{savesPath}{Path.Slash}{agentName}";
            model = KerasNet.Load(path);

            isFirstPlayer = bool.Parse(File.ReadAllText($"{path}{Path.Slash}IsFirstPlayer.txt"));
        }
        public virtual void Save(string agentName) 
        {
            string path = $"{savesPath}{Path.Slash}{agentName}";
            model.Save(path);

            File.WriteAllText($"{path}{Path.Slash}IsFirstPlayer.txt", isFirstPlayer.ToString());
        }
        #endregion

        /// <summary>
        /// Trains the neural network model on the current state and best future state.
        /// </summary>
        public abstract void Explore(Environment env, Verbosity verbosity = Verbosity.High);
        /// <summary>
        /// Evaluates the current state of the environment/board.
        /// </summary>
        /// <returns>The best valid action according to the neural network</returns>
        public abstract Action Exploit(Environment env);

        #region Helpers
        protected float MaxValidActionValue(Environment env, float[] values)
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }
            float maxVal = float.MinValue;
            for (int i = 0; i < env.validActions.Length; i++)
                if (values[i] > maxVal)
                    maxVal = values[i];
            return maxVal;
        }
        protected int MaxValidActionId(Environment env, float[] values)
        {
            return MaxValidAction(env, values).Id;
        }
        protected Action MaxValidAction(Environment env, float[] values)
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }
            Action maxAction = null;
            float maxVal = float.MinValue;
            for (int i = 0; i < env.validActions.Length; i++)
            {
                if (values[i] > maxVal)
                {
                    maxVal = values[i];
                    maxAction = env.validActions[i];
                }
            }
            return maxAction;
        }

        protected float MinValidActionValue(Environment env, float[] values)
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }
            float minVal = float.MaxValue;
            for (int i = 0; i < env.validActions.Length; i++)
                if (values[i] < minVal)
                    minVal = values[i];
            return minVal;
        }
        protected int MinValidActionId(Environment env, float[] values)
        {
            return MinValidAction(env, values).Id;
        }
        protected Action MinValidAction(Environment env, float[] values)
        {
            if (env.isDone)
            {
                Log.RotateError();
                throw new ArgumentNullException("No valid actions to return.");
            }
            Action minAction = null;
            float minVal = float.MaxValue;
            for (int i = 0; i < env.validActions.Length; i++)
            {
                if (values[i] < minVal)
                {
                    minVal = values[i];
                    minAction = env.validActions[i];
                }
            }
            return minAction;
        }
        #endregion
    }



    /// <summary>
    /// All the dynamic programming stuff.
    /// </summary>
    public class DynamicProgrammingAgent : Agent
    {
        public DynamicProgrammingAgent(int inputSize, bool isFirstPlayer) : base(isFirstPlayer, inputSize, outputSize: 1, hiddenSizes: 10) { }
        public DynamicProgrammingAgent(string agentName) : base(agentName) { }

        public override void Explore(Environment env, Verbosity verbosity = Verbosity.High)
        {
            //bool isFirstPlayer = env.stepCount % 2 == 0;
            //bool isFirstPlayer = true;                      //AI plays more like X on even turns and more like O on odd turns, than the line above

            // Maximize value
            Action bestAction = Exploit(env);
            Environment future = env.Clone();
            Observation obs = future.Step(bestAction, calculateValidActions: true);

            if (!obs.isDone)
            {
                // Minimize value
                Action worstAction = Exploit(future);
                Environment future2 = future.Clone();
                Observation obs2 = future2.Step(worstAction, calculateValidActions: false);

                //// Train value function (V(t+1) <- V(t+2))
                //NDarray truth = model.Predict(future.features);
                //truth *= discount;
                //truth += obs2.reward;
                //model.Train(future.features, truth, verbosity);

                Train(future.features, future.features, obs2.reward, verbosity);

                //// Train value function (V(t) <- V(t+2))
                //NDarray truth2 = model.Predict(future.features);
                //truth2 *= discount;
                //truth2 += obs2.reward;
                //model.Train(env.features, truth2, verbosity);

                Train(future.features, env.features, obs2.reward, verbosity);
            } // Search/Train another step ahead.
            else
            {
                //// Train value function (V(t) <- V(t+1))
                //NDarray truth = model.Predict(env.features);
                //truth *= discount;
                //truth += obs.reward;
                //model.Train(env.features, truth, verbosity);

                Train(env.features, env.features, obs.reward, verbosity);
            } // Train on this one step only
        }
        /// <summary>
        /// Trains value function at step t+1 towards, (value function at step t) * discount + reward 
        /// </summary>
        private void Train(NDarray features, NDarray futureFeatures, float reward, Verbosity verbosity) 
        {
            NDarray truth = model.Predict(features);
            truth *= discount;
            truth += reward;
            model.Train(futureFeatures, truth, verbosity);
        }
        public override Action Exploit(Environment env)
        {
            bool isFirstPlayer = env.stepCount % 2 == 0;

            NDarray[] futureFeatures = new NDarray[env.validActions.Length];
            int count = 0;
            foreach (Action action in env.validActions)
            {
                Environment future = env.Clone();
                Observation obs = future.Step(action, calculateValidActions: false);

                futureFeatures[count++] = future.features;
            }
            NDarray values = model.Predict(futureFeatures);
            return isFirstPlayer ? MaxValidAction(env, values.GetData<float>()) : MinValidAction(env, values.GetData<float>());
        }
    }
}

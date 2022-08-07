﻿using System;
using System.Collections.Generic;
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
            for (int i = 0; i < values.Length; i++)
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


        protected int Argmax(float[] values) 
        {
            if (values.Length == 0)
                return -1;

            int maxArg = 0;
            for (int i = 0; i < values.Length; i++)
                if (values[i] >= values[maxArg])
                    maxArg = i;
            return maxArg;
        }
        protected int Argmin(float[] values) 
        {
            if (values.Length == 0)
                return -1;

            int minArg = 0;
            for (int i = 0; i < values.Length; i++)
                if (values[i] <= values[minArg])
                    minArg = i;
            return minArg;
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

        public void ExploreTreestrap(Environment env, Verbosity verbosity = Verbosity.High)
        {
            Environment future;
            Observation obs;
            foreach (Action action in env.validActions)
            {
                future = env.Clone();
                obs = future.Step(action);
                if (obs.isDone)
                    continue;
                Action counterAction = Exploit(future);

                TrainValueFunction(future, counterAction, verbosity); // Train t+1 <- t+2
            }

            // 2 Step look ahead
            Action bestAction = Exploit(env);
            future = env.Clone();
            obs = future.Step(bestAction);
            if (obs.isDone) 
                return;
            Action worstAction = Exploit(future);

            TrainValueFunction(env, bestAction, worstAction, verbosity); // Train t <- t+2
        }

        public override void Explore(Environment env, Verbosity verbosity = Verbosity.High)
        {
            Action bestAction = Exploit(env);
            TrainValueFunction(env, bestAction, verbosity); // Train t <- t+1
        }
        /// <summary>
        /// Trains value function at step t towards better value function at step t+1 (improvement comes from experience).
        /// </summary>
        private void TrainValueFunction(Environment env, Action bestAction, Verbosity verbosity) 
        {
            Environment future = env.Clone();
            Observation obs = future.Step(bestAction);

            NDarray truth = model.Predict(future.features);
            truth *= discount;
            truth += obs.reward;

            model.Train(env.features, truth, verbosity);
        }
        /// <summary>
        /// Trains value function at step t towards better value function at step t+2 (improvement comes from experience).
        /// </summary>
        private void TrainValueFunction(Environment env, Action bestAction, Action worstAction, Verbosity verbosity) 
        {
            Environment future = env.Clone();
            Observation obs = future.Step(bestAction);
            Observation obs2 = future.Step(worstAction);

            NDarray truth = model.Predict(future.features);
            truth *= discount;
            truth += obs2.reward;
            truth *= discount;
            truth += obs.reward;

            model.Train(env.features, truth, verbosity);
        }
        public override Action Exploit(Environment env)
        {
            if (env.stepCount == 0)
                return Action.GetAction(548);

            List<int> indexToActionId = new List<int>();
            List<NDarray> futureFeatures = new List<NDarray>();
            foreach (Action action in env.validActions)
            {
                Environment future = env.Clone();
                Observation obs = future.Step(action);
                if (obs.isDone)
                {
                    futureFeatures.Add(future.features);
                    indexToActionId.Add(action.Id);
                }
                else
                {
                    foreach (Action futureAction in future.validActions)
                    {
                        Environment future2 = future.Clone();
                        Observation obs2 = future2.Step(futureAction, calculateValidActions: false);
                        futureFeatures.Add(future2.features);
                        indexToActionId.Add(action.Id);
                    } // 2 Step look ahead
                }
            } // 1 Step look ahead
            bool isFirstPlayer = env.stepCount % 2 == 0;
            NDarray values = model.Predict(futureFeatures);
            int index = isFirstPlayer ? Argmax(values.GetData<float>()) : Argmin(values.GetData<float>());
            return Action.GetAction(indexToActionId[index]);
        }
        public Action ExploitNOTminimax(Environment env)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLitsMadeSimpler
{
    public class Agent
    {
        const float Exploration = 0.2f;
        static string savesPath = "C:\\Users\\jleis\\Documents\\Visual Studio 2019\\Projects\\LitsGitRL\\LitsConsole\\Agents";
        Random rnd = new System.Random();


        Tree litsTree; //Tree trunk
        Tree cwt; //Current working tree
        Environment environment;

        List<Tree> optimumPath;

        public Agent()
        {
            environment = new Environment();
            Observation initial = environment.Reset();
            litsTree = new Tree(initial);
            optimumPath = new List<Tree>();
        }
        public Agent(int agent) 
        {
            environment = new Environment();
            Load();
            optimumPath = new List<Tree>();
        }



        public void Explore() 
        {
            cwt = litsTree;
            while (!environment.isDone) 
            {
                Action action = environment.GetRandomAction();
                Observation obs = environment.Step(action);
                cwt = cwt.Branch(obs, action);
            }
        }
        /// <summary>
        /// Finds the optimum child/path. Sets the current working tree to the optimum child.
        /// </summary>
        public void Exploit()
        {
            cwt = litsTree;
            while (!(cwt.Leaf || cwt.Empty))
            {
                float maxVal = float.MinValue;
                Tree favChild = null;
                foreach (Tree child in cwt)
                {
                    float childVal = child.Value;
                    if (childVal > maxVal)
                    {
                        maxVal = childVal;
                        favChild = child;
                    }
                }
                if (favChild != null)
                {
                    cwt = favChild;
                    optimumPath.Add(favChild);
                }
                else
                    throw new NullReferenceException();
            }
        } // Needs to search recursively
        public IEnumerable<string> DisplayOptimumPath()
        {
            foreach (Tree favChild in optimumPath)
                yield return Environment.ToString(favChild.State);
        }

        public void Save() 
        {
            Tree.Save(litsTree, $"{savesPath}\\Save1");
        }
        public void Load() 
        {
            litsTree = Tree.Load($"{savesPath}\\Save1");
        }
    }
}

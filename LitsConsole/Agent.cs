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

        string name;
        Tree litsTree; //Tree trunk
        Tree cwt; //Current working tree
        Environment environment = new Environment();

        List<Tree> optimumPath = new List<Tree>();

        public Agent()
        {
            Observation initial = environment.Reset();
            litsTree = new Tree(initial);
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
                Tree favChild = cwt.FavouriteChild;
                if (favChild != null)
                {
                    cwt = favChild;
                    optimumPath.Add(favChild);
                }
                else
                    throw new NullReferenceException();
            }
        }
        public IEnumerable<string> DisplayOptimumPath()
        {
            foreach (Tree favChild in optimumPath)
                yield return Environment.ToString(favChild.State);
        }

        /// <summary>
        /// Saves the agents state tree to file.
        /// If the directory already exists it will be overwritten.
        /// Otherwise, it will be created.
        /// </summary>
        public void Save(string agentName) 
        {
            Tree.Save(litsTree, $"{savesPath}\\{agentName}");
        }
        /// <summary>
        /// Loads a the agents state tree from file
        /// </summary>
        public void Load(string agentName) 
        {
            name = agentName;
            litsTree = Tree.Load($"{savesPath}\\{name}");
        }
    }
}

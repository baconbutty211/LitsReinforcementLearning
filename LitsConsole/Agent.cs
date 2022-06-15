using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLitsMadeSimpler
{
    public class Agent
    {
        const float Exploration = 0.2f;
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
            cwt = litsTree;
            optimumPath = new List<Tree>();
        }


        public void Explore(int explorationDepth=1) 
        {
            while (!environment.isDone) 
            {
                if(!cwt.Leaf)
                    Explore(cwt, explorationDepth);
                Exploit();
            }
        }
        private void Explore(Tree cwt, int explorationDepth)
        {
            if (explorationDepth == 0)
                return;
            if (cwt.Leaf)
                return;

            foreach (int action in Action.GetActions())
            {
                environment.Reset(cwt.State);
                try 
                { 
                    Observation obs = environment.Step(action); 
                    cwt.Branch(obs, action);
                }
                catch (IndexOutOfRangeException) { continue; }
            }
            foreach (Tree child in cwt)
                Explore(child, explorationDepth - 1);

        } //Recursively explores the state spaceup to a maximum depth
        /// <summary>
        /// Finds the optimum child/path. Sets the current working tree to the optimum child.
        /// </summary>
        private void Exploit()
        {
            while (!(cwt.Leaf || cwt.Empty))
            {
                float maxVal = float.MinValue;
                Tree favChild = null;
                foreach (Tree child in cwt)
                {
                    if (child == null)
                        continue;
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
        }

        public IEnumerable<string> DisplayOptimumPath()
        {
            foreach (Tree favChild in optimumPath)
                yield return Environment.ToString(favChild.State);
            
        }
    }
}

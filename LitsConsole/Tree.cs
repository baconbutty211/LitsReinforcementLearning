using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SimpleLitsMadeSimpler
{
    public class Tree
    {
        static int size = Action.actionSpaceSize;
        static float Discount = 0.95f;

        int depth;
        Observation root;
        Tree[] children;

        public bool[] State { get { return root.state; } }
        public float Value
        { 
            get 
            {
                if(Empty)
                    return root.reward;

                float maxVal = float.MinValue;
                foreach (Tree child in children)
                {
                    if (child == null)
                        continue;

                    float childVal = child.Value;
                    if (childVal > maxVal)
                        maxVal = childVal;
                }
                return root.reward + (maxVal * Discount);
            }
        }
        public bool Leaf { get { return root.isDone; } }
        public bool Empty { get { return ChildCount == 0; } }
        public int ChildCount
        {
            get
            {
                if (Leaf)
                    return 0;

                int count = 0;
                foreach (Tree child in children)
                    if (child != null)
                        count++;
                return count;
            }
        }

        public Tree(Observation initialObservation)
        {
            depth = 0;
            root = initialObservation;
            children = new Tree[size];
        }
        private Tree(Observation root, int depth)
        {
            this.depth = depth;
            this.root = root;
            if(!Leaf)
                children = new Tree[size];
        }

        public Tree Branch(Observation observation, Action action) 
        {
            if (Leaf)
                throw new Exception("Tree is a leaf (state is done). Should not be adding any children here.");
            Tree child = new Tree(observation, depth+1);
            children[action.Id] = child;
            return child;
        }

        public IEnumerator<Tree> GetEnumerator() 
        {
            foreach (Tree child in children)
                if(child != null)
                    yield return child;
        }

        #region Save/Load
        public string[] GetContents()
        {
            string state = "";
            foreach (bool bit in root.state)
                state += $"{(bit ? 1 : 0)},";

            return new string[] {
                $"depth:{depth}",
                $"done:{ (root.isDone ? 1 : 0) }",
                $"reward:{root.reward}",
                $"state:{state}",
            };
        }
        public void SetContents(string[] contents) 
        {
            
        }
        //Don't add '\\' on the end of the path.
        public static void Save(Tree tree, string path) 
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //Save current branch variables
            File.WriteAllLines(path+"\\root.txt", tree.GetContents());

            //Save all child variables
            if (tree.Leaf || tree.Empty)
                return;
            int count = 0;
            foreach (Tree child in tree)
                Save(child, $"{path}\\child{count++}");
        }
        public static Tree Load(string path) 
        {
            return null;
        }

        #endregion
    }

}

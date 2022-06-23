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
        public int prevActionId;
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
        public Tree FavouriteChild 
        {
            get 
            {
                float maxVal = float.MinValue;
                Tree favChild = null;
                foreach (Tree child in this)
                {
                    float childVal = child.Value;
                    if (childVal > maxVal)
                    {
                        maxVal = childVal;
                        favChild = child;
                    }
                }
                return favChild;
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
        private Tree(string[] contents) 
        {
            SetContents(contents);
            if (!Leaf)
                children = new Tree[size];
        }

        public Tree Branch(Observation observation, Action action) 
        {
            if (Leaf)
                throw new Exception("Tree is a leaf (state is done). Should not be adding any children here.");
            Tree child = new Tree(observation, depth+1);
            return Branch(child, action.Id);
        }
        private Tree Branch(Tree child, int actionId) 
        {
            child.prevActionId = actionId;
            if(children[actionId] == null)
                children[actionId] = child;
            return children[actionId];
        }

        public IEnumerator<Tree> GetEnumerator() 
        {
            foreach (Tree child in children)
                if(child != null)
                    yield return child;
        }

        #region Save/Load
        private string[] GetContents()
        {
            string state = "";
            foreach (bool bit in root.state)
                state += $"{(bit ? 1 : 0)},";

            return new string[] {
                $"depth:{depth}",
                $"done:{ (root.isDone ? 1 : 0) }",
                $"reward:{root.reward}",
                $"state:{state}",
                //$"previousActionId:{prevActionId}",
            };
        }
        private void SetContents(string[] contents) 
        {
            depth = int.Parse(contents[0].Split(':')[1]);
            //prevActionId = int.Parse(contents[4].Split(':')[1]);
            bool done = (contents[1].Split(':')[1] == "1");
            float reward = float.Parse(contents[2].Split(':')[1]);

            string[] stateStr = contents[3].Split(':')[1].Split(',');
            bool[] state = new bool[stateStr.Length - 1];
            for (int i = 0; i < state.Length; i++)
                state[i] = (stateStr[i] == "1");

            root = new Observation(state, reward, done);
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
            for(int i = 0; i < tree.children.Length; i++)
                if(tree.children[i] != null)
                    Save(tree.children[i], $"{path}\\child{i}");
        }
        public static Tree Load(string path) 
        {
            if (!File.Exists($"{path}\\root.txt"))
                throw new FileNotFoundException();
            
            //Load tree
            string[] contents = File.ReadAllLines($"{path}\\root.txt");
            Tree tree = new Tree(contents);

            //Load children
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                int actionId = int.Parse(dir.Split('\\')[dir.Split('\\').Length - 1].Substring(5));
                tree.Branch(Load(dir), actionId);
            }

            return tree;
        }

        #endregion
    }
}

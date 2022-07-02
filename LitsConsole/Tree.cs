using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LitsReinforcementLearning
{
    public class Tree
    {
        static int size = Action.actionSpaceSize;
        static float Discount = 0.95f;

        int depth;
        int visitCount = 0; // Number of times this state/tree has been visited
        float success = 0;
        float value = 0; // The average reward to be gained from this state / The average value of the current state.
        int prevActionId;
        Observation root;
        Tree[] children;

        public bool[] State { get { return root.state; } }
        public float Value
        { 
            get 
            {
                if(Empty)
                    return root.reward;
                else
                    return root.reward + (FavouriteChild.Value * Discount);
            }
        }
        public Action PreviousAction { get { return Action.GetAction(prevActionId); } }
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
                if (Leaf || Empty)
                    return null;
                float maxVal = float.MinValue;
                Tree favChild = null;
                foreach (Tree child in this)
                {
                    float childVal = child.value;
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
            visitCount = -1; // because I don't care how often the trunk is visited
            root = initialObservation;
            children = new Tree[size];
        } // Constructs the tree trunk
        private Tree(Observation root, int depth)
        {
            this.depth = depth;
            this.root = root;
            if(!Leaf)
                children = new Tree[size];
        } // Constructs new branches
        private Tree(string[] contents) 
        {
            SetContents(contents);
            if (!Leaf)
                children = new Tree[size];
        } // Constructs new & file branches

        public Tree Branch(Observation observation, Action action) 
        {
            if (Leaf)
            {
                string exception = "Tree is a leaf (state is done). Should not be adding any children here.";
                Log.Write(exception);
                Log.RotateError();
                //throw new Exception(exception); // Tree thinks the state isDone, when the environment disagrees. I suspect this is because on a previous episode the environment thought this state was done, but this time round the environment disagrees. Seems to be unpredictable.
                return null;
            }
            Tree child = new Tree(observation, depth+1);
            return Branch(child, action.Id);
        } // Agent calls this branch method
        private Tree Branch(Tree child, int actionId) 
        {
            child.prevActionId = actionId;
            if (children[actionId] == null)
                children[actionId] = child;
            return children[actionId];
        } // Tree Loading & Agent calls this branch method

        public void ErrorCorrect(float newValue) 
        {
            value = ((visitCount * value) + newValue) / ++visitCount;
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
                $"visits:{visitCount}",
                $"value:{value}",
            };
        }
        private void SetContents(string[] contents) 
        {
            depth = int.Parse(contents[0].Split(':')[1]);
         
            bool done = (contents[1].Split(':')[1] == "1");
            float reward = float.Parse(contents[2].Split(':')[1]);

            string[] stateStr = contents[3].Split(':')[1].Split(',');
            bool[] state = new bool[stateStr.Length - 1];
            for (int i = 0; i < state.Length; i++)
                state[i] = (stateStr[i] == "1");
            root = new Observation(state, reward, done);

            visitCount = int.Parse(contents[4].Split(':')[1]);
            value = float.Parse(contents[5].Split(':')[1]);
        }
        public static void Save(Tree tree, string path) 
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //Save current branch variables
            File.WriteAllLines(path+$"{Path.Slash}root.txt", tree.GetContents());

            //Save all child variables
            if (tree.Leaf || tree.Empty)
                return;
            for(int i = 0; i < tree.children.Length; i++)
                if(tree.children[i] != null)
                    Save(tree.children[i], $"{path}{Path.Slash}child{i}");
        }
        public static Tree Load(string path) 
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            if (!File.Exists($"{path}{Path.Slash}root.txt"))
                throw new FileNotFoundException();
            
            //Load tree
            string[] contents = File.ReadAllLines($"{path}{Path.Slash}root.txt");
            Tree tree = new Tree(contents);

            //Load children
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                int actionId = int.Parse(dir.Split(Path.Slash)[dir.Split(Path.Slash).Length - 1].Substring(5));
                tree.Branch(Load(dir), actionId);
            }

            return tree;
        }

        #endregion
    }
}

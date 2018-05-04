using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class MerkleTree
    {
        private class MerkleNode
        {
            public string hash;

            public int parent;
            public int left, right;
        }

        private MerkleNode[] tree;

        public MerkleTree(HashObject[] txs)
        {
            if (txs.Length < 4)
            {
                txs = (new List<HashObject>(txs)).Concat(
                    new HashObject[] {
                    HashObject.Empty(), HashObject.Empty(), HashObject.Empty(),
                    HashObject.Empty() }
                    ).ToArray();
            }

            var depth = (int)Math.Ceiling(Math.Sqrt(txs.Length) + 1);
            tree = new MerkleNode[(int)Math.Pow(2, depth)];

            for (int i = 0; i < tree.Length; i++)
                tree[i] = new MerkleNode();

            for (int i = 0; i < txs.Length; i++)
            {
                var idx = tree.Length - i - 1;
                var pidx = (idx) / 2;
                    
                tree[idx].parent = pidx;
                tree[idx].hash = txs[i].hash;

                if (IsLeftNode(i))
                    tree[pidx].left = idx;
                else
                {
                    tree[pidx].right = idx;
                    CalcHashUpward(pidx);
                }
            }

            tree[0].hash = Hash.Calc2(tree[1].hash, tree[2].hash);

            /*
            for (int i = 0; i < tree.Length; i++)
            {
                Console.WriteLine($"Node({i}): " + tree[i].hash);
                Console.WriteLine("  Parent: " + tree[i].parent);
                Console.WriteLine("  Left: " + tree[i].left);
                Console.WriteLine("  Right: " + tree[i].right);
            }
            */
        }

        public string GetRootHash()
        {
            return tree[0].hash;
        }
        private bool IsLeftNode(int idx)
        {
            return idx % 2 == 0;
        }
        private void CalcHashUpward(int idx)
        {
            if (idx == 0) return;

            var node = tree[idx];
            if (node.left == 0 && node.right == 0) return;

            node.parent = idx / 2;
            if (IsLeftNode(idx)) tree[node.parent].left = idx;
            else tree[node.parent].right = idx;
            node.hash = Hash.Calc2(tree[node.left].hash, tree[node.right].hash);

            CalcHashUpward(node.parent);
        }
    }
}

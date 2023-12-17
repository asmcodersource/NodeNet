using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.ReceiveMiddleware
{
    internal class HashTree
    {
        Dictionary<byte, HashTree> tree = new Dictionary<byte, HashTree>();

        public void Clear()
        {
            tree.Clear();
        }

        public void Add(byte[] hash, int offset = 0)
        {
            if (offset >= hash.Length)
                return;
            tree.TryAdd(hash[offset], new HashTree());
            tree[hash[offset]].Add(hash, offset + 1);
        }

        public bool Contains(byte[] hash, int offset = 0)
        {
            if (offset >= hash.Length) 
                return true;
            if (!tree.ContainsKey(hash[offset]))
                return false;
            return tree[hash[offset]].Contains(hash, offset + 1);
        }
    }
}

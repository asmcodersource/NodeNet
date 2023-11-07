using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNet.NodeNet.SignOptions;

namespace NodeNet.NodeNet.RSASigner
{
    internal class SenderSignOptions : ISenderSignOptions
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        public SenderSignOptions(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }
    }
}

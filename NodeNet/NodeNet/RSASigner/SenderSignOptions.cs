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
        public string PublicKey { get; protected set; }
        public string PrivateKey { get; protected set; }
        public string KeysXML { get; protected set; }

        public SenderSignOptions(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        public void SetKeysXML(string keys)
        {
            KeysXML = keys;
        }
    }
}

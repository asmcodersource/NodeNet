using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.SignOptions
{
    public interface IReceiverSignOptions
    {
        public string PublicKey { get; protected set; }
    }
}

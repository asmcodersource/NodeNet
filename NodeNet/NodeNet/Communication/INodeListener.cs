using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.Communication
{
    internal interface INodeListener
    {
        public event Action<INodeConnection> ConnectionOpened;
        public string GetConnectionAddress();
        public void StartListening();
        public void StopListening();
    }
}

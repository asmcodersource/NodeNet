using Newtonsoft.Json;
using NodeNet.NodeNet.Message;
using NodeNet.NodeNet.NetworkExplorer.Requests;
using NodeNet.NodeNet.NetworkExplorer.Responses;
using NodeNet.NodeNet.ReceiveMiddleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.NetworkExplorer
{
    internal class NetworkExplorerMiddleware : IReceiveMiddleware
    {
        public NetworkExplorer Explorer { get; protected set; }
        public Node Node { get; protected set; }
        public IReceiveMiddleware NextReceiveMiddleware { get; protected set; }
        public IReceiveMiddleware Next { get; protected set; } = null;

        public NetworkExplorerMiddleware(Node node ,NetworkExplorer explorer)
        {
            // would be used to store new connections
            this.Node = node;
            this.Explorer = explorer;
        }

        public bool Invoke(MessageContext messageContext)
        {
            if( messageContext.Message.Info.IsTechnical != true)
                return Next == null ? true : Next.Invoke(messageContext);
            AcceptExporerMessages(messageContext);
            return Next == null ? true : Next.Invoke(messageContext);
        }

        public void SetNext(IReceiveMiddleware next)
        {
            Next = next;
        }

        public void AcceptExporerMessages(MessageContext messageContext)
        {
            string requestJson = messageContext.Message.Data.ToString();
            var message = JsonConvert.DeserializeObject(requestJson);
            if( message is IRequest request)
            {
                switch(request)
                {
                    case EchoRequest echoRequest:
                        // Should be ok?
                        Explorer.UpdateConnectionInfo(echoRequest.MyAddress);
                        var echoRequestResponse = new EchoResponse();
                        echoRequestResponse.MyAddress = messageContext.SenderConnection.GetConnectionAddress();
                        Node.SendMessage(JsonConvert.SerializeObject(echoRequestResponse), messageContext.Message.Info.ReceiverPublicKey);
                        break;
                };
            }
            if( message is IResponse response)
            {
                switch (response)
                {
                    case EchoResponse echoResponse:
                        Explorer.UpdateConnectionInfo(echoResponse.MyAddress);
                        break;
                };
            }
        }
    }
}

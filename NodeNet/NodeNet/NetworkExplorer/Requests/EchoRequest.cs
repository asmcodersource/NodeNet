namespace NodeNet.NodeNet.NetworkExplorer.Requests
{
    public class EchoRequest : IRequest
    {
        public string MyAddress { get; set; }
        public string MessageType { get { return typeof(EchoRequest).FullName; } }
    }
}

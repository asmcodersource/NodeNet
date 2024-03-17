namespace NodeNet.NodeNet.NetworkExplorer.Responses
{
    public class EchoResponse : IResponse
    {
        public string MyAddress { get; set; }
        public string MessageType { get { return typeof(EchoResponse).FullName; } }
    }
}

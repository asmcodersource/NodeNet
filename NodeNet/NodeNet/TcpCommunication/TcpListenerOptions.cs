namespace NodeNet.NodeNet.TcpCommunication
{
    public class TcpListenerOptions
    {
        public int Port { get; set; } = 8080;


        public TcpListenerOptions() { }

        public TcpListenerOptions(int port)
        {
            Port = port;
        }
    }
}

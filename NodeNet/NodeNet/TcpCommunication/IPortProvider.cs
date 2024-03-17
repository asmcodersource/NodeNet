namespace NodeNet.NodeNet.TcpCommunication
{
    /// <summary>
    /// Used to get TCP-address of node listener
    /// it used to get specific port in case of multiple network interfaces used by one node
    /// </summary>
    public interface ITcpAddressProvider
    {
        public int GetNodeTcpPort();
        public string GetNodeTcpIP();
    }
}

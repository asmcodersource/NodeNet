namespace NodeNet.NodeNet.Message
{
    // Mean one full received message
    // Can be broadcast, or personal
    [Serializable]
    public class Message
    {
        public MessageInfo Info { get; protected set; }
        public string Data { get; protected set; } = "";
        public string MessageSign { get; protected set; } = "";
        public int TimeToLive { get; set; } = 128;

        public Message(MessageInfo info, string data, string messageSign = "")
        {
            Info = info;
            Data = data;
            MessageSign = messageSign;
        }

        public void SetMessageSign(string sign)
        {
            // should be valid...
            MessageSign = sign;
        }
    }
}

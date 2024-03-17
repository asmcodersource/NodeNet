namespace NodeNet.NodeNet.Message
{
    // Main information about message
    // This part of message is signing by sender, and can't be change
    [Serializable]
    public class MessageInfo
    {
        public string SenderPublicKey { get; set; }
        public string ReceiverPublicKey { get; set; } = string.Empty; // Empty means broadcast
        public DateTime SendingTime { get; set; } = DateTime.UtcNow;
        public bool IsTechnical { get; set; } = false;

        public MessageInfo(string senderPublicKey, string receiverPublicKey, bool isTechnical = false)
        {
            SenderPublicKey = senderPublicKey;
            ReceiverPublicKey = receiverPublicKey;
            SendingTime = DateTime.UtcNow;
            IsTechnical = isTechnical;
        }
    }
}

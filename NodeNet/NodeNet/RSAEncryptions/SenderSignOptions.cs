namespace NodeNet.NodeNet.RSAEncryptions
{
    public class SenderSignOptions : ISenderSignOptions
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        // just for serrialization
        public SenderSignOptions() { }

        public SenderSignOptions(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }
    }
}

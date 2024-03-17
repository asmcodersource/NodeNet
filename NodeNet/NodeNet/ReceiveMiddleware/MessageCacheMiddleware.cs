using NodeNet.NodeNet.Message;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace NodeNet.NodeNet.ReceiveMiddleware
{
    // Caching used to know if same message was received earlier
    // Resolves problem of cycles on connections network
    // TODO: Use hashtree to make it effective 
    public class MessageCacheMiddleware : IReceiveMiddleware
    {
        public IReceiveMiddleware Next { get; protected set; } = null;
        public uint MessageStorageSize { get; protected set; } = 10000;
        protected HashTree hashTree = new HashTree();
        protected int hashCountCounter = 0;

        public bool Invoke(MessageContext messageContext)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {

                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, messageContext.Message.Info);
                    formatter.Serialize(memoryStream, messageContext.Message.Data);
                    var hash = sha512.ComputeHash(memoryStream.ToArray());
                    lock (this)
                    {
                        if (hashTree.Contains(hash))
                            return false;
                        hashTree.Add(hash);
                    }
                    hashCountCounter++;
                    if (hashCountCounter > MessageStorageSize)
                    {
                        hashCountCounter = 0;
                        hashTree.Clear();
                    }
                    if (Next != null)
                        return Next.Invoke(messageContext);
                    else
                        return true;
                }
            }
        }

        public void SetNext(IReceiveMiddleware next)
        {
            Next = next;
        }
    }
}

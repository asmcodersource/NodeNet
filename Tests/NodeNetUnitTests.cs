using NodeNet.NodeNet;
using NodeNet.NodeNet.RSAEncryptions;
using NodeNet.NodeNetSession.MessageWaiter;
using NodeNet.NodeNetSession.SessionListener;
using Tests.Generators;

namespace Tests
{
    public class NodeNetUnitTests
    {
        static object wallLock = new object();

        [Fact]
        public void NodeNet_Communications_Messaging_SingleMessage_Test()
        {
            using (var connections = new NodeNetConnectionPair())
            {
                Node first_node = connections.first_node;
                Node second_node = connections.second_node;

                string message = "Example message for testing";
                int receivedMessagesCount = 0;
                second_node.MessageReceived += (msgContext) =>
                {
                    receivedMessagesCount++;
                    Assert.True(msgContext.Message.Data == message);
                };
                first_node.MessageReceived += (msgContext) =>
                {
                    receivedMessagesCount++;
                    Assert.True(msgContext.Message.Data == message);
                };

                first_node.SendMessage(message).Wait();
                second_node.SendMessage(message).Wait();

                Thread.Sleep(50);

                Assert.Equal(2, receivedMessagesCount);
            }
        }


        [Fact]
        public void NodeNet_Communications_Messaging_MultipleMessage_Test()
        {
            using (var connections = new NodeNetConnectionPair())
            {
                Node first_node = connections.first_node;
                Node second_node = connections.second_node;
                int first_received_summary = 0;
                int second_received_summary = 0;
                int sending_summary = 0;
                first_node.MessageReceived += (msgcontext) => { lock (this) { first_received_summary += Convert.ToInt32(msgcontext.Message.Data); } };
                second_node.MessageReceived += (msgcontext) => { lock (this) { second_received_summary += Convert.ToInt32(msgcontext.Message.Data); } };
                for (int i = 0; i < 1024; i++)
                {
                    sending_summary += i;
                    first_node.SendMessage(i.ToString()).Wait();
                    second_node.SendMessage((1023-i).ToString()).Wait();
                }

                Thread.Sleep(150);

                Assert.Equal(sending_summary, first_received_summary);
                Assert.Equal(sending_summary, second_received_summary);
            }
        }


        [Fact]
        public void NodeNet_Communcations_Messaging_NetworkTest()
        {
            // Create for test performing
            var nodeNetNetworkConnections = NodeNetTestNetworksGenerator.Shared;

            // Verifies that data passes through the network from sender to recipient
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                var firstPeer = nodeNetNetworkConnections.GetRandomNode();
                var secondPeer = nodeNetNetworkConnections.GetRandomNode();
                if (firstPeer == secondPeer)
                {
                    i--;
                    continue;
                }
                var task = Task.Run(() => TestBroadcastConnectionBetweenNodes(firstPeer, secondPeer));
                tasks.Add(task);
            }
            Task.WhenAll(tasks).Wait();
        }

        protected async Task TestBroadcastConnectionBetweenNodes(Node first_node, Node second_node)
        {
            object atomicLock = new object();
            string message = Random.Shared.Next().ToString();
            int receivedMessagesCount = 0;
            second_node.PersonalMessageReceived += (msgContext) =>
            {
                if (msgContext.Message.Data == message)
                    lock (atomicLock)
                        receivedMessagesCount |= 1;
            };
            first_node.PersonalMessageReceived += (msgContext) =>
            {
                if (msgContext.Message.Data == message)
                    lock (atomicLock)
                        receivedMessagesCount |= 2;
            };

            await first_node.SendMessage(message, second_node.SignOptions.PublicKey);
            await second_node.SendMessage(message, first_node.SignOptions.PublicKey);
            await Task.Delay(4000);
            Assert.Equal(3, receivedMessagesCount);
        }
    }
}

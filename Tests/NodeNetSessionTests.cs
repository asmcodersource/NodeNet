using NodeNet.NodeNet;
using NodeNet.NodeNetSession.SessionListener;
using NodeNet.NodeNetSession.Session;
using Tests.Generators;

namespace Tests
{
    public class NodeNetSessionTests
    {
        [Fact]
        public async void NodeNet_Communications_Session_Connection_Test()
        {
            using (var connections = new NodeNetConnectionPair())
            {
                Node first_node = connections.first_node;
                Node second_node = connections.second_node;

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                //cancellationTokenSource.CancelAfter(500);
                SessionListener sessionListener = new SessionListener(first_node, "socket");
                Session secondNodeSession = new Session(second_node);
                sessionListener.StartListening();
                var success = await secondNodeSession.Connect(first_node.SignOptions.PublicKey, "socket", cancellationTokenSource.Token);
                Assert.True(success == ConnectionResult.Connected, "Connection isn't succesful");
                sessionListener.Dispose();

            }
        }

        [Fact]
        public async void NodeNet_Communications_Session_Communication_Test()
        {
            using (var connections = new NodeNetConnectionPair())
            {
                Node first_node = connections.first_node;
                Node second_node = connections.second_node;
                {
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    cancellationTokenSource.CancelAfter(1000);
                    SessionListener sessionListener = new SessionListener(first_node, "socket");
                    Session secondNodeSession = new Session(second_node);
                    sessionListener.NewSessionCreated += async (session) =>
                    {
                        await session.SendMessage("Ping");
                        var pong = await session.WaitForMessage();
                    };
                    sessionListener.StartListening();
                    var success = await secondNodeSession.Connect(first_node.SignOptions.PublicKey, "socket", cancellationTokenSource.Token);
                    Assert.True(success == ConnectionResult.Connected, "Connection isn't succesful");
                    var pingMsg = await secondNodeSession.WaitForMessage(cancellationTokenSource.Token);
                    await secondNodeSession.SendMessage("Pong");
                    sessionListener.Dispose();
                }
            }
        }
    }
}


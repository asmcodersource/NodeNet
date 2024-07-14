using NodeNet.NodeNet;
using NodeNet.NodeNetSession.SessionListener;
using NodeNet.NodeNetSession.Session;
using Tests.Generators;
using System.Text.Json;
using NodeNet.NodeNetSession.SessionMessage;

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
                cancellationTokenSource.CancelAfter(5000);
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
                    cancellationTokenSource.CancelAfter(5000);
                    SessionListener sessionListener = new SessionListener(first_node, "socket");
                    Session secondNodeSession = new Session(second_node);
                    sessionListener.NewSessionCreated += async (session) =>
                    {
                        session.SendMessage("Ping");
                        var pong = await session.WaitForMessage();
                    };
                    sessionListener.StartListening();
                    var success = await secondNodeSession.Connect(first_node.SignOptions.PublicKey, "socket", cancellationTokenSource.Token);
                    Assert.True(success == ConnectionResult.Connected, "Connection isn't succesful");
                    var pingMsg = await secondNodeSession.WaitForMessage(cancellationTokenSource.Token);
                    secondNodeSession.SendMessage("Pong");
                    sessionListener.Dispose();
                }
            }
        }

        [Fact]
        public async void NodeNet_Communications_Session_Multiple_Communication_Test()
        {
            using (var connections = new NodeNetConnectionPair())
            {
                Node first_node = connections.first_node;
                Node second_node = connections.second_node;
                {
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    //cancellationTokenSource.CancelAfter(5000);
                    SessionListener sessionListener = new SessionListener(first_node, "socket");
                    Session secondNodeSession = new Session(second_node);
                    sessionListener.NewSessionCreated += (session) =>
                    {
                        for (int i = 1; i <= 10; i++)
                            session.SendMessage(i.ToString());
                    };
                    sessionListener.StartListening();
                    var success = await secondNodeSession.Connect(first_node.SignOptions.PublicKey, "socket", cancellationTokenSource.Token);
                    Assert.True(success == ConnectionResult.Connected, "Connection isn't succesful");
                    int sum = 0;
                    for (int i = 1; i <= 10; i++)
                    {
                        var msgContext = await secondNodeSession.WaitForMessage(cancellationTokenSource.Token);
                        var sessionMsg = JsonSerializer.Deserialize<SessionMessage>(msgContext.Message.Data);
                        sum += Convert.ToInt32(sessionMsg.Data);
                    }
                    Assert.Equal((1 + 10) * 10 / 2, sum);
                    sessionListener.Dispose();
                }
            }
        }
    }
}


var nodeHttpListener = new NodeNet.NodeNet.HttpCommunication.NodeHttpListener();
nodeHttpListener.ConnectionOpened += (object sender, object connection) => {
    Console.WriteLine("{0}, {1}", sender, connection);
};
nodeHttpListener.StartListening();
var connection = new NodeNet.NodeNet.HttpCommunication.NodeHttpConnection();
await connection.Connect($"http://127.0.0.1:8080/");
while (true);

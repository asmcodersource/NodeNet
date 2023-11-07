using NodeNet.NodeNet;
using NodeNet.NodeNet.HttpCommunication;
using NodeNet.NodeNet.RSASigner;

var options = RSAEncryption.CreateSignOptions();
var node = Node.CreateRSAHttpNode(options);
Task.Delay(5000).Wait();

NodeHttpConnection connection = new NodeHttpConnection();
bool result = connection.Connect("ws://localhost:8080/websock");
Console.WriteLine("Is connected? Result: {0}", result.ToString());
connection.ListenMessages();
Task.Delay(5000).Wait();

connection.MessageReceived += (connection) =>
{
    var message = connection.GetLastMessage();
    connection.CloseConnection();
};

node.SendMessage("Hello, world!");
while (true) ;
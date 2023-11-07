using NodeNet.NodeNet;
using NodeNet.NodeNet.HttpCommunication;
using NodeNet.NodeNet.RSASigner;

var options = RSAEncryption.CreateSignOptions();
var node = Node.CreateRSAHttpNode(options);

NodeHttpConnection connection = new NodeHttpConnection();
bool result = await connection.Connect("ws://localhost:8081/websock");

Console.WriteLine("Is connected? Result: {0}", result.ToString());
while (true) ;
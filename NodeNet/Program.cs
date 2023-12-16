using NodeNet.NodeNet;
using NodeNet.NodeNet.HttpCommunication;
using NodeNet.NodeNet.RSASigner;

var options1 = RSAEncryption.CreateSignOptions();
var node1 = Node.CreateRSAHttpNode(options1, new HttpListenerOptions(8082, "websock/"));
var options2 = RSAEncryption.CreateSignOptions();
var node2 = Node.CreateRSAHttpNode(options2, new HttpListenerOptions(8083, "websock/"));
node2.MessageReceived += (msgContext) => { Console.WriteLine(msgContext.Message.Data); };

bool connectSuccesful = node1.Connect("ws://localhost:8083/websock/");
Console.WriteLine("Is node1 connected to node2? " + connectSuccesful.ToString());

node1.SendMessage("Hello b#tch!");
node1.SendMessage("Second message");

Thread.Sleep(1000 * 10);
node2.Close();


while (true);
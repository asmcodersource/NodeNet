using NodeNet.NodeNet;
using NodeNet.NodeNet.HttpCommunication;
using NodeNet.NodeNet.RSASigner;

var options1 = RSAEncryption.CreateSignOptions();
var options2 = RSAEncryption.CreateSignOptions();
var node1 = Node.CreateRSAHttpNode(options1, new HttpListenerOptions(8082, "websock/"));
var node2 = Node.CreateRSAHttpNode(options2, new HttpListenerOptions(8083, "websock/"));
node1.MessageReceived += (msgContext) => { Console.WriteLine(msgContext.Message.Data); };
node2.MessageReceived += (msgContext) => { Console.WriteLine(msgContext.Message.Data); };
node1.Connect("ws://localhost:8083/websock/");

new Thread(() => {
    while (true)
    {
        node2.SendMessage("Message1");
        node1.SendMessage("Message2");
    }
}).Start();


while (true);
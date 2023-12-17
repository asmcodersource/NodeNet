using NodeNet.NodeNet;
using NodeNet.NodeNet.HttpCommunication;
using NodeNet.NodeNet.RSASigner;

var options1 = RSAEncryption.CreateSignOptions();
var node1 = Node.CreateRSAHttpNode(options1, new HttpListenerOptions(8082, "websock/"));
node1.MessageReceived += (msgContext) => { Console.WriteLine(msgContext.Message.Data); };
var options2 = RSAEncryption.CreateSignOptions();
var node2 = Node.CreateRSAHttpNode(options2, new HttpListenerOptions(8083, "websock/"));
node2.MessageReceived += (msgContext) => { Console.WriteLine(msgContext.Message.Data); };

bool connectSuccesful = node1.Connect("ws://localhost:8083/websock/");
Console.WriteLine("Is node1 connected to node2? " + connectSuccesful.ToString());

new Thread(() => {
    while (true)
    {
        node2.SendMessage("Message1");
        node1.SendMessage("Message2");
    }
}).Start();



Thread.Sleep(1000 * 10);
//node2.Close();


while (true);
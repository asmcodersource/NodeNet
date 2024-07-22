using NodeNet.NodeNet;
using NodeNet.NodeNet.TcpCommunication;
using NodeNet.NodeNet.RSAEncryptions;

var options1 = RSAEncryption.CreateSignOptions();
var options2 = RSAEncryption.CreateSignOptions();
var node1 = Node.CreateRSAHttpNode(options1, new TcpListenerOptions(8082));
var node2 = Node.CreateRSAHttpNode(options2, new TcpListenerOptions(8083));
node1.MessageReceived += (msgContext) => { Console.WriteLine(msgContext.Message.Data); };
node2.MessageReceived += (msgContext) => { Console.WriteLine(msgContext.Message.Data); };
node1.Connect("127.0.0.1:8083");

node1.NetworkExplorer.SendExploreEcho();
//node2.NetworkExplorer.SendExploreEcho();

new Thread(() => {
    for( int i = 0; i < 10; i++ ) 
    {
        node2.SendMessageAsync("Message1");
        node1.SendMessageAsync("Message2");
        Thread.Sleep(500);
    }
    node2.Close();
}).Start();


while (true);
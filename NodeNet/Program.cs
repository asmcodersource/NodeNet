using NodeNet.NodeNet.Message;
using NodeNet.NodeNet.RSASigner;

var senderOptions = RSAEncryption.CreateSignOptions();
var messageInfo = new MessageInfo(senderOptions.PublicKey, "");
var message = new Message(messageInfo, "My super message!");

var signer = new MessageSigner();
signer.SetSignOptions(senderOptions);
signer.Sign(message);

var validator = new MessageValidator();
validator.SetValidateOptions(new ReceiverSignOptions(message));
bool result = validator.Validate(message);

Console.WriteLine("Message validate result = {0}", result.ToString());
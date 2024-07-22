using NodeNet.NodeNet.Communication;
using NodeNet.NodeNet.NetworkExplorer.Responses;
using NodeNet.NodeNet.RSAEncryptions;
using NodeNet.NodeNet.SignOptions;
using System.Text;
using System.Text.Json;

namespace NodeNet.NodeNet.NodeActions
{
    [Serializable]
    public class PingPongData
    {
        public string? MyPublicKey { get; set; }
        public string? Signature { get; set; }
        public long RandomNumber { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }

    [Serializable]
    public class PingRequest : PingPongData {}

    [Serializable]
    public class PongResponse : PingPongData {}

    /// <summary>
    /// These actions allow you to make sure that both sides of the connection use the NodeNet architecture for communication processing
    /// </summary>
    public class PingPong
    {
        // Send ping request to active I node connection
        // Should be used before any other data transfers
        public async static Task<bool> Ping(INodeConnection connection, ISenderSignOptions senderSignOptions)
        {
            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(5000);
                
                var requestTime = DateTime.UtcNow;
                var requestedRandomNumber = Random.Shared.NextInt64();
                await connection.SendRawDataAsync(Encoding.UTF8.GetBytes(requestedRandomNumber.ToString()), cancellationTokenSource.Token);
                
                var pongResponseBytes = await connection.ReceiveRawData(cancellationTokenSource.Token);
                var pongResponseJson = Encoding.UTF8.GetString(pongResponseBytes, 0, pongResponseBytes.Length);
                var pongResponse = JsonSerializer.Deserialize<PongResponse>(pongResponseJson);
                if (IsPublicKeyConfirmed(pongResponse, requestedRandomNumber, requestTime))
                    connection.OppositeSidePublicKey = pongResponse.MyPublicKey;

                var randomNumber = Convert.ToInt64(Encoding.UTF8.GetString(await connection.ReceiveRawData(cancellationTokenSource.Token)));
                var pingRequest = new PingRequest()
                {
                    MyPublicKey = senderSignOptions.PublicKey,
                    RandomNumber = randomNumber,
                    DateTime = DateTime.UtcNow,
                };
                SignPingPongData(pingRequest, senderSignOptions);
                var pingRequestJson = JsonSerializer.Serialize(pingRequest);
                await connection.SendRawDataAsync(Encoding.UTF8.GetBytes(pingRequestJson), cancellationTokenSource.Token);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public async static Task<bool> Pong(INodeConnection connection, ISenderSignOptions senderSignOptions)
        {
            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(5000);
                var randomNumber = Convert.ToInt64(Encoding.UTF8.GetString(await connection.ReceiveRawData(cancellationTokenSource.Token)));
                var pongResponse = new PongResponse()
                {
                    MyPublicKey = senderSignOptions.PublicKey,
                    RandomNumber = randomNumber,
                    DateTime = DateTime.UtcNow,
                };
                SignPingPongData(pongResponse, senderSignOptions);
                var pongResponseJson = JsonSerializer.Serialize(pongResponse);
                await connection.SendRawDataAsync(Encoding.UTF8.GetBytes(pongResponseJson), cancellationTokenSource.Token);

                var requestTime = DateTime.UtcNow;
                var requestedRandomNumber = Random.Shared.NextInt64();
                await connection.SendRawDataAsync(Encoding.UTF8.GetBytes(requestedRandomNumber.ToString()), cancellationTokenSource.Token);
                var pingRequestBytes = await connection.ReceiveRawData(cancellationTokenSource.Token);
                var pingRequestString = Encoding.UTF8.GetString(pingRequestBytes, 0, pingRequestBytes.Length);
                var pingRequest = JsonSerializer.Deserialize<PingRequest>(pingRequestString);
                if (IsPublicKeyConfirmed(pingRequest, requestedRandomNumber, requestTime))
                    connection.OppositeSidePublicKey = pingRequest.MyPublicKey;
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        private static bool IsPublicKeyConfirmed(PingPongData? pingPongData, long requestedRandomNumber, DateTime requestTime)
        {
            if (pingPongData is not null && pingPongData.Signature is not null && pingPongData.MyPublicKey is not null)
            {
                if (pingPongData.DateTime < requestTime)
                    throw new Exception("The response time cannot be earlier than the request time");
                if (pingPongData.RandomNumber != requestedRandomNumber)
                    throw new Exception("Response with wrong signature magik number");
                var receiverSignOptions = new ReceiverSignOptions(pingPongData.MyPublicKey);
                return VerifySignature(pingPongData, receiverSignOptions);
            }
            return false;
        }

        private static bool VerifySignature(PingPongData pingPongData, IReceiverSignOptions receiverSignOptions)
        {
            var signature = pingPongData.Signature;
            var tempObject = new PingPongData()
            {
                MyPublicKey = pingPongData.MyPublicKey,
                DateTime = pingPongData.DateTime,
                RandomNumber = pingPongData.RandomNumber,
                Signature = null,
            };
            return RSAEncryptions.RSAEncryption.VerifySign(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(tempObject)), signature, receiverSignOptions);
        }

        private static void SignPingPongData(PingPongData pingPongData, ISenderSignOptions senderSignOptions)
        {
            pingPongData.Signature = null;
            var signature = RSAEncryptions.RSAEncryption.Sign(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(pingPongData)), senderSignOptions);
            pingPongData.Signature = signature;
        }
    }
}

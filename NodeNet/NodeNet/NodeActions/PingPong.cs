using NodeNet.NodeNet.Communication;
using NodeNet.NodeNet.NetworkExplorer.Responses;
using System.Text;
using System.Text.Json;

namespace NodeNet.NodeNet.NodeActions
{
    [Serializable]
    public class PingRequest
    {
        public string? MyPublicKey { get; set; }
    }

    [Serializable]
    public class PongResponse
    {
        public string? MyPublicKey { get; set; }
    }

    public class PingPong
    {
        // Send ping request to active I node connection
        // Should be used before any other data transfers
        public async static Task<bool> Ping(INodeConnection connection, string? myPublicKey)
        {
            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(5000);
                var pingRequestJson = JsonSerializer.Serialize(new PingRequest() {MyPublicKey = myPublicKey});
                await connection.SendRawData(Encoding.UTF8.GetBytes(pingRequestJson), cancellationTokenSource.Token);
                var pongResponseBytes = await connection.ReceiveRawData(cancellationTokenSource.Token);
                var pongResponseJson = Encoding.UTF8.GetString(pongResponseBytes, 0, pongResponseBytes.Length);
                var pongResponse = JsonSerializer.Deserialize<PongResponse>(pongResponseJson);
                connection.OppsiteSidePublicKey = pongResponse?.MyPublicKey;
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public async static Task<bool> Pong(INodeConnection connection, string? myPublicKey)
        {
            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(5000);
                var pingRequestBytes = await connection.ReceiveRawData(cancellationTokenSource.Token);
                var pingRequestString = Encoding.UTF8.GetString(pingRequestBytes, 0, pingRequestBytes.Length);
                var pingRequest = JsonSerializer.Deserialize<PingRequest>(pingRequestString);
                var pongResponse = new PongResponse() { MyPublicKey = myPublicKey };
                var pongResponseJson = JsonSerializer.Serialize(pongResponse);
                await connection.SendRawData(Encoding.UTF8.GetBytes(pongResponseJson), cancellationTokenSource.Token);
                connection.OppsiteSidePublicKey = pingRequest?.MyPublicKey;
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
    }
}

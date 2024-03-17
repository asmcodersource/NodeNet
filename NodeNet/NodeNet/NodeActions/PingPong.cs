using NodeNet.NodeNet.Communication;
using System.Text;

namespace NodeNet.NodeNet.NodeActions
{
    public class PingPong
    {
        protected const string PingData = "NodeNetPingMessage";
        protected const string PongData = "NodeNetPongMessage";


        // Send ping request to active I node connection
        // Should be used before any other data transfers
        public async static Task<bool> Ping(INodeConnection connection)
        {
            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(5000);
                await connection.SendRawData(Encoding.UTF8.GetBytes(PingData), cancellationTokenSource.Token);
                var response = await connection.ReceiveRawData(cancellationTokenSource.Token);
                var responseString = Encoding.UTF8.GetString(response, 0, response.Length);
                if (responseString.Equals(PongData))
                    return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public async static Task<bool> Pong(INodeConnection connection)
        {
            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(5000);
                var response = await connection.ReceiveRawData(cancellationTokenSource.Token);
                var responseString = Encoding.UTF8.GetString(response, 0, response.Length);
                if (responseString.Equals(PingData))
                {
                    await connection.SendRawData(Encoding.UTF8.GetBytes(PongData), cancellationTokenSource.Token);
                    return true;
                }
            }
            catch (Exception ex) { }
            return false;
        }
    }
}

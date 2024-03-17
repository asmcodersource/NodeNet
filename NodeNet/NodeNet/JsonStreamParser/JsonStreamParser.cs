using System.Text;
using System.Text.Json;


namespace NodeNet.NodeNet.JsonStreamParser
{
    /// <summary>
    /// This thing simply reads data from a stream and tries to parse it as a Json serialized object. 
    /// This kind of thing has already been used before, but now it is implemented better (it seems to me).
    /// </summary>
    /// Should be base for chain storaging, and transfers via network
    public class JsonStreamParser
    {
        protected char[] readBuffer;
        protected Queue<JsonDocument> objectsQueue = new Queue<JsonDocument>();
        protected StringBuilder dataBuffer = new StringBuilder();

        public JsonStreamParser(int bufferSize = 1024 * 32)
        {
            // bigger array faster parsing, but more space complexity
            readBuffer = new char[bufferSize];
        }
        public async Task<List<JsonDocument>> ParseJsonObjects(Stream jsonStream, CancellationToken cancellationToken)
        {
            await ParseObjectsFromJsonToQueue(jsonStream, cancellationToken);
            var objectsList = objectsQueue.ToList();
            objectsQueue.Clear();
            return objectsList;
        }

        public async Task<JsonDocument> ParseJsonObject(Stream jsonStream, CancellationToken cancellationToken)
        {
            while (objectsQueue.Count() == 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ParseObjectsFromJsonToQueue(jsonStream, cancellationToken);
            }
            return objectsQueue.Dequeue();
        }

        protected async Task ParseObjectsFromJsonToQueue(Stream jsonStream, CancellationToken cancellationToken)
        {
            StreamReader reader = new StreamReader(jsonStream);
            while (reader.EndOfStream is not true)
            {
                var receivedSize = await reader.ReadAsync(readBuffer, cancellationToken);
                if (receivedSize == 0)
                    await Task.Delay(100);
                ParsePart(readBuffer, receivedSize);
                if (objectsQueue.Count() != 0)
                    break;
            }
        }

        protected void ParsePart(char[] chars, int length)
        {
            for (int i = 0; i < length; i++)
            {
                // read each symbol, and add it to buffer
                var symbol = chars[i];
                dataBuffer.Append(symbol);
                if (symbol != '}')
                    continue;

                // symbol is '}', then it can be end of json serialized object
                try
                {
                    var streamPart = dataBuffer.ToString();
                    var parsedObject = JsonDocument.Parse(streamPart);
                    objectsQueue.Enqueue(parsedObject);
                    dataBuffer.Clear();
                }
                catch (JsonException) { /* This part isn't correct json */ }
            }
        }
    }
}

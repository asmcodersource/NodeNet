﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NodeNet.NodeNet.Communication;

namespace NodeNet.NodeNet.HttpCommunication
{
    internal class NodeHttpListener : INodeListener
    {
        public bool IsListening { get; set; } = false;
        public int ListenPort { get; set; } = 8080;
        protected Task? ListeningTask { get; set; } = null;
        public HttpListener HttpListener { get; protected set; }
        public HttpListenerOptions Options { get; set; } = new HttpListenerOptions(8080, "websock/");

        public event Action<INodeConnection> ConnectionOpened;

        public void StartListening()
        {
            if (IsListening == true)
                throw new Exception("Multiple listening");
            HttpListener = new HttpListener();
            Console.WriteLine($"http://*:{Options.Port}/{Options.Resource}");
            HttpListener.Prefixes.Add($"http://*:{Options.Port}/{Options.Resource}");
            HttpListener.Start();
            ListeningTask = Task.Run(() => Listener());
            IsListening = true;
        }

        public void StopListening()
        {
            if (ListeningTask == null)
                throw new Exception("Is not listening");
            IsListening = false;
            HttpListener.Stop();
            ListeningTask.Wait();
        }

        protected async Task Listener()
        {
            try
            {
                while (IsListening == true)
                {
                    var context = await HttpListener.GetContextAsync();
                    if (context.Request.IsWebSocketRequest != true)
                        continue;
                    var webSocketContext = await context.AcceptWebSocketAsync(null, new TimeSpan(0, 0, 10));
                    var connection = new NodeHttpConnection(webSocketContext);
                    ConnectionOpened?.Invoke(connection);
                    connection.ListenMessages();
                }
            } catch (HttpListenerException exception) { 
                IsListening = false;
            }
        }
    }
}

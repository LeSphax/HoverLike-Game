using Byn.Common;
using System;
using UnityEngine;
using WebSocketSharp;

namespace Byn.Net.Native
{
    public class WebSocketConnection
    {
        private WebSocket mSocket;

        internal WebsocketConnectionStatus status;

        private string mUrl;

        private ServerConnection serverConnection;

        public WebSocketConnection(ServerConnection connection, string url)
        {
            this.serverConnection = connection;
            this.mUrl = url;
            this.status = WebsocketConnectionStatus.NotConnected;
        }

        private void Connect()
        {
            this.mSocket = new WebSocket(this.mUrl, new string[0]);
            this.mSocket.OnOpen += new EventHandler(this.OnWebsocketOnOpen);
            this.mSocket.OnError += new EventHandler<ErrorEventArgs>(this.OnWebsocketOnError);
            this.mSocket.OnMessage += new EventHandler<MessageEventArgs>(this.OnWebsocketOnMessage);
            this.mSocket.OnClose += new EventHandler<CloseEventArgs>(this.OnWebsocketOnClose);
            this.status = WebsocketConnectionStatus.Connecting;
            this.mSocket.ConnectAsync();
        }

        internal void Cleanup()
        {
            this.mSocket.OnOpen -= new EventHandler(this.OnWebsocketOnOpen);
            this.mSocket.OnError -= new EventHandler<ErrorEventArgs>(this.OnWebsocketOnError);
            this.mSocket.OnMessage -= new EventHandler<MessageEventArgs>(this.OnWebsocketOnMessage);
            this.mSocket.OnClose -= new EventHandler<CloseEventArgs>(this.OnWebsocketOnClose);
            if (this.mSocket.ReadyState == WebSocketState.Open || this.mSocket.ReadyState == WebSocketState.Connecting)
            {
                this.mSocket.Close();
            }
            this.mSocket = null;
        }

        internal void EnsureServerConnection()
        {
            if (this.status == WebsocketConnectionStatus.NotConnected)
            {
                this.Connect();
            }
        }

        private void OnWebsocketOnOpen(object sender, EventArgs evt)
        {
            Debug.Log("onWebsocketOnOpen");
            this.status = WebsocketConnectionStatus.Connected;
        }

        private void OnWebsocketOnClose(object sender, CloseEventArgs evt)
        {
            SLog.L("Closed: " + evt, new string[]
            {
                ServerConnection.LOGTAG
            });
            serverConnection.Cleanup();
        }

        private void OnWebsocketOnMessage(object sender, MessageEventArgs evt)
        {
            if (this.status == WebsocketConnectionStatus.Disconnecting || this.status == WebsocketConnectionStatus.NotConnected)
            {
                return;
            }
            NetworkEvent nevt = NetworkEvent.FromByteArray(evt.RawData);
            serverConnection.HandleIncomingEvent(nevt);
        }

        private void OnWebsocketOnError(object sender, ErrorEventArgs e)
        {
            SLog.LE("OnError: " + e, new string[]
            {
                ServerConnection.LOGTAG
            });
            if (this.status != WebsocketConnectionStatus.Disconnecting)
            {
                //WebsocketConnectionStatus arg_2F_0 = this.mStatus;
            }
        }

        internal void Send(NetworkEvent networkEvent)
        {
            byte[] msg = NetworkEvent.ToByteArray(networkEvent);
            mSocket.Send(msg);
        }
    }
}
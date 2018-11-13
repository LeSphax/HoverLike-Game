using Byn.Net;
using PlayerManagement;
using System.Collections.Generic;
using UnityEngine;

public delegate void NewMessageHandler(string message);

public class ChatManager : SlideBall.NetworkMonoBehaviour
{
    private List<ChatMessage> messageHistory = new List<ChatMessage>();

    public event NewMessageHandler NewMessage;

    public void UserWriteMessage(string content, bool sendToAll)
    {
        RPCTargets targets;
        if (sendToAll)
            targets = RPCTargets.All;
        else
            targets = RPCTargets.Team;
        View.RPC("ReceiveMessage", targets, content,MyComponents.Players.MyPlayerId, sendToAll);
    }

    [MyRPC]
    public void ReceiveMessage(string content, ConnectionId playerId, bool sentToAll)
    {
        ChatMessage newMessage = new ChatMessage(content, playerId, sentToAll);
        messageHistory.Add(newMessage);
        if (NewMessage != null)
        {
            NewMessage.Invoke(newMessage.ToString(MyComponents.Players.players));
        }
    }

    private class ChatMessage
    {
        public ConnectionId sender;
        public string content;
        public bool sentToAll;

        public ChatMessage(string content, ConnectionId sender, bool sentToAll)
        {
            this.content = content;
            this.sender = sender;
            this.sentToAll = sentToAll;
        }

        public string ToString(Dictionary<ConnectionId, Player> players)
        {
            string result;
            if (sentToAll)
                result = "[ALL] ";
            else
                result = "[TEAM] ";
            return result + players[sender].nickname + " : " + content + "\n";
        }
    }
}


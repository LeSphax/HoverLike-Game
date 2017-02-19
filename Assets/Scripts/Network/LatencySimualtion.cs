using Byn.Net;
using System.Collections.Generic;

public class LatencySimulation
{
    private int numberFramesToWait;

    private Queue<List<Message>> messagesWaitingToBeSent = new Queue<List<Message>>();
    private List<Message> currentList = new List<Message>();

    private WebRtcNetwork network;

    public LatencySimulation(WebRtcNetwork network, int numberFramesToWait)
    {
        this.network = network;
        this.numberFramesToWait = numberFramesToWait;
    }

    public void NewFrame()
    {
        messagesWaitingToBeSent.Enqueue(new List<Message>(currentList));
        if (messagesWaitingToBeSent.Count > numberFramesToWait)
        {
            List<Message> messages = messagesWaitingToBeSent.Dequeue();
            foreach (var message in messages)
            {
                network.peerNetwork.SendData(message.id, message.data, 0, message.data.Length, message.reliable);
            }
        }
        currentList.Clear();
    }

    public void AddMessage(byte[] data, ConnectionId id, bool reliable)
    {
        currentList.Add(new Message(data, id, reliable));
    }

    private struct Message
    {
        public byte[] data;
        public ConnectionId id;
        public bool reliable;

        public Message(byte[] data, ConnectionId id, bool reliable)
        {
            this.data = data;
            this.id = id;
            this.reliable = reliable;
        }
    }
}
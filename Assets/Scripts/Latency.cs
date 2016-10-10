using Byn.Net;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Latency : ObservedComponent
{
    public Dictionary<ConnectionId, Text> textfields = new Dictionary<ConnectionId, Text>();
    private float currentYPosition = 0;
    public GameObject textPrefab;

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        //LatencyPacket packet = NetworkExtensions.Deserialize<LatencyPacket>(data);
        //if (packet.isPing)
        //{
        //    ReceivePing(id, packet.time);
        //}
        //else
        //{
        //    ReceivePong(id, packet.time);
        //}
    }

    void ReceivePing(ConnectionId id, float time)
    {
        //SendData(MessageType.ViewPacket, new LatencyPacket(false, time).Serialize(), id);

    }

    public void ReceivePong(ConnectionId id, float time)
    {
        Debug.Log("ReceivePong");
        Text textfield;
        if (!textfields.TryGetValue(id, out textfield))
        {
            GameObject text = this.InstantiateAsChild(textPrefab);
            textfield = text.GetComponent<Text>();
            currentYPosition -= 50;
            textfield.rectTransform.localPosition += new Vector3(0, 1, 0) * currentYPosition;
            textfields.Add(id, textfield);
        }
        float latency = (Time.time - time) * 1000;

        textfield.text = id + " : " + latency + " ms";
    }

    protected override void OwnerUpdate()
    {
        //DoNothing
    }

    protected override void SimulationUpdate()
    {
        //DoNothing
    }
    void Update()
    {
        Debug.Log("Latencu");
    }

    protected override byte[] CreatePacket(long sendId)
    {
        return new LatencyPacket(true, Time.time).Serialize();
    }

    protected override bool IsSendingPackets()
    {
        return true;
    }

    [Serializable]
    public struct LatencyPacket
    {
        public bool isPing;
        public float time;

        public LatencyPacket(bool isPing, float time)
        {
            this.isPing = isPing;
            this.time = time;
        }
    }
}



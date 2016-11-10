using System;
using System.Collections.Generic;
using Byn.Net;
using UnityEngine;

public class PhysicsModelsManager : ObservedComponent
{
    private SortedDictionary<short,PhysicsModel> physicModels;

    private float lastServerTime;

    protected void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        physicModels = new SortedDictionary<short, PhysicsModel>();
    }

    public override void OwnerUpdate()
    {
        SimulateViews(Time.fixedDeltaTime);
    }

    public override void SimulationUpdate()
    {
        SimulateViews(Time.fixedDeltaTime);
    }

    private void SimulateViews(float dt)
    {
        foreach (var pair in physicModels)
        {
            pair.Value.CheckForPreSimulationActions();
        }
        foreach (var pair in physicModels)
        {
            pair.Value.Simulate(dt);
        }
        foreach (var pair in physicModels)
        {
            pair.Value.CheckForPostSimulationActions();
        }
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        int currentIndex = 0;
        float timePacketSent = BitConverter.ToSingle(data, currentIndex);
        if (timePacketSent > lastServerTime)
        {
            lastServerTime = timePacketSent;
            currentIndex += 4;

            foreach (var pair in physicModels)
            {
                currentIndex += pair.Value.DeserializeAndRewind(data, currentIndex);
            }

            float time = timePacketSent + Time.fixedDeltaTime;
            int x = 0;
            while (time < MyComponents.TimeManagement.NetworkTimeInSeconds - Time.fixedDeltaTime)
            {
                x++;
                if (x > 100)
                {
                    Debug.LogError("The loop ran more than 100 times");
                    break;
                }
                SimulateViews(Time.fixedDeltaTime);
                time += Time.fixedDeltaTime;
            }
            SimulateViews(MyComponents.TimeManagement.NetworkTimeInSeconds - time);
        }
    }

    protected override byte[] CreatePacket(long sendId)
    {
        byte[] data = BitConverter.GetBytes(MyComponents.TimeManagement.NetworkTimeInSeconds);
        foreach (var pair in physicModels)
        {
            data = ArrayExtensions.Concatenate(data, pair.Value.Serialize());
        }
        return data;
    }

    protected override bool IsSendingPackets()
    {
        return MyComponents.NetworkManagement.isServer;
    }

    public void RegisterView(short viewId, PhysicsModel model)
    {
        physicModels.Add(viewId,model);
    }

    internal void UnregisterView(short viewId)
    {
        physicModels.Remove(viewId);
    }
}

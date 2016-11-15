using System;
using System.Collections.Generic;
using Byn.Net;
using UnityEngine;
using PlayerManagement;

namespace PhysicsManagement
{

    public class ClientPhysicsMMStrategy : PhysicsModelManagerStrategy
    {
        public ClientPhysicsMMStrategy(PhysicsModelsManager manager) : base(manager)
        {
        }

        public override byte[] CreatePacket(out Dictionary<ConnectionId, byte[]> dataSpecificToClients)
        {
            dataSpecificToClients = null;
            if (Players.MyPlayer != null && Players.MyPlayer.physicsModel == null)
                return null;
            byte[] data = BitConverter.GetBytes(manager.lastSimulatedFrame);
            data = ArrayExtensions.Concatenate(data, Players.MyPlayer.physicsModel.SerializeInputs(manager.lastSimulatedFrame));
            //Debug.LogError((InputFlag)data[2] + " " +data.Length);
            return data;
        }

        public override void PacketReceived(ConnectionId id, byte[] data)
        {
            int currentIndex = 0;
            short ackFrame = BitConverter.ToInt16(data, currentIndex);

            if (ackFrame > manager.lastAckFrame)
            {
                currentIndex += 2;

                foreach (var pair in manager.physicModels)
                {
                    pair.Value.RemoveAcknowledgedInputs(manager.lastAckFrame, ackFrame);
                    currentIndex += pair.Value.DeserializeAndRewind(data, currentIndex);
                }
                manager.lastAckFrame = ackFrame;

                for (short i = manager.lastAckFrame; i <= manager.lastSimulatedFrame; i++)
                {
                    manager.SimulateViews(i, Time.fixedDeltaTime, false);
                }
            }
        }

        public override void RunTimeStep(short frameId, float dt)
        {
            manager.SimulateViews(frameId, Time.fixedDeltaTime, true);
        }
    }
}
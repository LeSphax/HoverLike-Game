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
            return data;
        }

        public override void PacketReceived(ConnectionId id, byte[] data)
        {
            int currentIndex = 0;
            short ackFrame = BitConverter.ToInt16(data, currentIndex);
            if (ackFrame > manager.lastAckFrame)
            {
                manager.lastAckFrame = ackFrame;
                currentIndex += 2;

                foreach (var pair in manager.physicModels)
                {
                    //Debug.LogError(pair.Value + "    " + currentIndex + "   " + data.Length);
                    currentIndex += pair.Value.DeserializeAndRewind(data, currentIndex);
                }
                for (short i = manager.lastAckFrame; i <= manager.lastSimulatedFrame; i++)
                {
                    manager.SimulateViews(i, Time.fixedDeltaTime, false);
                }
            }
        }

        public override void RunTimeStep(float dt)
        {
            throw new NotImplementedException();
        }
    }
}
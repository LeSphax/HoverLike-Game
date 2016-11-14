using System;
using System.Collections.Generic;
using Byn.Net;
using UnityEngine;
using PlayerBallControl;
using PlayerManagement;

namespace PhysicsManagement
{

    public class ServerPhysicsMMStrategy : PhysicsModelManagerStrategy
    {
        public ServerPhysicsMMStrategy(PhysicsModelsManager manager) : base(manager)
        {
        }

        private Dictionary<ConnectionId, short> lastAckFrameForEachClient = new Dictionary<ConnectionId, short>();

        private Queue<short> savedFrames = new Queue<short>();
        private Dictionary<short, List<AbilityEffect>> unacknowlegedEffects = new Dictionary<short, List<AbilityEffect>>();

        public override byte[] CreatePacket(out Dictionary<ConnectionId, byte[]> dataSpecificToClients)
        {
            dataSpecificToClients = new Dictionary<ConnectionId, byte[]>();
            foreach (var pair in lastAckFrameForEachClient)
            {
                dataSpecificToClients.Add(pair.Key, BitConverter.GetBytes(pair.Value));
            }
            byte[] data = new byte[0];
            foreach (var pair in manager.physicModels)
            {
                data = ArrayExtensions.Concatenate(data, pair.Value.Serialize());
            }
            //Debug.LogError(data.Length + "   " + manager.physicModels.Count);
            return data;
        }

        public override void PacketReceived(ConnectionId id, byte[] data)
        {
            int currentIndex = 0;

            short frameId = BitConverter.ToInt16(data, currentIndex);
            currentIndex += 2;

            if (!lastAckFrameForEachClient.ContainsKey(id))
                lastAckFrameForEachClient.Add(id, frameId);
            else
                lastAckFrameForEachClient[id] = frameId;

            InputFlag flags = (InputFlag)data[currentIndex];
            currentIndex++;
            List<AbilityEffect> effects = AbilityEffect.Deserialize(flags, data, currentIndex);
            foreach(var effect in effects)
            {
                effect.ApplyEffect(Players.players[id].physicsModel);
            }
        }

        public override void RunTimeStep(float dt)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using Byn.Net;
using UnityEngine;


namespace PhysicsManagement
{

    public class NotConnectedPhysicsMMStrategy : PhysicsModelManagerStrategy
    {
        public NotConnectedPhysicsMMStrategy(PhysicsModelsManager manager) : base(manager)
        {
        }

        public override byte[] CreatePacket(out Dictionary<ConnectionId, byte[]> dataSpecificToClients)
        {
            //DoNothing
            dataSpecificToClients = null;
            return null;
        }

        public override void PacketReceived(ConnectionId id, byte[] data)
        {
            //DoNothing
        }

        public override void RunTimeStep(short frameId, float dt)
        {
            //DoNothing
        }
    }
}
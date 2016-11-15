using Byn.Net;
using System.Collections.Generic;

namespace PhysicsManagement
{
    public abstract class PhysicsModelManagerStrategy
    {
        public int x = 100;

        protected PhysicsModelsManager manager;

        public PhysicsModelManagerStrategy(PhysicsModelsManager manager)
        {
            this.manager = manager;
        }

        public abstract void PacketReceived(ConnectionId id, byte[] data);

        public abstract byte[] CreatePacket(out Dictionary<ConnectionId, byte[]> dataSpecificToClients);

        public abstract void RunTimeStep(short frameId, float dt);

    }
}
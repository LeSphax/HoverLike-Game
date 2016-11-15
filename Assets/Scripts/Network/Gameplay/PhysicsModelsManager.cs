using System;
using System.Collections.Generic;
using Byn.Net;
using UnityEngine;

namespace PhysicsManagement
{
    public class PhysicsModelsManager : ObservedComponent
    {
        internal SortedDictionary<short, PhysicsModel> physicModels;

        internal short lastAckFrame;

        internal short lastSimulatedFrame = 0;

        private PhysicsModelManagerStrategy strategy;

        public bool Activated
        {
            get;
            set;
        }

        protected void Awake()
        {
            Reset();
            Activated = false;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                foreach (var model in physicModels.Values)
                {
                    if (model.transform.parent != null)
                        Debug.LogError(model.transform.parent.name);
                    else
                        Debug.LogError("Ball");
                }
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                strategy.x = 100;
            }
        }


        public void Reset()
        {
            physicModels = new SortedDictionary<short, PhysicsModel>();
            MyComponents.NetworkManagement.ConnectedToRoom -= CreateClientStrategy;
            MyComponents.NetworkManagement.RoomCreated -= CreateServerStrategy;
            strategy = new NotConnectedPhysicsMMStrategy(this);
            MyComponents.NetworkManagement.ConnectedToRoom += CreateClientStrategy;
            MyComponents.NetworkManagement.RoomCreated += CreateServerStrategy;
        }

        private void CreateClientStrategy()
        {
            strategy = new ClientPhysicsMMStrategy(this);
        }

        private void CreateServerStrategy()
        {
            strategy = new ServerPhysicsMMStrategy(this);
        }

        public override void SimulationUpdate()
        {
            float time = Time.realtimeSinceStartup;
            if (Activated)
            {
                lastSimulatedFrame++;
                strategy.RunTimeStep(lastSimulatedFrame,Time.fixedDeltaTime);
            }
            if (Time.realtimeSinceStartup - time > 0.01f)
            {
                Debug.LogWarning("Very long Physics" + (Time.realtimeSinceStartup - time));
            }
        }

        internal void SimulateViews(short frameNumber, float dt, bool isRealSimulation)
        {
            foreach (var pair in physicModels)
            {
                pair.Value.CheckForPreSimulationActions();
            }
            foreach (var pair in physicModels)
            {
                pair.Value.Simulate(frameNumber, dt, isRealSimulation);
            }
            foreach (var pair in physicModels)
            {
                pair.Value.CheckForPostSimulationActions();
            }
        }

        public override void PacketReceived(ConnectionId id, byte[] data)
        {
            strategy.PacketReceived(id, data);
        }

        protected override byte[] CreatePacket(out Dictionary<ConnectionId, byte[]> dataSpecificToClients)
        {
            return strategy.CreatePacket(out dataSpecificToClients);
        }

        protected override bool IsSendingPackets()
        {
            return Activated;
        }

        public void RegisterView(short viewId, PhysicsModel model)
        {
            physicModels.Add(viewId, model);
        }

        internal void UnregisterView(short viewId)
        {
            physicModels.Remove(viewId);
        }

        protected override bool SetFlags(out MessageFlags flags)
        {
            flags = MessageFlags.NotDistributed | MessageFlags.SceneDependant;
            return true;
        }
    }
}
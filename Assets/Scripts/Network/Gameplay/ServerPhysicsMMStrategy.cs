using System;
using System.Collections.Generic;
using Byn.Net;
using UnityEngine;
using PlayerBallControl;
using PlayerManagement;
using UnityEngine.Assertions;

namespace PhysicsManagement
{

    public class ServerPhysicsMMStrategy : PhysicsModelManagerStrategy
    {
        public ServerPhysicsMMStrategy(PhysicsModelsManager manager) : base(manager)
        {
        }

        private Dictionary<ConnectionId, short> lastAckFrameForEachClient = new Dictionary<ConnectionId, short>();

        private SortedList<float, byte[]> savedFrames = new SortedList<float, byte[]>();

        private SortedList<float, PlayerInput> inputsBuffer = new SortedList<float, PlayerInput>();

        private float? timeFirstUnacknowlegedInput;

        public override byte[] CreatePacket(out Dictionary<ConnectionId, byte[]> dataSpecificToClients)
        {
            dataSpecificToClients = new Dictionary<ConnectionId, byte[]>();

            byte[] data = new byte[0];
            foreach (var pair in manager.physicModels)
            {
                data = ArrayExtensions.Concatenate(data, pair.Value.Serialize());
            }
            savedFrames.Add(MyComponents.NetworkManagement.currentFrameTimestamp, data);
            if (savedFrames.Count > 20)
            {
                savedFrames.RemoveAt(0);
            }
            //
            foreach (var player in Players.players.Values)
            {
                if (player.id != Players.myPlayerId)
                {
                    float timeFrame = MyComponents.NetworkManagement.currentFrameTimestamp - MyComponents.TimeManagement.GetLatencyInMiliseconds(player.id) / 2000F;
                    int indexFrameToSend = Functions.GetIndexClosestFrame(timeFrame, savedFrames);
                    Debug.LogWarning(indexFrameToSend);
                    dataSpecificToClients.Add(player.id, ArrayExtensions.Concatenate(BitConverter.GetBytes(lastAckFrameForEachClient[player.id]), savedFrames.Values[indexFrameToSend]));
                }
            }
            //Debug.LogError(data.Length + "   " + manager.physicModels.Count);
            return new byte[0];
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
            if (effects.Count > 0)
            {
                float timestamp = MyComponents.NetworkManagement.currentFrameTimestamp - MyComponents.TimeManagement.GetLatencyInMiliseconds(id) / 2000f;
                if (!inputsBuffer.ContainsKey(timestamp))
                    inputsBuffer.Add(timestamp, new PlayerInput(id, effects));
                else
                    inputsBuffer[timestamp].AddEffects(effects);

                if (timeFirstUnacknowlegedInput == null || timestamp < timeFirstUnacknowlegedInput)
                {
                    timeFirstUnacknowlegedInput = timestamp;
                }
            }
        }

        public override void RunTimeStep(short frameId, float dt)
        {
            float time = Time.realtimeSinceStartup;
            float currentTime = MyComponents.NetworkManagement.currentFrameTimestamp;
            short numberFramesToSimulate = 1;
            if (timeFirstUnacknowlegedInput != null)
            {
                byte[] frameToRewindTo = null;

                for (int i = 1; i < savedFrames.Count; i++)
                {
                    if (savedFrames.Keys[i] > timeFirstUnacknowlegedInput.Value)
                    {
                        currentTime = savedFrames.Keys[i];
                        frameToRewindTo = savedFrames.Values[i - 1];
                        numberFramesToSimulate = (short)(savedFrames.Count - i);
                        break;
                    }
                }
                if (frameToRewindTo != null)
                {
                    int currentDataIndex = 0;
                    foreach (var pair in manager.physicModels)
                    {
                        currentDataIndex += pair.Value.DeserializeAndRewind(frameToRewindTo, currentDataIndex);
                    }
                }
            }
            int? currentInputIndex = null;
            if (timeFirstUnacknowlegedInput != null)
            {
                currentInputIndex = inputsBuffer.IndexOfKey(timeFirstUnacknowlegedInput.Value);
            }

            for (short i = 0; i < numberFramesToSimulate; i++)
            {
                currentTime += Time.fixedDeltaTime;
                while (currentInputIndex != null && inputsBuffer.Count > currentInputIndex && inputsBuffer.Keys[currentInputIndex.Value] < currentTime)
                {
                    PlayerPhysicsModel model = Players.players[inputsBuffer.Values[currentInputIndex.Value].playerId].physicsModel;
                    foreach (var effect in inputsBuffer.Values[currentInputIndex.Value].effects)
                    {
                        effect.ApplyEffect(model);
                    }
                    currentInputIndex++;
                }
                manager.SimulateViews((short)(frameId - numberFramesToSimulate + 1 + i), Time.fixedDeltaTime, i == numberFramesToSimulate - 1);
            }
            //manager.SimulateViews(frameId, Time.fixedDeltaTime, true);
            timeFirstUnacknowlegedInput = null;
            if (Time.realtimeSinceStartup - time > 0.01f)
            {
                Debug.LogWarning("Very long timeStep" + (Time.realtimeSinceStartup - time) + "    " + numberFramesToSimulate);
            }
        }
    }
}
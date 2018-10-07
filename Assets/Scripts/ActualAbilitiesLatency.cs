﻿using System;
using System.Linq;
using UnityEngine;

class ActualAbilitiesLatency
{

    public static QueueDictionary<Type, float> commandsSent = new QueueDictionary<Type, float>();
    public static ListDictionary<Type, float> abilitiesLatency = new ListDictionary<Type, float>();

    public static void Received(params Type[] types)
    {
        if (!NetworkingState.IsServer)
        {
            foreach (Type type in types)
            {
                float timeReceived = Time.realtimeSinceStartup * 1000;
                float timeSent = 0;
                while (timeSent == 0 && commandsSent.QueueCount(type) > 0)
                {
                    timeSent = commandsSent.PopQueue(type);
                }
                if (timeSent == 0)
                {
                    //Debug.LogError("The command sent wasn't registered");
                    break;
                }
                while (timeReceived - timeSent > 1 && commandsSent[type].Count > 0)
                {
                    timeSent = commandsSent.PopQueue(type);
                }
                abilitiesLatency.AddItem(type, timeReceived - timeSent);
                return;
            }
        }
    }

    public static string Print()
    {
        return String.Join(System.Environment.NewLine, abilitiesLatency.Keys.Select(key => key + " : Average(" + abilitiesLatency[key].Average()
        + "), Max(" + abilitiesLatency[key].Max()
        + "), Median(" + abilitiesLatency[key].Median()
        + "), 9/10percentile(" + abilitiesLatency[key].Percentile(9, 10)).ToArray());
    }

    public static string PrintAll()
    {
        return String.Join(System.Environment.NewLine, abilitiesLatency.Keys.Select(key => key + " : " + abilitiesLatency[key].PrintContent()).ToArray());
    }
}


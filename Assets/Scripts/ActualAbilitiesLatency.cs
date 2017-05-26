using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ActualAbilitiesLatency
{

    public static QueueDictionary<Type, float> commandsSent = new QueueDictionary<Type, float>();
    public static ListDictionary<Type, float> abilitiesLatency = new ListDictionary<Type, float>();

    public static void Received(params Type[] types)
    {
        if (!MyComponents.NetworkManagement.isServer)
        {
            float timeReceived = Time.realtimeSinceStartup * 1000;
            float timeSent = 0;
            int i = 0;
            while (timeSent == 0 && i < types.Length)
            {
                timeSent = commandsSent.PopQueue(types[i]);
            }
            if (timeSent == 0)
            {
                Debug.LogError("The command sent wasn't registered");
                return;
            }
            while (timeReceived - timeSent > 1 && commandsSent[types[i]].Count > 0)
            {
                timeSent = commandsSent.PopQueue(types[i]);
            }
            abilitiesLatency.AddItem(types[i], timeReceived - timeSent);
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


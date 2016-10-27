//using UnityEngine;
//using System.Collections;
//using UnityEngine.UI;
//using System.Collections.Generic;

//public class PacketLoss : MonoBehaviour {

//    static List<PlayerMovementView> views = new List<PlayerMovementView>();
//    public Text textfield;

//    // Use this for initialization
//    void Start()
//    {
//        InvokeRepeating("RefreshText", 0f, 1f);
//    }

//    public static void AddView(PlayerMovementView view)
//    {
//        views.Add(view);
//    }

//    void RefreshText()
//    {
//        float packetLossSum = 0;
//        for(int i =0; i<views.Count; i++)
//        {
//            packetLossSum += views[i].packetLossRatio;
//        }
//        float packetLoss = packetLossSum / views.Count;
//        textfield.text = (packetLoss * 100) + " %";
//    }

//}

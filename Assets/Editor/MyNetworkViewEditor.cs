//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(MyNetworkView))]
//public class MyNetworkViewEditor : Editor
//{

//    void OnEnable()
//    {
//        MyNetworkView myTarget = (MyNetworkView)target;
//        if (myTarget.viewId == 0)
//        {
//            myTarget.viewId = MyNetworkView.nextViewId;
//            MyNetworkView.nextViewId++;
//        }
//    }

//}
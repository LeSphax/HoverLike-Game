using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlideBall
{
    public class MonoBehaviour : UnityEngine.MonoBehaviour
    {
        private MyNetworkView view;
        public MyNetworkView View
        {
            get
            {
                if (view == null)
                {
                    view = GetComponent<MyNetworkView>();
                }
                return view;
            }
        }

    }
}

using UnityEngine;

namespace SlideBall
{

    public class MonoBehaviour : UnityEngine.MonoBehaviour
    {
        private MyComponents components;
        public MyComponents MyComponents
        {
            get
            {
                if (components == null)
                {
                    components = GetComponentInParent<MyComponents>();
                }
                return components;
            }
        }

    }

    [RequireComponent(typeof(MyNetworkView))]
    public class NetworkMonoBehaviour : MonoBehaviour
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

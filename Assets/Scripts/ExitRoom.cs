using UnityEngine;
using System.Collections;

public class ExitRoom : MonoBehaviour {

	public void Exit()
    {
        MyComponents.ResetNetworkComponents();
    }
}

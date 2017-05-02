using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayersOnCage : MonoBehaviour
{

    public static List<GameObject> playersOnCage = new List<GameObject>();

    public static Dictionary<GameObject, Coroutine> playersRemoval = new Dictionary<GameObject, Coroutine>();

    private const float delayTime = 0.1f;

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("OnCollisiionEnter " + collision.gameObject.name + "   " + collision.gameObject.tag);
        GameObject player = collision.gameObject;
        if (LayersGetter.IsAttacker(collision.gameObject.layer))
        {
            //Debug.Log("Add gameobject " + player.name);
            playersOnCage.Add(player);
            Coroutine coroutine = StartCoroutine(RemovePlayerOnCage(player, delayTime));
            playersRemoval.Add(player, coroutine);
        }
    }

    private IEnumerator RemovePlayerOnCage(GameObject player, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        RemovePlayer(player);
    }

    public static void RemovePlayer(GameObject player)
    {
        playersOnCage.Remove(player);
        playersRemoval.Remove(player);
    }

}

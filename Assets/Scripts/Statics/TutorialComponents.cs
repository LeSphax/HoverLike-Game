using BaseNetwork;
using Navigation;
using PlayerManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class TutorialComponents
{

    private static Spawns spawns;
    public static Spawns Spawns
    {
        get
        {
            Assert.IsTrue(Scenes.IsCurrentScene(Scenes.MainIndex));
            if (spawns == null)
            {
                    spawns = GetTaggedComponent<Spawns>(Tags.Spawns);
            }
            return spawns;
        }
    }

    private static Type GetTaggedComponent<Type>(string tag)
    {
        GameObject go = GameObject.FindGameObjectWithTag(tag);
        if (go != null)
            return go.GetComponent<Type>();
        return default(Type);
    }
}



using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextureAtlas : MonoBehaviour
{

    public float xMin = 0;
    public float xMax = 1;
    public float yMin = 0;
    public float yMax = 1;

    public float tilingX = 1;
    public float tilingY = 1;

    public bool log = false;


    private void Start()
    {
        var meshs = from cust in GetComponentsInChildren<MeshRenderer>()
                    select cust.GetComponent<MeshFilter>().mesh;
        //Texture texture = GetComponentInChildren<MeshRenderer>().material.mainTexture;
        //Debug.Log(texture.width + "   " + texture.height);

        foreach (Mesh mesh in meshs)
        {
            Debug.Log(mesh.vertexCount);
            Debug.Log(mesh.name);


            Vector2[] uvs = mesh.uv;
            for (int i = 0; i < uvs.Length; i++)
            {
                if (log)
                    Debug.Log("Before " + uvs[i].x);
                uvs[i].x = uvs[i].x * tilingX * (xMax-xMin) + xMin;
                uvs[i].y = uvs[i].y * tilingY * (yMax-yMin) + yMin;
                if (log)
                    Debug.Log(uvs[i].x);
            }
            mesh.uv = uvs;
        }

        

    }
}
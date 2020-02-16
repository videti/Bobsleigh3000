using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLoadMesh : MonoBehaviour
{
    MeshFilter meshFilter;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter.sharedMesh == null)
        {
            meshFilter.sharedMesh =  Resources.Load<Mesh>("Meshes/TobogganGenerator_" + name + "_mesh");
            foreach(MeshCollider meshCollider in GetComponents<MeshCollider>())
            {
                Destroy(meshCollider);
            }
            gameObject.AddComponent<MeshCollider>();
        }
    }
}

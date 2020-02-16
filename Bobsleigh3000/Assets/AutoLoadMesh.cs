using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLoadMesh : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Mesh _mesh;

    private void Awake()
    {
        _mesh = Resources.Load<Mesh>("Meshes/TobogganGenerator_" + name + "_mesh");
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    // Start is called before the first frame update
    void Update()
    {
        
        if (meshFilter.sharedMesh == null)
        {
            meshFilter.sharedMesh = _mesh;
        }
        if (meshCollider.sharedMesh == null)
        {
            meshCollider.sharedMesh = _mesh;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform t in GetComponentsInChildren<Transform>())
        {
            if (t != transform)
            {
                t.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
                t.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
                t.GetComponent<MeshFilter>().sharedMesh.RecalculateTangents();
                t.gameObject.AddComponent<MeshCollider>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

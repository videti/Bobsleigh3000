using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingOffset : MonoBehaviour
{
    MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        StartCoroutine(MoveMatOffset());
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    IEnumerator MoveMatOffset()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            meshRenderer.material.SetTextureOffset("_MainTex", meshRenderer.material.GetTextureOffset("_MainTex") + Vector2.right * 0.33f);
        }
    }
}

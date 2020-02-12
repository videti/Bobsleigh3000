using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    public float speedBoost = 10f;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.GetComponent<AdjustRotation>() != null)
        {
            collision.collider.GetComponent<AdjustRotation>().speedBoost = speedBoost;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.GetComponent<AdjustRotation>() != null)
        {
            collision.collider.GetComponent<AdjustRotation>().speedBoost = 0;
        }
    }
}

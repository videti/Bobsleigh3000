using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlePlayerCollisions : MonoBehaviour
{
    public float boostSpeed = 1.2f;
    FollowingBezierCurve playerController;

    private void Start()
    {
        playerController = GetComponentInParent<FollowingBezierCurve>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Boost")
        {
            playerController.frontSpeed = Mathf.Max(playerController.frontSpeed, boostSpeed);
            Debug.Log(playerController.frontSpeed);
        }
    }
}

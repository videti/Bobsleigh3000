using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlePlayerCollisions : MonoBehaviour
{
    public float boostSpeed = 1.2f;
    FollowingBezierCurve playerController;
    Rigidbody rigidBody;
    FollowingBezierCurve followingBezierCurve;

    private void Start()
    {
        playerController = GetComponentInParent<FollowingBezierCurve>();
        rigidBody = GetComponent<Rigidbody>();
        followingBezierCurve = GetComponent<FollowingBezierCurve>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Boost")
        {
            playerController.frontSpeed = Mathf.Max(playerController.frontSpeed, boostSpeed);
        }
        else
        if(other.tag == "Hole")
        {
            rigidBody.isKinematic = false;
            rigidBody.constraints = RigidbodyConstraints.None;
            followingBezierCurve.enabled = false;
            Invoke("ResetFall", 2f);
        }
    }

    void ResetFall()
    {
        followingBezierCurve.AddTimeToTimeBuffer(0.02f);
        rigidBody.isKinematic = true;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        followingBezierCurve.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Destructible")
        {
            playerController.frontSpeed = Mathf.Max(0.05f, playerController.frontSpeed - 0.1f);
            Destroy(collision.collider);
        }
    }
}

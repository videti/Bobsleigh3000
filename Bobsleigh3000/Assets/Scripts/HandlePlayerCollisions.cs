﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlePlayerCollisions : MonoBehaviour
{
    public float boostSpeed = 1.2f;
    FollowingBezierCurve playerController;
    Rigidbody rigidBody;
    FollowingBezierCurve followingBezierCurve;
    List<GameObject> groundTriggerList = new List<GameObject>();
    bool falling = false;

    private void Start()
    {
        playerController = GetComponentInParent<FollowingBezierCurve>();
        rigidBody = GetComponent<Rigidbody>();
        followingBezierCurve = GetComponent<FollowingBezierCurve>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boost")
        {
            playerController.frontSpeed = Mathf.Max(playerController.frontSpeed, boostSpeed);
        }
        else if (other.gameObject.layer == 10)
        {
            groundTriggerList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            groundTriggerList.Remove(other.gameObject);
            if (groundTriggerList.Count == 0 && followingBezierCurve.jumping == false && !falling)
            {
                rigidBody.isKinematic = false;
                rigidBody.constraints = RigidbodyConstraints.None;
                followingBezierCurve.enabled = false;
                falling = true;
                Invoke("ResetFall", 2f);
            }
        }
    }

    void ResetFall()
    {
        followingBezierCurve.AddTimeToTimeBuffer(0.02f);
        rigidBody.isKinematic = true;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        followingBezierCurve.enabled = true;
        Invoke("StopFalling", 0.5f);
    }

    void StopFalling()
    {
        falling = false;
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

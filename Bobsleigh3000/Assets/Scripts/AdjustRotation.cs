using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustRotation : MonoBehaviour
{

    public float speedBoost = 0f;
    Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit raycastHit;

        Vector3 downPos = new Vector3(transform.position.x, transform.position.y - (GetComponent<Collider>().bounds.size.y / 1.9f), transform.position.z);

        if (Physics.Raycast(downPos, -transform.up, out raycastHit))
        {
            //Quaternion inputFrame = TurretLookRotation(transform.forward, raycastHit.normal);
            //inputFrame.y = raycastHit.normal.z;
            //transform.rotation = Quaternion.Lerp(transform.rotation, inputFrame, Time.deltaTime * 5f);

            bool grounded = raycastHit.distance <= 0.1f;
            Vector3 dir = Vector3.Normalize(raycastHit.point - downPos);
            Quaternion lookat = Quaternion.FromToRotation(ComputeForward(raycastHit, grounded), transform.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookat, Time.deltaTime * 3f);

            //Debug.Log(transform.up + " - " + raycastHit.normal);
            //transform.LookAt(transform.position, raycastHit.normal);
            //Quaternion lookat = Quaternion.FromToRotation(dir, -raycastHit.normal);
            //lookat = Quaternion.LookRotation(Vector3.Cross(transform.right, raycastHit.normal));

            // Getting the forward(pitch angle), based on current ground normal(raycastHit result).
            Vector3 SlopeForward = Vector3.Cross(transform.right, raycastHit.normal);

            // therefore
            float SlopeAngle = Vector3.SignedAngle(transform.forward, SlopeForward, Vector3.up);
            Debug.Log(name + " - "+SlopeAngle);
        }

        _rigidbody.AddRelativeForce(transform.rotation * Vector3.forward * speedBoost);
    }

    Vector3 ComputeForward(RaycastHit raycastHit, bool grounded = true)
    {
        if (!grounded)
        {
            //return transform.forward;
        }
        return Vector3.Cross(raycastHit.normal, -transform.right);
    }

    Quaternion TurretLookRotation(Vector3 approximateForward, Vector3 exactUp)
    {
        Quaternion rotateZToUp = Quaternion.LookRotation(exactUp, -approximateForward);
        Quaternion rotateYToZ = Quaternion.Euler(90f, 0f, 0f);

        return rotateZToUp * rotateYToZ;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPermanentForce : MonoBehaviour
{
    public float force = 2f;
    public enum Axe{
        axeX,
        axeY,
        axeZ
    }

    public Axe axe = Axe.axeX;
    Rigidbody _rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion inverseRotation = Quaternion.Euler(-1f * transform.rotation.eulerAngles.x, -1f * transform.rotation.eulerAngles.y, -1f * transform.rotation.eulerAngles.z);
        _rigidbody.AddRelativeForce(inverseRotation * transform.forward * force);
    }
}

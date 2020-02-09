using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingBezierCurve : MonoBehaviour
{
    public Vector3[] ctrlPoints = {
        new Vector3(-27.86408f, 11.84466f),
        new Vector3(-16.08414f, -14.69256f),
        new Vector3(13.81877f, -13.20388f),
        new Vector3(14.33657f, 13.33333f)
    };

    public float totalTime = 10f;
    public float angle = 40f;
    public float radius = 1f;

    float timeBuffer = 0f;


    Vector3 origin, nextOrigin;

    private void Start()
    {
        nextOrigin = Vector3.zero;
        radius = 1f - GetComponentInChildren<Collider>().bounds.size.y * 0.7f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 origin = nextOrigin == Vector3.zero ? BezierShape.GetBezierCurvePointAtT(ctrlPoints, timeBuffer) : nextOrigin;
        timeBuffer += Time.deltaTime / totalTime;
        nextOrigin = BezierShape.GetBezierCurvePointAtT(ctrlPoints, timeBuffer);
        Vector3 dir = Vector3.Normalize(nextOrigin - origin);        
        Vector3 newPosition = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        newPosition = Quaternion.LookRotation(dir) * newPosition;
        newPosition += origin;
        transform.LookAt(newPosition);
        transform.position = newPosition;
    }
}

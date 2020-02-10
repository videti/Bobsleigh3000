using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingBezierCurve : MonoBehaviour
{
    [Range(0, 3f)]
    public float speed = 1f;
    [Range(10f, 30f)]
    public float totalTime = 30f;
    [Range(0f, 360f)]
    public float angle = 40f;
    [Range(0f, 1f)]
    public float radius = 1f;

    public Vector3 localCamPos = new Vector3(0, 0.8f, -0.8f);

    public Camera thirdPersonCam;
    float timeBuffer = 0f;

    public ScriptableControlPoints scriptableControl;


    Vector3 origin, nextOrigin;

    private void Start()
    {
        nextOrigin = Vector3.zero;
        radius = 1f - GetComponentInChildren<Collider>().bounds.size.y * 0.7f;
    }

    private void OnEnable()
    {
        thirdPersonCam.enabled = true;
    }

    private void OnDisable()
    {
        if(thirdPersonCam!= null)
            thirdPersonCam.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //origin (x, y z) est un point de la courbe à l'instant t
        origin = nextOrigin == Vector3.zero ? BezierShape.GetBezierCurvePointAtT(scriptableControl.ctrlPoints, timeBuffer) : nextOrigin;
        timeBuffer += speed * Time.deltaTime / totalTime;
        //nextOrigin (x, y z) est un point de la courbe à l'instant t + delta
        nextOrigin = BezierShape.GetBezierCurvePointAtT(scriptableControl.ctrlPoints, timeBuffer);
        //dir => le vecteur directeur entre le point t et t+1
        Vector3 dir = Vector3.Normalize(nextOrigin - origin);
        //position sur l'arc de cercle autour de l'origine
        Vector3 newPosition = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        newPosition = Quaternion.LookRotation(dir) * newPosition;
        newPosition += origin;
        //on modifie le transform
        transform.LookAt(newPosition);
        transform.position = newPosition;
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, angle + 90f);

        //on replace la caméra
        Vector3 farOrigin = BezierShape.GetBezierCurvePointAtT(scriptableControl.ctrlPoints, timeBuffer + 0.05f);
        thirdPersonCam.transform.position = origin + thirdPersonCam.transform.rotation * localCamPos;
        thirdPersonCam.transform.LookAt(farOrigin + thirdPersonCam.transform.rotation * localCamPos);
        thirdPersonCam.transform.Rotate(30f, 0, 0);

    }
}

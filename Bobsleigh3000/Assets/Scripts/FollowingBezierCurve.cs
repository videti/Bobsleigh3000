using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingBezierCurve : MonoBehaviour
{
    [Range(-1, 3f)]
    public float speed = 1f;
    [Range(10f, 30f)]
    public float totalTime = 30f;
    [Range(0f, 360f)]
    public float angle = 40f;
    [Range(0f, 1.5f)]
    public float radius = 1f;
    public float accelerateForce = 3f;
    public float moveSpeed = 300f;
    public float maxDistanceToClick = 5f;


    public Vector3 localCamPos = new Vector3(0, 0.8f, -0.8f);
    public float localCamRotationX = 25f;

    public Camera thirdPersonCam;
    float timeBuffer = 0f;

    public ScriptableControlPoints scriptableControl;


    Vector3 origin, nextOrigin;

    private void Start()
    {
        nextOrigin = Vector3.zero;
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
        speed += -dir.y * accelerateForce * Time.deltaTime;
        speed = Mathf.Min(Mathf.Max(0.05f, speed), 3f);

        //position sur l'arc de cercle autour de l'origine
        Vector3 newPosition = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        newPosition = Quaternion.LookRotation(dir) * newPosition;
        newPosition += origin;

        //on modifie le transform
        transform.LookAt(transform.position + dir * 10f);
        transform.position = newPosition;
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, angle + 90f);

        //on replace la caméra
        thirdPersonCam.transform.position = origin + thirdPersonCam.transform.rotation * localCamPos;
        thirdPersonCam.transform.LookAt(thirdPersonCam.transform.position + dir * 10f);
        thirdPersonCam.transform.Rotate(localCamRotationX, 0, 0);

        //handle click
        if (Input.GetMouseButton(0))
            ClickOnGround();
    }

    //adjust angle when a click on ground is performed
    void ClickOnGround()
    {
        var ray = thirdPersonCam.ScreenPointToRay( Input.mousePosition);

        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, 10))
        {
            if (hit.distance <= maxDistanceToClick)
            {
                float signedDifAngle = Vector3.SignedAngle(-transform.up, -hit.normal, transform.forward);
                angle += signedDifAngle * moveSpeed * Time.deltaTime;
            }
        }
    }
}

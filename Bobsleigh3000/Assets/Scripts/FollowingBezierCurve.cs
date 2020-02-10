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


    public Vector3 localCamPos = new Vector3(0, 0.8f, -0.8f);

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
        Debug.Log(speed);

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
        thirdPersonCam.transform.Rotate(30f, 0, 0);
    }

    private void OnMouseDown()
    {
        Ray ray = thirdPersonCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.collider);
            //ground touched
            if(hit.collider.gameObject.layer == 10)
            {
                Debug.Log("ground touched");
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingBezierCurve : MonoBehaviour
{
    //[Header("Control Points Array")]
    //public ScriptableControlPoints[] scriptablesControls;
    [Space()]
    [Header("Game General Params")]
    //game general params
    [Range(1f, 30f)]
    public float totalTime = 30f; //the time to finish the game if speed = 1
    [Range(0.01f, 3f)]
    public float frontSpeed = 1f; //default front speed
    [Range(0.01f, 3f)]
    public float minFrontSpeed = 0.3f; //min front speed
    [Range(1f, 20f)]
    public float lateralSpeed = 3f; //to change the angle
    [Range(0.1f, 20f)]
    public float lateralGravityForce = 0.5f; //to recenter the character
    [Range(0f, 360f)]
    public float angle = 40f; //default angle
    [Range(0f, 1.5f)]
    public float radius = 1f; //radius of invisible pipe to move around
    [Range(0.001f, 20f)]
    public float accelerateForce = 3f; //increase acceleration and deceleration for front speed
    [Range(1f, 20f)]
    public float maxDistanceToClick = 5f; //max dist of the clickable area on the ground
    [Space()]
    [Header("Camera Params")]
    //Camera public params
    public Camera thirdPersonCam;
    public Vector3 localCamPos = new Vector3(0, 0.8f, -0.8f);
    [Range(0f, 90f)]
    public float localCamRotationX = 25f;
    [Space()]
    [Header("Jump params")]
    public bool jumping = false;
    public float initialForce = 50f;
    public float gravityForce = 9.8f;

    Coroutine jumpCoroutine;
    float currentJumpForce;
    float timeBuffer = 0f;
    Vector3 origin, nextOrigin;
    int currentPartIndex = 0;

    TobogganGenerator tobogganGenerator;

    private void Start()
    {
        tobogganGenerator = GameObject.FindObjectOfType<TobogganGenerator>();
        nextOrigin = Vector3.zero;
        currentJumpForce = initialForce;
    }

    private void OnEnable()
    {
        thirdPersonCam.enabled = true;
    }

    private void OnDisable()
    {
        //if(thirdPersonCam!= null)
        //    thirdPersonCam.enabled = false;
    }

    public void AddTimeToTimeBuffer(float time)
    {
        timeBuffer += time;
    }

    // Update is called once per frame
    void Update()
    {
        if(tobogganGenerator.transform.childCount == 0)
        {
            return;
        }
        timeBuffer += frontSpeed * Time.deltaTime / totalTime;
        if(timeBuffer >= 1f)
        {
            timeBuffer -= 1f;
            currentPartIndex++;
            if (currentPartIndex >= tobogganGenerator.tobogganParts.Length)
                currentPartIndex = 0;
        }
        //origin (x, y z) est un point de la courbe à l'instant t
        origin = nextOrigin == Vector3.zero ? BezierShape.GetBezierCurvePointAtT(tobogganGenerator.tobogganParts[currentPartIndex].ctrlPoints, timeBuffer) : nextOrigin;


        //nextOrigin (x, y z) est un point de la courbe à l'instant t + delta
        nextOrigin = BezierShape.GetBezierCurvePointAtT(tobogganGenerator.tobogganParts[currentPartIndex].ctrlPoints, timeBuffer);

        //dir => le vecteur directeur entre le point t et t+1
        Vector3 dir = Vector3.Normalize(nextOrigin - origin);
        if(frontSpeed > minFrontSpeed || dir.y <= 0)
            frontSpeed += -dir.y * accelerateForce * Time.deltaTime * frontSpeed;
        frontSpeed = Mathf.Min(Mathf.Max(0.05f, frontSpeed), 3f);
        if(frontSpeed < minFrontSpeed)
        {
            frontSpeed += Time.deltaTime * accelerateForce;
        }

        //position sur l'arc de cercle autour de l'origine
        Vector3 newPosition = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        newPosition = Quaternion.LookRotation(dir) * newPosition;
        newPosition += origin;

        //on modifie le transform
        transform.LookAt(transform.position + dir * 10f);
        transform.position = newPosition;
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, angle + 90f);
        transform.Translate(tobogganGenerator.translations[currentPartIndex]);

        //on replace la caméra
        thirdPersonCam.transform.position = origin + thirdPersonCam.transform.rotation * localCamPos;
        thirdPersonCam.transform.LookAt(thirdPersonCam.transform.position + dir * 10f);
        thirdPersonCam.transform.Rotate(localCamRotationX, 0, 0);

        //handle jump
        if (Input.GetButtonDown("Jump"))
        {
            jumping = true;
        }
        if (!jumping)
        {
            //handle click
            if (Input.GetMouseButton(0))
                ClickOnGround();

            //gravity force simulation
            RecenterCharacter();
        } else if(jumpCoroutine == null)
        {
            jumpCoroutine = StartCoroutine(Jump());
        }
    }

    IEnumerator Jump()
    {
        currentJumpForce = initialForce * Mathf.Max(0.5f, (Mathf.Min(2f, frontSpeed)));
        Transform bodyTransform = transform.GetChild(0);
        while(currentJumpForce > 0 || bodyTransform.transform.localPosition.y > 0)
        {
            yield return null;
            bodyTransform.transform.localPosition = new Vector3(0, bodyTransform.transform.localPosition.y + currentJumpForce * Time.deltaTime, 0);
            currentJumpForce -= gravityForce * Time.deltaTime;
        }
        bodyTransform.transform.localPosition = Vector3.zero;
        jumping = false;
        jumpCoroutine = null;
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
                if(Mathf.Abs(signedDifAngle) > 5f)
                    angle += signedDifAngle * lateralSpeed * Time.deltaTime;
            }
        }
    }

    void RecenterCharacter()
    {
        float signedDifAngle = Vector3.SignedAngle(-transform.up, Vector3.down, transform.forward);
        if (Mathf.Abs(signedDifAngle) > 50f)
            angle += signedDifAngle * Time.deltaTime * 0.5f;
    }
}

using System.Collections;
using UnityEngine;

public class FollowingBezierCurve : MonoBehaviour
{
    //[Header("Control Points Array")]
    //public ScriptableControlPoints[] scriptablesControls;
    [Space()]
    [Header("Game General Params")]
    //game general params
    [Range(1f, 50f)]
    public float totalTime = 30f; //the time to finish the game if speed = 1
    [Range(0.01f, 6f)]
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
    [Range(0.000f, 20f)]
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

    [System.Serializable]
    public enum CreateMode
    {
        None,
        Boost,
        Wall
    }
    public CreateMode createMode;

    Coroutine jumpCoroutine;
    float currentJumpForce;
    float timeBuffer = 0f;
    Vector3 origin, nextOrigin;
    int currentPartIndex = 0;

    public TobogganGenerator tobogganGenerator;

    private void Start()
    {
        //tobogganGenerator = GameObject.FindObjectOfType<TobogganGenerator>();
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
        
        if (timeBuffer >= 1f - (frontSpeed * Time.deltaTime / totalTime))
        {
            nextOrigin = Vector3.zero;
            //change current index
            currentPartIndex++;
            if (currentPartIndex >= tobogganGenerator.tobogganParts.Length)
            {
                currentPartIndex = 0;
                timeBuffer = 0;
            }
            else
            {
                timeBuffer -= 1f;
                //Vector3 estimatedBezierPoint = BezierShape.GetBezierCurvePointAtT(tobogganGenerator.tobogganParts[currentPartIndex - 1].ctrlPoints, 1f);
                //estimatedBezierPoint = tobogganGenerator.rotations[currentPartIndex - 1] * estimatedBezierPoint + tobogganGenerator.translations[currentPartIndex - 1];
                //Vector3 newEstimatedBezierPoint = BezierShape.GetBezierCurvePointAtT(tobogganGenerator.tobogganParts[currentPartIndex].ctrlPoints, 0f);
                //newEstimatedBezierPoint = tobogganGenerator.rotations[currentPartIndex] * newEstimatedBezierPoint + tobogganGenerator.translations[currentPartIndex];

                //Debug.Log(estimatedBezierPoint);
                //Debug.Log(newEstimatedBezierPoint);
                //Debug.Log(Vector3.Distance(estimatedBezierPoint, newEstimatedBezierPoint));
            }
            
        }

        //origin (x, y z) est un point de la courbe à l'instant t
        origin = nextOrigin == Vector3.zero ? BezierShape.GetBezierCurvePointAtT(tobogganGenerator.tobogganParts[currentPartIndex].ctrlPoints, timeBuffer) : nextOrigin;
        timeBuffer += frontSpeed * Time.deltaTime / totalTime;
        //nextOrigin (x, y z) est un point de la courbe à l'instant t + delta
        nextOrigin = BezierShape.GetBezierCurvePointAtT(tobogganGenerator.tobogganParts[currentPartIndex].ctrlPoints, timeBuffer);

        //dir => le vecteur directeur entre le point t et t+1
        Vector3 dir = Vector3.Normalize(nextOrigin - origin);
        if(frontSpeed > minFrontSpeed || dir.y <= 0)
            frontSpeed += -dir.y * accelerateForce * Time.deltaTime * frontSpeed;
        frontSpeed = Mathf.Min(Mathf.Max(0.01f, frontSpeed), 6f);
        if(frontSpeed < minFrontSpeed)
        {
            frontSpeed += Time.deltaTime * accelerateForce;
        }

        //position sur l'arc de cercle autour de l'origine
        Vector3 newPosition = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        newPosition = tobogganGenerator.rotations[currentPartIndex] * Quaternion.LookRotation(dir) * newPosition;
        Vector3 translatedAndRotatedOrigin = tobogganGenerator.rotations[currentPartIndex] * origin + tobogganGenerator.translations[currentPartIndex];
        newPosition += translatedAndRotatedOrigin;

        //on modifie le transform
        Vector3 newDir = tobogganGenerator.rotations[currentPartIndex] * (dir * 10f);
        transform.position = newPosition;
        transform.LookAt(transform.position + newDir);
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, angle + 90f);
        
        //arrondi
        //transform.position = new Vector3(Mathf.Round(transform.position.x * 100f) * 0.01f, Mathf.Round(transform.position.y * 100f) * 0.01f, Mathf.Round(transform.position.z * 100f) * 0.01f);

        //on replace la caméra
        thirdPersonCam.transform.position = translatedAndRotatedOrigin + thirdPersonCam.transform.rotation * localCamPos;
        thirdPersonCam.transform.LookAt(thirdPersonCam.transform.position + newDir);
        thirdPersonCam.transform.Rotate(localCamRotationX, 0, 0);
        //arrondi
        //thirdPersonCam.transform.position = new Vector3(Mathf.Round(thirdPersonCam.transform.position.x * 100f) * 0.01f, Mathf.Round(thirdPersonCam.transform.position.y * 100f) * 0.01f, Mathf.Round(thirdPersonCam.transform.position.z * 100f) * 0.01f);

        //handle jump
        if (Input.GetButtonDown("Jump"))
        {
            jumping = true;
        }
        if (!jumping)
        {
            //handle click
            if (Input.GetMouseButton(0))
                ClickOnGround(newDir);

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
    void ClickOnGround(Vector3 newDir)
    {
        var ray = thirdPersonCam.ScreenPointToRay( Input.mousePosition);

        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.distance <= maxDistanceToClick)
            {
                float signedDifAngle = Vector3.SignedAngle(-transform.up, -hit.normal, transform.forward);
                if(Mathf.Abs(signedDifAngle) > 5f)
                    angle += signedDifAngle * lateralSpeed * Time.deltaTime;

                if (createMode == CreateMode.Boost && Input.GetMouseButtonDown(0))
                {
                    GameObject boost = Instantiate(Resources.Load<GameObject>("Prefabs/Boost"), GameObject.Find("Boosts").transform);
                    boost.name = "Boost" + GameObject.Find("Boosts").transform.childCount;
                    boost.transform.position = hit.point;
                    boost.transform.LookAt(newDir + transform.position);
                    boost.transform.Rotate(0, -90f, 0);
                    //boost
                } else if (createMode == CreateMode.Wall && Input.GetMouseButtonDown(0))
                {
                    GameObject wall = Instantiate(Resources.Load<GameObject>("Prefabs/Wall"), GameObject.Find("Walls").transform);
                    wall.name = "Wall" + GameObject.Find("Walls").transform.childCount;
                    wall.transform.position = hit.point;
                    wall.transform.LookAt(newDir + transform.position);
                }
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

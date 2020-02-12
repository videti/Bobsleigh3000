using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControlPoints : MonoBehaviour
{
    public float sphereSize = 1f;
    [Range(-9, 9)]
    public float sphereZPos = 1f;

    public ScriptableControlPoints savedControlPoints;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseP = Input.mousePosition;
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(mouseP.x, mouseP.y, 0f));
            sphere.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mouseP.x, mouseP.y, 10f));
            sphere.transform.position = new Vector3(sphere.transform.position.x, sphere.transform.position.y, sphereZPos);
            sphere.transform.localScale = sphereSize * Vector3.one;
            sphere.transform.parent = transform;
            sphere.name = "s" + transform.childCount;
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Pressed right click.");

        }
        if (Input.GetMouseButtonDown(2)) Debug.Log("Pressed middle click.");
    }

    public void GenerateBezierShapeScript()
    {
        List<Vector3> controlPoints = new List<Vector3>();
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        children.ToList().Sort((x, y) => string.Compare(x.name, y.name));
        foreach (Transform child in children)
        {
            if (transform == child)
                continue;
            controlPoints.Add(child.position);
        }
        BezierShape bz = gameObject.AddComponent<BezierShape>();
        bz.controlPoints = controlPoints.ToArray();
        Destroy(this);
    }


    public void RegenerateSphereFromSavedCtrlPoints()
    {
        BezierShape.DestroyChildren(transform);

        Vector3[] controlPoints = savedControlPoints.ctrlPoints;
        foreach(Vector3 point in controlPoints)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = point;
            sphere.transform.localScale = sphereSize * Vector3.one;
            sphere.transform.parent = transform;
            sphere.name = "s" + transform.childCount;
        }
    }

    public void GeneratePipesFromSavedParams()
    {
        foreach (ScriptableControlPoints.PipeParams param in savedControlPoints.pipesParams)
        {
            GameObject child = new GameObject("Pipe_" + (transform.childCount - 1));
            child.transform.parent = transform;
            CustomPipe cp = child.AddComponent<CustomPipe>();
            cp.shapeIndex = param.shapeIndex;
            cp.minArchNum = param.minArchNum;
            cp.maxArchNum = param.maxArchNum;
            cp.startAngle = param.startAngle;
            cp.endAngle = param.endAngle;
            cp.pipeWidth = param.pipeWidth;
            cp.thickness = param.thickness;
            cp.borderHeight = param.borderHeight;
            cp.borderWidth = param.borderWidth;
            cp.bezierPoints = param.bezierPoints.ToList();
            cp.CreateMesh();
        }
        BezierShape bz = gameObject.AddComponent<BezierShape>();
        bz.controlPoints = savedControlPoints.ctrlPoints;
        Destroy(this);
    }
}

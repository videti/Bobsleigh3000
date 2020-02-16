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
            string num = transform.childCount < 10 ? "0" + transform.childCount : transform.childCount+"";
            sphere.name = "s" + num;
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
        for (int i = 0; i < transform.childCount; i++)
        {
            Debug.Log(transform.GetChild(i).name);
            controlPoints.Add(transform.GetChild(i).position);
        }
        BezierShape bz = gameObject.AddComponent<BezierShape>();
        bz.controlPoints = controlPoints.ToArray();
        Destroy(this);
    }


    public void RegenerateSphereFromSavedCtrlPoints()
    {
        BezierShape.DestroyChildren(transform);

        Vector3[] controlPoints = savedControlPoints.ctrlPoints;
        for (int i = 0; i < controlPoints.Length; i++)
        {
            Vector3 point = controlPoints[i];
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = point;
            sphere.transform.localScale = sphereSize * Vector3.one;
            sphere.transform.parent = transform;
            string num = transform.childCount < 10 ? "0" + transform.childCount : transform.childCount + "";
            sphere.name = "s" + num;
        }
    }

    public void GeneratePipesFromSavedParams()
    {
        for (int i = 0; i < savedControlPoints.pipesParams.Count; i++)
        {
            ScriptableControlPoints.PipeParams param = savedControlPoints.pipesParams[i];
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
            child.AddComponent<MeshCollider>();
        }
        BezierShape bz = gameObject.AddComponent<BezierShape>();
        bz.controlPoints = savedControlPoints.ctrlPoints;
        Destroy(this);
    }
}

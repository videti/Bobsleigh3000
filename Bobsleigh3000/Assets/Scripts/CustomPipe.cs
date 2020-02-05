using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPipe : MonoBehaviour
{
    [HideInInspector]
    public int minArchNum, maxArchNum;
    [HideInInspector]
    public List<Vector3> bezierPoints;
    [Range(0, 360f)]
    public float startAngle, endAngle;
    [Range(0.5f, 5f)]
    public float pipeWidth;
    [Range(0.05f, 1f)]
    public float thickness = 0.2f;
    [Range(0.05f, 0.5f)]
    public float borderWidth = 0.2f, borderHeight = 0.05f;
    public bool noBorderLeft = false, noBorderRight = false;

    private void Awake()
    {
        gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = GetComponentInParent<BezierShape>().shapeMat;
    }

    public void CreateMesh()
    {
        Vector3[] archPoints, archBottomPoints, previousArchPoints = new Vector3[0], previousArchBottomPoints = new Vector3[0];

        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<int> trianglesBorders = new List<int>();
        List<Vector3> verticesBorders = new List<Vector3>();

        for (int archNum = minArchNum; archNum <= maxArchNum; archNum++)
        {
            Vector3 vecDir = (archNum == 0) ? bezierPoints[1] - bezierPoints[0] : bezierPoints[archNum] - bezierPoints[archNum - 1];

            archPoints = BezierShape.GetArchPoints(bezierPoints[archNum], vecDir, pipeWidth, startAngle, endAngle, bezierPoints.Count);
            archBottomPoints = BezierShape.GetArchPoints(bezierPoints[archNum], vecDir, pipeWidth + thickness, startAngle, endAngle, bezierPoints.Count);
            if (archNum != 0)
            {
                BezierShape.JointTwoArches(ref triangles, ref vertices, previousArchPoints, archPoints);
                BezierShape.JointTwoArches(ref triangles, ref vertices, previousArchBottomPoints, archBottomPoints);
                BezierShape.CreateBordersFromTwoArches(ref trianglesBorders, ref verticesBorders, archPoints, archBottomPoints, previousArchPoints, previousArchBottomPoints, borderWidth, borderHeight, noBorderLeft, noBorderRight);
                
            }
            BezierShape.JointFrontAndBottomArches(ref triangles, ref vertices, archPoints, archBottomPoints);
            previousArchPoints = archPoints;
            previousArchBottomPoints = archBottomPoints;
        }

        Mesh _mesh = new Mesh();
        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = triangles.ToArray();
        GetComponent<MeshFilter>().sharedMesh = _mesh;
        Mesh borderMesh = new Mesh();
        borderMesh.vertices = verticesBorders.ToArray();
        borderMesh.triangles = trianglesBorders.ToArray();
        BezierShape.DestroyChildren(transform);
        GameObject go = new GameObject();
        MeshFilter childMF = go.AddComponent<MeshFilter>();
        MeshRenderer childMR = go.AddComponent<MeshRenderer>();
        childMF.mesh = borderMesh;
        go.name = name + "_border";
        go.transform.parent = transform;
        childMR.material = GetComponentInParent<BezierShape>().borderMat;
        Destroy(gameObject.GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();
    }

    void OnValidate()
    {
        if(bezierPoints!=null)
            CreateMesh();
    }
}

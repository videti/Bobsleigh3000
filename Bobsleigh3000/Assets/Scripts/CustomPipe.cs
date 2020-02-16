using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
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

    [HideInInspector]
    public int shapeIndex = 0;

    private void Start()
    {
        if(!GetComponent<MeshFilter>())
            gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = (!GetComponent<MeshRenderer>()) ? gameObject.AddComponent<MeshRenderer>() : GetComponent<MeshRenderer>();
        if(mr != null && GetComponentInParent<BezierShape>() != null && GetComponentInParent<BezierShape>().shapeMat != null)
            mr.material = GetComponentInParent<BezierShape>().shapeMat;
        //set the ground layer = 10
        gameObject.layer = 10;
    }

    public void CreateMesh()
    {
        Vector3[] archPoints, archBottomPoints, previousArchPoints = new Vector3[0], previousArchBottomPoints = new Vector3[0];
        //pipe mesh params
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        //border mesh params
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
        BezierShape.DestroyChildren(transform);
        Mesh _mesh = new Mesh();
        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = triangles.ToArray();
        GetComponent<MeshFilter>().sharedMesh = _mesh;
        Mesh borderMesh = new Mesh();
        borderMesh.vertices = verticesBorders.ToArray();
        borderMesh.triangles = trianglesBorders.ToArray();
        
        GameObject go = new GameObject();
        MeshFilter childMF = go.AddComponent<MeshFilter>();
        MeshRenderer childMR = go.AddComponent<MeshRenderer>();
        childMF.mesh = borderMesh;
        go.name = name + "_border";
        go.transform.parent = transform;

        MeshRenderer mr = (!GetComponent<MeshRenderer>()) ? gameObject.AddComponent<MeshRenderer>() : GetComponent<MeshRenderer>();
        Material[] shapeMaterials, borderShapeMaterials;
        shapeMaterials = Resources.LoadAll<Material>("Mat/ShapeMat");
        borderShapeMaterials = Resources.LoadAll<Material>("Mat/BorderShapeMat");
        mr.material = shapeMaterials[shapeIndex];
        transform.GetChild(0).GetComponent<MeshRenderer>().material = borderShapeMaterials[shapeIndex];
    }

    public void SaveAssets()
    {
        AssetDatabase.CreateAsset(GetComponent<MeshFilter>().sharedMesh, "Assets/Resources/Meshes/" + transform.parent.name + "_" + name + "_mesh.asset");
        AssetDatabase.CreateAsset(transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh, "Assets/Resources/Meshes/" + transform.parent.name + "_" + transform.GetChild(0).name + "_mesh.asset");
        AssetDatabase.SaveAssets();
        foreach (MeshCollider meshCollider in gameObject.GetComponents<MeshCollider>())
            Destroy(meshCollider);
        //gameObject.AddComponent<MeshCollider>();
        foreach (MeshCollider meshCollider in transform.GetChild(0).GetComponents<MeshCollider>())
            Destroy(meshCollider);
        transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
        //layer obstacle = 9
        transform.GetChild(0).gameObject.layer = 9;
    }

    void OnValidate()
    {
        if (EditorApplication.isPlaying && bezierPoints != null)
            CreateMesh();
    }

    //Material[] shapeMaterials, borderShapeMaterials;
    //public void OnInspectorGUI()
    //{
    //    if (GetComponent<MeshRenderer>() == null)
    //        return;

    //    if (shapeMaterials == null)
    //        shapeMaterials = Resources.LoadAll<Material>("Mat/ShapeMat");
    //    if (borderShapeMaterials == null)
    //        borderShapeMaterials = Resources.LoadAll<Material>("Mat/BorderShapeMat");
    //    string[] matNames = shapeMaterials.Select(x => x.name).ToArray();
    //    shapeIndex = EditorGUILayout.Popup(shapeIndex, matNames);
    //    GetComponent<MeshRenderer>().material = shapeMaterials[shapeIndex];
    //    transform.GetChild(0).GetComponent<MeshRenderer>().material = borderShapeMaterials[shapeIndex];
    //}
}



[CustomEditor(typeof(CustomPipe), true)]
public class CustomPipeEditor : Editor
{
    Material[] shapeMaterials, borderShapeMaterials;

    public override void OnInspectorGUI()
    {
        CustomPipe myTarget = (CustomPipe)target;
        if (myTarget.GetComponent<MeshRenderer>() == null)
            return;

        if (shapeMaterials == null)
            shapeMaterials = Resources.LoadAll<Material>("Mat/ShapeMat");
        if (borderShapeMaterials == null)
            borderShapeMaterials = Resources.LoadAll<Material>("Mat/BorderShapeMat");
        string[] matNames = shapeMaterials.Select(x => x.name).ToArray();
        myTarget.shapeIndex = EditorGUILayout.Popup(myTarget.shapeIndex, matNames);
        myTarget.GetComponent<MeshRenderer>().material = shapeMaterials[myTarget.shapeIndex];
        myTarget.transform.GetChild(0).GetComponent<MeshRenderer>().material = borderShapeMaterials[myTarget.shapeIndex];
        DrawDefaultInspector();
    }
}
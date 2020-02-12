using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BezierShape : MonoBehaviour
{
    //public params
    public Vector3[] controlPoints;
    [Range(3,300)]
    public int nbArches = 30;
    [Range(1,20)]
    public int nbArchesBakedTogether = 5;
    public float thickness = 0.05f;
    public String matName = "opaqueWhite";
    [Range(0.1f, 2f)]
    public float pipeWidth = 1f, endWidth = 1f;
    [Range(0, 359f)]
    public float orientation = 0, totalAngle = 180f;
    [Range(0.05f, 0.5f)]
    public float borderWidth = 0.2f, borderHeight = 0.05f;
    public Material shapeMat, borderMat;

    public int startChildIndex, endChildIndex;
    public float shiftingValue;
    ScriptableControlPoints scriptableControlPoints;

    //private params
    static double[] FactorialLookup = new double[0];

    [HideInInspector]
    public int shapeIndex = 0;

    private void Awake()
    {
        //shapeMat = Resources.Load<Material>("Mat/" + matName);
        scriptableControlPoints = GameObject.FindObjectOfType<FollowingBezierCurve>().scriptableControl;
    }

    /**
     * Create a LineRenderer with the bezier points
     * */
    public void DrawBezierCurve()
    {
        Destroy(GetComponent<LineRenderer>());
        StartCoroutine(CreateLineRenderer());
    }

    IEnumerator CreateLineRenderer()
    {
        yield return null;
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = pipeWidth;
        lineRenderer.material = shapeMat;
        lineRenderer.positionCount = controlPoints.Count();
        lineRenderer.SetPositions(GetBezierPoints(controlPoints.ToList()).ToArray());
    }

    /**
     * Get all bezier points coordonates
     * */
    List<Vector3> GetBezierPoints(List<Vector3> controlPoints)
    {
        List<Vector3> bezierPoints = new List<Vector3>();
        for (int i = 0; i < nbArches; i++)
        {
            bezierPoints.Add(GetBezierCurvePointAtT(controlPoints.ToArray(), (double)i / (double)nbArches));
        }
        return bezierPoints;
    }

    /**
     * Compute bernstein polynom result for a point
     * i = index point
     * n = nb total points
     * t = float between 0 and 1 (x axis)
     * */
    static double BernsteinPolynom(int i, int n, double t)
    {
        double result = factorial(n) / (factorial(i) * factorial(n - i));
        result *= Math.Pow(t, i) * Math.Pow(1f - t, n - i);
        return result;
    }

    /**
     * Get bezier point coordonates from control points list
     * */
    public static Vector3 GetBezierCurvePointAtT(Vector3[] ctrlPoints, double t)
    {
        int n = ctrlPoints.Length;
        Vector3 bPoint = Vector3.zero;
        for (int i = 0; i < ctrlPoints.Length; i++)
        {
            double b_i = BernsteinPolynom(i, n - 1, t);
            bPoint += new Vector3((float)(ctrlPoints[i].x * b_i), (float)(ctrlPoints[i].y * b_i), (float)(ctrlPoints[i].z * b_i));
        }
        return bPoint;
    }

    /**
     * Get Arch Point from an Origin, oriented on X axis
     * */
    public static Vector3[] GetArchPoints(Vector3 origin, Vector3 dir, float radius = 1f, float orientation = 0, float totalAngle = 100f, int nbPoints = 16)
    {
        dir = Vector3.Normalize(dir);
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i <= nbPoints; i++)
        {
            float teta = orientation - Mathf.Deg2Rad * totalAngle * i / (1f * nbPoints);
            Vector3 newPoint = new Vector3(radius * Mathf.Cos(teta), radius * Mathf.Sin(teta), 0);
            newPoint = Quaternion.LookRotation(dir) * newPoint;
            newPoint += origin;
            points.Add(newPoint);
        }
        return points.ToArray();
    }

    public void CreatePipeMeshesFromBezier()
    {
        List<Vector3> bezierPoints = GetBezierPoints(controlPoints.ToList());

        int minArchNum = 0;
        for (int archNum = 0; archNum < bezierPoints.Count(); archNum++)
        {
            if (archNum != 0 && archNum % nbArchesBakedTogether == 0 || archNum == bezierPoints.Count() - 1)
            {
                CreateChildPipe(minArchNum, archNum, bezierPoints, orientation, totalAngle);
                minArchNum = archNum;
            }
        }
    }

    public void CreatePipes(bool destroyChildren = true)
    {
        if (destroyChildren)
            DestroyChildren(transform);

        CreatePipeMeshesFromBezier();
    }

    public void SaveModel()
    {
        GameObject.FindObjectOfType<FollowingBezierCurve>().scriptableControl.ctrlPoints = controlPoints;
    }

    public void RemoveScripts()
    {
        scriptableControlPoints.pipesParams = new List<ScriptableControlPoints.PipeParams>();
        List<Vector3> bezierPoints = GetBezierPoints(controlPoints.ToList());
        foreach (CustomPipe child in GetComponentsInChildren<CustomPipe>())
        {
            child.SaveAssets();
            ScriptableControlPoints.PipeParams pipeParams = new ScriptableControlPoints.PipeParams();
            pipeParams.bezierPoints = bezierPoints.ToArray();
            pipeParams.minArchNum = child.minArchNum;
            pipeParams.maxArchNum = child.maxArchNum;
            pipeParams.startAngle = child.startAngle;
            pipeParams.endAngle = child.endAngle;
            pipeParams.pipeWidth = child.pipeWidth;
            pipeParams.thickness = child.thickness;
            pipeParams.borderWidth = child.borderWidth;
            pipeParams.noBorderLeft = child.noBorderLeft;
            pipeParams.noBorderRight = child.noBorderRight;
            pipeParams.shapeIndex = child.shapeIndex;
            scriptableControlPoints.pipesParams.Add(pipeParams);
            Destroy(child);
        }
        Destroy(GetComponent<ControlPoints>());
        Destroy(this);
    }

    public static void DestroyChildren(Transform _transform)
    {
        foreach (Transform t in _transform.GetComponentsInChildren<Transform>())
        {
            if (t == _transform)
                continue;
            Destroy(t.gameObject);
        }
    }

    void CreateChildPipe(int minArchNum, int maxArchNum, List<Vector3> bezierPoints, float startAngle, float endAngle)
    {
        if (bezierPoints.Count == 0)
            return;
        GameObject child = new GameObject("Pipe_" + (transform.childCount - 1));
        child.transform.parent = transform;
        CustomPipe cp = child.AddComponent<CustomPipe>();
        cp.shapeIndex = shapeIndex;
        cp.minArchNum = minArchNum;
        cp.maxArchNum = maxArchNum;
        cp.startAngle = startAngle;
        cp.endAngle = endAngle;
        cp.pipeWidth = pipeWidth;
        cp.thickness = thickness;
        cp.borderHeight = borderHeight;
        cp.borderWidth = borderWidth;
        cp.bezierPoints = bezierPoints;
        cp.CreateMesh();
    }

    public void ShiftingChildrenDepth(int startChildIndex, int endChildIndex, float shiftingValue = 2f)
    {
        if (transform.childCount < endChildIndex + 1)
            return;

        if (endChildIndex <= startChildIndex)
            return;

        float firstChildPosZ = transform.GetChild(startChildIndex).transform.position.z;

        for (int i = 1; i <= endChildIndex - startChildIndex; i++)
        {
            Transform childTransform = transform.GetChild(i + startChildIndex).transform;
            childTransform.position = new Vector3(childTransform.position.x, childTransform.position.y, firstChildPosZ + i * shiftingValue);
            controlPoints[i] = childTransform.position;
        }
    }

    public static void JointTwoArches(ref List<int> triangles, ref List<Vector3> vertices, Vector3[] arch1, Vector3[] arch2)
    {
        for (int i = 0; i < arch1.Length - 1; i++)
        {
            AddRecToMesh(
                ref triangles, ref vertices,
                arch1[i], arch1[i+1],
                arch2[i], arch2[i+1]
            );
        }
    }

    public static void JointFrontAndBottomArches(ref List<int> triangles, ref List<Vector3> vertices, Vector3[] archPoints, Vector3[] archBottomPoints)
    {
        for (int archPointNum = 0; archPointNum < archPoints.Count() - 1; archPointNum++)
        {
            if (archPointNum != archPoints.Count())
            {
                AddRecToMesh(
                    ref triangles, ref vertices, archPoints[archPointNum], 
                    archPoints[archPointNum + 1], archBottomPoints[archPointNum], 
                    archBottomPoints[archPointNum + 1]
                );
            }
        }
    }

    public static void CreateBordersFromTwoArches
        (ref List<int> triangles, ref List<Vector3> vertices, Vector3[] archPoints, Vector3[] archBottomPoints, Vector3[] previousArchPoints, Vector3[] previousArchBottomPoints, float borderWidth = 0.1f, float borderHeight = 0.1f, bool noBorderLeft = false, bool noBorderRight = false)
    {
        if(previousArchPoints.Length == 0)
            return;
        if(!noBorderRight)
            CreateShapeFromRect(ref triangles, ref vertices, archPoints[0], archBottomPoints[0], previousArchPoints[0], previousArchBottomPoints[0], borderWidth, borderHeight);
        int lastElem = archPoints.Length - 1;
        if(!noBorderLeft)
            CreateShapeFromRect(ref triangles, ref vertices, previousArchPoints[lastElem], previousArchBottomPoints[lastElem], archPoints[lastElem], archBottomPoints[lastElem], borderWidth, borderHeight);

    }

    public static void CreateShapeFromRect (ref List<int> triangles, ref List<Vector3> vertices, Vector3 a, Vector3 b, Vector3 c, Vector3 d, float borderWidth = 0.1f, float borderHeight = 0.1f)
    {
        Vector3 ab = b - a;
        Vector3 ad = d - a;
        Vector3 a_prim = a + ab * 0.5f - borderWidth * Vector3.Normalize(ab);
        Vector3 b_prim = a + ab * 0.5f + borderWidth * Vector3.Normalize(ab);
        Vector3 c_prim = c + ab * 0.5f - borderWidth * Vector3.Normalize(ab);
        Vector3 d_prim = c + ab * 0.5f + borderWidth * Vector3.Normalize(ab);

        AddRecToMesh(
            ref triangles, ref vertices,
            a_prim, b_prim, c_prim, d_prim
        );

        Vector3 normale = Vector3.Normalize(Vector3.Cross(ab, ad));
        Vector3 e = a_prim + borderHeight * normale;
        Vector3 f = b_prim + borderHeight * normale;
        Vector3 g = c_prim + borderHeight * normale;
        Vector3 h = d_prim + borderHeight * normale;
        AddRecToMesh(
            ref triangles, ref vertices,
            e, f, g, h
        );
        AddRecToMesh(
            ref triangles, ref vertices,
            a_prim, b_prim, e, f
        );
        AddRecToMesh(
            ref triangles, ref vertices,
            b_prim, d_prim, f, h
        );
        AddRecToMesh(
            ref triangles, ref vertices,
            d_prim, c_prim, h, g
        );
        AddRecToMesh(
            ref triangles, ref vertices,
            c_prim, a_prim, g, e
        );
    }


    public static void AddRecToMesh(ref List<int> triangles, ref List<Vector3> vertices, Vector3 v0_0, Vector3 v0_1, Vector3 v1_0, Vector3 v1_1)
    {
        int s0_0, s0_1, s1_0, s1_1;
        vertices.Add(v0_0);
        vertices.Add(v1_0);
        vertices.Add(v0_1);
        vertices.Add(v1_1);
        s0_0 = vertices.Count() - 4; //arch0_0
        s0_1 = vertices.Count() - 3; //arch0_1
        s1_0 = vertices.Count() - 2; //arch1_0
        s1_1 = vertices.Count() - 1; //arch1_1

        //two faced
        triangles.Add(s0_0);
        triangles.Add(s0_1);
        triangles.Add(s1_0);

        triangles.Add(s0_0);
        triangles.Add(s1_0);
        triangles.Add(s0_1);

        triangles.Add(s1_1);
        triangles.Add(s1_0);
        triangles.Add(s0_1);

        triangles.Add(s1_1);
        triangles.Add(s0_1);
        triangles.Add(s1_0);
    }


    Vector3[] GetBottomArchPoints(Vector3[] arch, float thickness = 0.5f)
    {
        Vector3[] archBis = new Vector3[arch.Length];
        for (int i = 0; i < arch.Length; i++)
        {
            archBis[i] = arch[i] + new Vector3(0, thickness, 0);
        }
        return archBis;
    }


    static float NormalizeDegAngle(float angle)
    {
        while (angle < 0)
        {
            angle += 360f;
        }
        while (angle >= 360)
        {
            angle -= 360f;
        }
        return angle;
    }


    /**
     * Compute factorial operation
     * */

    // just check if n is appropriate, then return the result
    private static double factorial(int n)
    {
        if (FactorialLookup.Length == 0)
            CreateFactorialTable();
        if (n < 0) { throw new Exception("n is less than 0"); }
        if (n > 32) { throw new Exception("n is greater than 32"); }

        return FactorialLookup[n]; /* returns the value n! as a SUMORealing point number */
    }

    // create lookup table for fast factorial calculation
    private static void CreateFactorialTable()
    {
        // fill untill n=32. The rest is too high to represent
        double[] a = new double[33];
        a[0] = 1.0;
        a[1] = 1.0;
        a[2] = 2.0;
        a[3] = 6.0;
        a[4] = 24.0;
        a[5] = 120.0;
        a[6] = 720.0;
        a[7] = 5040.0;
        a[8] = 40320.0;
        a[9] = 362880.0;
        a[10] = 3628800.0;
        a[11] = 39916800.0;
        a[12] = 479001600.0;
        a[13] = 6227020800.0;
        a[14] = 87178291200.0;
        a[15] = 1307674368000.0;
        a[16] = 20922789888000.0;
        a[17] = 355687428096000.0;
        a[18] = 6402373705728000.0;
        a[19] = 121645100408832000.0;
        a[20] = 2432902008176640000.0;
        a[21] = 51090942171709440000.0;
        a[22] = 1124000727777607680000.0;
        a[23] = 25852016738884976640000.0;
        a[24] = 620448401733239439360000.0;
        a[25] = 15511210043330985984000000.0;
        a[26] = 403291461126605635584000000.0;
        a[27] = 10888869450418352160768000000.0;
        a[28] = 304888344611713860501504000000.0;
        a[29] = 8841761993739701954543616000000.0;
        a[30] = 265252859812191058636308480000000.0;
        a[31] = 8222838654177922817725562880000000.0;
        a[32] = 263130836933693530167218012160000000.0;
        FactorialLookup = a;
    }
}

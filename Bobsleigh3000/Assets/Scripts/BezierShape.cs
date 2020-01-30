using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BezierShape : MonoBehaviour
{
    //public params
    public Vector3[] controlPoints;
    [Range(3,20)]
    public int nbPoints = 10;
    public String matName = "opaqueWhite";
    [Range(0.1f, 2f)]
    public float startWidth = 1f, endWidth = 1f;
    [Range(0, 359f)]
    public float startAngle = 0, endAngle = 359f;

    Material _shapeMat;

    //private params
    double[] FactorialLookup;

    //const
    public BezierShape()
    {
        CreateFactorialTable();
    }

    private void Awake()
    {
        _shapeMat = Resources.Load<Material>("Mat/" + matName);
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
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.material = _shapeMat;
        lineRenderer.positionCount = controlPoints.Count();
        lineRenderer.SetPositions(GetBezierPoints(controlPoints.ToList()).ToArray());
    }

    /**
     * Get all bezier points coordonates
     * */
    List<Vector3> GetBezierPoints(List<Vector3> controlPoints)
    {
        List<Vector3> bezierPoints = new List<Vector3>();
        for (int i = 0; i < nbPoints; i++)
        {
            bezierPoints.Add(GetPointAtT(controlPoints.ToArray(), (double)i / (double)nbPoints));
        }
        return bezierPoints;
    }

    /**
     * Compute bernstein polynom result for a point
     * i = index point
     * n = nb total points
     * t = float between 0 and 1 (x axis)
     * */
    double BernsteinPolynom(int i, int n, double t)
    {
        double result = factorial(n) / (factorial(i) * factorial(n - i));
        result *= Math.Pow(t, i) * Math.Pow(1f - t, n - i);
        return result;
    }

    /**
     * Get bezier point coordonates from control points list
     * */
    Vector3 GetPointAtT(Vector3[] points, double t)
    {
        int n = points.Length;
        Vector3 bPoint = Vector3.zero;
        for (int i = 0; i < points.Length; i++)
        {
            double b_i = BernsteinPolynom(i, n - 1, t);
            bPoint += new Vector3((float)(points[i].x * b_i), (float)(points[i].y * b_i), (float)(points[i].z * b_i));
        }
        return bPoint;
    }

    /**
     * Get Arch Point from an Origin, oriented on X axis
     * */
    public Vector3[] GetArchPoints(Vector3 origin, float radius = 1f, float startAngle = 360f, float endAngle = 0, int nbPoints = 16)
    {
        startAngle = NormalizeDegAngle(startAngle);
        endAngle = NormalizeDegAngle(endAngle);

        if (endAngle < startAngle)
        {
            float tmp = startAngle;
            startAngle = endAngle;
            endAngle = tmp;
        } else if (endAngle == startAngle)
        {
            endAngle += 360f;
        }
        
        List<Vector3> points = new List<Vector3>();
        float totalAngle = (endAngle - startAngle);
        for (int i = 0; i <= nbPoints; i++)
        {
            float teta = Mathf.Deg2Rad * totalAngle * i / (1f * nbPoints);
            points.Add(new Vector3(origin.x, origin.y + radius * Mathf.Sin(teta), origin.z + radius * Mathf.Cos(teta)));
        }
        return points.ToArray();
    }

    public Mesh GetPipeMeshFromBezier()
    {
        Vector3[] previousArchPoints = new Vector3[0];
        Vector3[] archPoints;
        List<Vector3> bezierPoints = GetBezierPoints(controlPoints.ToList());

        Mesh _mesh = new Mesh();
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();

        for (int archNum = 0; archNum < bezierPoints.Count(); archNum++)
        {
            archPoints = GetArchPoints(bezierPoints[archNum]); //todo : change with correct params
            if (previousArchPoints.Length != 0)
            {
                JointTwoArches(ref triangles, ref vertices, previousArchPoints, archPoints);
            }
            previousArchPoints = archPoints;
        }
        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = triangles.ToArray();
        
        return _mesh;
    }

    public void CreateMeshPipe()
    {
        if (GetComponent<MeshFilter>())
        {
            Destroy(GetComponent<MeshFilter>());
        }
        if (GetComponent<MeshRenderer>())
        {
            Destroy(GetComponent<MeshRenderer>());
        }

        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mf.mesh = GetPipeMeshFromBezier();
        mr.material = _shapeMat;
    }

    void JointTwoArches(ref List<int> triangles, ref List<Vector3> vertices, Vector3[] arch1, Vector3[] arch2)
    {
        for (int i = 0; i < arch1.Length - 1; i++)
        {
            vertices.Add(arch1[i]);
            vertices.Add(arch1[i + 1]);
            vertices.Add(arch2[i]);
            vertices.Add(arch2[i + 1]);
            int s0_0 = vertices.Count() - 4; //arch0_0
            int s0_1 = vertices.Count() - 3; //arch0_1
            int s1_0 = vertices.Count() - 2; //arch1_0
            int s1_1 = vertices.Count() - 1; //arch1_1


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
    }

    float NormalizeDegAngle(float angle)
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
    private double factorial(int n)
    {
        if (n < 0) { throw new Exception("n is less than 0"); }
        if (n > 32) { throw new Exception("n is greater than 32"); }

        return FactorialLookup[n]; /* returns the value n! as a SUMORealing point number */
    }

    // create lookup table for fast factorial calculation
    private void CreateFactorialTable()
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

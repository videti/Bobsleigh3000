using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TobogganGenerator : MonoBehaviour
{
    public ScriptableControlPoints[] tobogganParts;
    public Vector3[] translations;
    public int nbArches = 100;

    private void Awake()
    {
        translations = new Vector3[tobogganParts.Length];
    }

    // joint different bezier curve together
    public void GeneratePipes()
    {
        for (int i = 0; i < tobogganParts.Length; i++)
        {
            if (i == 0)
            {
                translations[i] = -tobogganParts[i].pipesParams[0].bezierPoints[0];
            }
            else
            {
                int previousIndex = i - 1;
                int nbPipesParamsPreviousPart = tobogganParts[previousIndex].pipesParams.Count;
                int nbBezierPointsPreviousPart = tobogganParts[previousIndex].pipesParams[nbPipesParamsPreviousPart - 1].bezierPoints.Length;
                Vector3 lastBezierPointPreviousPart = tobogganParts[previousIndex].pipesParams[nbPipesParamsPreviousPart - 1].bezierPoints[nbBezierPointsPreviousPart - 1];
                translations[i] = lastBezierPointPreviousPart - tobogganParts[i].pipesParams[0].bezierPoints[0] + translations[i - 1];
            }

            List<Vector3> newBezierPoints = new List<Vector3>();
            for (int k = 0; k < tobogganParts[i].pipesParams[0].bezierPoints.Length; k++)
            {
                newBezierPoints.Add(tobogganParts[i].pipesParams[0].bezierPoints[k] + translations[i]);
            }

            foreach (ScriptableControlPoints.PipeParams param in tobogganParts[i].pipesParams)
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
                cp.bezierPoints = newBezierPoints;
                cp.CreateMesh();
                //child.AddComponent<MeshCollider>();
            }
            //BezierShape bz = gameObject.AddComponent<BezierShape>();
            //bz.controlPoints = tobogganParts[i].ctrlPoints;
        }
        //Destroy(this);
    }
}

[CustomEditor(typeof(TobogganGenerator))]
public class TobogganGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate Pipes"))
        {
            ((TobogganGenerator)target).GeneratePipes();
        }
    }
}
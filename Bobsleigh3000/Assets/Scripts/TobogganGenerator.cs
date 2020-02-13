using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TobogganGenerator : MonoBehaviour
{
    public ScriptableControlPoints[] tobogganParts;
    public int nbArches = 100;
    
    // joint different bezier curve together
    public void GeneratePipes()
    {
        for (int i = 0; i < tobogganParts.Length; i++)
        {
            int previousIndex = Mathf.Max(0, i - 1);
            int nbPipesParamsPreviousPart = tobogganParts[previousIndex].pipesParams.Count;
            int nbBezierPointsPreviousPart = tobogganParts[previousIndex].pipesParams[nbPipesParamsPreviousPart - 1].bezierPoints.Length;
            Vector3 lastBezierPointPreviousPart = tobogganParts[previousIndex].pipesParams[nbPipesParamsPreviousPart - 1].bezierPoints[nbBezierPointsPreviousPart - 1];
            Vector3 translation = lastBezierPointPreviousPart - tobogganParts[i].pipesParams[0].bezierPoints[0];

            for (int j = 0; j < tobogganParts[i].pipesParams.Count; j++)
            {
                for (int k = 0; k < tobogganParts[i].pipesParams[j].bezierPoints.Length; k++)
                {
                    tobogganParts[i].pipesParams[j].bezierPoints[k] += translation;
                }
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
                cp.bezierPoints = tobogganParts[i].pipesParams[0].bezierPoints.ToList();
                cp.CreateMesh();
                child.AddComponent<MeshCollider>();
            }
            BezierShape bz = gameObject.AddComponent<BezierShape>();
            bz.controlPoints = tobogganParts[i].ctrlPoints;
        }
        Destroy(this);
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
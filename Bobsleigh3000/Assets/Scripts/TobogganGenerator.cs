using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TobogganGenerator : MonoBehaviour
{
    public ScriptableControlPoints[] tobogganParts;
    public int nbArches = 100;
    
    public void GeneratePipes()
    {
        for (int i = 0; i < tobogganParts.Length; i++)
        {
            int previousIndex = Mathf.Max(0, i - 1);
            Vector3 lastPointPreviousPart = tobogganParts[previousIndex].ctrlPoints[tobogganParts[previousIndex].ctrlPoints.Length - 1];
            Vector3 translation = lastPointPreviousPart - tobogganParts[i].ctrlPoints[0];
            for (int j = 0; j < tobogganParts[i].ctrlPoints.Length; j++)
            {
                tobogganParts[i].ctrlPoints[j] += translation;
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = tobogganParts[i].ctrlPoints[j];
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
                cp.CreateMesh(tobogganParts[i].ctrlPoints, nbArches);
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TobogganGenerator : MonoBehaviour
{
    public ScriptableControlPoints[] tobogganParts;
    public Vector3[] translations;
    public Quaternion[] rotations;
    public int nbArches = 100;

    private void Awake()
    {
        translations = new Vector3[tobogganParts.Length];
        rotations = new Quaternion[tobogganParts.Length];
    }

    // joint different bezier curve together
    public void GeneratePipes()
    {
        translations[0] = -tobogganParts[0].pipesParams[0].bezierPoints[0];
        rotations[0] = Quaternion.Euler(0, 0, 0);
        
        for (int i = 0; i < tobogganParts.Length; i++)
        {
            if (i != 0)
            {
                int previousIndex = i - 1;
                int nbPipesParamsPreviousPart = tobogganParts[previousIndex].pipesParams.Count;
                int nbBezierPointsPreviousPart = tobogganParts[previousIndex].pipesParams[nbPipesParamsPreviousPart - 1].bezierPoints.Length;
                Vector3 lastBezierPointPreviousPart = tobogganParts[previousIndex].pipesParams[nbPipesParamsPreviousPart - 1].bezierPoints[nbBezierPointsPreviousPart - 1];
                Vector3 previousPartLastDirection = tobogganParts[i - 1].pipesParams[0].bezierPoints[nbBezierPointsPreviousPart - 1] - tobogganParts[i - 1].pipesParams[0].bezierPoints[nbBezierPointsPreviousPart - 2];
                Vector3 firstDirection = tobogganParts[i].pipesParams[0].bezierPoints[0] - tobogganParts[i].pipesParams[0].bezierPoints[1];

                //rotation
                rotations[i] = Quaternion.FromToRotation(Vector3.Normalize(firstDirection), Vector3.Normalize(-previousPartLastDirection));
                rotations[i] *= rotations[i - 1];

                //translation
                Vector3 rotatedFirstPoint = rotations[i] * tobogganParts[i].pipesParams[0].bezierPoints[0];
                Vector3 rotatedLastPointPreviousPart = rotations[i - 1] * lastBezierPointPreviousPart + translations[i - 1];
                translations[i] = rotatedLastPointPreviousPart - rotatedFirstPoint;
            }

            List<Vector3> newBezierPoints = new List<Vector3>();
            for (int k = 0; k < tobogganParts[i].pipesParams[0].bezierPoints.Length; k++)
            {
                newBezierPoints.Add(rotations[i] * tobogganParts[i].pipesParams[0].bezierPoints[k] + translations[i]);
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
                child.AddComponent<MeshCollider>();
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
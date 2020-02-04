using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierShape))]
public class BezierShapeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BezierShape myTarget = (BezierShape)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Draw Bezier curve"))
        {
            myTarget.DrawBezierCurve();
        } else if (GUILayout.Button("Create Mesh Pipe"))
        {
            myTarget.CreatePipes();
        }
    }
}
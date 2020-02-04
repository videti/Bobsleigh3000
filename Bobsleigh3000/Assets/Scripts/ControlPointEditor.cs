using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ControlPoints))]
public class ControlPointsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ControlPoints myTarget = (ControlPoints)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Bezier Shape Script"))
        {
            myTarget.GenerateBezierShapeScript();
        }
    }
}
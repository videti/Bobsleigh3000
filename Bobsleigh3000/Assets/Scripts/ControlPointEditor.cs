using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ControlPoints))]
public class ControlPointsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ControlPoints myTarget = (ControlPoints)target;
        if (GUILayout.Button("Regenerate Sphere From Saved CtrlPoints"))
        {
            myTarget.RegenerateSphereFromSavedCtrlPoints();
        }
        DrawDefaultInspector();
        if (GUILayout.Button("Generate Bezier Shape Script"))
        {
            myTarget.GenerateBezierShapeScript();
        }
    }
}
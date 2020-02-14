using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(BezierShape))]
public class BezierShapeEditor : Editor
{
    Material[] shapeMaterials, borderShapeMaterials;
    SerializedProperty startChildIndex, endChildIndex, shiftingValue;

    void OnEnable()
    {
        startChildIndex = serializedObject.FindProperty("startChildIndex");
        endChildIndex = serializedObject.FindProperty("endChildIndex");
        shiftingValue = serializedObject.FindProperty("shiftingValue");
    }

    public override void OnInspectorGUI()
    {
        BezierShape myTarget = (BezierShape)target;

        //phase pre-creation
        if (myTarget.GetComponentInChildren<CustomPipe>() == null)
        {
            //create material drop down
            if (shapeMaterials == null)
                shapeMaterials = Resources.LoadAll<Material>("Mat/ShapeMat");
            if (borderShapeMaterials == null)
                borderShapeMaterials = Resources.LoadAll<Material>("Mat/BorderShapeMat");
            string[] matNames = shapeMaterials.Select(x => x.name).ToArray();
            myTarget.shapeIndex = EditorGUILayout.Popup(myTarget.shapeIndex, matNames);
            myTarget.shapeMat = shapeMaterials[myTarget.shapeIndex];
            myTarget.borderMat = borderShapeMaterials[myTarget.shapeIndex];

            //create button to create meshes
            if (GUILayout.Button("Create Mesh Pipe"))
            {
                myTarget.CreatePipes();
            }
        }
        //phase post-creation
        else
        {
            //save control point scriptable model
            if (GUILayout.Button("Save scriptable points as model"))
            {
                myTarget.SaveModel();
            }
            //save mesh and remove scripts
            if (GUILayout.Button("Remove All Scripts and Save Meshes"))
            {
                myTarget.RemoveScripts();
            }
        }

        DrawDefaultInspector();

        if (myTarget.GetComponentInChildren<CustomPipe>() == null)
        {
            EditorGUILayout.Space(10f, true);
            if (GUILayout.Button("Shifting Pipe"))
            {
                myTarget.ShiftingChildrenDepth(startChildIndex.intValue, endChildIndex.intValue, shiftingValue.floatValue);
            }
        }
    }
}
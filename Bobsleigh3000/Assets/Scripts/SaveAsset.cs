using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SaveAsset : MonoBehaviour
{
    public void Save()
    {
        AssetDatabase.CreateAsset(GetComponent<MeshFilter>().sharedMesh, "Assets/Meshes/"+name+ "_mesh.asset");
    }
}

//cutom editor
[CustomEditor(typeof(SaveAsset))]
class SaveAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SaveAsset myTarget = (SaveAsset)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Save Asset"))
        {
            myTarget.Save();
        }
    }
}
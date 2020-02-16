using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AutoSaveTransform : MonoBehaviour
{
    public ScriptableTransform scriptableTransform;

    private void Start()
    {
        if (scriptableTransform != null)
        {
            transform.position = scriptableTransform.position;
            transform.rotation = scriptableTransform.rotation;
            transform.localScale = scriptableTransform.localScale;
        }
    }

    private void Update()
    {
        if(scriptableTransform == null)
        {
            scriptableTransform = new ScriptableTransform();
        }
    }

    public void SaveTransform()
    {
        scriptableTransform.position = transform.position;
        scriptableTransform.rotation = transform.rotation;
        scriptableTransform.localScale = transform.localScale;
        if (AssetDatabase.FindAssets(name + "_transform").Length == 0)
        {
            AssetDatabase.CreateAsset(scriptableTransform, "Assets/Scripts/ScriptableObject/" + name + "_transform.asset");
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(scriptableTransform);
    }
}

[CustomEditor(typeof(AutoSaveTransform), true)]
public class AutoSaveTransformEditor : Editor { 

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AutoSaveTransform myTarget = (AutoSaveTransform)target;
        if (GUILayout.Button("Save SCriptable Transform"))
        {
            myTarget.SaveTransform();
        }
    }
}
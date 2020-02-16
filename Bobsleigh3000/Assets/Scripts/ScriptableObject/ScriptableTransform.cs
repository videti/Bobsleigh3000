using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableTransform", menuName = "Scriptable Transform")]
public class ScriptableTransform : ScriptableObject
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;
}

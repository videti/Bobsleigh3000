using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CtrlPoints", menuName = "Control Points")]
public class ScriptableControlPoints : ScriptableObject
{
    public Vector3[] ctrlPoints;
}

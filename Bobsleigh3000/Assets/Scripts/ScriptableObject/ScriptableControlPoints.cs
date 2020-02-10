using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CtrlPoints", menuName = "Control Points")]
public class ScriptableControlPoints : ScriptableObject
{
    public Vector3[] ctrlPoints = {
        new Vector3(-27.86408f, 11.84466f),
        new Vector3(-16.08414f, -14.69256f),
        new Vector3(13.81877f, -13.20388f),
        new Vector3(14.33657f, 13.33333f)
    };
}

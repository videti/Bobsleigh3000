using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CtrlPoints", menuName = "Control Points")]
public class ScriptableControlPoints : ScriptableObject
{
    public Vector3[] ctrlPoints;

    [System.Serializable]
    public struct PipeParams
    {
        public Vector3[] bezierPoints;
        public int minArchNum, maxArchNum;
        public float startAngle, endAngle;
        public float pipeWidth;
        public float thickness;
        public float borderWidth, borderHeight;
        public bool noBorderLeft, noBorderRight;
        public int shapeIndex;
    }

    public List<PipeParams> pipesParams;
}

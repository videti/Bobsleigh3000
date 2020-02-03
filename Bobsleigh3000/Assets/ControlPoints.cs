using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControlPoints : MonoBehaviour
{
    public float sphereSize = 1f;
    [Range(-9, 9)]
    public float sphereZPos = 1f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseP = Input.mousePosition;
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mouseP.x, mouseP.y, 10f));
            sphere.transform.position = new Vector3(sphere.transform.position.x, sphere.transform.position.y, sphereZPos);
            sphere.transform.localScale = sphereSize * Vector3.one;
            sphere.transform.parent = transform;
            sphere.name = "s" + transform.childCount;
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Pressed right click.");

        }
        if (Input.GetMouseButtonDown(2)) Debug.Log("Pressed middle click.");
    }

    public void GenerateBezierShapeScript()
    {
        List<Vector3> controlPoints = new List<Vector3>();
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        children.ToList().Sort((x, y) => string.Compare(x.name, y.name));
        foreach (Transform child in children)
        {
            if (transform == child)
                continue;
            controlPoints.Add(child.position);
        }
        BezierShape bz = gameObject.AddComponent<BezierShape>();
        bz.controlPoints = controlPoints.ToArray();
    }
}

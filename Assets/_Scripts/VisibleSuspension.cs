using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VisibleSuspension : MonoBehaviour
{
    public LayerMask layerColl;
    public Vector3 startPoint = new Vector3(0, -0.05f, 0);
    public Vector3 offset = new Vector3(0, 0.058f, 0);
    public float distanceMax = 0.025f;
    public Transform root;
    [Space]
    public bool debug = false;
    //public float val;

    private int hit;
    public RaycastHit[] raycastHits = new RaycastHit[1];

    void Update()
    {
        hit = Physics.RaycastNonAlloc(transform.position + startPoint, -transform.up, raycastHits, distanceMax, layerColl);
        //val = Vector3.Distance(transform.position + startPoint, raycastHits[0].point);
        if (hit == 1)
        {
            root.position = raycastHits[0].point + offset;
        }
        //else if (val >)
        //{
        //    //root.localPosition = new Vector3(0, -((offset.y - (-startPoint.y)) / 4) * 10, 0);
        //}

        if (debug)
        {
            Debug.DrawLine(transform.position + startPoint, raycastHits[0].point, Color.red);
        }
    }
}
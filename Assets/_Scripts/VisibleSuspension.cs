using UnityEngine;

[ExecuteInEditMode]
public class VisibleSuspension : MonoBehaviour
{
    public LayerMask layerColl;
    public Vector3 startPoint = new(0, -0.05f, 0);
    public Vector3 offset = new(0, 0.058f, 0);
    public float distanceMax = 0.025f;
    public Transform root;
    [Space]
    public bool debug = false;

    private int hit;
    public RaycastHit[] raycastHits = new RaycastHit[1];

    void Update()
    {
        hit = Physics.RaycastNonAlloc(transform.position + startPoint, -transform.up, raycastHits, distanceMax, layerColl);

        if (hit == 1)
        {
            root.position = raycastHits[0].point + offset;
        }

        if (debug)
        {
            Debug.DrawLine(transform.position + startPoint, raycastHits[0].point, Color.red);
        }
    }
}
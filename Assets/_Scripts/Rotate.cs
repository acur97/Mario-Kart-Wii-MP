using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 axis = new(0, 1, 0);
    public float velocidad;

    private float val = 0;

    private void LateUpdate()
    {
        val = velocidad * Time.deltaTime;
        transform.Rotate(axis.x * val, axis.y * val, axis.z * val);
    }
}
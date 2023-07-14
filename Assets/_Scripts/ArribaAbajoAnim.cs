using UnityEngine;

public class ArribaAbajoAnim : MonoBehaviour
{
    public float max;
    public float velocidad = 1;

    private float speed;
    private float min;

    private void Awake()
    {
        min = transform.localPosition.y;
    }

    private void Update()
    {
        speed = Mathf.SmoothStep(min, max, Mathf.PingPong(Time.time * velocidad, 1));

        transform.localPosition = new Vector3(transform.localPosition.x, speed, transform.localPosition.z);
    }
}
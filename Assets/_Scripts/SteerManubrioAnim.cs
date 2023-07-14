using UnityEngine;

[ExecuteInEditMode]
public class SteerManubrioAnim : MonoBehaviour
{
    public Transform steer;
    public Animator anim;

    private Vector3 rotation = Vector3.zero;
    public float animManejo = 0.5f;
    private const string _manejo = "manejo";

    private void Update()
    {
        rotation = new Vector3(15.856f, 0, -(steer.localEulerAngles.y * 2));
        transform.localEulerAngles = rotation;

        animManejo = (steer.localRotation.y * 2.381f) + 0.5f;
        animManejo = Mathf.Clamp(animManejo, 0, 0.999f);
        if (Application.isPlaying && anim.gameObject.activeSelf)
        {
            anim.Play(_manejo, 0, animManejo);
        }
    }
}
using UnityEngine;

public class TurnAnimator : MonoBehaviour
{
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _anim.enabled = true;
    }

    private void OnDisable()
    {
        _anim.enabled = false;
    }
}
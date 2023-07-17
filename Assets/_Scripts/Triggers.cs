using UnityEngine;

public class Triggers : MonoBehaviour
{
    private ArcadeDriftController controller;

    private const string _Turbo = "Turbo";
    private const string _Aceite = "Aceite";
    private const string _Meta = "Meta";

    private void Start()
    {
        controller = ArcadeDriftController._controller;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_Turbo))
        {
            controller.Boost();
        }
        if (other.CompareTag(_Aceite))
        {
            controller.Aceite();
        }
        if (other.CompareTag(_Meta))
        {
            //meta
        }
    }
}
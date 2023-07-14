using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Triggers : MonoBehaviour
{
    private ArcadeDriftController controller;

    private void Start()
    {
        controller = ArcadeDriftController._controller;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Turbo"))
        {
            controller.Boost();
        }
        if (other.CompareTag("Aceite"))
        {
            controller.Aceite();
        }
        if (other.CompareTag("Meta"))
        {
            //meta
        }
    }
}
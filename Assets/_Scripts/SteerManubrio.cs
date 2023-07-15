using UnityEngine;
using UnityEngine.Animations;

[ExecuteInEditMode]
public class SteerManubrio : MonoBehaviour
{
    [Header("localEulerAngles.z +-100")]
    [Space]
    public RotationConstraint L0;
    public float _L0 = 0.5f;
    public PositionConstraint L1;
    public float _L1 = 0.25f;
    public AimConstraint L2;
    public AimConstraint L3;
    public float _L3 = 0.25f;
    public AimConstraint L4;

    [Space]
    public RotationConstraint R0;
    public float _R0 = 0.5f;
    public PositionConstraint R1;
    public float _R1 = 0.25f;
    public AimConstraint R2;
    public AimConstraint R3;
    public float _R3 = 0.25f;
    public AimConstraint R4;

    //[Space]
    //public Transform Steer;

    private float valor = 0;

    private void Update()
    {
        valor = transform.localEulerAngles.z;

        if (valor >= 0 && valor < 100)
        {
            //R recive todo
            L0.weight = valor.Remap(0, 100, 0, _L0);
            R0.weight = valor.Remap(0, 100, 0, 1);

            L1.weight = valor.Remap(0, 100, 0, _L1);
            R1.weight = valor.Remap(0, 100, 0, 1);

            L2.weight = 0;
            R2.weight = valor.Remap(0, 100, 0, 1);

            L3.weight = valor.Remap(0, 100, 0, _L3);
            R3.weight = valor.Remap(0, 100, 0, 1);

            L4.weight = 0;
            R4.weight = valor.Remap(0, 80, 0, 1);

            //Steer.localEulerAngles = new Vector3(0, valor.Remap(0, 100, 360, 335), 0);
        }
        else if (valor < 360 && valor > 260)
        {
            //L recibe todo
            L0.weight = valor.Remap(360, 260, 0, 1);
            R0.weight = valor.Remap(360, 260, 0, _R0);

            L1.weight = valor.Remap(360, 260, 0, 1);
            R1.weight = valor.Remap(360, 260, 0, _R1);

            L2.weight = valor.Remap(360, 260, 0, 1);
            R2.weight = 0;

            L3.weight = valor.Remap(360, 260, 0, 1);
            R3.weight = valor.Remap(360, 260, 0, _R3);

            L4.weight = valor.Remap(360, 260, 0, 1);
            R4.weight = 0;

            //Steer.localEulerAngles = new Vector3(0, valor.Remap(360, 260, 0, 25), 0);
        }
    }
}
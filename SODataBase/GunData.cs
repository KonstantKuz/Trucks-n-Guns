using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewGun", menuName = ("Data/GunData"))]
public class GunData : Data
{
    public string battleUnit;

    public float rateofFire;

    public float targetingSpeed;

    public AnimationCurve spreadCurve;
    public float spreadForce;
}

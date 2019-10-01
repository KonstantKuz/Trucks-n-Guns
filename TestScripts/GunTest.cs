using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTest : MonoBehaviour
{
    public Rigidbody target;

    private void Start()
    {
        GetComponent<GunParent>().targetData = new TargetData(target);
    }

    private void Update()
    {
        GetComponent<GunParent>().Fire();
    }
}

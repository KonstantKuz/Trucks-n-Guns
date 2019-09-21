using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetData
{
    public Rigidbody target_rigidbody;

    public TargetData (Rigidbody target_rigidbody)
    {
        this.target_rigidbody = target_rigidbody;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetData
{
    public Rigidbody target_rigidbody;
    public EntityCondition target_condition;

    public TargetData (Rigidbody target_rigidbody, EntityCondition target_condition)
    {
        this.target_rigidbody = target_rigidbody;
        this.target_condition = target_condition;
    }
}
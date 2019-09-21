using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GunParent : MonoCached
{
    public abstract GunData myData { get; set; }
    public abstract TargetData targetData { get; set; }

    public abstract void Fire();
    public abstract void SetTargetData(TargetData targetData);
}

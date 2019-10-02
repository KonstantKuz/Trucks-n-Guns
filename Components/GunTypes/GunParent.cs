using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Gun
{
    GunData myData { get; set; }
    TargetData targetData { get; set; }

    int HeadHolderMaxAngle { get; set; }
    int HeadMaxAngle { get; set; }
    int HeadMinAngle { get; set; }

    void SetStandardAllowableAngles();
    void SetAllowableAngles(int headHolderMaxAngle, int headMaxAngle, int headMinAngle);

    void SetTargetData(TargetData targetData);
    void Fire();
}

public abstract class GunParent : MonoCached
{
    public abstract GunData myData { get; set; }
    public abstract TargetData targetData { get; set; }
    public abstract GunAnglesData allowableAngles { get; set; }

    public abstract void SetUpAngles(GunAnglesData anglesData);
    public abstract void Fire();
    public abstract void SetTargetData(TargetData targetData);
}

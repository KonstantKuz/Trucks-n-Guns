using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Gun
{
    GunData gunData { get; set; }
    TargetData targetData { get; set; }
    GunAnglesData allowableAngles { get; set; }

    void SetGunData(GameEnums.GunDataType gunData);
    void SetAnglesData(GunAnglesData anglesData);
    void SetTargetData(TargetData targetData);
    void Fire();
}

//public abstract class GunParent : MonoCached
//{
//    public abstract GunData gunData { get; set; }
//    public abstract TargetData targetData { get; set; }
//    public abstract GunAnglesData allowableAngles { get; set; }

//    public abstract void SetUpAngles(GunAnglesData anglesData);
//    public abstract void SetTargetData(TargetData targetData);
//    public abstract void Fire();
//}

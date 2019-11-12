using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookGun : MonoCached, Gun
{
    public GunData gunData { get ; set ; }
    public TargetData targetData { get ; set ; }
    public GunAnglesData allowableAngles { get ; set ; }

    public void Fire()
    {
        throw new System.NotImplementedException();
    }

    public void SetTargetData(TargetData targetData)
    {
        throw new System.NotImplementedException();
    }

    public void SetAnglesData(GunAnglesData anglesData)
    {
        throw new System.NotImplementedException();
    }

    public void SetGunData(GameEnums.GunDataType gunData)
    {
        throw new System.NotImplementedException();
    }
}

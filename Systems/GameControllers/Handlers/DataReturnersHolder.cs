using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;
public class DataReturnersHolder : Singleton<DataReturnersHolder>
{
    public DataReturnerBase
        TurretGunDataReturner,
        RocketGunDataReturner;
    public void AwakeDataReturners()
    {
        Data.DataReturnersHolder = this;
        TurretGunDataReturner.AwakeDataHolder();
        RocketGunDataReturner.AwakeDataHolder();
    }
}

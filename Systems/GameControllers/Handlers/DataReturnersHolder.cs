using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;
public class DataReturnersHolder : Singleton<DataReturnersHolder>
{
    public DataReturnerBase
        TurretGunDataReturner,
        RocketGunDataReturner,
        TaskDataReturner;
    private void Awake()
    {
        AwakeDataReturners();
    }

    public void AwakeDataReturners()
    {
        Data.DataReturnersHolder = this;
        TurretGunDataReturner.AwakeDataHolder();
        RocketGunDataReturner.AwakeDataHolder();
        TaskDataReturner.AwakeDataHolder();
    }
}

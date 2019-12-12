using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;
public class DataReturnersHolder : Singleton<DataReturnersHolder>
{
    public DataReturnerBase
        TurretGunDataReturner,
        RocketGunDataReturner,
        EnemyDataReturner,
        HelicopterDataReturner,
        TaskDataReturner;

    public void AwakeDataReturners()
    {
        Data.DataReturnersHolder = this;
        TurretGunDataReturner.AwakeDataHolder();
        RocketGunDataReturner.AwakeDataHolder();
        //EnemyDataReturner.AwakeDataHolder();
        //HelicopterDataReturner.AwakeDataHolder();
        TaskDataReturner.AwakeDataHolder();
    }
}

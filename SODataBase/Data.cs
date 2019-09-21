using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Data : ScriptableObject
{

    public static ObjectPoolersHolder objectPoolersHolder;

    public enum ImproveType
    {
        TrucksCondition,
        TrucksSteeringSpeed,
        GunsRateOfFire,
        GunsSpreadForce,
        GunsBattleUnitsSpeed,
        GunsBattleUnitsDamage,
    }

    public static void ImproveValuePositive(ref float value, float improvingPercent)
    {
        value += value * improvingPercent;
    }
    public static void ImproveValueNegative(ref float value, float improvingPercent)
    {
        value -= value * improvingPercent;
    }
}

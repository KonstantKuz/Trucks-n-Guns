﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
public static class GameEnums
{
    public enum Language
    {
        RU,
        ENG
    }

    public enum EnemyFollowType
    {
        PathFollow = 0,
        PlayerFollow = 1,
        //PlayerRam = 2,
        //PlayerOverTaking = 3,
        //SingleTest = 4 
    }

    public enum FirePointType
    {
        D_FPType /*= 2*/,
        DM_FPType /*= 3*/,
        DP_FPType /*= 4*/,
        DMP_FPType /*= 5*/,
        DC_FPType /*= 7*/,
        DCM_FPType /*= 8*/,
        DCP_FPType /*= 10*/,
        DCMP_FPType /*= 11*/
    }

    public enum GunLocation
    {
        DoorsRight,
        DoorsLeft,
        MountCenter,
        PlatformRight,
        PlatformLeft,
        ContainerRight,
        ContainerLeft,
        ContainerTopForward,
        ContainerTopBackward,
        BodyRight,
        BodyLeft,
        BodyBack
    }

    public enum TrackingGroup
    {
        FirstTrackingGroup,
        SecondTrackingGroup,
        StaticGroup
    }

    public enum DriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive
    }

    public enum BattleType
    {
        Tracking,
        Static
    }

    public enum Gun
    {
        None,
        TurretGun1_Level0,
        TurretGun1_Level1,
        TurretGun2_Level0,
        TurretGun2_Level1,
        TurretGun3_Level0,
        TurretGun3_Level1,
        RocketLauncher1,
        RocketLauncher2
    }

    public enum Truck
    {
        APACH,
        FLATNOSE,
        GAZ,
        GMC,
        KENWORTH,
        LILMAC,
        RUBY,
        ZIL
    }

    public enum GunDataType
    {
        LrLdLs = 111,
        LrMdLs = 121,
        LrHdLs = 131,
        MrLdLs = 211,
        MrMdLs = 221,
        MrHdLs = 231,
        HrLdLs = 311,
        HrMdLs = 321,
        HrHdLs = 331,
        LrLdMs = 112,
        LrMdMs = 122,
        LrHdMs = 132,
        MrLdMs = 212,
        MrMdMs = 222,
        MrHdMs = 232,
        HrLdMs = 312,
        HrMdMs = 322,
        HrHdMs = 332,
        LrLdHs = 113,
        LrMdHs = 123,
        LrHdHs = 133,
        MrLdHs = 213,
        MrMdHs = 223,
        MrHdHs = 233,
        HrLdHs = 313,
        HrMdHs = 323,
        HrHdHs = 333,
    }

    public enum ShopItemType
    {
        Truck,
        FirePoint,
        Gun,
        GunLevel
    }

    public enum RoadPropsType
    {
        Town,
        FromTownToDesert,
        Desert,
        FromDesertToTown,
    }
    public enum RoadAsphaltType
    {
        SingleCenterRoad,
        SingleLeftRoad,
        SingleRightRoad,
        DoubleRoad,
        NoneRoad
    }    
    public enum RoadShapeType
    {
        Start,
        Middle,
        End
    }

    public enum TaskType
    {
        DestroyEnemies,
        TravelDistance,
        TravelTime
    }

    public enum SessionComplexity
    {
        Low,
        Medium,
        High
    }
}

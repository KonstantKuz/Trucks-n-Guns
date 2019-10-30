﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
public static class GameEnums
{
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
        ContainerCenterBackward
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
}

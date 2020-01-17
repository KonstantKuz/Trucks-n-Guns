using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FirePointData", menuName = "Data/FirePointData")]
[System.Serializable]
public class FirePointData : Data
{
    [System.Serializable]
    public class GunConfiguration
    {
        public GameEnums.Gun gunType;
        public GameEnums.BattleType battleType;
        public GameEnums.GunLocation locationPath;
        public GameEnums.TrackingGroup trackingGroup;
        public GameEnums.GunDataType gunDataType;
    }

    public GunConfiguration[] gunsConfigurations;


    public GunConfiguration GetConfigOnLocation(string locationPath)
    {
        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            if (gunsConfigurations[i].locationPath.ToString() == locationPath)
            {
                return gunsConfigurations[i];
            }
        }
        Debug.LogError($"GunConfig with {locationPath} path does not contains in FPData");
        return null;
    }

    public void PermanentSetUpFirePoint(FirePoint firePoint)
    {
        firePoint.CreateFirePointsDictionaries();

        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            if (gunsConfigurations[i].gunType == GameEnums.Gun.None)
            {
                continue;
            }

            if (firePoint.GunPointsDictionary.ContainsKey(gunsConfigurations[i].locationPath.ToString()))
            {
                GameObject gun = ObjectPoolersHolder.GunsPooler.PermanentSpawn(gunsConfigurations[i].gunType.ToString());

                FirePoint.GunPoint gunPoint = firePoint.GunPointsDictionary[gunsConfigurations[i].locationPath.ToString()];

                gun.transform.parent = gunPoint.gunsLocation;
                gun.transform.localPosition = Vector3.zero;

                Gun gunComponent = gun.GetComponent<Gun>();
                gunComponent.battleType = gunsConfigurations[i].battleType;
                gunComponent.SetAnglesData(null);
                gunComponent.SetAnglesData(gunPoint.allowableAnglesOnPoint);
                gunComponent.SetGunData(gunsConfigurations[i].gunDataType);
                firePoint.TrackingGroupsDictionary[gunsConfigurations[i].trackingGroup].Add(gunComponent);
            }
        }
    }

    public void ReturnObjectsToPool(FirePoint owner)
    {
        for (int i = 0; i < owner.gunsPoints.Count; i++)
        {
            if (owner.gunsPoints[i].gunsLocation.childCount > 0)
            {
                foreach (Transform gun in owner.gunsPoints[i].gunsLocation)
                {
                    ObjectPoolersHolder.GunsPooler.ReturnToPool(gun.gameObject, gun.name);
                }
            }
        }
    }

    public void RewriteData(FirePointData dataToCopy)
    {
        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            gunsConfigurations[i].battleType = dataToCopy.gunsConfigurations[i].battleType;
            gunsConfigurations[i].gunDataType = dataToCopy.gunsConfigurations[i].gunDataType;
            gunsConfigurations[i].gunType = dataToCopy.gunsConfigurations[i].gunType;
            gunsConfigurations[i].locationPath = dataToCopy.gunsConfigurations[i].locationPath;
            gunsConfigurations[i].trackingGroup = dataToCopy.gunsConfigurations[i].trackingGroup;

        }
    }

    [ContextMenu("ResetData")]
    public void ResetData()
    {
        int locationPath = 0;
        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            locationPath = i;
            gunsConfigurations[i].gunDataType = GameEnums.GunDataType.LrLdLs;
            if(i < 4)
            {
                gunsConfigurations[i].gunType = GameEnums.Gun.TurretGun1_Level0;
            }
            else
            {
                gunsConfigurations[i].gunType = GameEnums.Gun.None;
            }
            gunsConfigurations[i].locationPath = (GameEnums.GunLocation)locationPath;
            gunsConfigurations[i].trackingGroup = GameEnums.TrackingGroup.FirstTrackingGroup;
        }
    }
    [ContextMenu("SetAllToFirstTrackingGroup")]
    public void SetAllToFirstTrackingGroup()
    {
        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            gunsConfigurations[i].trackingGroup = GameEnums.TrackingGroup.FirstTrackingGroup;
        }
    }
    [ContextMenu("SetAllToSecondTrackingGroup")]
    public void SetAllToSecondTrackingGroup()
    {
        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            gunsConfigurations[i].trackingGroup = GameEnums.TrackingGroup.SecondTrackingGroup;
        }
    }
    [ContextMenu("SetAllToRandomTrackingGroup")]
    public void SetAllToRandomTrackingGroup()
    {
        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            int randomTrackingGroupNumber = Random.Range(0, System.Enum.GetNames(typeof(GameEnums.TrackingGroup)).Length);
            GameEnums.TrackingGroup randomTrackingGroup = (GameEnums.TrackingGroup)randomTrackingGroupNumber;
            gunsConfigurations[i].trackingGroup = randomTrackingGroup;
            if (gunsConfigurations[i].trackingGroup == GameEnums.TrackingGroup.StaticGroup)
            {
                gunsConfigurations[i].battleType = GameEnums.BattleType.Static;
            }
        }
    }
    [ContextMenu("SetAllGunsToStatic")]
    public void SetAllGunsToStatic()
    {
        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            gunsConfigurations[i].battleType = GameEnums.BattleType.Static;
        }
    }
    [ContextMenu("SetAllGunsToTracking")]
    public void SetAllGunsToTracking()
    {
        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            gunsConfigurations[i].battleType = GameEnums.BattleType.Tracking;
        }
    }
    [ContextMenu("SetAllGunsToNone")]
    public void SetAllGunsToNone()
    {
        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            gunsConfigurations[i].gunType = GameEnums.Gun.None;
        }
    }
    [ContextMenu("SetAllGunsToRandom")]
    public void SetAllGunsToRandom()
    {
        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            int randomGunNumber = Random.Range(0, System.Enum.GetNames(typeof(GameEnums.Gun)).Length);
            GameEnums.Gun randomGun = (GameEnums.Gun)randomGunNumber;
            gunsConfigurations[i].gunType = randomGun;
        }
    }
    [ContextMenu("SetAllGunDataTypesToLrLd")]
    public void SetAllGunDataTypesToLrLd()
    {
        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            gunsConfigurations[i].gunDataType = GameEnums.GunDataType.LrLdLs;
        }
    }

    [ContextMenu("SetAllGunDataTypesToRandom")]
    public void SetAllGunDataTypesToRandom()
    {
        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            int randomGunDataTypeNumber = Random.Range(0, System.Enum.GetNames(typeof(GameEnums.GunDataType)).Length);
            GameEnums.GunDataType randomGunDataType = (GameEnums.GunDataType)randomGunDataTypeNumber;
            gunsConfigurations[i].gunDataType = randomGunDataType;
        }
    }
}

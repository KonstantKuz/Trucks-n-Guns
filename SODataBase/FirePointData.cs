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
        public GameEnums.GunLocation locationPath;
        public GameEnums.TrackingGroup trackingGroup;
        public GameEnums.GunDataType gunDataType;
    }

    public GunConfiguration[] gunsConfigurations;

    public void PermanentSetUpFirePoint(FirePoint firePoint)
    {
        firePoint.CreateFirePointsDictionaries();

        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            if(gunsConfigurations[i].gunType == GameEnums.Gun.None)
            {
                continue;
            }

            if(firePoint.GunPointsDictionary.ContainsKey(gunsConfigurations[i].locationPath.ToString()))
            {
                GameObject gun = ObjectPoolersHolder.GunsPooler.PermanentSpawn(gunsConfigurations[i].gunType.ToString());
                
                FirePoint.GunPoint gunPoint = firePoint.GunPointsDictionary[gunsConfigurations[i].locationPath.ToString()];

                gun.transform.parent = gunPoint.gunsLocation;
                gun.transform.localPosition = Vector3.zero;

                Gun gunComponent = gun.GetComponent<Gun>();
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
            gunsConfigurations[i] = dataToCopy.gunsConfigurations[i];
        }
    }
}

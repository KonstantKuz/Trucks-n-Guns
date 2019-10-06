using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FirePointData", menuName = "Data/FirePointData")]
public class FirePointData : Data
{
    [System.Serializable]
    public class GunConfiguration
    {
        public GameEnums.Gun gun;
        public GameEnums.GunLocation locationPath;
        public GameEnums.TrackingGroup trackingGroup;
        public GunData gunDataToSet;
    }

    //public GameEnums.FirePointType firePointType;

    public GunConfiguration[] gunsConfigurations;

    public void PermanentSetUpFirePoint(FirePoint firePoint)
    {
        firePoint.CreateFirePointsDictionaries();

        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            if(gunsConfigurations[i].gun == GameEnums.Gun.None)
            {
                continue;
            }

            if(firePoint.GunPointsDictionary.ContainsKey(gunsConfigurations[i].locationPath.ToString()))
            {
                GameObject gun = objectPoolersHolder.GunsPooler.PermanentSpawnFromPool(gunsConfigurations[i].gun.ToString());
                GunParent gunComponent = gun.GetComponent<GunParent>();
                FirePoint.GunPoint gunPoint = firePoint.GunPointsDictionary[gunsConfigurations[i].locationPath.ToString()];

                gun.transform.parent = gunPoint.gunsLocation;
                gun.transform.localPosition = Vector3.zero;
                gunComponent.myData = gunsConfigurations[i].gunDataToSet;
                gunComponent.SetUpAngles(gunPoint.allowableAnglesOnPoint);
                firePoint.TrackingGroupsDictionary[gunsConfigurations[i].trackingGroup].Add(gunComponent);
            }
        }
    }

    public void ReturnObjectToPool(FirePoint owner, FirePointData ownerData)
    {
        for (int i = 0; i < owner.gunsPoints.Length; i++)
        {
            if (owner.gunsPoints[i].gunsLocation.childCount > 0)
            {
                GameObject gunToReturn = owner.gunsPoints[i].gunsLocation.GetChild(0).gameObject;
                objectPoolersHolder.GunsPooler.ReturnGameObjectToPool(gunToReturn, ownerData.gunsConfigurations[i].gun.ToString());
            }
        }
    }
}

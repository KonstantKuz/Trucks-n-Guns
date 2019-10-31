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
                GameObject gun = objectPoolersHolder.GunsPooler.PermanentSpawn(gunsConfigurations[i].gun.ToString());
                
                FirePoint.GunPoint gunPoint = firePoint.GunPointsDictionary[gunsConfigurations[i].locationPath.ToString()];

                gun.transform.parent = gunPoint.gunsLocation;
                gun.transform.localPosition = Vector3.zero;

                if(gun.name != "None")
                {

                    GunParent gunComponent = gun.GetComponent<GunParent>();
                    gunComponent.SetUpAngles(null);
                    gunComponent.SetUpAngles(gunPoint.allowableAnglesOnPoint);
                    firePoint.TrackingGroupsDictionary[gunsConfigurations[i].trackingGroup].Add(gunComponent);
                }
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
                    objectPoolersHolder.GunsPooler.ReturnToPool(gun.gameObject, gun.name);
                }
            }
        }
    }
}

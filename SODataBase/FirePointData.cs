using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FirePointData", menuName = "Data/FirePointData")]
public class FirePointData : Data
{
    [System.Serializable]
    public class GunConfiguration
    {
        public GameEnums.Gun gunType;
        public GameEnums.GunLocation locationPath;
        public GameEnums.TrackingGroup trackingGroup;
        public GunData gunDataToSet;
    }

    public GameEnums.FirePointType firePointType;

    public GunConfiguration[] gunsConfigurations;

    private Dictionary<string, string> gunsConfigurationsDictionary;


    public void SetUpGunsConfigurationsDictionary()
    {
        //if (gunsConfigurations.Length > (int)firePointType)
        //{
        //    throw new System.Exception("Количество конфигураций оружия превышает количество точек для установки." + gunsConfigurations.Length + ">>" + (int)firePointType);
        //}
        //else
        //{
            gunsConfigurationsDictionary = new Dictionary<string, string>(gunsConfigurations.Length);

            for (int i = 0; i < gunsConfigurations.Length; i++)
            {
                gunsConfigurationsDictionary.Add(gunsConfigurations[i].locationPath.ToString(), gunsConfigurations[i].gunType.ToString());
            }
        //}
    }

    public void PermanentSetUpFirePoint(FirePoint firePoint)
    {
        firePoint.CreateFirePointsDictionaries();

        for (int i = 0; i < gunsConfigurations.Length; i++)
        {
            if(firePoint.GunPointsDictionary.ContainsKey(gunsConfigurations[i].locationPath.ToString()))
            {
                GameObject gun = objectPoolersHolder.GunsPooler.PermanentSpawnFromPool(gunsConfigurations[i].gunType.ToString());
                gun.transform.parent = firePoint.GunPointsDictionary[gunsConfigurations[i].locationPath.ToString()];
                gun.transform.localPosition = Vector3.zero;
                gun.transform.localEulerAngles = Vector3.zero;
                gun.GetComponent<GunParent>().myData = gunsConfigurations[i].gunDataToSet;
                firePoint.TrackingGroupsDictionary[gunsConfigurations[i].trackingGroup].Add(gun.GetComponent<GunParent>());
            }
        }
    }
}

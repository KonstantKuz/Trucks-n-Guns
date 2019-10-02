using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewTruck", menuName = "Data/TruckData")]
public class TruckData : Data
{
    #region NEWSYSTEM
    [Header("NEWSYSTEM")]

    public FirePointData firePointDataToCopy;

    public FirePointData firePointData { get; set; }

    public GameEnums.Truck truckType;

    public GameEnums.FirePointType firePointType;
    #endregion

    [Header("Настройки авто")]
    public GameEnums.DriveType driveType;

    [Space]
    [Tooltip("Максимально возможный угол поворота передних колес")]

    public float maxSteerAngle = 30f;
    [Tooltip("Максимально возможный вращающий момент колес")]

    public float maxMotorTorque = 450f;
    [Tooltip("Скорость поворота колес")]

    public float wheelSteeringSpeed = 0.7f;

    public float maxTrucksCondition = 10000;

    #region NEWSYSTEM

    public void SetUpTruck(Truck owner)
    {
        PermanentSetUpTruck(owner);
        PermanentSetUpFirePoint(owner);
    }
    
    public void PermanentSetUpTruck(Truck owner)
    {
        string truckTypeName = truckType.ToString();
        GameObject truck = objectPoolersHolder.TrucksPooler.PermanentSpawnFromPool(truckTypeName);
        truck.transform.parent = owner._transform.GetChild(0);
        truck.transform.localPosition = Vector3.zero;
        truck.transform.localEulerAngles = Vector3.zero;
        owner.SetUpWheelsVisual(truck.transform);
    }

    public void PermanentSetUpFirePoint(Truck owner)
    {
        string firePointTypeName = firePointType.ToString();
        GameObject firePoint = objectPoolersHolder.FirePointPooler.PermanentSpawnFromPool(firePointTypeName);
        firePoint.transform.parent = owner._transform;
        firePoint.transform.localPosition = Vector3.zero;
        firePoint.transform.localEulerAngles = Vector3.zero;
        owner.firePoint = firePoint.GetComponent<FirePoint>();
        owner.TruckData.firePointData.PermanentSetUpFirePoint(owner.firePoint);
    }

    public void ReturnObjectsToPool(Truck owner)
    {
        for (int i = 0; i < owner.firePoint.gunsPoints.Length; i++)
        {
            if(owner.firePoint.gunsPoints[i].gunsLocation.childCount>0)
            {
                GameObject gunToReturn = owner.firePoint.gunsPoints[i].gunsLocation.GetChild(0).gameObject;
                objectPoolersHolder.GunsPooler.ReturnGameObjectToPool(gunToReturn, firePointData.gunsConfigurations[i].gun.ToString());
            }
        }

        GameObject firePointToReturn = owner.transform.GetChild(3).gameObject;
        objectPoolersHolder.FirePointPooler.ReturnGameObjectToPool(firePointToReturn, firePointType.ToString());

        GameObject truckVisualToReturn = owner.transform.GetChild(0).transform.GetChild(0).gameObject;
        objectPoolersHolder.TrucksPooler.ReturnGameObjectToPool(truckVisualToReturn, truckType.ToString());

    }
    #endregion
}

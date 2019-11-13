using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewTruck", menuName = "Data/TruckData")]
[System.Serializable]
public class TruckData : Data
{

    public FirePointData firePointData;

    //public FirePointData firePointData { get { return firePointDataToCopy; } set { } }

    public GameEnums.Truck truckType;

    public GameEnums.FirePointType firePointType;

    public GameEnums.DriveType driveType;

    public float maxSteerAngle = 30f;

    public float maxMotorTorque = 450f;

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
        GameObject truck = ObjectPoolersHolder.TrucksPooler.PermanentSpawn(truckTypeName);
        truck.transform.parent = owner._transform.GetChild(0);
        truck.transform.localPosition = Vector3.zero;
        truck.transform.localEulerAngles = Vector3.zero;
        owner.SetUpWheelsVisual(truck.transform);
    }

   

    public void PermanentSetUpFirePoint(Truck owner)
    {
        //owner.TruckData.firePointData = owner.TruckData.firePointDataToCopy;
        string firePointTypeName = firePointType.ToString();
        GameObject firePoint = ObjectPoolersHolder.TrucksFirePointPooler.PermanentSpawn(firePointTypeName);
        firePoint.transform.parent = owner._transform;
        firePoint.transform.localPosition = Vector3.zero;
        firePoint.transform.localEulerAngles = Vector3.zero;
        owner.firePoint = firePoint.GetComponent<FirePoint>();

        owner.firePoint.MergeFirePoints(owner.transform.GetChild(0).transform.GetChild(0).GetComponent<FirePoint>());

        owner.TruckData.firePointData.PermanentSetUpFirePoint(owner.firePoint);
    }

    public void ReturnObjectsToPool(Truck owner)
    {
        owner.TruckData.firePointData.ReturnObjectsToPool(owner.firePoint);

        ObjectPoolersHolder.TrucksFirePointPooler.ReturnToPool(owner.firePoint.gameObject, owner.firePoint.gameObject.name);
        foreach (Transform visualTruck in owner.transform.GetChild(0).transform)
        {
            ObjectPoolersHolder.TrucksPooler.ReturnToPool(visualTruck.gameObject, visualTruck.name);
        }
    }
    #endregion

    public void RewriteData(TruckData dataToCopy)
    {
        truckType = dataToCopy.truckType;
        firePointType = dataToCopy.firePointType;
        driveType = dataToCopy.driveType;
        maxTrucksCondition = dataToCopy.maxTrucksCondition;
    }
}

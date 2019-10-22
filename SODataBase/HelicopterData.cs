using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HelicopterData", menuName = "Data/HelicopterData")]
public class HelicopterData : Data
{
    public GameEnums.FirePointType firePointType;
    public FirePointData firePointData;
    public float maxCondition;
    public Vector3 offsetFromTarget;

    public void PermanentSetUpHelicopter(Helicopter owner)
    {
        string firePointName = firePointType.ToString();
        GameObject firePoint = objectPoolersHolder.TrucksFirePointPooler.PermanentSpawnFromPool(firePointName);
        firePoint.transform.parent = owner.transform;
        owner.firePoint = firePoint.GetComponent<FirePoint>();
        firePoint.transform.localPosition = Vector3.zero;
        firePoint.transform.localEulerAngles = Vector3.zero;

        owner.helicopterData.firePointData.PermanentSetUpFirePoint(owner.firePoint);
    }

    public void ReturnObjectsToPool(Helicopter owner)
    {
        owner.helicopterData.firePointData.ReturnObjectsToPool(owner.firePoint, owner.helicopterData.firePointData);

        GameObject firePointToReturn = owner.transform.GetChild(2).gameObject;
        objectPoolersHolder.TrucksFirePointPooler.ReturnGameObjectToPool(firePointToReturn, firePointType.ToString());
    }
}

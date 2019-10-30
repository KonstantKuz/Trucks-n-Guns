using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoadBlockData", menuName = "Data/RoadBlockData")]
public class RoadBlockData : Data
{
    public LayerMask damageable;
    public LayerMask obstacle;

    public GameEnums.FirePointType firePointType;
    public FirePointData firePointData;

    public float conditionPerGun = 0;

    public void PermanentSetUpRoadBlock(RoadBlock owner)
    {
        string firePointName = firePointType.ToString();
        GameObject block = objectPoolersHolder.RoadBlocksFirePointPooler.PermanentSpawn(firePointName);
        block.transform.parent = owner.transform;
        owner.firePoint = block.GetComponent<FirePoint>();
        block.transform.localPosition = Vector3.zero;
        block.transform.localEulerAngles = Vector3.zero;

        owner.blocksToDestroy = new Transform[5];

        for (int i = 0; i < block.transform.GetChild(0).transform.childCount; i++)
        {
            owner.blocksToDestroy[i] = block.transform.GetChild(0).transform.GetChild(i).transform;
        }
        owner.roadBlockData.firePointData.PermanentSetUpFirePoint(owner.firePoint);
    }

    public void ReturnObjectsToPool(RoadBlock owner)
    {
        owner.roadBlockData.firePointData.ReturnObjectsToPool(owner.firePoint);
        GameObject firePointToReturn = owner.firePoint.gameObject;
        objectPoolersHolder.RoadBlocksFirePointPooler.ReturnToPool(firePointToReturn, firePointToReturn.name);
        objectPoolersHolder.EventPooler.ReturnToPool(owner.gameObject, "RoadBlock");
    }

}

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

    public int zSpawnOffset;
    public float conditionPerGun = 0;
    public float conditionPerWall = 0;

    public void PermanentSetUpRoadBlock(RoadBlock owner)
    {
        string firePointName = firePointType.ToString();
        GameObject block = ObjectPoolersHolder.RoadBlocksFirePointPooler.PermanentSpawn(firePointName);
        block.transform.parent = owner.transform;
        owner.firePoint = block.GetComponent<FirePoint>();
        block.transform.localPosition = Vector3.zero;
        block.transform.localEulerAngles = Vector3.zero;

        //for (int i = 0; i < block.transform.GetChild(0).transform.childCount; i++)
        //{
        //    bool randomActiveExplosion = Random.value > 0.7f ? true : false;
        //    block.transform.GetChild(0).transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(randomActiveExplosion);
        //    block.transform.GetChild(0).transform.GetChild(i).GetComponent<EntityCondition>().ResetCurrentCondition(conditionPerWall);
        //}

        owner.roadBlockData.firePointData.PermanentSetUpFirePoint(owner.firePoint);
    }

    public void ReturnObjectsToPool(RoadBlock owner)
    {
        owner.roadBlockData.firePointData.ReturnObjectsToPool(owner.firePoint);
        GameObject firePointToReturn = owner.firePoint.gameObject;
        ObjectPoolersHolder.RoadBlocksFirePointPooler.ReturnToPool(firePointToReturn, firePointToReturn.name);
        ObjectPoolersHolder.EventPooler.ReturnToPool(owner.gameObject, "RoadBlock");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBlockage : MonoCached, IRoadEvent, IPoolReturner
{
    private GameObject concreteBlockage;

    public void AwakeEvent()
    {
        concreteBlockage = ObjectPoolersHolder.Instance.StoneBlockagePooler.SpawnRandomItemFromPool(transform.position, Quaternion.identity, 10);
    }

    public void ReturnObjectsToPool()
    {
        ObjectPoolersHolder.Instance.StoneBlockagePooler.ReturnGameObjectToPool(concreteBlockage, concreteBlockage.name);
    }
}

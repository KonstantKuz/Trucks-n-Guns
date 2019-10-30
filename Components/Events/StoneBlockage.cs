using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBlockage : MonoCached, IRoadEvent, IPoolReturner
{
    private GameObject concreteBlockage;

    public bool isActive { get { return gameObject.activeInHierarchy; } set { } }

    public void AwakeEvent()
    {
        concreteBlockage = ObjectPoolersHolder.Instance.StoneBlockagePooler.SpawnRandom(transform.position, Quaternion.identity);
    }

    public void ReturnObjectsToPool()
    {
        ObjectPoolersHolder.Instance.StoneBlockagePooler.ReturnToPool(concreteBlockage, concreteBlockage.name);
        ObjectPoolersHolder.Instance.EventPooler.ReturnToPool(gameObject, "StoneBlockage");
    }
}

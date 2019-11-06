using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBlockage : MonoCached, IRoadEvent, IPoolReturner
{
    private GameObject concreteBlockage;

    public bool isActive { get { return gameObject.activeInHierarchy; } set { } }

    public void AwakeEvent()
    {
        concreteBlockage = ObjectPoolersHolder.Instance.StoneBlockagePooler.SpawnWeightedRandom(transform.position, Quaternion.identity);
        DisableNearestObstacles();
    }

    private void DisableNearestObstacles()
    {
        Collider[] obstacles = Physics.OverlapSphere(transform.position, 40f, 1 << 10);
        for (int i = 0; i < obstacles.Length; i++)
        {
            if (!ReferenceEquals(obstacles[i], concreteBlockage.GetComponentInChildren<Collider>()))
            {
                obstacles[i].gameObject.SetActive(false);
            }
        }
    }
    public void ReturnObjectsToPool()
    {
        ObjectPoolersHolder.Instance.StoneBlockagePooler.ReturnToPool(concreteBlockage, concreteBlockage.name);
        ObjectPoolersHolder.Instance.EventPooler.ReturnToPool(gameObject, "StoneBlockage");
    }
}

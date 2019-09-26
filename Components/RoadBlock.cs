using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoadBlock : MonoCached
{
    public LayerMask damageable;

    public LayerMask obstacle;

    public FirePoint firePoint;
    public FirePointData firePointData;
    public float conditionPerGun;
    public Transform[] blocksToDestroy;

    private TargetData[] targets;
    private float totalStartCondition, conditionToDestroy = 100f;

    private bool activated = false, destroyed = false;

    private void OnEnable()
    {
        StartCoroutine(SetUpRoadBlock());
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == obstacle)
        {
            other.gameObject.SetActive(false);
        }

        if(other.gameObject.layer == damageable)
        {
            StartCoroutine(SetTargets());
            activated = true;
        }
    }

    private IEnumerator SetUpRoadBlock()
    {
        SetUpConditions();
        SetUpTargets();
        yield return new WaitForEndOfFrame();
        StartCoroutine(UpdateRoadBlock());
    }
    
    private void SetUpConditions()
    {

        for (int i = 0; i < firePoint.FirstTrackingGroupGuns.Count; i++)
        {
            firePoint.FirstTrackingGroupGuns[i].gameObject.SetActive(true);
            firePoint.FirstTrackingGroupGuns[i].GetComponent<EntityCondition>().maxCondition = conditionPerGun;
            totalStartCondition += firePoint.FirstTrackingGroupGuns[i].GetComponent<EntityCondition>().currentCondition;
        }
    }

    private void SetUpTargets()
    {
        targets = new TargetData[firePoint.FirstTrackingGroupGuns.Count + firePoint.SecondTrackingGroupGuns.Count];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = new TargetData(null);
        }
    }

    private IEnumerator UpdateRoadBlock()
    {
        yield return new WaitForEndOfFrame();
        CheckCondition();
        if (activated)
        {
            AttackTargets();
        }
        if(gameObject.activeInHierarchy == true)
        {
            yield return StartCoroutine(UpdateRoadBlock());
        }
    }

    private IEnumerator SetTargets()
    {
        var hits = Physics.OverlapSphere(transform.position, 100f, damageable);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].GetComponent<Truck>() != null)
            {
                targets[Random.Range(0, targets.Length)].target_rigidbody = hits[i].GetComponent<Rigidbody>();
            }
        }
        for (int i = 0; i < targets.Length; i++)
        {
            firePoint.FirstTrackingGroupGuns[i].SetTargetData(targets[i]);
        }

        yield return null;
    }

    private void AttackTargets()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if(targets[i].target_rigidbody != null)
            {
                if(targets[i].target_rigidbody.gameObject.activeInHierarchy == false)
                {
                    StartCoroutine(SetTargets());
                }
                firePoint.StaticAttack();
                firePoint.FirstTrackingAttack();
            }
            else if(targets[i].target_rigidbody == null)
            {
                StartCoroutine(SetTargets());
            }
        }
    }

    private void CheckCondition()
    {
        float currentTotalCondition = 0;

        for (int i = 0; i < firePoint.FirstTrackingGroupGuns.Count; i++)
        {
            currentTotalCondition += firePoint.FirstTrackingGroupGuns[i].GetComponent<EntityCondition>().currentCondition;
        }
        if (currentTotalCondition <= conditionToDestroy && destroyed == false)
        {
            destroyed = true;
            OpenRoad();
        }
    }
    
    private void OpenRoad()
    {
        int randomBlockoDestroyNumber = Random.Range(0, blocksToDestroy.Length);

        for (int i = 0; i < 5; i++)
        {
            GameObject expl = ObjectPoolersHolder.Instance.EffectPooler.SpawnFromPool("BigExplosion",
                blocksToDestroy[randomBlockoDestroyNumber].position + new Vector3(Random.Range(-3,3), Random.Range(-1, 2), 
                Random.Range(-3, 3)), Quaternion.identity);
            expl.GetComponent<ParticleSystem>().Play();
            foreach (var item in expl.GetComponentsInChildren<ParticleSystem>())
            {
                item.Play();
            }
        }
        blocksToDestroy[randomBlockoDestroyNumber].gameObject.SetActive(false);

        StartCoroutine(DeactivateAll());
    }

    private IEnumerator DeactivateAll()
    {
        yield return new WaitForSeconds(20f);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < firePoint.FirstTrackingGroupGuns.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(firePoint.FirstTrackingGroupGuns[i].transform.position, firePoint.FirstTrackingGroupGuns[i].transform.forward * 15f);
        }
    }
}

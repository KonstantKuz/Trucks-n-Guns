using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoadBlock : MonoCached
{

    public RoadBlockData roadBlockData;

    public FirePoint firePoint { get; set; }

    public Transform[] blocksToDestroy { get; set; }

    private TargetData[] targets;

    private float totalStartCondition = 0, conditionToDestroy = 100f;

    private bool activated = false, destroyed = false;


    private void OnEnable()
    {
        roadBlockData.PermanentSetUpRoadBlock(this);
        StartCoroutine(SetUpRoadBlock());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.layer == 9 && other.GetComponent<Truck>()!= null)
        {
            Debug.Log("<color=red> SettingUpTargets on RoadBlock </color>");
            StartCoroutine(SetTargets());
        }
    }

    private IEnumerator SetUpRoadBlock()
    {
        SetUpConditions();
        SetUpTargets();
        yield return new WaitForEndOfFrame();
        //StartCoroutine(UpdateRoadBlock());
    }

    private void SetUpConditions()
    {
        for (int i = 0; i < firePoint.gunsPoints.Length; i++)
        {
            firePoint.gunsPoints[i].gunsLocation.gameObject.SetActive(true);
            firePoint.gunsPoints[i].gunsLocation.gameObject.GetComponent<EntityCondition>().ResetCurrentCondition(roadBlockData.conditionPerGun);
            totalStartCondition = firePoint.gunsPoints[i].gunsLocation.gameObject.GetComponent<EntityCondition>().currentCondition;
        }
    }

    private void SetUpTargets()
    {
        targets = new TargetData[firePoint.gunsPoints.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = new TargetData(null);
        }
    }

    public override void OnTick()
    {
        CheckCondition();
        if (activated)
        {
            AttackTargets();
        }
    }

    //private IEnumerator UpdateRoadBlock()
    //{
    //    yield return new WaitForEndOfFrame();
    //    Debug.Log($"<color=blue> {activated } </color>");
    //    CheckCondition();
        
    //    if (gameObject.activeInHierarchy == true)
    //    {
    //        yield return StartCoroutine(UpdateRoadBlock());
    //    }
    //}

    private IEnumerator SetTargets()
    {
        var hits = Physics.OverlapSphere(transform.position, 100f, roadBlockData.damageable);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].GetComponent<Truck>() != null)
            {
                targets[Random.Range(0, targets.Length)].target_rigidbody = hits[i].GetComponent<Rigidbody>();
                Debug.Log($"<color=green> Selected target is {hits[i].name} </color>");
            }
        }
        for (int i = 0; i < firePoint.FirstTrackingGroupGuns.Count; i++)
        {
                firePoint.FirstTrackingGroupGuns[i].SetTargetData(NotNullTarget());
        }
        for (int i = 0; i < firePoint.SecondTrackingGroupGuns.Count; i++)
        {
                firePoint.SecondTrackingGroupGuns[i].SetTargetData(NotNullTarget());
        }

        activated = true;

        yield return null;
    }

    public TargetData NotNullTarget()
    {
        int randomTargetIndex = Random.Range(0, targets.Length);
        if (targets[randomTargetIndex].target_rigidbody != null)
        {
            return targets[randomTargetIndex];
        }
        else
        {
            return NotNullTarget();
        }
    }

    private void AttackTargets()
    {
        firePoint.StaticAttack();
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].target_rigidbody != null)
            {
                if (targets[i].target_rigidbody.gameObject.activeInHierarchy == false)
                {
                    StartCoroutine(SetTargets());
                }
                firePoint.FirstTrackingAttack();
                firePoint.SecondTrackingAttack();
            }
            else if (targets[i].target_rigidbody == null)
            {
                StartCoroutine(SetTargets());
            }
        }
    }

    private void CheckCondition()
    {
        float currentTotalCondition = 0;

        for (int i = 0; i < firePoint.gunsPoints.Length; i++)
        {
            currentTotalCondition += firePoint.gunsPoints[i].gunsLocation.GetComponent<EntityCondition>().currentCondition;
        }

        if (currentTotalCondition <= conditionToDestroy && destroyed == false)
        {
            destroyed = true;
            OpenRoad();
        }
    }

    private void OpenRoad()
    {
        int randomBlockToDestroyNumber = Random.Range(0, blocksToDestroy.Length);

        for (int i = 0; i < 5; i++)
        {
            GameObject expl = ObjectPoolersHolder.Instance.EffectPooler.SpawnFromPool("BigExplosion",
                blocksToDestroy[randomBlockToDestroyNumber].position + new Vector3(Random.Range(-3, 3), Random.Range(-1, 2),
                Random.Range(-3, 3)), Quaternion.identity);
            expl.GetComponent<ParticleSystem>().Play();
            foreach (var item in expl.GetComponentsInChildren<ParticleSystem>())
            {
                item.Play();
            }
        }
        blocksToDestroy[randomBlockToDestroyNumber].gameObject.SetActive(false);

        StartCoroutine(DeactivateAll());
    }

    private IEnumerator DeactivateAll()
    {
        yield return new WaitForSeconds(20f);
        gameObject.SetActive(false);
    }
}

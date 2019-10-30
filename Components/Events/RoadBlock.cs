using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoadBlock : MonoCached, IRoadEvent, IPoolReturner
{
    public bool isActive { get { return gameObject.activeInHierarchy; } set { } }

    public RoadBlockData roadBlockData;

    public FirePoint firePoint { get; set; }

    public Transform[] blocksToDestroy { get; set; }

    private TargetData[] targets;

    private float totalStartCondition = 0, conditionToDestroy = 100f;

    private bool activated = false;

    public void AwakeEvent()
    {
        roadBlockData.firePointType = (GameEnums.FirePointType)Random.Range(0, System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length - 1);


        if (firePoint!=null)
        {
            roadBlockData.ReturnObjectsToPool(this);
        }
        activated = false;
        gameObject.SetActive(true);
        roadBlockData.PermanentSetUpRoadBlock(this);
        StartCoroutine(SetUpRoadBlock());
    }

    private void OnEnable()
    {
        allTicks.Add(this);
       
    }
    private void OnDisable()
    {
        allTicks.Remove(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.layer == 9 && other.GetComponentInParent<Truck>()!= null)
        {
            StartCoroutine(SetTargets());
        }

        if(other.gameObject.GetComponentInParent<Enemy>() != null)
        {
            StopEnemy(other);
        }
    }
    private void StopEnemy(Collider enemy)
    {
        enemy.gameObject.GetComponentInParent<Enemy>();
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
        for (int i = 0; i < firePoint.gunsPoints.Count; i++)
        {
            firePoint.gunsPoints[i].gunsLocation.gameObject.SetActive(true);
            firePoint.gunsPoints[i].gunsLocation.gameObject.GetComponent<EntityCondition>().ResetCurrentCondition(roadBlockData.conditionPerGun);
            totalStartCondition = firePoint.gunsPoints[i].gunsLocation.gameObject.GetComponent<EntityCondition>().currentCondition;
        }

        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);

        bool randomBool;

        randomBool = Random.value > 0.5f ? true : false;
        transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(randomBool);
        randomBool = Random.value > 0.5f ? true : false;
        transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(randomBool);

        transform.GetChild(0).GetComponent<EntityCondition>().ResetCurrentCondition(3000f);
        transform.GetChild(1).GetComponent<EntityCondition>().ResetCurrentCondition(3000f);

        //transform.GetChild(0).GetComponent<EntityCondition>().OnZeroCondition += AutoReturnObjects;
        //transform.GetChild(1).GetComponent<EntityCondition>().OnZeroCondition += AutoReturnObjects;
    }
    private void AutoReturnObjects()
    {
        transform.GetChild(0).GetComponent<EntityCondition>().OnZeroCondition -= AutoReturnObjects;
        transform.GetChild(1).GetComponent<EntityCondition>().OnZeroCondition -= AutoReturnObjects;
        StartCoroutine(AutoReturnObjects(15f));
    }
    private IEnumerator AutoReturnObjects(float delay)
    {
        yield return new WaitForSeconds(25f);
        ReturnObjectsToPool();
    }
    private void SetUpTargets()
    {
        targets = new TargetData[firePoint.gunsPoints.Count];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = new TargetData(null);
        }
    }

    public override void OnTick()
    {
        if(firePoint!=null)
        {
            CheckCondition();
        }
        if (activated)
        {
            AttackTargets();
        }
    }

    private IEnumerator SetTargets()
    {
        var hits = Physics.OverlapSphere(transform.position, 100f, roadBlockData.damageable);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].GetComponentInParent<Truck>() != null)
            {
                targets[Random.Range(0, targets.Length)].target_rigidbody = hits[i].GetComponentInParent<Rigidbody>();
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
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].target_rigidbody != null)
            {
                if (targets[i].target_rigidbody.gameObject.activeInHierarchy == false)
                {
                    StartCoroutine(SetTargets());
                }
            }
            else if (targets[i].target_rigidbody == null)
            {
                StartCoroutine(SetTargets());
            }
        }
        
        firePoint.StaticAttack();
        firePoint.FirstTrackingAttack();
        firePoint.SecondTrackingAttack();

    }

    private void CheckCondition()
    {
        float currentTotalCondition = 0;
        if(activated)
        {
            for (int i = 0; i < firePoint.gunsPoints.Count; i++)
            {
                currentTotalCondition += firePoint.gunsPoints[i].gunsLocation.GetComponent<EntityCondition>().currentCondition;
                if (firePoint.gunsPoints[i].gunsLocation.GetComponent<EntityCondition>().currentCondition < 50)
                {
                    firePoint.RemoveGun(firePoint.gunsPoints[i].gunsLocation.GetChild(0).GetComponent<GunParent>());
                }
            }
        }

        if (currentTotalCondition <= conditionToDestroy && activated)
        {
            activated = false;
            OpenRoad();
        }
    }

    private void OpenRoad()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject expl = ObjectPoolersHolder.Instance.EffectPooler.Spawn("BigExplosion",
                transform.position + new Vector3(Random.Range(-25, 25), Random.Range(0, 2),
                Random.Range(-3, 3)), Quaternion.identity);
            expl.GetComponent<ParticleSystem>().Play();
            foreach (var item in expl.GetComponentsInChildren<ParticleSystem>())
            {
                item.Play();
            }
        }
        for (int i = 0; i < 10; i++)
        {
            GameObject expl = ObjectPoolersHolder.Instance.EffectPooler.Spawn("SmallExplosion",
                transform.position + new Vector3(Random.Range(-21, 21), Random.Range(0, 2),
                Random.Range(-3, 3)), Quaternion.identity);
            expl.GetComponent<ParticleSystem>().Play();
            foreach (var item in expl.GetComponentsInChildren<ParticleSystem>())
            {
                item.Play();
            }
        }

        ReturnObjectsToPool();

        gameObject.SetActive(false);
    }

    public void ReturnObjectsToPool()
    {
        activated = false;
        roadBlockData.ReturnObjectsToPool(this);
    }
}

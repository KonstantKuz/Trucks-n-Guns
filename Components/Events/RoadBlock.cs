using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoadBlock : MonoCached, IRoadEvent, IPoolReturner
{
    public bool isActive { get { return gameObject.activeInHierarchy; } set { } }

    public RoadBlockData roadBlockData;

    public FirePoint firePoint { get; set; }

    private TargetData[] targets;

    private float totalStartCondition = 0, conditionToDestroy = 100f;

    private bool activated = false;

    public void AwakeEvent(Vector3 playerPosition)
    {

        transform.position = new Vector3(0,0, playerPosition.z + roadBlockData.zSpawnOffset);
        roadBlockData.firePointType = (GameEnums.FirePointType)Random.Range(0, System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length - 1);
        
        if (firePoint!=null)
        {
            roadBlockData.ReturnObjectsToPool(this);
        }
        activated = false;
        gameObject.SetActive(true);
        roadBlockData.PermanentSetUpRoadBlock(this);
        StartCoroutine(SetUpRoadBlock());

        DisableNearestObstacles();
    }

    public void RandomizeData()
    {
        int playersFirePointType = (int)PlayerHandler.playerInstance.truck.TruckData.firePointType;

        int randomFirePoint = Random.Range(playersFirePointType - 2, playersFirePointType + 3);
        if (randomFirePoint < (int)GameEnums.FirePointType.D_FPType)
        {
            randomFirePoint = (int)GameEnums.FirePointType.D_FPType;
        }
        if (randomFirePoint > (int)GameEnums.FirePointType.DCMP_FPType)
        {
            randomFirePoint = (int)GameEnums.FirePointType.DCMP_FPType;
        }
        roadBlockData.firePointType = (GameEnums.FirePointType)Random.Range(0, System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length);
        int[] gunDataTypes = { 11, 12, 13, 21, 22, 23, 31, 32, 33 };
        for (int i = 0; i < roadBlockData.firePointData.gunsConfigurations.Length; i++)
        {
            int randomGun = Random.Range(0, System.Enum.GetNames(typeof(GameEnums.Gun)).Length);
            int randomGunData = Random.Range(0, gunDataTypes.Length);
            roadBlockData.firePointData.gunsConfigurations[i].gunType = (GameEnums.Gun)randomGun;
            roadBlockData.firePointData.gunsConfigurations[i].gunDataType = (GameEnums.GunDataType)gunDataTypes[randomGunData];
        }
    }
    private void DisableNearestObstacles()
    {
        List<Collider> obstacles = new List<Collider>();
        for (int i = 0; i < 4; i++)
        {
            Collider[] obstaclesArray = Physics.OverlapSphere(transform.position + Vector3.right * i, 20f, 1 << 10);
            for (int j = 0; j < obstaclesArray.Length; j++)
            {
                obstacles.Add(obstaclesArray[j]);
            }
        }
        for (int i = 0; i < obstacles.Count; i++)
        {
            if(!ReferenceEquals(obstacles[i], gameObject.GetComponentInChildren<Collider>()))
            {
                obstacles[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        customUpdates.Add(this);
       
    }
    private void OnDisable()
    {
        customUpdates.Remove(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Truck other_truck = other.GetComponentInParent<Truck>();
        Enemy other_enemy = other.GetComponentInParent<Enemy>();

        if (!ReferenceEquals(other_truck, null))
        {
            StartCoroutine(SetTargets());
        }
        if (!ReferenceEquals(other_enemy, null))
        {
            StartCoroutine(EnemyStopAndLaunch(other_enemy));
        }
    }

    private IEnumerator EnemyStopAndLaunch(Enemy enemy)
    {
        StateMachine.State<Enemy> previousState = enemy.followTypeStateController.currentState;
        enemy.followTypeStateController.ChangeState(IdleState.Instance);
        yield return new WaitForSeconds(1f);
        enemy.followTypeStateController.ChangeState(previousState);
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
    }

    private void SetUpTargets()
    {
        targets = new TargetData[firePoint.gunsPoints.Count];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = new TargetData(null, null);
        }
    }

    public override void CustomUpdate()
    {
        if(!ReferenceEquals(firePoint, null))
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
            if (!ReferenceEquals(hits[i].GetComponentInParent<Truck>(), null))
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
        if (!ReferenceEquals(targets[randomTargetIndex].target_rigidbody, null))
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
            if (!ReferenceEquals(targets[i].target_rigidbody, null))
            {
                if (ReferenceEquals(targets[i].target_rigidbody.gameObject.activeInHierarchy, false))
                {
                    StartCoroutine(SetTargets());
                }
            }
            else if (ReferenceEquals(targets[i].target_rigidbody, null))
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
                    firePoint.RemoveGun(firePoint.gunsPoints[i].gunsLocation.GetChild(0).GetComponent<Gun>());
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
            expl.GetComponent<CachedParticles>().PlayParticles();
        }
        for (int i = 0; i < 10; i++)
        {
            GameObject expl = ObjectPoolersHolder.Instance.EffectPooler.Spawn("SmallExplosion",
                transform.position + new Vector3(Random.Range(-21, 21), Random.Range(0, 2),
                Random.Range(-3, 3)), Quaternion.identity);
            expl.GetComponent<CachedParticles>().PlayParticles();
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

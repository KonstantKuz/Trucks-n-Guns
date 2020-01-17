using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoCached
{
    public Truck truck { get; set; }
    public Transform seekPoint { get; set; }
    public Transform targetPoint { get; set; }
    #region NEWSYSTEM

    public Dictionary<GameEnums.TrackingGroup, TargetData> TrackingGroupsTargetsDictionary { get; private set; }

    public TargetData FirstTrackingGroupsTarget { get; set; }
    public TargetData SecondTrackingGroupTarget { get; set; }
    #endregion

    private Vector3 relativeToSeekPoint = Vector3.zero;
    private float steeringForce;
    private float movingForce;

    private List<EntityCondition> bumpedEnemies = new List<EntityCondition>(10);

    private const string EnemyTag = "Enemy";

    private float backwardBoostValue;

    private void OnEnable()
    {
        customUpdates.Add(this);
    }
    private void OnDisable()
    {
        customUpdates.Remove(this);
    }

    private void Awake()
    {
        truck = GetComponent<Truck>();
        truck.TruckData = PlayerStaticRunTimeData.playerTruckData;
        truck.SetUpTruck();

        FirstTrackingGroupsTarget = new TargetData(null,null);
        SecondTrackingGroupTarget = new TargetData(null, null);
        TrackingGroupsTargetsDictionary = new Dictionary<GameEnums.TrackingGroup, TargetData>(2);
        TrackingGroupsTargetsDictionary.Add(GameEnums.TrackingGroup.FirstTrackingGroup, FirstTrackingGroupsTarget);
        TrackingGroupsTargetsDictionary.Add(GameEnums.TrackingGroup.SecondTrackingGroup, SecondTrackingGroupTarget);
        steeringForce = 0;
        movingForce = 0;
        backwardBoostValue = 10f;

        int playerFPType = (int)truck.TruckData.firePointType;
        int playerTruck = (int)truck.TruckData.truckType;
        if (playerFPType == 0)
        {
            truck.trucksCondition.ResetCondition(30000 + (30000*playerTruck / 10));
        }
        if(playerFPType == 1)
        {
            truck.trucksCondition.ResetCondition(45000 + (45000 * playerTruck / 10));
        }
        if(playerFPType == 3)
        {
            truck.trucksCondition.ResetCondition(75000 + (75000 * playerTruck / 10));
        }
        if (playerFPType == 7)
        {
            truck.trucksCondition.ResetCondition(105000 + (105000 * playerTruck / 10));
        }
    }

    public void ForwardBoost()
    {
        truck.LaunchTruck();
        truck.SetBoost(25f);
    }
    public void BackwardBoost()
    {
        truck.LaunchTruck();
        truck.SetBoost(-10f);
    }
    public void StopPlayerTruck()
    {
        truck.StopTruck();
    }

    public void SetUpTargets(Rigidbody targetRigidbody, GameEnums.TrackingGroup trackingGroup)
    {
        TrackingGroupsTargetsDictionary[trackingGroup].target_rigidbody = targetRigidbody;
        TrackingGroupsTargetsDictionary[trackingGroup].target_condition = targetRigidbody.GetComponent<EntityCondition>();

        truck.firePoint.SetUpTargets(TrackingGroupsTargetsDictionary[trackingGroup], trackingGroup);
        StartListenTarget(trackingGroup);
    }

    private void StartListenTarget(GameEnums.TrackingGroup trackingGroup)
    {
        if (!ReferenceEquals(TrackingGroupsTargetsDictionary[trackingGroup].target_condition, null))
        {
            StopListenTargetCondition(trackingGroup);
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnZeroCondition += PlayerSessionHandler.IncreaseDefeatedEnemiesCount;
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnCurrentConditionChanged += PlayerSessionHandler.UpdateTargetConditionValue;
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnZeroCondition += delegate { StopListenTargetCondition(trackingGroup); };
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnZeroCondition += delegate { AddHealthForEnemyDestroy(TrackingGroupsTargetsDictionary[trackingGroup].target_condition.maxCondition); };

            GeneralGameUIHolder.Instance.otherUI.targetConditionValue.maxValue = TrackingGroupsTargetsDictionary[trackingGroup].target_condition.maxCondition;
            targetPoint.gameObject.SetActive(true);
        }
    }

    public void StopListenTargetCondition(GameEnums.TrackingGroup trackingGroup)
    {
        if (!ReferenceEquals(TrackingGroupsTargetsDictionary[trackingGroup].target_condition, null))
        {
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnZeroCondition -= delegate { AddHealthForEnemyDestroy(TrackingGroupsTargetsDictionary[trackingGroup].target_condition.maxCondition); };
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnZeroCondition -= delegate { StopListenTargetCondition(trackingGroup); };
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnCurrentConditionChanged -= PlayerSessionHandler.UpdateTargetConditionValue;
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnZeroCondition -= PlayerSessionHandler.IncreaseDefeatedEnemiesCount;
            targetPoint.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == EnemyTag)
        {
            EntityCondition bumpedConditionNow = collision.collider.GetComponentInParent<EntityCondition>();
            if (!ReferenceEquals(bumpedConditionNow, null))
            {
                if (!ReferenceEquals(bumpedConditionNow, FirstTrackingGroupsTarget.target_condition))
                {
                    StartListenBumpedEnemy(bumpedConditionNow);
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == EnemyTag)
        {
            EntityCondition bumpedConditionNow = collision.collider.GetComponentInParent<EntityCondition>();
            if (!ReferenceEquals(bumpedConditionNow, null))
            {
                if (!ReferenceEquals(bumpedConditionNow, FirstTrackingGroupsTarget.target_condition))
                {
                    StopListenBumpedEnemy(bumpedConditionNow);
                }
            }
        }
    }

    public void StartListenBumpedEnemy(EntityCondition bumpedConditionNow)
    {
        if(!bumpedEnemies.Contains(bumpedConditionNow))
        {
            bumpedEnemies.Add(bumpedConditionNow);
            bumpedConditionNow.OnZeroCondition += delegate { AddHealthForEnemyDestroy(bumpedConditionNow.maxCondition); };
            bumpedConditionNow.OnZeroCondition += PlayerSessionHandler.IncreaseDefeatedEnemiesCount;
            bumpedConditionNow.OnZeroCondition += delegate { StopListenBumpedEnemy(bumpedConditionNow); };
        }
    }

    public void StopListenBumpedEnemy(EntityCondition bumpedConditionNow)
    {
        bumpedEnemies.Remove(bumpedConditionNow);
        bumpedConditionNow.OnZeroCondition -= PlayerSessionHandler.IncreaseDefeatedEnemiesCount;
        bumpedConditionNow.OnZeroCondition -= delegate { AddHealthForEnemyDestroy(bumpedConditionNow.maxCondition); };
        bumpedConditionNow.OnZeroCondition -= delegate { StopListenBumpedEnemy(bumpedConditionNow); };
    }

    public void AddHealthForEnemyDestroy(float targetConditionMaxValue)
    {
        switch (PlayerSessionHandler.SessionComplexity)
        {
            case GameEnums.SessionComplexity.Low:
                truck.trucksCondition.AddHealth(targetConditionMaxValue * 0.2f);
                break;
            case GameEnums.SessionComplexity.Medium:
                truck.trucksCondition.AddHealth(targetConditionMaxValue * 0.05f);
                break;
            case GameEnums.SessionComplexity.High:
                truck.trucksCondition.AddHealth(targetConditionMaxValue * 0.005f);
                break;
        }
    }

    public void MovePlayerTruck()
    {
        truck.SetBoost(MovingForce());
        truck.Moving(MovingForce());
        truck.Steering(SteeringForce());
    }

    public float MovingForce()
    {
        backwardBoostValue -= 0.0015f;
        if (backwardBoostValue < 1f)
            backwardBoostValue = 1f;
        movingForce += 0.0002f;
        if (movingForce > 0.7f)
            movingForce = 0.7f;
        return movingForce;
    }

    public float SteeringForce()
    {
        relativeToSeekPoint = truck._transform.InverseTransformPoint(seekPoint.position);
        steeringForce = (relativeToSeekPoint.x / relativeToSeekPoint.magnitude);
        return steeringForce;
    }

    public override void CustomUpdate()
    {
        if(!ReferenceEquals(FirstTrackingGroupsTarget.target_rigidbody, null) && FirstTrackingGroupsTarget.target_rigidbody.gameObject.activeInHierarchy)
        {
            targetPoint.transform.position = FirstTrackingGroupsTarget.target_rigidbody.position;
            truck.firePoint.FirstTrackingAttack();
        }
    }
}

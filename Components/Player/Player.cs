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
    }

    public void ForwardBoost()
    {
        truck.LaunchTruck();
        truck.SetBoost(100f);
    }
    public void BackwardBoost()
    {
        truck.LaunchTruck();
        truck.SetBoost(-50f);
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

            GeneralGameUIHolder.Instance.otherUI.targetConditionValue.maxValue = TrackingGroupsTargetsDictionary[trackingGroup].target_condition.maxCondition;
            targetPoint.gameObject.SetActive(true);
        }
    }

    public void StopListenTargetCondition(GameEnums.TrackingGroup trackingGroup)
    {
        if (!ReferenceEquals(TrackingGroupsTargetsDictionary[trackingGroup].target_condition, null))
        {
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnZeroCondition -= delegate { StopListenTargetCondition(trackingGroup); };
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnCurrentConditionChanged -= PlayerSessionHandler.UpdateTargetConditionValue;
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnZeroCondition -= PlayerSessionHandler.IncreaseDefeatedEnemiesCount;
            targetPoint.gameObject.SetActive(false);
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
        movingForce += Time.deltaTime / 200;
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
        //truck.firePoint.StaticAttack();
        if(!ReferenceEquals(FirstTrackingGroupsTarget.target_rigidbody, null) && FirstTrackingGroupsTarget.target_rigidbody.gameObject.activeInHierarchy)
        {
            targetPoint.transform.position = FirstTrackingGroupsTarget.target_rigidbody.position;
            truck.firePoint.FirstTrackingAttack();
        }
        //if (!ReferenceEquals(SecondTrackingGroupTarget.target_rigidbody, null) && SecondTrackingGroupTarget.target_rigidbody.gameObject.activeInHierarchy)
        //{
        //    truck.firePoint.SecondTrackingAttack();
        //}
    }
}

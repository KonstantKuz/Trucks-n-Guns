using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoCached
{
    public Truck truck { get; set; }
    public Transform seekPoint { get; set; }

    #region NEWSYSTEM

    public Dictionary<GameEnums.TrackingGroup, TargetData> TrackingGroupsTargetsDictionary { get; private set; }

    public TargetData FirstTrackingGroupsTarget { get; set; }
    public TargetData SecondTrackingGroupTarget { get; set; }
    #endregion

    private Vector3 relativeToSeekPoint = Vector3.zero;
    private float newSteeringForce;

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

    }
   
    public void InjectPlayerIntoInput(InputHandler inputHandler)
    {
        inputHandler.player = this;
    }

    public void ForwardBoost()
    {
        truck.LaunchTruck();
        truck.SetBoost(250f);
    }
    public void BackwardBoost()
    {
        truck.LaunchTruck();
        truck.SetBoost(-200f);
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
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnZeroCondition += delegate { StopListenTargetCondition(trackingGroup); };
        }
    }

    public void StopListenTargetCondition(GameEnums.TrackingGroup trackingGroup)
    {
        if (!ReferenceEquals(TrackingGroupsTargetsDictionary[trackingGroup].target_condition, null))
        {
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnZeroCondition -= delegate { StopListenTargetCondition(trackingGroup); };
            TrackingGroupsTargetsDictionary[trackingGroup].target_condition.OnZeroCondition -= PlayerSessionHandler.IncreaseDefeatedEnemiesCount;
        }
    }

    public void MovePlayerTruck()
    {
        truck.Moving(1);
        truck.SetBoost(5);
        SteeringWheels();
    }

    public void SteeringWheels()
    {
        relativeToSeekPoint = truck._transform.InverseTransformPoint(seekPoint.position);
        newSteeringForce = (relativeToSeekPoint.x / relativeToSeekPoint.magnitude);
        truck.Steering(newSteeringForce);
    }

    public override void CustomUpdate()
    {
        truck.firePoint.StaticAttack();
        if(!ReferenceEquals(FirstTrackingGroupsTarget.target_rigidbody, null) && FirstTrackingGroupsTarget.target_rigidbody.gameObject.activeInHierarchy)
        {
            truck.firePoint.FirstTrackingAttack();
        }
        if (!ReferenceEquals(SecondTrackingGroupTarget.target_rigidbody, null) && SecondTrackingGroupTarget.target_rigidbody.gameObject.activeInHierarchy)
        {
            truck.firePoint.SecondTrackingAttack();
        }
    }
}

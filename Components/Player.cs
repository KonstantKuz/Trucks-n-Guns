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
        allTicks.Add(this);
    }
    private void OnDisable()
    {
        allTicks.Remove(this);
    }

    private void Awake()
    {
        truck = GetComponent<Truck>();
        truck.TruckData = PlayerStaticRunTimeData.playerTruckData;
        truck.SetUpTruck();

        #region NEWSYSTEM
        FirstTrackingGroupsTarget = new TargetData(null,null);
        SecondTrackingGroupTarget = new TargetData(null, null);
        #endregion
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
        StartListenTarget();
    }

    private void StartListenTarget()
    {
        if(!ReferenceEquals(TrackingGroupsTargetsDictionary[GameEnums.TrackingGroup.FirstTrackingGroup].target_rigidbody, null))
        {
            TrackingGroupsTargetsDictionary[GameEnums.TrackingGroup.FirstTrackingGroup].target_condition.OnZeroCondition -= StopListenTargetCondition;
            TrackingGroupsTargetsDictionary[GameEnums.TrackingGroup.FirstTrackingGroup].target_condition.OnZeroCondition -= PlayerHandler.IncreaseDefeatedEnemiesOnThisSession;


            TrackingGroupsTargetsDictionary[GameEnums.TrackingGroup.FirstTrackingGroup].target_condition =
                TrackingGroupsTargetsDictionary[GameEnums.TrackingGroup.FirstTrackingGroup].target_rigidbody.GetComponent<EntityCondition>();
            TrackingGroupsTargetsDictionary[GameEnums.TrackingGroup.FirstTrackingGroup].target_condition.OnZeroCondition += PlayerHandler.IncreaseDefeatedEnemiesOnThisSession;
            TrackingGroupsTargetsDictionary[GameEnums.TrackingGroup.FirstTrackingGroup].target_condition.OnZeroCondition += StopListenTargetCondition;
            Debug.Log($"Player starts listen to {TrackingGroupsTargetsDictionary[GameEnums.TrackingGroup.FirstTrackingGroup].target_rigidbody.name}");
        }
    }

    public void StopListenTargetCondition()
    {
        TrackingGroupsTargetsDictionary[GameEnums.TrackingGroup.FirstTrackingGroup].target_condition.OnZeroCondition -= StopListenTargetCondition;
        TrackingGroupsTargetsDictionary[GameEnums.TrackingGroup.FirstTrackingGroup].target_condition.OnZeroCondition -= PlayerHandler.IncreaseDefeatedEnemiesOnThisSession;
        Debug.Log($"Player stops listen to {TrackingGroupsTargetsDictionary[GameEnums.TrackingGroup.FirstTrackingGroup].target_rigidbody.name}");
    }
    public void MovePlayerTruck()
    {
        truck.MoveTruck(1);

        SteeringWheels();
    }

    public void SteeringWheels()
    {
        relativeToSeekPoint = truck._transform.InverseTransformPoint(seekPoint.position);
        newSteeringForce = (relativeToSeekPoint.x / relativeToSeekPoint.magnitude);
        truck.SteeringWheels(newSteeringForce);
    }

    public override void OnTick()
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

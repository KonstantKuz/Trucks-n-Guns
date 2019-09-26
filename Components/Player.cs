using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoCached
{
    public Transform seekPoint;

    public Truck truck { get; set; }

    #region NEWSYSTEM

    public Dictionary<GameEnums.TrackingGroup, TargetData> TrackingGroupsTargetsDictionary { get; private set; }

    public TargetData FirstTrackingGroupsTarget { get; set; }
    public TargetData SecondTrackingGroupTarget { get; set; }
    #endregion

    private void Awake()
    {
        truck = GetComponent<Truck>();
        truck.SetUpTruck();

        #region NEWSYSTEM
        FirstTrackingGroupsTarget = new TargetData(null);
        SecondTrackingGroupTarget = new TargetData(null);
        #endregion
        TrackingGroupsTargetsDictionary = new Dictionary<GameEnums.TrackingGroup, TargetData>(2);
        TrackingGroupsTargetsDictionary.Add(GameEnums.TrackingGroup.FirstTrackingGroup, FirstTrackingGroupsTarget);
        TrackingGroupsTargetsDictionary.Add(GameEnums.TrackingGroup.SecondTrackingGroup, SecondTrackingGroupTarget);
    }
    private void OnEnable()
    {
        allTicks.Add(this);
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
        truck.firePoint.SetUpTargets(TrackingGroupsTargetsDictionary[trackingGroup], trackingGroup);
    }
    public void MovePlayerTruck(float steeringForce)
    {
        if(steeringForce!=0)
        {
            truck.MoveTruck(1 - Mathf.Abs(steeringForce));
        }
        SteeringWheels();
    }

    public void SteeringWheels()
    {
        Vector3 relativeToSeekPoint = truck._transform.InverseTransformPoint(seekPoint.position);
        float newsteer = (relativeToSeekPoint.x / relativeToSeekPoint.magnitude);
        truck.SteeringWheels(newsteer);
    }

    public override void OnTick()
    {
        truck.firePoint.StaticAttack();

        if(FirstTrackingGroupsTarget.target_rigidbody != null)
        {
            if(FirstTrackingGroupsTarget.target_rigidbody.gameObject.activeInHierarchy == true)
            {
                truck.firePoint.FirstTrackingAttack();
            }
        }
        if (SecondTrackingGroupTarget.target_rigidbody != null)
        {
            if (SecondTrackingGroupTarget.target_rigidbody.gameObject.activeInHierarchy == true)
            {
                truck.firePoint.SecondTrackingAttack();
            }
        }
    }
}

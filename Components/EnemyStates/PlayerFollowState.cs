using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class PlayerFollowState : State<Enemy>
{
    public static PlayerFollowState _instance;


    private PlayerFollowState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static PlayerFollowState Instance
    {
        get
        {
            if (_instance == null)
                new PlayerFollowState();

            return _instance;
        }
    }

    public override void EnterState(Enemy _owner)
    {      
    }

    public override void ExitState(Enemy _owner)
    {
    }

    public override void UpdateState(Enemy _owner)
    {
        _owner.truck.MoveTruck(MovingForce(_owner.targetData, _owner.truck));
        _owner.truck.SteeringWheels(PlayerFollowSteeringForce(_owner.targetData, _owner.truck._transform) + _owner.AvoidForce());
        _owner.truck.firePoint.FirstTrackingAttack();
        _owner.truck.firePoint.StaticAttack();
    }

    public float PlayerFollowSteeringForce(TargetData targetData, Transform enemy_transform)
    {
        Vector3 relativeToPlayer = enemy_transform.InverseTransformPoint(targetData.target_rigidbody.position);
        float newsteer = (relativeToPlayer.x / relativeToPlayer.magnitude);
        return newsteer;
    }

    public float MovingForce(TargetData targetData, Truck truck)
    {
        float movingForce = 0;
        float boostForce = 0;
        float distanceToTarget = targetData.target_rigidbody.position.z - truck._transform.position.z;

        movingForce = distanceToTarget * 0.1f;

        if (distanceToTarget < 0f)
        {
            if (truck.CurrentSpeed() > 25f)
            {
                movingForce = -2f;
            }
            else
            {
                movingForce = 0;
            }

            boostForce = -truck._rigidbody.velocity.magnitude / truck._rigidbody.mass;
        }
        else if (distanceToTarget > 50f)
        {
            movingForce = distanceToTarget * 0.1f - truck._rigidbody.velocity.magnitude * 0.05f;
            boostForce = truck._rigidbody.velocity.magnitude / truck._rigidbody.mass;
        }
        else if (distanceToTarget < -5f)
        {
            truck.StopTruck();
        }

        truck.SetBoost(boostForce);

        return movingForce;
    }
}

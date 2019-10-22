using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using Pathfinding;

public class PathFollowState : State<Enemy>
{
    public static PathFollowState _instance;


    private PathFollowState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static PathFollowState Instance
    {
        get
        {
            if (_instance == null)
                new PathFollowState();

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
        _owner.truck.MoveTruck(MovingForce(_owner));
        _owner.truck.SteeringWheels(PathFollowSteeringForce(_owner) + _owner.AvoidForce());
    }

    public float MovingForce(Enemy _owner)
    {
        float movingForce = 0;
        float distanceToTarget = _owner.targetData.target_rigidbody.position.z - _owner.truck._transform.position.z;
        float targetForwardVelocity = _owner.targetData.target_rigidbody.velocity.z;

        if (distanceToTarget < 0)
        {
            if(targetForwardVelocity>0)
            {
                _owner.truck.StopTruckSlow(distanceToTarget * 0.00005f);
                movingForce = 1;
            }
            else
            {
                _owner.truck.StopTruckSlow(distanceToTarget * 0.00005f);
                movingForce = 0;
            }
        }
        else
        {
            _owner.truck.LaunchTruck();
            movingForce = distanceToTarget*targetForwardVelocity*0.005f;
        }

        movingForce += distanceToTarget * targetForwardVelocity* 0.00005f;
        return movingForce;
    }

    public float PathFollowSteeringForce(Enemy _owner)
    {

        if (_owner.PathCheck())
        {
            float currentSpeedClamped = _owner.truck.CurrentSpeed();

            currentSpeedClamped = Mathf.Clamp(currentSpeedClamped, 0, 10);

            if ((_owner.currentNode.worldPosition.z - _owner.truck._transform.position.z) <= currentSpeedClamped)
            {
                _owner.targetIndex++;
                if (_owner.targetIndex > _owner.path.Count - 1)
                    return 0;
                else
                {
                    _owner.currentNode = _owner.path[_owner.targetIndex];
                    _owner.path.Remove(_owner.path[_owner.targetIndex--]);
                }
            }

            Vector3 relativeToCurrentNode = _owner.truck._transform.InverseTransformPoint(_owner.currentNode.worldPosition);
            float newsteer = (relativeToCurrentNode.x / relativeToCurrentNode.magnitude);
            return newsteer;

        }
        else return 0;
    }
}

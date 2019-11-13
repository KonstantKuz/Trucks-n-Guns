using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class PlayerOvertakingState : State<Enemy>
{
    public static PlayerOvertakingState _instance;


    private PlayerOvertakingState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static PlayerOvertakingState Instance
    {
        get
        {
            if (_instance == null)
                new PlayerOvertakingState();

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
        _owner.truck.Moving(MovingForce(_owner));
        _owner.truck.Steering(SteeringForceOnDistanceBased(_owner) + _owner.AvoidForce());
    }


    public float SteeringForceOnDistanceBased(Enemy _owner)
    {
        float distanceToTarget = _owner.targetData.target_rigidbody.position.z - _owner.truck._transform.position.z;
        if (distanceToTarget > -5 || distanceToTarget < -15)
        {
            return PathFollowSteeringForce(_owner);
        }
        else
        {
            return PlayerOverTakingSteeringForce(_owner);
        }
    }

    public float PlayerOverTakingSteeringForce(Enemy _owner)
    {
        Vector3 relativeToPlayer = _owner.truck._transform.InverseTransformPoint(_owner.targetData.target_rigidbody.position + 10 * _owner.truck._transform.forward);
        float newsteer = (relativeToPlayer.x / relativeToPlayer.magnitude);
        return newsteer;
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
    public float MovingForce(Enemy _owner)
    {
        float movingForce = 0;
        float distanceToTarget = (_owner.targetData.target_rigidbody.position.z + 10 * _owner.truck._transform.forward.z) - _owner.truck._transform.position.z;
        float targetForwardVelocity = _owner.targetData.target_rigidbody.velocity.z;

        if (distanceToTarget < -15)
        {
            if (targetForwardVelocity > 0)
            {
                _owner.truck.StopTruck(distanceToTarget * 0.00005f * targetForwardVelocity);
                movingForce = 1;
            }
            else
            {
                _owner.truck.StopTruck(distanceToTarget * 0.00005f);
                movingForce = 0;
            }
        }
        else
        {
            _owner.truck.LaunchTruck();
            movingForce = distanceToTarget * targetForwardVelocity * 0.005f;
        }
        return movingForce;
    }
}

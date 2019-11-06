using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class PlayerFollowState : State<Enemy>
{
    private float movingForce, distanceToTarget, targetForwardVelocity;
    private float currentSpeedClamped, newSteeringForce;
    private Vector3 relativeToCurrentNode, relativeToPlayer;

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
        _owner.truck.MoveTruck(MovingForce(_owner));
        _owner.truck.SteeringWheels(SteeringForceOnDistanceBased(_owner) + _owner.AvoidForce());
    }

    
    public float SteeringForceOnDistanceBased(Enemy _owner)
    {
        distanceToTarget = _owner.targetData.target_rigidbody.position.z - _owner.truck._transform.position.z;
        if(distanceToTarget > 30 || distanceToTarget < 0)
        {
            return PathFollowSteeringForce(_owner);
        }
        else
        {
            return PlayerFollowSteeringForce(_owner);
        }
    }

    public float PlayerFollowSteeringForce(Enemy _owner)
    {
        relativeToPlayer = _owner.truck._transform.InverseTransformPoint(_owner.targetData.target_rigidbody.position - 10 * _owner.truck._transform.forward);
        newSteeringForce = (relativeToPlayer.x / relativeToPlayer.magnitude);
        return newSteeringForce;
    }

    public float PathFollowSteeringForce(Enemy _owner)
    {
        if (_owner.PathCheck())
        {
            currentSpeedClamped = _owner.truck.CurrentSpeed();

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

            relativeToCurrentNode = _owner.truck._transform.InverseTransformPoint(_owner.currentNode.worldPosition);
            newSteeringForce = (relativeToCurrentNode.x / relativeToCurrentNode.magnitude);
            return newSteeringForce;

        }
        else return 0;
    }
    public float MovingForce(Enemy _owner)
    {
        movingForce = 0;
        distanceToTarget = (_owner.targetData.target_rigidbody.position.z - 10 * _owner.truck._transform.forward.z) - _owner.truck._transform.position.z;
        targetForwardVelocity = _owner.targetData.target_rigidbody.velocity.z;

        if (distanceToTarget < 5)
        {
            _owner.truck.StopTruck(-(distanceToTarget - targetForwardVelocity) * 0.005f);
        }
        else
        {
            _owner.truck.LaunchTruck();

            movingForce = distanceToTarget * Mathf.Abs(targetForwardVelocity) * 0.005f;
        }
        return movingForce;
    }
}

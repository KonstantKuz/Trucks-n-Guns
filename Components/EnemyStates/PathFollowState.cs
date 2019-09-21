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
        _owner.truck.MoveTruck(MovingForce(_owner.targetData, _owner.truck));
        _owner.truck.SteeringWheels(PathFollowSteeringForce(_owner) + _owner.AvoidForce());
        _owner.truck.firePoint.FirstTrackingAttack();
        _owner.truck.firePoint.StaticAttack();
    }

    public float MovingForce(TargetData targetData, Truck truck)
    {
        float movingForce = 0;
        float boostForce = 0;
        float distanceToTarget = targetData.target_rigidbody.position.z - truck._transform.position.z;

        movingForce = distanceToTarget * 0.2f;

        if (distanceToTarget < 0f)
        {
                movingForce = 0;
        }
        else if (distanceToTarget > 50f)
        {
            movingForce = distanceToTarget * 0.1f - truck._rigidbody.velocity.magnitude * 0.05f;
            Debug.Log(movingForce);
            boostForce = distanceToTarget * 0.1f;
        }
        else if (distanceToTarget < -5f)
        {
            truck.StopTruck();
        }

        truck.SetBoost(boostForce);

        return movingForce;
    }

    public float PathFollowSteeringForce(Enemy _owner)
    {
        Node currentNode = _owner.currentNode;
        int targetIndex = _owner.targetIndex;
        List<Node> path = _owner.path;
        Transform _ownerTruck = _owner.truck._transform;

        if (_owner.PathCheck())
        {
            if ((currentNode.worldPosition.z - _ownerTruck.position.z) <= 1)
            {
                targetIndex++;
                if (targetIndex > path.Count - 1)
                    return 0;
                else
                {
                    _owner.currentNode = path[targetIndex];
                    path.Remove(path[targetIndex--]);
                }
            }

            Vector3 relativeToCurrentNode = _ownerTruck.InverseTransformPoint(currentNode.worldPosition);
            float newsteer = (relativeToCurrentNode.x / relativeToCurrentNode.magnitude);
            return newsteer;

        }
        else return 0;

    }
}

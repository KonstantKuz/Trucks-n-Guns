using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using Pathfinding;

public class SingleTestState : State<Enemy>
{
    public static SingleTestState _instance;


    private SingleTestState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static SingleTestState Instance
    {
        get
        {
            if (_instance == null)
                new SingleTestState();

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
        _owner.truck.MoveTruck(1);
        _owner.truck.SteeringWheels(PathFollowSteeringForce(_owner) + _owner.AvoidForce());
    }

    public float PathFollowSteeringForce(Enemy _owner)
    {
        
        if (_owner.PathCheck())
        {
            float currentSpeedClamped = _owner.truck.CurrentSpeed();

            currentSpeedClamped = Mathf.Clamp(currentSpeedClamped, 0, 10);

            //Debug.Log(currentSpeedClamped);
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

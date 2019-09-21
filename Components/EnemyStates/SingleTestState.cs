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
        Node currentNode = _owner.currentNode;
        int targetIndex = _owner.targetIndex;
        List<Node> path = _owner.path;
        Transform _ownerTruck = _owner.truck._transform;

        if (_owner.PathCheck())
        {
            if ((currentNode.worldPosition.z - _ownerTruck.position.z) <= _owner.truck.CurrentSpeed() / 10)
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

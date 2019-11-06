using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using Pathfinding;

public class IdleState : State<Enemy>
{
    public static IdleState _instance;

    private IdleState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static IdleState Instance
    {
        get
        {
            if (_instance == null)
                new IdleState();

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
        _owner.truck.StopTruck();
    }

}

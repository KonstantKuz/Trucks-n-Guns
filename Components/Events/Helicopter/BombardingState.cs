using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class BombardingState : State<Helicopter>
{
    public static BombardingState _instance;

    private BombardingState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static BombardingState Instance
    {
        get
        {
            if (_instance == null)
                new BombardingState();

            return _instance;
        }
    }

    public override void EnterState(Helicopter _owner)
    {

    }

    public override void ExitState(Helicopter _owner)
    {

    }

    public override void UpdateState(Helicopter _owner)
    {

    }
}

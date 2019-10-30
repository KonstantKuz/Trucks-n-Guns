using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class FollowingState : State<Helicopter>
{
    public static FollowingState _instance;

    private FollowingState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static FollowingState Instance
    {
        get
        {
            if (_instance == null)
                new FollowingState();

            return _instance;
        }
    }
    
    public override void EnterState(Helicopter _owner)
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState(Helicopter _owner)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(Helicopter _owner)
    {
        throw new System.NotImplementedException();
    }
}

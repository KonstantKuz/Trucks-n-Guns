using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class SettingsState : State<MenuHandler>
{
    public static SettingsState _instance;

    private SettingsState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static SettingsState Instance
    {
        get
        {
            if (_instance == null)
                new SettingsState();

            return _instance;
        }
    }

    public override void EnterState(MenuHandler _owner)
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState(MenuHandler _owner)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(MenuHandler _owner)
    {
        throw new System.NotImplementedException();
    }
}

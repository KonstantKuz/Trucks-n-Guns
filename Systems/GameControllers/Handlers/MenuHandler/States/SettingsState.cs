using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using UnityEngine.UI;

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
        _owner.settings.SettingsWindow.SetActive(true);

        _owner.BackButton.SetActive(true);
        _owner.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _owner.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToMain(_owner));
    }

    public override void ExitState(MenuHandler _owner)
    {
        _owner.settings.SettingsWindow.SetActive(false);

        _owner.BackButton.SetActive(false);
    }

    public override void UpdateState(MenuHandler _owner)
    {
        throw new System.NotImplementedException();
    }

    public void BackToMain(MenuHandler _owner)
    {
        _owner.menu.ChangeState(MainMenuState.Instance);
    }
}

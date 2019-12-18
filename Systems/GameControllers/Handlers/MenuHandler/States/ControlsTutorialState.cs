using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using UnityEngine.UI;

public class ControlsTutorialState : State<MenuHandler>
{
    public static ControlsTutorialState _instance;

    private ControlsTutorialState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static ControlsTutorialState Instance
    {
        get
        {
            if (_instance == null)
                new ControlsTutorialState();

            return _instance;
        }
    }

    public override void EnterState(MenuHandler _owner)
    {
        _owner.controlsTutorial.controlsTutprialWindow.SetActive(true);

        _owner.BackButton.SetActive(true);
        _owner.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _owner.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToMainMenu(_owner));
    }

    public override void ExitState(MenuHandler _owner)
    {
        _owner.controlsTutorial.controlsTutprialWindow.SetActive(false);

        _owner.BackButton.SetActive(false);
    }

    public override void UpdateState(MenuHandler _owner)
    {
        throw new System.NotImplementedException();
    }

    public void BackToMainMenu(MenuHandler _owner)
    {
        _owner.menu.ChangeState(MainMenuState.Instance);
    }
}

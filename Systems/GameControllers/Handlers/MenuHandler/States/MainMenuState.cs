using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using UnityEngine.UI;
public class MainMenuState : State<MenuHandler>
{
    public static MainMenuState _instance;

    private MainMenuState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static MainMenuState Instance
    {
        get
        {
            if (_instance == null)
                new MainMenuState();

            return _instance;
        }
    }
   
    public override void EnterState(MenuHandler _owner)
    {
        PlayerStaticRunTimeData.LoadData();

        _owner.mainMenu.MainMenuWindow.SetActive(true);

        _owner.mainMenu.PlayButton.GetComponent<Button>().onClick.AddListener(() => PlayGeneralGame(_owner));
        _owner.mainMenu.PlayButton.GetComponent<Button>().onClick.AddListener(() => GoogleAdmobHandler.Instance.ShowInterstitialAd());

        _owner.mainMenu.CustomizeButton.GetComponent<Button>().onClick.AddListener(() => OpenCustomization(_owner));
        _owner.mainMenu.SettingsButton.GetComponent<Button>().onClick.AddListener(() => OpenSettings(_owner));
        _owner.mainMenu.ViewStatisticsButton.GetComponent<Button>().onClick.AddListener(() => OpenStatistics(_owner));
        _owner.mainMenu.QuitButton.GetComponent<Button>().onClick.AddListener(() => CloseGame());

        _owner.mainMenu.SendFeedbackButton.GetComponent<Button>().onClick.AddListener(() => OpenFeedback(_owner));
        _owner.mainMenu.ControlsTutorialButton.GetComponent<Button>().onClick.AddListener(() => OpenControlsTutorial(_owner));

        _owner.mainMenu.LogInButton.GetComponent<Button>().onClick.AddListener(() => GooglePlayServicesHandler.Instance.LogIn());
    }

    public override void ExitState(MenuHandler _owner)
    {
        _owner.mainMenu.MainMenuWindow.SetActive(false);
        _owner.mainMenu.PlayButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _owner.mainMenu.CustomizeButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _owner.mainMenu.SettingsButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _owner.mainMenu.ViewStatisticsButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _owner.mainMenu.QuitButton.GetComponent<Button>().onClick.RemoveAllListeners();

        _owner.mainMenu.SendFeedbackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _owner.mainMenu.ControlsTutorialButton.GetComponent<Button>().onClick.RemoveAllListeners();

        _owner.mainMenu.LogInButton.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public override void UpdateState(MenuHandler _owner)
    {
        throw new System.NotImplementedException();
    }

    public void PlayGeneralGame(MenuHandler _owner)
    {
        PlayerStaticDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, new PlayerSessionData(0, 0, 0));
        WarningWindow.Instance.ShowWarning(WarningStrings.LoadGeneralGameWarning());
        MenuHandler.Instance.ActivateGeneralGameScene();
    }

    public void OpenCustomization(MenuHandler _owner)
    {
        _owner.menu.ChangeState(CustomizationState.Instance);
    }

    public void OpenSettings(MenuHandler _owner)
    {
        _owner.menu.ChangeState(SettingsState.Instance);
    }

    public void OpenStatistics(MenuHandler _owner)
    {
        _owner.menu.ChangeState(StatisticsState.Instance);
    }

    public void OpenFeedback(MenuHandler _owner)
    {
        _owner.menu.ChangeState(FeedbackSendState.Instance);
    }

    public void OpenControlsTutorial(MenuHandler _owner)
    {
        _owner.menu.ChangeState(ControlsTutorialState.Instance);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}

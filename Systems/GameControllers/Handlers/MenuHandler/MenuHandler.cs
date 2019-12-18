using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using Singleton;
using UnityEngine.UI;

[System.Serializable]
public class MainMenu
{
    public GameObject MainMenuWindow;

    public GameObject PlayButton;
    public GameObject CustomizeButton;
    public GameObject ViewStatisticsButton;
    public GameObject SettingsButton;
    public GameObject LogInButton;

    public GameObject SendFeedbackButton;
    public GameObject ControlsTutorialButton;

    public GameObject QuitButton;
}
[System.Serializable]
public class Customization
{
    public GameObject CustomizationWindow;

    public GameObject TruckSectionWindow;
    public GameObject TrucksSectionButton;
    public GameObject ChangeTruckButton;
    public GameObject UpgradeFirePointButton;

    public GameObject GunSectionWindow;
    public GameObject GunSectionButton;
    public GameObject SelectGunButtonPrefab;
    public GameObject[] selectGunButtons { get; set; }
    public GameObject ChangeGunButton;
    public GameObject RateOfFireStat;
    public GameObject DamageStat;
    public GameObject TargetingSpeedStat;
    public GameObject GunInfoWindow;

    public GameObject coins;
    public GameObject experience;

    public GameObject BuyButton;
}
[System.Serializable]
public class Statistics
{
    public GameObject StatisticsWindow;
}
[System.Serializable]
public class Settings
{
    public GameObject SettingsWindow;
}
[System.Serializable]
public class Feedback
{
    public GameObject FeedbackWindow;
    public FeedbackQuestion[] QuickQuestions;
    public GameObject SendButton;
    public GameObject GoToGoogleButton;
    public Text senderReview;
    public Text senderDevice;
    public Text senderMail_Adress;
}
[System.Serializable]
public class ControlsTutorial
{
    public GameObject controlsTutprialWindow;
}

public class MenuHandler : Singleton<MenuHandler>
{
    public StateMachine<MenuHandler> menu;
    public MainMenu mainMenu;
    public Customization customization;
    public Statistics statistics;
    public Settings settings;

    public Feedback feedBack;
    public ControlsTutorial controlsTutorial;

    public GameObject BackButton;

    [HideInInspector]
    public Vector3 cameraStartPosition, cameraStartRotation;

    public AsyncOperation asyncGeneralGameLoad;

    private void OnEnable()
    {
        menu = new StateMachine<MenuHandler>(this);

        menu.ChangeState(MainMenuState.Instance);

        cameraStartPosition = Camera.main.transform.position;
        cameraStartRotation = Camera.main.transform.eulerAngles;

        GameObject.Find("PlayerTruckPreset(Clone)").GetComponent<Rigidbody>().isKinematic = true;

        StartCoroutine(AsyncGeneralGameSceceLoad());
    }

    private IEnumerator AsyncGeneralGameSceceLoad()
    {
        yield return new WaitForSecondsRealtime(1f);
        asyncGeneralGameLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GeneralGameState");
        asyncGeneralGameLoad.allowSceneActivation = false;
    }

    public void ActivateGeneralGameScene()
    {
        StopCoroutine(StartGeneralGame());
        StartCoroutine(StartGeneralGame());
    }

    private IEnumerator StartGeneralGame()
    {
        if(!ReferenceEquals(asyncGeneralGameLoad, null))
        {
            asyncGeneralGameLoad.allowSceneActivation = true;
            yield return null;
        }
        else
        {
            yield return new WaitForSecondsRealtime(1f);
            StartCoroutine(StartGeneralGame());
        }
    }
}

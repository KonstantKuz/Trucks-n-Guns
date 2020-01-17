using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSessionHandler : MonoBehaviour
{
    public static GameEnums.SessionComplexity SessionComplexity { get; private set; }

    private static PlayerSessionData currentSessionData;
    private CurrentSessionTaskData taskToSession;

    private Vector3 startPostion, endPosition;
    private int startTime, endTime;

    private AsyncOperation asyncLoad;

    public void StartHandleSession()
    {
        //Time.timeScale = 0;

        taskToSession = DataReturnersHolder.Instance.TaskDataReturner.GetRandomData() as CurrentSessionTaskData;

        taskToSession.IsDoneToFalse();

        GeneralGameUIHolder.Instance.windows.taskWindow.ShowTaskToSession(taskToSession, delegate { StartSession(); });
    }

    
    public void StartSession()
    {
        //Time.timeScale = 2;

        currentSessionData = new PlayerSessionData(0, 0, 0);
        StartCoroutine(lateUIupdate());

        GeneralGameUIHolder.Instance.otherUI.targetConditionValue = PlayerHandler.PlayerInstance.targetPoint.GetComponentInChildren<Slider>();
        GeneralGameUIHolder.Instance.otherUI.targetConditionValue.minValue = 0;


        GeneralGameUIHolder.Instance.otherUI.changeCameraBehaviour.onClick.RemoveAllListeners();
        GeneralGameUIHolder.Instance.otherUI.changeCameraBehaviour.onClick.AddListener(() => ChangeCameraBehaviour());

        GeneralGameUIHolder.Instance.otherUI.restartButton.onClick.RemoveAllListeners();
        GeneralGameUIHolder.Instance.otherUI.restartButton.onClick.AddListener(() => Restart());

        GeneralGameUIHolder.Instance.otherUI.returnToMenuButton.onClick.RemoveAllListeners();
        GeneralGameUIHolder.Instance.otherUI.returnToMenuButton.onClick.AddListener(() => BackToMainMenu());

        PlayerHandler.PlayerInstance.truck.trucksCondition.OnZeroCondition += SaveCurrentSessionData;
        UpdatePlayerConditionValueAndSessionComplexity();
        PlayerHandler.PlayerInstance.truck.trucksCondition.OnCurrentConditionChanged += UpdatePlayerConditionValueAndSessionComplexity;

        startPostion = PlayerHandler.PlayerInstance.truck._transform.position;
        startTime = (int)Time.time;

        StartCoroutine(WriteTimeTraveledDistances());
    }

    private IEnumerator lateUIupdate()
    {
        yield return new WaitForSecondsRealtime(1f);

        GeneralGameUIHolder.Instance.otherUI.playerConditionValue.minValue = 0;
        GeneralGameUIHolder.Instance.otherUI.playerConditionValue.maxValue = PlayerHandler.PlayerInstance.truck.trucksCondition.maxCondition;
        GeneralGameUIHolder.Instance.otherUI.playerConditionValue.value = PlayerHandler.PlayerInstance.truck.trucksCondition.currentCondition;
        GeneralGameUIHolder.Instance.otherUI.playerConditionValue.GetComponentInChildren<Text>().text = "";
    }

    public void ChangeCameraBehaviour()
    {
        PlayerHandler.staticCamera = !PlayerHandler.staticCamera;
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GeneralGameState");
    }

    public void BackToMainMenu()
    {
        SaveCurrentSessionData();
    }

    private IEnumerator WriteTimeTraveledDistances()
    {
        StartCoroutine(oneMin());
        StartCoroutine(threeMin());
        StartCoroutine(fiveMin());
        yield return null;
    }
    private IEnumerator oneMin()
    {
        yield return new WaitForSecondsRealtime(60);
        endPosition = PlayerHandler.PlayerInstance.truck._transform.position;
        currentSessionData.traveledDistance_1Minute = (int)(endPosition.z - startPostion.z);
    }
    private IEnumerator threeMin()
    {
        yield return new WaitForSecondsRealtime(180);
        endPosition = PlayerHandler.PlayerInstance.truck._transform.position;
        currentSessionData.traveledDistance_3Minutes = (int)(endPosition.z - startPostion.z);
    }
    private IEnumerator fiveMin()
    {
        yield return new WaitForSecondsRealtime(300);
        endPosition = PlayerHandler.PlayerInstance.truck._transform.position;
        currentSessionData.traveledDistance_5Minutes = (int)(endPosition.z - startPostion.z);
    }

    private void UpdatePlayerConditionValueAndSessionComplexity()
    {
        GeneralGameUIHolder.Instance.otherUI.playerConditionValue.value = PlayerHandler.PlayerInstance.truck.trucksCondition.currentCondition;
        //GeneralGameUIHolder.Instance.otherUI.playerConditionValue.GetComponentInChildren<Text>().text = GeneralGameUIHolder.Instance.otherUI.playerConditionValue.value.ToString();

        float traveledDistance = CurrentSessionProgress().traveledDistance;

        if(traveledDistance <= 5000)
        {
            SessionComplexity = GameEnums.SessionComplexity.Low;
        }
        else if(traveledDistance > 5000 && traveledDistance < 10000)
        {
            SessionComplexity = GameEnums.SessionComplexity.Medium;
        }
        else
        {
            SessionComplexity = GameEnums.SessionComplexity.High;
        }
    }

    public static void UpdateTargetConditionValue()
    {
        GeneralGameUIHolder.Instance.otherUI.targetConditionValue.value = PlayerHandler.PlayerInstance.FirstTrackingGroupsTarget.target_condition.currentCondition;
    }

    public static void IncreaseDefeatedEnemiesCount()
    {
        currentSessionData.defeatedEnemies++;
        GeneralGameUIHolder.Instance.otherUI.defeatedEnemiesCount.text = currentSessionData.defeatedEnemies.ToString();
        GeneralGameUIHolder.Instance.otherUI.traveledDistanceCount.text = currentSessionData.traveledDistance.ToString();
    }

    public void SaveCurrentSessionData()
    {
        PlayerHandler.PlayerInstance.StopListenTargetCondition(GameEnums.TrackingGroup.FirstTrackingGroup);
        PlayerHandler.PlayerInstance.truck.trucksCondition.OnCurrentConditionChanged -= UpdatePlayerConditionValueAndSessionComplexity;
        PlayerHandler.PlayerInstance.truck.trucksCondition.OnZeroCondition -= SaveCurrentSessionData;

        taskToSession.CheckSession(CurrentSessionProgress());

        PlayerStaticRunTimeData.coins += taskToSession.totalReward;
        PlayerStaticDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, currentSessionData);

        ShowDoneTasks();
    }

    public PlayerSessionData CurrentSessionProgress()
    {
        endPosition = PlayerHandler.PlayerInstance.truck._transform.position;
        endTime = (int)Time.time;

        currentSessionData.traveledDistance = (int)(endPosition.z - startPostion.z);
        currentSessionData.traveledTime = endTime - startTime;

        return currentSessionData;
    }

    public void ShowDoneTasks()
    {
        StartCoroutine(LoadCustomiz());
        GeneralGameUIHolder.Instance.windows.taskWindow.ShowSessionProgress(currentSessionData, taskToSession, delegate { ActivateScene(); });
    }

    private void ActivateScene()
    {
        if(!ReferenceEquals(asyncLoad,null))
        {
            asyncLoad.allowSceneActivation = true;
        }
    }

    private IEnumerator LoadCustomiz()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenuWithCustomization");
        asyncLoad.allowSceneActivation = false;
    }
}

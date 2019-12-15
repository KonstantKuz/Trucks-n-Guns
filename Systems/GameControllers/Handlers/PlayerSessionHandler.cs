using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSessionHandler : MonoCached
{
    private static PlayerSessionData currentSessionData;
    private CurrentSessionTaskData taskToSession;

    private Vector3 startPostion, endPosition;
    private int startTime, endTime;

    public void StartHandleSession()
    {
        Time.timeScale = 0;

        taskToSession = DataReturnersHolder.Instance.TaskDataReturner.GetRandomData() as CurrentSessionTaskData;

        taskToSession.IsDoneToFalse();

        GeneralGameUIHolder.Instance.windows.taskWindow.ShowTaskToSession(taskToSession, delegate { StartSession(); });
    }

    
    public void StartSession()
    {
        Time.timeScale = 2;

        currentSessionData = new PlayerSessionData(0, 0, 0);

        //playerConditionValue = GameObject.Find("PlayerHealth").GetComponent<Slider>();
        GeneralGameUIHolder.Instance.otherUI.playerConditionValue.minValue = 0;
        GeneralGameUIHolder.Instance.otherUI.playerConditionValue.maxValue = PlayerHandler.PlayerInstance.truck.TruckData.maxTrucksCondition;
        //taskWindow = GameObject.Find("TaskWindow");

        GeneralGameUIHolder.Instance.otherUI.targetConditionValue = PlayerHandler.PlayerInstance.targetPoint.GetComponentInChildren<Slider>();
        GeneralGameUIHolder.Instance.otherUI.targetConditionValue.minValue = 0;


        GeneralGameUIHolder.Instance.otherUI.changeCameraBehaviour.onClick.RemoveAllListeners();
        GeneralGameUIHolder.Instance.otherUI.changeCameraBehaviour.onClick.AddListener(() => ChangeCameraBehaviour());

        GeneralGameUIHolder.Instance.otherUI.restartButton.onClick.RemoveAllListeners();
        GeneralGameUIHolder.Instance.otherUI.restartButton.onClick.AddListener(() => Restart());

        GeneralGameUIHolder.Instance.otherUI.returnToMenuButton.onClick.RemoveAllListeners();
        GeneralGameUIHolder.Instance.otherUI.returnToMenuButton.onClick.AddListener(() => BackToMainMenu());

        PlayerHandler.PlayerInstance.truck.trucksCondition.OnZeroCondition += SaveCurrentSessionData;
        UpdatePlayerConditionValue();
        PlayerHandler.PlayerInstance.truck.trucksCondition.OnCurrentConditionChanged += UpdatePlayerConditionValue;

        startPostion = PlayerHandler.PlayerInstance.truck._transform.position;
        startTime = (int)Time.time;

        StartCoroutine(WriteTimeTraveledDistances());
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
        yield return new WaitForSecondsRealtime(60);
        endPosition = PlayerHandler.PlayerInstance.truck._transform.position;
        currentSessionData.traveledDistance_1Minute = (int)(endPosition.z - startPostion.z);
        yield return new WaitForSecondsRealtime(120);
        endPosition = PlayerHandler.PlayerInstance.truck._transform.position;
        currentSessionData.traveledDistance_3Minutes = (int)(endPosition.z - startPostion.z);
        yield return new WaitForSecondsRealtime(120);
        endPosition = PlayerHandler.PlayerInstance.truck._transform.position;
        currentSessionData.traveledDistance_5Minutes = (int)(endPosition.z - startPostion.z);
    }

    private void UpdatePlayerConditionValue()
    {
        GeneralGameUIHolder.Instance.otherUI.playerConditionValue.value = PlayerHandler.PlayerInstance.truck.trucksCondition.currentCondition;
    }

    public static void UpdateTargetConditionValue()
    {
        GeneralGameUIHolder.Instance.otherUI.targetConditionValue.value = PlayerHandler.PlayerInstance.FirstTrackingGroupsTarget.target_condition.currentCondition;
    }

    public static void IncreaseDefeatedEnemiesCount()
    {
        currentSessionData.defeatedEnemies++;
    }

    public void SaveCurrentSessionData()
    {
        PlayerHandler.PlayerInstance.StopListenTargetCondition(GameEnums.TrackingGroup.FirstTrackingGroup);
        PlayerHandler.PlayerInstance.truck.trucksCondition.OnCurrentConditionChanged -= UpdatePlayerConditionValue;
        PlayerHandler.PlayerInstance.truck.trucksCondition.OnZeroCondition -= SaveCurrentSessionData;
        //endPosition = PlayerHandler.PlayerInstance.truck._transform.position;
        //endTime = (int)Time.time;

        //currentSessionData.traveledDistance = (int)(endPosition.z - startPostion.z);
        //currentSessionData.traveledTime = endTime - startTime;

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

    private AsyncOperation asyncLoad;
    private IEnumerator LoadCustomiz()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        //PlayerStaticRunTimeData.LoadData();
        //UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenuWithCustomization");
        asyncLoad.allowSceneActivation = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSessionHandler : MonoCached
{
    public Slider playerConditionValue;
    public TaskWindow taskWindow;

    private static PlayerSessionData currentSessionData;
    private CurrentSessionTaskData taskToSession;

    private Vector3 startPostion, endPosition;
    private int startTime, endTime;

    public void StartHandleSession()
    {
        Time.timeScale = 0;

        taskToSession = DataReturnersHolder.Instance.TaskDataReturner.GetRandomData() as CurrentSessionTaskData;

        taskToSession.IsDoneToFalse();

        taskWindow.ShowTaskToSession(taskToSession, delegate { StartSession(); });
    }

    public void StartSession()
    {
        Time.timeScale = 2;

        currentSessionData = new PlayerSessionData(0, 0, 0);

        //playerConditionValue = GameObject.Find("PlayerHealth").GetComponent<Slider>();
        playerConditionValue.minValue = 0;
        playerConditionValue.maxValue = PlayerHandler.playerInstance.truck.TruckData.maxTrucksCondition;
        //taskWindow = GameObject.Find("TaskWindow");

        PlayerHandler.playerInstance.truck.trucksCondition.OnZeroCondition += SaveCurrentSessionData;
        UpdatePlayerConditionValue();
        PlayerHandler.playerInstance.truck.trucksCondition.OnCurrentConditionChanged += UpdatePlayerConditionValue;

        startPostion = PlayerHandler.playerInstance.truck._transform.position;
        startTime = (int)Time.time;

        StartCoroutine(WriteTimeTraveledDistances());
    }

    private IEnumerator WriteTimeTraveledDistances()
    {
        yield return new WaitForSecondsRealtime(60);
        endPosition = PlayerHandler.playerInstance.truck._transform.position;
        currentSessionData.traveledDistance_1Minute = (int)(endPosition.z - startPostion.z);
        yield return new WaitForSecondsRealtime(120);
        endPosition = PlayerHandler.playerInstance.truck._transform.position;
        currentSessionData.traveledDistance_3Minutes = (int)(endPosition.z - startPostion.z);
        yield return new WaitForSecondsRealtime(120);
        endPosition = PlayerHandler.playerInstance.truck._transform.position;
        currentSessionData.traveledDistance_5Minutes = (int)(endPosition.z - startPostion.z);
    }

    private void UpdatePlayerConditionValue()
    {
        playerConditionValue.value = PlayerHandler.playerInstance.truck.trucksCondition.currentCondition;
    }

    public static void IncreaseDefeatedEnemiesCount()
    {
        currentSessionData.defeatedEnemies++;
    }

    public void SaveCurrentSessionData()
    {
        PlayerHandler.playerInstance.truck.trucksCondition.OnCurrentConditionChanged -= UpdatePlayerConditionValue;
        PlayerHandler.playerInstance.truck.trucksCondition.OnZeroCondition -= SaveCurrentSessionData;
        endPosition = PlayerHandler.playerInstance.truck._transform.position;
        endTime = (int)Time.time;

        currentSessionData.traveledDistance = (int)(endPosition.z - startPostion.z);
        currentSessionData.traveledTime = endTime - startTime;

        taskToSession.CheckSession(currentSessionData);

        PlayerStaticRunTimeData.coins += taskToSession.totalReward;
        PlayerStaticDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, currentSessionData);

        ShowDoneTasks();
    }

    public void ShowDoneTasks()
    {
        StartCoroutine(LoadCustomiz());
        taskWindow.ShowSessionProgress(currentSessionData, taskToSession, delegate { ActivateScene(); });
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
        yield return new WaitForSecondsRealtime(0.5f);
        //PlayerStaticRunTimeData.LoadData();
        //UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        yield return new WaitForSecondsRealtime(1f);
        asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenuWithCustomization");
        asyncLoad.allowSceneActivation = false;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskWindow : MonoCached
{
    public Button okButton;

    public Text totalRewardText;
    public GameObject[] taskViewer;
    //public Text[] rewardTexts;
    //public Toggle[] tasksProgressToggle;

    public void HideWindow()
    {
        gameObject.SetActive(false);
    }
    
    public void ShowTaskToSession(CurrentSessionTaskData sessionTask, UnityEngine.Events.UnityAction onOKclick)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < sessionTask.tasks.Length; i++)
        {
            taskViewer[i].SetActive(true);
            taskViewer[i].GetComponentInChildren<Toggle>().gameObject.SetActive(true);
            taskViewer[i].GetComponent<Text>().text = GetLocalizedTaskText(sessionTask.tasks[i].taskType, sessionTask.tasks[i].taskAmount);
            taskViewer[i].GetComponentInChildren<Toggle>().GetComponent<UnityEngine.UI.Toggle>().isOn = false;
            taskViewer[i].transform.GetChild(2).GetComponent<Text>().text = sessionTask.tasks[i].reward.ToString() + "$";
        }
        totalRewardText.gameObject.SetActive(false);
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() => onOKclick.Invoke());
        okButton.onClick.AddListener(() => HideWindow());
    }

    public void ShowSessionProgress(PlayerSessionData sessionData, CurrentSessionTaskData sessionTask, UnityEngine.Events.UnityAction onOKclick)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < sessionTask.tasks.Length; i++)
        {
            taskViewer[i].SetActive(true);
            taskViewer[i].GetComponentInChildren<Toggle>().gameObject.SetActive(true);
            taskViewer[i].GetComponent<Text>().text = GetLocalizedTaskText(sessionTask.tasks[i].taskType, sessionTask.tasks[i].taskAmount);
            taskViewer[i].GetComponentInChildren<Toggle>().GetComponent<UnityEngine.UI.Toggle>().isOn = sessionTask.tasks[i].isDone;
            taskViewer[i].transform.GetChild(2).GetComponent<Text>().text = sessionTask.tasks[i].reward.ToString() + "$";
        }
        totalRewardText.gameObject.SetActive(true);
        totalRewardText.text = GetLocalizedRewardText(sessionTask.totalReward, sessionData.defeatedEnemies * 10 + sessionData.traveledDistance / 10);
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() => onOKclick.Invoke());
        okButton.onClick.AddListener(() => HideWindow());
    }

    public string GetLocalizedRewardText(int totalRewardForTasks, int rewardForSession)
    {
        if(Localization.currentLanguage == GameEnums.Language.RU)
        {
            return $"Получено денег : {totalRewardForTasks} + {rewardForSession} $";
        }
        else
        {
            return $"Total reward : {totalRewardForTasks} + {rewardForSession} $";
        }
    }


    public string GetLocalizedTaskText(GameEnums.TaskType taskType, int taskAmount)
    {
        switch (taskType)
        {
            case GameEnums.TaskType.DestroyEnemies:
                if(Localization.currentLanguage == GameEnums.Language.RU)
                {
                    return $"Уничтожь {taskAmount} врагов";
                }
                else
                {
                    return $"Destroy {taskAmount} enemies";
                }
            case GameEnums.TaskType.TravelDistance:
                if (Localization.currentLanguage == GameEnums.Language.RU)
                {
                    return $"Проедь {taskAmount} метров";
                }
                else
                {
                    return $"Travel {taskAmount} meters";
                }
            case GameEnums.TaskType.TravelTime:
                if (Localization.currentLanguage == GameEnums.Language.RU)
                {
                    return $"Проедь {taskAmount} секунд";
                }
                else
                {
                    return $"Travel {taskAmount} seconds";
                }
        }
        return null;
    }
}

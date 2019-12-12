using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskWindow : MonoCached
{
    public Button okButton;

    public Text totalReward;
    public Text[] tasks;
    public Toggle[] tasksProgress;

    public void HideWindow()
    {
        gameObject.SetActive(false);
    }
    
    public void ShowTaskToSession(CurrentSessionTaskData sessionTask, UnityEngine.Events.UnityAction onOKclick)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < sessionTask.tasks.Length; i++)
        {
            tasks[i].gameObject.SetActive(true);
            tasksProgress[i].gameObject.SetActive(true);
            tasks[i].text = $"{sessionTask.tasks[i].taskType.ToString()} more or equal to {sessionTask.tasks[i].taskAmount}";
            tasksProgress[i].GetComponent<UnityEngine.UI.Toggle>().isOn = false;
        }
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() => onOKclick.Invoke());
        okButton.onClick.AddListener(() => HideWindow());
    }

    public void ShowSessionProgress(PlayerSessionData sessionData, CurrentSessionTaskData sessionTask, UnityEngine.Events.UnityAction onOKclick)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < sessionTask.tasks.Length; i++)
        {
            tasks[i].gameObject.SetActive(true);
            tasksProgress[i].gameObject.SetActive(true);
            tasks[i].text = $"{sessionTask.tasks[i].taskType.ToString()} more or equal to {sessionTask.tasks[i].taskAmount}";
            tasksProgress[i].GetComponent<UnityEngine.UI.Toggle>().isOn = sessionTask.tasks[i].isDone;
        }
        totalReward.text = $"Total reward = {sessionTask.totalReward} $ for tasks + {sessionData.defeatedEnemies * 10 + sessionData.traveledDistance / 10} $ for session";
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() => onOKclick.Invoke());
        okButton.onClick.AddListener(() => HideWindow());
    }


}

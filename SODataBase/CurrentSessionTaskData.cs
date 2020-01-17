using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SessionTask", menuName = "Data/SessionTask")]
public class CurrentSessionTaskData : Data
{
    [System.Serializable]
    public class Task
    {
        public GameEnums.TaskType taskType;
        public int taskAmount;
        public int reward;
        public bool isDone { get; set; }
    }

    public Task[] tasks;
    public int totalReward { get; private set; }

    public void IsDoneToFalse()
    {
        totalReward = 0;
        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i].isDone = false;
        }
    }

    public void CheckSession(PlayerSessionData sessionData)
    {
        //totalReward = 0;
        for (int i = 0; i < tasks.Length; i++)
        {
            if(!tasks[i].isDone)
            {
                switch (tasks[i].taskType)
                {
                    case GameEnums.TaskType.DestroyEnemies:
                        if (sessionData.defeatedEnemies >= tasks[i].taskAmount)
                        {
                            tasks[i].isDone = true;
                            totalReward += tasks[i].reward;
                        }
                        break;
                    case GameEnums.TaskType.TravelDistance:
                        if (sessionData.traveledDistance >= tasks[i].taskAmount)
                        {
                            tasks[i].isDone = true;
                            totalReward += tasks[i].reward;
                        }
                        break;
                    case GameEnums.TaskType.TravelTime:
                        if (sessionData.traveledTime >= tasks[i].taskAmount)
                        {
                            tasks[i].isDone = true;
                            totalReward += tasks[i].reward;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    [ContextMenu("SetUpRewards")]
    public void SetUpRewards()
    {
        for (int i = 0; i < tasks.Length; i++)
        {
            switch (tasks[i].taskType)
            {
                case GameEnums.TaskType.DestroyEnemies:
                    tasks[i].reward = tasks[i].taskAmount * 100;
                    break;
                case GameEnums.TaskType.TravelDistance:
                    tasks[i].reward = tasks[i].taskAmount / 2;
                    break;
                case GameEnums.TaskType.TravelTime:
                    tasks[i].reward = tasks[i].taskAmount * 5;
                    break;
                default:
                    break;
            }
            //UnityEditor.AssetDatabase.SaveAssets();
        }
    }
}

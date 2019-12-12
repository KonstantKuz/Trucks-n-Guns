using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Singleton;

public class PlayerStatisticsViewer : Singleton<PlayerStatisticsViewer>
{
    public Text totalTraveledDistance;
    public Text maxTraveledDistancePerSession;
    public Text totalDefeatedEnemies;
    public Text maxDefeatedEnemiesPerSession;

    public Text totalTraveledTime;
    public Text maxTraveledTimePerSession;

    public Text maxTraveledDistance_1Minute;
    public Text maxTraveledDistance_3Minutes;
    public Text maxTraveledDistance_5Minutes;

    [ContextMenu("RefresshStatistics")]
    public void RefreshStatistics()
    {
        PlayerStaticRunTimeData.LoadData();

        totalTraveledDistance.text = $"{totalTraveledDistance.name} : {PlayerStaticRunTimeData.totalTraveledDistance} meters";
        maxTraveledDistancePerSession.text = $"{maxTraveledDistancePerSession.name} : {PlayerStaticRunTimeData.maxTraveledDistancePerSession} meters";
        totalDefeatedEnemies.text = $"{totalDefeatedEnemies.name} : {PlayerStaticRunTimeData.totalDefeatedEnemies}";
        maxDefeatedEnemiesPerSession.text = $"{maxDefeatedEnemiesPerSession.name} : {PlayerStaticRunTimeData.maxDefeatedEnemiesPerSession}";
        totalTraveledTime.text = $"{totalTraveledTime.name} : {PlayerStaticRunTimeData.totalTraveledTime} seconds";
        maxTraveledTimePerSession.text = $"{maxTraveledTimePerSession.name} : {PlayerStaticRunTimeData.maxTraveledTimePerSession} seconds";

        maxTraveledDistance_1Minute.text = $"{maxTraveledDistance_1Minute.name} : {PlayerStaticRunTimeData.maxTraveledDistance_1Minute} meters";
        maxTraveledDistance_3Minutes.text = $"{maxTraveledDistance_3Minutes.name} : {PlayerStaticRunTimeData.maxTraveledDistance_3Minutes} meters";
        maxTraveledDistance_5Minutes.text = $"{maxTraveledDistance_5Minutes.name} : {PlayerStaticRunTimeData.maxTraveledDistance_5Minutes} meters";

        maxTraveledDistancePerSession.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        maxDefeatedEnemiesPerSession.GetComponentInChildren<Button>().onClick.RemoveAllListeners();

        maxTraveledDistance_1Minute.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        maxTraveledDistance_3Minutes.GetComponentInChildren<Button>().onClick.RemoveAllListeners(); 
        maxTraveledDistance_5Minutes.GetComponentInChildren<Button>().onClick.RemoveAllListeners();

        ///////////////////////////////////////////////////////////////////////////////////////////

        maxTraveledDistancePerSession.GetComponentInChildren<Button>().onClick.AddListener(() =>
        GooglePlayServicesHandler.Instance.ShowLeaderboard(GoogleDataHolder.PlayServicesData.LeaderBoards.MaxTraveledDistanceInOneSession_ID));

        maxDefeatedEnemiesPerSession.GetComponentInChildren<Button>().onClick.AddListener(() =>
        GooglePlayServicesHandler.Instance.ShowLeaderboard(GoogleDataHolder.PlayServicesData.LeaderBoards.MaxDefeatedEnemiesInOneSession_ID));

        maxTraveledDistance_1Minute.GetComponentInChildren<Button>().onClick.AddListener(() =>
        GooglePlayServicesHandler.Instance.ShowLeaderboard(GoogleDataHolder.PlayServicesData.LeaderBoards.MaxTraveledDistanceIn_1_Minute_ID));
        maxTraveledDistance_3Minutes.GetComponentInChildren<Button>().onClick.AddListener(() =>
        GooglePlayServicesHandler.Instance.ShowLeaderboard(GoogleDataHolder.PlayServicesData.LeaderBoards.MaxTraveledDistanceIn_3_Minute_ID));
        maxTraveledDistance_5Minutes.GetComponentInChildren<Button>().onClick.AddListener(() =>
        GooglePlayServicesHandler.Instance.ShowLeaderboard(GoogleDataHolder.PlayServicesData.LeaderBoards.MaxTraveledDistanceIn_5_Minute_ID));
    }

}

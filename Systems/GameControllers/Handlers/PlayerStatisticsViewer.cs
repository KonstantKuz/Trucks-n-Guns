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

        totalTraveledDistance.transform.GetChild(1).GetComponent<Text>().text = $"{PlayerStaticRunTimeData.totalTraveledDistance} m";
        maxTraveledDistancePerSession.transform.GetChild(1).GetComponent<Text>().text = $"{PlayerStaticRunTimeData.maxTraveledDistancePerSession} m";
        totalDefeatedEnemies.transform.GetChild(1).GetComponent<Text>().text = $"{PlayerStaticRunTimeData.totalDefeatedEnemies}";
        maxDefeatedEnemiesPerSession.transform.GetChild(1).GetComponent<Text>().text = $"{PlayerStaticRunTimeData.maxDefeatedEnemiesPerSession}";
        totalTraveledTime.transform.GetChild(1).GetComponent<Text>().text = $"{PlayerStaticRunTimeData.totalTraveledTime} sec";
        maxTraveledTimePerSession.transform.GetChild(1).GetComponent<Text>().text = $"{PlayerStaticRunTimeData.maxTraveledTimePerSession} sec";

        maxTraveledDistance_1Minute.transform.GetChild(1).GetComponent<Text>().text = $"{PlayerStaticRunTimeData.maxTraveledDistance_1Minute} m";
        maxTraveledDistance_3Minutes.transform.GetChild(1).GetComponent<Text>().text = $"{PlayerStaticRunTimeData.maxTraveledDistance_3Minutes} m";
        maxTraveledDistance_5Minutes.transform.GetChild(1).GetComponent<Text>().text = $"{PlayerStaticRunTimeData.maxTraveledDistance_5Minutes} m";
        Debug.Log(PlayerStaticRunTimeData.maxTraveledDistance_3Minutes);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataDebug : MonoBehaviour
{
    public static Text coinText;
    public static Text defeatedEnemiesText;
    public static Text maxDefeatedEnemiesText;
    public static Text traveledDistance;
    public static Text maxTraveledDistance;

    private void Awake()
    {
        coinText = GameObject.Find("CoinDebugger").GetComponent<Text>();
        defeatedEnemiesText = GameObject.Find("DefeatedEnemies").GetComponent<Text>();
        maxDefeatedEnemiesText = GameObject.Find("MaxDefeatedEnemies").GetComponent<Text>();
        traveledDistance = GameObject.Find("TraveledDistance").GetComponent<Text>();
        maxTraveledDistance = GameObject.Find("MaxTraveledDistance").GetComponent<Text>();

        RefreshStatistics();

    }
    public static void RefreshStatistics()
    {
        coinText.text = PlayerStaticRunTimeData.coins.ToString();
        coinText.text = PlayerStaticRunTimeData.coins.ToString();
        defeatedEnemiesText.text = PlayerStaticRunTimeData.defeatedEnemies.ToString();
        maxDefeatedEnemiesText.text = PlayerStaticRunTimeData.maxDefeatedEnemiesPerSession.ToString();
        traveledDistance.text = PlayerStaticRunTimeData.traveledDistance.ToString();
        maxTraveledDistance.text = PlayerStaticRunTimeData.maxTraveledDistancePerSession.ToString();
    }
    
}

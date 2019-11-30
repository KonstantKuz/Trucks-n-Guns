using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSessionHandler : MonoBehaviour
{
    private static PlayerSessionData currentSessionData;
    private Slider playerConditionValue;


    public void StartHandleSession()
    {
        currentSessionData = new PlayerSessionData(0, 0);

        StartCoroutine(IncreaseTraveledDistance(PlayerHandler.playerInstance.truck._transform.position));


        playerConditionValue = GameObject.Find("PlayerHealth").GetComponent<Slider>();
        playerConditionValue.minValue = 0;
        playerConditionValue.maxValue = PlayerHandler.playerInstance.truck.TruckData.maxTrucksCondition;

        PlayerHandler.playerInstance.truck.trucksCondition.OnZeroCondition += SaveCurrentSessionData;

        PlayerHandler.playerInstance.truck.trucksCondition.OnCurrentConditionChanged += UpdatePlayerCondditionValue;
        PlayerHandler.playerInstance.truck.trucksCondition.OnZeroCondition+= SaveCurrentSessionData;

    }
    private void UpdatePlayerCondditionValue()
    {
        playerConditionValue.value = PlayerHandler.playerInstance.truck.trucksCondition.currentCondition;
    }

    private IEnumerator IncreaseTraveledDistance(Vector3 lastPlayerPosition)
    {
        yield return new WaitForSeconds(1f);
        IncreaseTraveledDistance((int)(PlayerHandler.playerInstance.truck._transform.position.z - lastPlayerPosition.z));
        yield return StartCoroutine(IncreaseTraveledDistance(PlayerHandler.playerInstance.truck._transform.position));
    }

    public static void IncreaseTraveledDistance(int traveledDistance)
    {
        currentSessionData.traveledDistance += traveledDistance;
    }

    public static void IncreaseDefeatedEnemiesCount()
    {
        currentSessionData.defeatedEnemies++;
    }

    public void SaveCurrentSessionData()
    {
        PlayerHandler.playerInstance.truck.trucksCondition.OnCurrentConditionChanged -= UpdatePlayerCondditionValue;
        PlayerHandler.playerInstance.truck.trucksCondition.OnZeroCondition -= SaveCurrentSessionData;

        PlayerHandler.playerInstance.truck.trucksCondition.OnZeroCondition -= SaveCurrentSessionData;

        PersistentPlayerDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, PlayerStaticRunTimeData.playerFirePointData, currentSessionData);
    }
}

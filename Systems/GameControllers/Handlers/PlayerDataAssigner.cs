using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataAssigner : MonoBehaviour
{
    public TruckData playerTruckData;
    public TruckData customizationTruckData;

    private void Awake()
    {
        PlayerStaticRunTimeData.playerTruckData = playerTruckData;
        PlayerStaticRunTimeData.customizationTruckData = customizationTruckData;
        customizationTruckData.RewriteData(playerTruckData);
        customizationTruckData.firePointData.RewriteData(playerTruckData.firePointData);
        if(PlayerStaticDataHandler.LoadData() == null)
        {
            PlayerStaticRunTimeData.coins += 10000;
            PlayerStaticRunTimeData.playerTruckData.ResetData();
            PlayerStaticRunTimeData.customizationTruckData.ResetData();
            PlayerStaticDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, new PlayerSessionData(0, 0, 0));
        }
        else
        {
            PlayerStaticRunTimeData.LoadData();
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuWithCustomization");
    }

    [ContextMenu("ApplicatoinDataPath")]
    public void DebugPersistentDataPath()
    {
        Debug.Log(Application.persistentDataPath);
    }
}

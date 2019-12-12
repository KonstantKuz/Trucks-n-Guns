using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataAssigner : MonoBehaviour
{
    public FirePointData playerFirePointData;
    public TruckData playerTruckData;

    public FirePointData customizationFirePointData;
    public TruckData customizationTruckData;

    private void Awake()
    {
        PlayerStaticRunTimeData.playerTruckData = playerTruckData;
        PlayerStaticRunTimeData.customizationTruckData = customizationTruckData;
        if(PlayerStaticDataHandler.LoadData() == null)
        {
            PlayerStaticRunTimeData.coins += 100000;
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

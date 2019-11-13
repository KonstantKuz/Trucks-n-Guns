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
        PlayerStaticRunTimeData.playerFirePointData = playerFirePointData;
        PlayerStaticRunTimeData.customizationFirePointData = customizationFirePointData;
        PlayerStaticRunTimeData.customizationTruckData = customizationTruckData;
        if(PersistentPlayerDataHandler.LoadData() == null)
        {
            PlayerStaticRunTimeData.coins += 20000;
            PersistentPlayerDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, PlayerStaticRunTimeData.playerFirePointData, new PlayerSessionData(0, 0));
        }
        else
        {
            PlayerStaticRunTimeData.LoadData();
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Customization");
    }

    [ContextMenu("ApplicatoinDataPath")]
    public void DebugPersistentDataPath()
    {
        Debug.Log(Application.persistentDataPath);
    }
}

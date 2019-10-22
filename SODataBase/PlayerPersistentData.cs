using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPersistentData", menuName = "Data/PlayerData/PlayerPersistentData")]
public class PlayerPersistentData : Data
{
    public FirePointData firePointData;
    public TruckData truckData;

    public void SetPlayerData(TruckData truckData, FirePointData firePointData)
    {
        PlayerData.playerFirePointData = firePointData;
        PlayerData.playerTruckData = truckData;
    }
}

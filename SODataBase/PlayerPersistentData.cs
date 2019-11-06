using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPersistentData", menuName = "Data/PlayerData/PlayerPersistentData")]
public class PlayerPersistentData : Data
{
    public FirePointData firePointData;
    public TruckData truckData;

    public int coins;
    public int maxTraveledDistance;
    public int defeatedEnemiesCount;
    public int defeatedPlayersCount;
}

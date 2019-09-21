using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSessionData", menuName = "Data/PlayerData/PlayerSessionData")]
public class PlayerSessionData : Data
{
    private float traveledDistance;
    private float maxGainedSpeed;
    private int defeatedEnemiesCount;

}

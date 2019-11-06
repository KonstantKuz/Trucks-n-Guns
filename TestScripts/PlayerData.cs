using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerData
{
    public FirePointData playerFirePointData;
    public TruckData playerTruckData;

    public int maxTraveledDistance;
    public int maxDefeatedEnemy;

    public PlayerData(PlayerData data)
    {
        playerFirePointData = data.playerFirePointData;
        playerTruckData = data.playerTruckData;

        maxTraveledDistance = data.maxTraveledDistance;
        maxDefeatedEnemy = data.maxDefeatedEnemy;
    }
}

public static class PlayerDataHandler
{
    public static void SaveData(PlayerData playerData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.data";

        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(playerData);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadData()
    {
        string path = Application.persistentDataPath + "/player.data";

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;

            return data;
        }
        else
        {
            Debug.Log("saved data not found in" + path);
            return null;
        }
    }
}

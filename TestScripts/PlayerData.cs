using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class PlayerStaticRunTimeData
{
    public static FirePointData customizationFirePointData;
    public static TruckData customizationTruckData;

    public static FirePointData playerFirePointData;
    public static TruckData playerTruckData;

    public static int traveledDistance;
    public static int maxTraveledDistancePerSession;
    public static int defeatedEnemies;
    public static int maxDefeatedEnemiesPerSession;

    public static int coins;
    public static int experience;

    public static void LoadData()
    {
        SerializablePlayerData data = PersistentPlayerDataHandler.LoadData();
        if(data != null)
        {
            playerTruckData.truckType = (GameEnums.Truck)data.truckType;
            playerTruckData.firePointType = (GameEnums.FirePointType)data.firePointType;

            for (int i = 0; i < playerFirePointData.gunsConfigurations.Length; i++)
            {
                var config = playerFirePointData.gunsConfigurations[i];
                config.gunType = (GameEnums.Gun)data.gunTypes[i];
                config.locationPath = (GameEnums.GunLocation)data.locationPaths[i];
                config.trackingGroup = (GameEnums.TrackingGroup)data.trackingGroups[i];
                config.gunDataType = (GameEnums.GunDataType)data.gunDataTypes[i];
            }

            maxTraveledDistancePerSession = data.maxTraveledDistancePerSession;
            traveledDistance = data.traveledDistance;
            maxDefeatedEnemiesPerSession = data.maxDefeatedEnemiesPerSession;
            defeatedEnemies = data.defeatedEnemies;
            coins = data.coins;
            experience = data.experience;
        }
    }
}
[System.Serializable]
public class SerializablePlayerData
{
    public int truckType;
    public int firePointType;
    public int[] gunTypes;
    public int[] locationPaths;
    public int[] trackingGroups;
    public int[] gunDataTypes;

    public int traveledDistance;
    public int maxTraveledDistancePerSession;
    public int defeatedEnemies;
    public int maxDefeatedEnemiesPerSession;

    public int coins;
    public int experience;
}
[System.Serializable]
public class PlayerSessionData
{
    public int traveledDistance;
    public int defeatedEnemies;

    public PlayerSessionData (int traveledDistance, int defeatedEnemies)
    {
        this.traveledDistance = traveledDistance;
        this.defeatedEnemies = defeatedEnemies;
    }
}
public static class PersistentPlayerDataHandler
{
    public static void SaveData(TruckData truckData, FirePointData firePointData, PlayerSessionData playerSessionData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.data";

        FileStream stream = new FileStream(path, FileMode.Create);

        SerializablePlayerData data = new SerializablePlayerData();

        data.firePointType = (int)truckData.firePointType;
        data.truckType = (int)truckData.truckType;

        data.gunTypes = new int[12];
        data.locationPaths = new int[12];
        data.trackingGroups = new int[12];
        data.gunDataTypes = new int[12];

        for (int i = 0; i < firePointData.gunsConfigurations.Length; i++)
        {
            var config = firePointData.gunsConfigurations[i];
            data.gunTypes[i] = (int)config.gunType;
            data.locationPaths[i] = (int)config.locationPath;
            data.trackingGroups[i] = (int)config.trackingGroup;
            data.gunDataTypes[i] = (int)config.gunDataType;
        }

            if (playerSessionData.traveledDistance > PlayerStaticRunTimeData.maxTraveledDistancePerSession)
            {
                data.maxTraveledDistancePerSession = playerSessionData.traveledDistance;
            }
            else
            {
                data.maxTraveledDistancePerSession = PlayerStaticRunTimeData.maxTraveledDistancePerSession;
            }
            data.traveledDistance = PlayerStaticRunTimeData.traveledDistance + playerSessionData.traveledDistance;
            if (playerSessionData.defeatedEnemies > PlayerStaticRunTimeData.maxDefeatedEnemiesPerSession)
            {
                data.maxDefeatedEnemiesPerSession = playerSessionData.defeatedEnemies;
            }
            else
            {
                data.maxDefeatedEnemiesPerSession = PlayerStaticRunTimeData.maxDefeatedEnemiesPerSession;
            }
            data.defeatedEnemies = PlayerStaticRunTimeData.defeatedEnemies + playerSessionData.defeatedEnemies;
            data.experience = PlayerStaticRunTimeData.experience + playerSessionData.defeatedEnemies * 100 + playerSessionData.traveledDistance / 10;
            data.coins = PlayerStaticRunTimeData.coins + playerSessionData.defeatedEnemies * 10 + +playerSessionData.traveledDistance / 10;

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SerializablePlayerData LoadData()
    {
        string path = Application.persistentDataPath + "/player.data";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SerializablePlayerData data = formatter.Deserialize(stream) as SerializablePlayerData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.Log("saved data not found in" + path);
            return null;
        }
    }
}

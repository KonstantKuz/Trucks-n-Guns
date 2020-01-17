using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class PlayerStaticRunTimeData
{
    //public static FirePointData customizationFirePointData;
    public static TruckData customizationTruckData;

    //public static FirePointData playerFirePointData;
    public static TruckData playerTruckData;

    public static int totalTraveledDistance;
    public static int maxTraveledDistancePerSession;
    public static int totalDefeatedEnemies;
    public static int maxDefeatedEnemiesPerSession;

    public static int totalTraveledTime;
    public static int maxTraveledTimePerSession;

    public static int maxTraveledDistance_1Minute;
    public static int maxTraveledDistance_3Minutes;
    public static int maxTraveledDistance_5Minutes;

    public static int coins;
    public static int experience;

    public static void LoadData()
    {
        PlayerSerializableData data = PlayerStaticDataHandler.LoadData();
        if(data != null)
        {
            playerTruckData.truckType = (GameEnums.Truck)data.truckType;
            playerTruckData.firePointType = (GameEnums.FirePointType)data.firePointType;

            for (int i = 0; i < playerTruckData.firePointData.gunsConfigurations.Length; i++)
            {
                var config = playerTruckData.firePointData.gunsConfigurations[i];
                config.gunType = (GameEnums.Gun)data.gunTypes[i];
                config.battleType = (GameEnums.BattleType)data.battleTypes[i];
                config.locationPath = (GameEnums.GunLocation)data.locationPaths[i];
                config.trackingGroup = (GameEnums.TrackingGroup)data.trackingGroups[i];
                config.gunDataType = (GameEnums.GunDataType)data.gunDataTypes[i];
            }

            maxTraveledDistancePerSession = data.maxTraveledDistancePerSession;
            totalTraveledDistance = data.totalTraveledDistance;
            maxDefeatedEnemiesPerSession = data.maxDefeatedEnemiesPerSession;
            totalDefeatedEnemies = data.totalDefeatedEnemies;

            totalTraveledTime = data.totalTraveledTime;
            maxTraveledTimePerSession = data.maxTraveledTimePerSession;
            maxTraveledDistance_1Minute = data.maxTraveledDistance_1Minute;
            maxTraveledDistance_3Minutes = data.maxTraveledDistance_3Minutes;
            maxTraveledDistance_5Minutes = data.maxTraveledDistance_5Minutes;

            coins = data.coins;
            experience = data.experience;
        }
    }
}
[System.Serializable]
public class PlayerSerializableData
{
    public int truckType;
    public int firePointType;

    public int[] gunTypes;
    public int[] battleTypes;
    public int[] locationPaths;
    public int[] trackingGroups;
    public int[] gunDataTypes;

    public int totalTraveledDistance;
    public int maxTraveledDistancePerSession;
    public int totalDefeatedEnemies;
    public int maxDefeatedEnemiesPerSession;

    public int totalTraveledTime;
    public int maxTraveledTimePerSession;

    public int maxTraveledDistance_1Minute;
    public int maxTraveledDistance_3Minutes;
    public int maxTraveledDistance_5Minutes;

    public int coins;
    public int experience;
}
[System.Serializable]
public class PlayerSessionData
{
    public int traveledDistance;
    public int defeatedEnemies;
    public int traveledTime;

    public int traveledDistance_1Minute;
    public int traveledDistance_3Minutes;
    public int traveledDistance_5Minutes;

    public PlayerSessionData(int traveledDistance, int defeatedEnemies, int traveledTime)
    {
        this.traveledDistance = traveledDistance;
        this.defeatedEnemies = defeatedEnemies;
        this.traveledTime = traveledTime;
    }
}

[System.Serializable]
public class AdvertisementConsentData
{
    public bool showNONPersonalizedAd;
}

[System.Serializable]
public class GPGSAutoEntry
{
    public bool enabled;
}

public static class PlayerStaticDataHandler
{
    public static void SaveData(TruckData truckData, PlayerSessionData playerSessionData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.data";

        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerSerializableData data = new PlayerSerializableData();

        data.firePointType = (int)truckData.firePointType;
        data.truckType = (int)truckData.truckType;

        data.gunTypes = new int[12];
        data.battleTypes = new int[12];
        data.locationPaths = new int[12];
        data.trackingGroups = new int[12];
        data.gunDataTypes = new int[12];

        for (int i = 0; i < truckData.firePointData.gunsConfigurations.Length; i++)
        {
            var config = truckData.firePointData.gunsConfigurations[i];
            data.gunTypes[i] = (int)config.gunType;
            data.battleTypes[i] = (int)config.battleType;
            data.locationPaths[i] = (int)config.locationPath;
            data.trackingGroups[i] = (int)config.trackingGroup;
            data.gunDataTypes[i] = (int)config.gunDataType;
        }

        if (playerSessionData.traveledDistance > PlayerStaticRunTimeData.maxTraveledDistancePerSession)
        {
            data.maxTraveledDistancePerSession = playerSessionData.traveledDistance;
            GooglePlayServicesHandler.Instance.SaveProgressToLeaderboard(playerSessionData.traveledDistance, GoogleDataHolder.PlayServicesData.LeaderBoards.MaxTraveledDistanceInOneSession_ID);
        }
        else
        {
            data.maxTraveledDistancePerSession = PlayerStaticRunTimeData.maxTraveledDistancePerSession;
        }
        if (playerSessionData.defeatedEnemies > PlayerStaticRunTimeData.maxDefeatedEnemiesPerSession)
        {
            data.maxDefeatedEnemiesPerSession = playerSessionData.defeatedEnemies;
            GooglePlayServicesHandler.Instance.SaveProgressToLeaderboard(playerSessionData.defeatedEnemies, GoogleDataHolder.PlayServicesData.LeaderBoards.MaxDefeatedEnemiesInOneSession_ID);
        }
        else
        {
            data.maxDefeatedEnemiesPerSession = PlayerStaticRunTimeData.maxDefeatedEnemiesPerSession;
        }
        if (playerSessionData.traveledTime > PlayerStaticRunTimeData.maxTraveledTimePerSession)
        {
            data.maxTraveledTimePerSession = playerSessionData.traveledTime;
        }
        else
        {
            data.maxTraveledTimePerSession = PlayerStaticRunTimeData.maxTraveledTimePerSession;
        }
        if (playerSessionData.traveledDistance_1Minute > PlayerStaticRunTimeData.maxTraveledDistance_1Minute)
        {
            data.maxTraveledDistance_1Minute = playerSessionData.traveledDistance_1Minute;
            GooglePlayServicesHandler.Instance.SaveProgressToLeaderboard(playerSessionData.traveledDistance_1Minute, GoogleDataHolder.PlayServicesData.LeaderBoards.MaxTraveledDistanceIn_1_Minute_ID);
        }
        else
        {
            data.maxTraveledDistance_1Minute = PlayerStaticRunTimeData.maxTraveledDistance_1Minute;
        }
        if (playerSessionData.traveledDistance_3Minutes > PlayerStaticRunTimeData.maxTraveledDistance_3Minutes)
        {
            data.maxTraveledDistance_3Minutes = playerSessionData.traveledDistance_3Minutes;
            GooglePlayServicesHandler.Instance.SaveProgressToLeaderboard(playerSessionData.traveledDistance_3Minutes, GoogleDataHolder.PlayServicesData.LeaderBoards.MaxTraveledDistanceIn_3_Minute_ID);
        }
        else
        {
            data.maxTraveledDistance_5Minutes = PlayerStaticRunTimeData.maxTraveledDistance_5Minutes;
        }
        if (playerSessionData.traveledDistance_5Minutes > PlayerStaticRunTimeData.maxTraveledDistance_5Minutes)
        {
            data.maxTraveledDistance_5Minutes = playerSessionData.traveledDistance_5Minutes;
            GooglePlayServicesHandler.Instance.SaveProgressToLeaderboard(playerSessionData.traveledDistance_5Minutes, GoogleDataHolder.PlayServicesData.LeaderBoards.MaxTraveledDistanceIn_5_Minute_ID);
        }
        else
        {
            data.maxTraveledDistance_5Minutes = PlayerStaticRunTimeData.maxTraveledDistance_5Minutes;
        }

        data.totalTraveledDistance = PlayerStaticRunTimeData.totalTraveledDistance + playerSessionData.traveledDistance;
        data.totalTraveledTime = PlayerStaticRunTimeData.totalTraveledTime + playerSessionData.traveledTime;
        data.totalDefeatedEnemies = PlayerStaticRunTimeData.totalDefeatedEnemies + playerSessionData.defeatedEnemies;

        data.experience = PlayerStaticRunTimeData.experience + RewardCoinsForSession(playerSessionData.defeatedEnemies, playerSessionData.traveledDistance);
        data.coins = PlayerStaticRunTimeData.coins + RewardCoinsForSession(playerSessionData.defeatedEnemies, playerSessionData.traveledDistance);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static int RewardCoinsForSession(int defeatedEnemies, int traveledDistance)
    {
        return defeatedEnemies * 25 + traveledDistance / 4;
    }

    public static PlayerSerializableData LoadData()
    {
        string path = Application.persistentDataPath + "/player.data";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerSerializableData data = formatter.Deserialize(stream) as PlayerSerializableData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.Log("saved data not found in" + path);
            return null;
        }
    }

    public static void SaveAdConsent(bool showNONPersonalzedAd)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/adconsent.data";

        FileStream stream = new FileStream(path, FileMode.Create);

        AdvertisementConsentData data = new AdvertisementConsentData();

        data.showNONPersonalizedAd = showNONPersonalzedAd;

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static AdvertisementConsentData LoadAdConsent()
    {
        string path = Application.persistentDataPath + "/adconsent.data";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            AdvertisementConsentData data = formatter.Deserialize(stream) as AdvertisementConsentData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.Log("saved data not found in" + path);
            return null;
        }
    }

    public static void SetGPGSAutoEntry(bool enabled)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/gpgsautoent.data";

        FileStream stream = new FileStream(path, FileMode.Create);

        GPGSAutoEntry data = new GPGSAutoEntry();

        data.enabled = enabled;

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GPGSAutoEntry GPGSAutoEntryStat()
    {
        string path = Application.persistentDataPath + "/gpgsautoent.data";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GPGSAutoEntry data = formatter.Deserialize(stream) as GPGSAutoEntry;
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

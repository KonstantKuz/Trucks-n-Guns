using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoadTileType
{
    public GameEnums.RoadPropsType propsType;
    public GameEnums.RoadAsphaltType asphaltType;
    public GameEnums.RoadShapeType shapeType;
    public string tileName { get; set; }
}

public static class RoadTilesDictionary
{
    public static List<RoadTileType> roadTiles = new List<RoadTileType>(1000);

    public static void AddNewTile(RoadTileType roadDataToAdd)
    {
        if(!roadTiles.Contains(roadDataToAdd))
        {
            roadTiles.Add(roadDataToAdd);
        }
        else
        {
            Debug.LogWarning($"<color=red> RoadTilesDictionary already exists {roadDataToAdd.tileName} roaddata </color>");
        }
    }
}
[CreateAssetMenu(fileName = "NextRoadTileData", menuName = "Data/NextRoadTileData")]
public class RoadTileTransitionCapabilitiesData : Data
{
    [System.Serializable]
    public class PossibleTile
    {
        public string TileName;
        [Range(10,100)]
        public int TileWeight;
    }

    public List<PossibleTile> possibleTiles;

    public string NextWeightedRandomTile()
    {
        int totalWeight = 0;
        for (int i = 0; i < possibleTiles.Count; i++)
        {
            totalWeight += possibleTiles[i].TileWeight;
        }
        int randomValue = Random.Range(0, totalWeight + 1);

        for (int i = 0; i < possibleTiles.Count; i++)
        {
            if (randomValue <= possibleTiles[i].TileWeight)
            {
                return possibleTiles[i].TileName;
            }

            randomValue -= possibleTiles[i].TileWeight;
        }

        Debug.Log("<color=red> Weigted Random was return null </color>");
        return null;
    }
    
    public void SetUpCapabilities(RoadTileType ownersRoadTileType)
    {
        Debug.Log("<color=green> Started seting up possibilities </color>");
        possibleTiles = new List<PossibleTile>(50);
        RoadTilesDictionary.AddNewTile(ownersRoadTileType);

        List<RoadTileType> possibleTypes = new List<RoadTileType>(100);
        List<GameEnums.RoadAsphaltType> possibleAsphaltTypesForOwner = PossibleAsphaltTypes(ownersRoadTileType);
        List<GameEnums.RoadPropsType> possiblePropsTypesForOwner = PossiblePropsType(ownersRoadTileType);
        List<GameEnums.RoadShapeType> possibleShapeTypesForOwner = PossibleShapeType(ownersRoadTileType);

        for (int asphaltTypeCounter = 0; asphaltTypeCounter < possibleAsphaltTypesForOwner.Count; asphaltTypeCounter++)
        {
            for (int propsTypeCounter = 0; propsTypeCounter < possiblePropsTypesForOwner.Count; propsTypeCounter++)
            {
                for (int shapeTypesCounter = 0; shapeTypesCounter < possibleShapeTypesForOwner.Count; shapeTypesCounter++)
                {
                    RoadTileType possibleType = new RoadTileType();
                    possibleType.asphaltType = possibleAsphaltTypesForOwner[asphaltTypeCounter];
                    possibleType.propsType = possiblePropsTypesForOwner[propsTypeCounter];
                    possibleType.shapeType = possibleShapeTypesForOwner[shapeTypesCounter];
                    possibleTypes.Add(possibleType);
                }
            }
        }

        for (int i = 0; i < RoadTilesDictionary.roadTiles.Count; i++)
        {
            for (int j = 0; j < possibleTypes.Count; j++)
            {
                RoadTileType tileFromDictionary = RoadTilesDictionary.roadTiles[i];
                RoadTileType possibleType = possibleTypes[j];
                if (tileFromDictionary.asphaltType == possibleType.asphaltType &&  tileFromDictionary.propsType == possibleType.propsType && tileFromDictionary.shapeType == possibleType.shapeType)
                {
                    Debug.Log($"Here was added a new possible tile with name {tileFromDictionary.tileName}");
                    PossibleTile possibleTile = new PossibleTile();
                    possibleTile.TileName = tileFromDictionary.tileName;
                    possibleTiles.Add(possibleTile);
                }
            }
        }

        for (int i = 0; i < possibleTiles.Count; i++)
        {
            if(possibleTiles[i].TileName.Contains("End"))
            {
                possibleTiles[i].TileWeight = 10;
            }
            else
            {
                possibleTiles[i].TileWeight = 100 / possibleTiles.Count;
            }
        }
        Debug.Log("<color=red> Stopped seting up possibilities </color>");
    }

    public List<GameEnums.RoadAsphaltType> PossibleAsphaltTypes(RoadTileType ownersType)
    {
        List<GameEnums.RoadAsphaltType> possibleTypes = new List<GameEnums.RoadAsphaltType>(System.Enum.GetNames(typeof(GameEnums.RoadAsphaltType)).Length);

        switch (ownersType.shapeType)
        {
            case GameEnums.RoadShapeType.Start:
                possibleTypes.Add(ownersType.asphaltType);
                break;
            case GameEnums.RoadShapeType.Middle:
                possibleTypes.Add(ownersType.asphaltType);
                break;
            case GameEnums.RoadShapeType.End:
                int typeCounter = 0;
                for (int i = 0; i < System.Enum.GetNames(typeof(GameEnums.RoadAsphaltType)).Length; i++)
                {
                    GameEnums.RoadAsphaltType asphType = (GameEnums.RoadAsphaltType)typeCounter;
                    possibleTypes.Add(asphType);
                    typeCounter++;
                }
                break;
        }
        if(possibleTypes.Count == 0)
        {
            throw new System.Exception("Cannot return AsphaltType");
        }
        Debug.Log($"<color=blue> {possibleTypes} was returned with count {possibleTypes.Count} </color>");

        return possibleTypes;
    }

    public List<GameEnums.RoadPropsType> PossiblePropsType(RoadTileType ownersType)
    {
        List<GameEnums.RoadPropsType> possibleTypes = new List<GameEnums.RoadPropsType>(System.Enum.GetNames(typeof(GameEnums.RoadPropsType)).Length);

        switch (ownersType.propsType)
        {
            case GameEnums.RoadPropsType.Town:
                possibleTypes.Add(GameEnums.RoadPropsType.Town);
                possibleTypes.Add(GameEnums.RoadPropsType.FromTownToDesert);
                break;
            case GameEnums.RoadPropsType.Desert:
                possibleTypes.Add(GameEnums.RoadPropsType.Desert);
                possibleTypes.Add(GameEnums.RoadPropsType.FromDesertToTown);
                break;
            case GameEnums.RoadPropsType.FromTownToDesert:
                possibleTypes.Add(GameEnums.RoadPropsType.Desert);
                break;
            case GameEnums.RoadPropsType.FromDesertToTown:
                possibleTypes.Add(GameEnums.RoadPropsType.Town);
                break;
        }
        if(possibleTypes.Count == 0)
        {
            throw new System.Exception("Cannot return PropsType");
        }
        Debug.Log($"<color=blue> {possibleTypes} was returned with count {possibleTypes.Count} </color>");

        return possibleTypes;
    }

    public List<GameEnums.RoadShapeType> PossibleShapeType(RoadTileType ownersRoadTileType)
    {
        List<GameEnums.RoadShapeType> possibleTypes = new List<GameEnums.RoadShapeType>(System.Enum.GetNames(typeof(GameEnums.RoadShapeType)).Length);

        switch (ownersRoadTileType.shapeType)
        {
            case GameEnums.RoadShapeType.Start:
                possibleTypes.Add(GameEnums.RoadShapeType.Middle);
                break;
            case GameEnums.RoadShapeType.Middle:
                possibleTypes.Add(GameEnums.RoadShapeType.Middle);
                possibleTypes.Add(GameEnums.RoadShapeType.End);
                break;
            case GameEnums.RoadShapeType.End:
                possibleTypes.Add(GameEnums.RoadShapeType.Start);
                break;
        }
        if(possibleTypes.Count == 0)
        {
            throw new System.Exception("Cannot return ShapeType");
        }
        Debug.Log($"<color=blue> {possibleTypes} was returned with count {possibleTypes.Count} </color>");

        return possibleTypes;
    }
}

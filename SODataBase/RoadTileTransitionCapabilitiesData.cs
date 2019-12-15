using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoadTileConfiguration
{
    public GameEnums.RoadPropsType propsType;
    public GameEnums.RoadAsphaltType asphaltType;
    public GameEnums.RoadShapeType shapeType;
    public string tileName { get; set; }
}

[CreateAssetMenu(fileName = "NextRoadTileData", menuName = "Data/NextRoadTileData")]
public class RoadTileTransitionCapabilitiesData : Data
{
    [System.Serializable]
    public class PossibleTile
    {
        public string TileName;
        [Range(1, 100)]
        public int TileWeight;
    }

    public List<PossibleTile> PossibleTilesForThis = new List<PossibleTile>(100);
    private List<string> currentAvailablePossibleTileNames = new List<string>(100);
    public static List<RoadTileConfiguration> allInGameAvailableRoadConfigs = new List<RoadTileConfiguration>(100);

    public string GetNextRandomWeightedRoadTileName()
    {
        int totalWeight = 0;
        for (int i = 0; i < PossibleTilesForThis.Count; i++)
        {
            totalWeight += PossibleTilesForThis[i].TileWeight;
        }
        int randomValue = Random.Range(0, totalWeight + 1);

        for (int i = 0; i < PossibleTilesForThis.Count; i++)
        {
            if (randomValue <= PossibleTilesForThis[i].TileWeight)
            {
                return PossibleTilesForThis[i].TileName;
            }

            randomValue -= PossibleTilesForThis[i].TileWeight;
        }

        Debug.Log("<color=red> Weigted Random was return null </color>");
        return null;
    }

    [ContextMenu("ClearAllData")]
    public void ClearAllData()
    {
        PossibleTilesForThis.Clear();
        currentAvailablePossibleTileNames.Clear();
        allInGameAvailableRoadConfigs.Clear();
    }

    public void SetUpCapabilities(RoadTileConfiguration newRoadTileConfiguration)
    {
        Debug.Log("<color=green> Started seting up possibilities </color>");

        if (!allInGameAvailableRoadConfigs.Contains(newRoadTileConfiguration))
        {
            allInGameAvailableRoadConfigs.Add(newRoadTileConfiguration);
        }

        List<RoadTileConfiguration> possibleConfigsForNewRoad = PossibleRoadConfigsFor(newRoadTileConfiguration);

        for (int i = 0; i < allInGameAvailableRoadConfigs.Count; i++)
        {
            for (int j = 0; j < possibleConfigsForNewRoad.Count; j++)
            {
                RoadTileConfiguration concreteConfigFromAllAvailable = allInGameAvailableRoadConfigs[i];
                RoadTileConfiguration concreteConfigFromAllPossible = possibleConfigsForNewRoad[j];

                if (concreteConfigFromAllAvailable.asphaltType == concreteConfigFromAllPossible.asphaltType && concreteConfigFromAllAvailable.propsType == concreteConfigFromAllPossible.propsType && concreteConfigFromAllAvailable.shapeType == concreteConfigFromAllPossible.shapeType)
                {
                    PossibleTile possibleTile = new PossibleTile();
                    possibleTile.TileName = concreteConfigFromAllAvailable.tileName;
                    possibleTile.TileWeight = 0;
                    if (!currentAvailablePossibleTileNames.Contains(possibleTile.TileName))
                    {
                        Debug.Log($"Here was added a new possible tile with name {concreteConfigFromAllAvailable.tileName}");

                        currentAvailablePossibleTileNames.Add(possibleTile.TileName);
                        PossibleTilesForThis.Add(possibleTile);
                    }
                }
            }
        }

        SetUpWeights();

        Debug.Log("<color=red> Stopped seting up possibilities </color>");
    }

    public List<RoadTileConfiguration> PossibleRoadConfigsFor(RoadTileConfiguration newRoadTile)
    {
        List<RoadTileConfiguration> possibleConfigsForNewRoad = new List<RoadTileConfiguration>(100);
        List<GameEnums.RoadAsphaltType> possibleAsphaltTypesForNewRoad = PossibleAsphaltTypes(newRoadTile);
        List<GameEnums.RoadPropsType> possiblePropsTypesForNewRoad = PossiblePropsType(newRoadTile);
        List<GameEnums.RoadShapeType> possibleShapeTypesForNewRoad = PossibleShapeType(newRoadTile);

        for (int asphaltTypeCounter = 0; asphaltTypeCounter < possibleAsphaltTypesForNewRoad.Count; asphaltTypeCounter++)
        {
            for (int propsTypeCounter = 0; propsTypeCounter < possiblePropsTypesForNewRoad.Count; propsTypeCounter++)
            {
                for (int shapeTypesCounter = 0; shapeTypesCounter < possibleShapeTypesForNewRoad.Count; shapeTypesCounter++)
                {
                    RoadTileConfiguration possibleConfig = new RoadTileConfiguration();
                    possibleConfig.asphaltType = possibleAsphaltTypesForNewRoad[asphaltTypeCounter];
                    possibleConfig.propsType = possiblePropsTypesForNewRoad[propsTypeCounter];
                    possibleConfig.shapeType = possibleShapeTypesForNewRoad[shapeTypesCounter];
                    possibleConfigsForNewRoad.Add(possibleConfig);
                }
            }
        }

        return possibleConfigsForNewRoad;
    }

    public void SetUpWeights()
    {
        int From_End_totalWeight = 0;
        int From_End_totalCount = 0;
        for (int i = 0; i < PossibleTilesForThis.Count; i++)
        {
            if (PossibleTilesForThis[i].TileName.Contains("End"))
            {
                PossibleTilesForThis[i].TileWeight = 20;
                From_End_totalWeight+= 20;
                From_End_totalCount++;
            }
            if (PossibleTilesForThis[i].TileName.Contains("From"))
            {
                PossibleTilesForThis[i].TileWeight = 10;
                From_End_totalWeight+=10;
                From_End_totalCount++;
            }
        }
        for (int i = 0; i < PossibleTilesForThis.Count; i++)
        {
            if(!PossibleTilesForThis[i].TileName.Contains("End") && !PossibleTilesForThis[i].TileName.Contains("From"))
            {
                PossibleTilesForThis[i].TileWeight = (100 - From_End_totalWeight) / (PossibleTilesForThis.Count - From_End_totalCount);
            }
        }
    }

    public List<GameEnums.RoadAsphaltType> PossibleAsphaltTypes(RoadTileConfiguration newRoadTilesConfig)
    {
        List<GameEnums.RoadAsphaltType> allPossibleAsphaltTypesForNewTile = new List<GameEnums.RoadAsphaltType>(System.Enum.GetNames(typeof(GameEnums.RoadAsphaltType)).Length);
        if (newRoadTilesConfig.propsType == GameEnums.RoadPropsType.FromDesertToTown || newRoadTilesConfig.propsType == GameEnums.RoadPropsType.FromTownToDesert)
        {
            int typeCounter = 0;
            for (int i = 0; i < System.Enum.GetNames(typeof(GameEnums.RoadAsphaltType)).Length; i++)
            {
                GameEnums.RoadAsphaltType asphType = (GameEnums.RoadAsphaltType)typeCounter;
                allPossibleAsphaltTypesForNewTile.Add(asphType);
                typeCounter++;
            }
        }
        else
        {
            switch (newRoadTilesConfig.shapeType)
            {
                case GameEnums.RoadShapeType.Start:
                    allPossibleAsphaltTypesForNewTile.Add(newRoadTilesConfig.asphaltType);
                    break;
                case GameEnums.RoadShapeType.Middle:
                    allPossibleAsphaltTypesForNewTile.Add(newRoadTilesConfig.asphaltType);
                    break;
                case GameEnums.RoadShapeType.End:
                    int typeCounter = 0;
                    for (int i = 0; i < System.Enum.GetNames(typeof(GameEnums.RoadAsphaltType)).Length; i++)
                    {
                        GameEnums.RoadAsphaltType asphType = (GameEnums.RoadAsphaltType)typeCounter;
                        allPossibleAsphaltTypesForNewTile.Add(asphType);
                        typeCounter++;
                    }
                    break;
            }
            if (allPossibleAsphaltTypesForNewTile.Count == 0)
            {
                throw new System.Exception("Cannot return AsphaltType");
            }
            Debug.Log($"<color=blue> {allPossibleAsphaltTypesForNewTile} was returned with count {allPossibleAsphaltTypesForNewTile.Count} </color>");
        }
        return allPossibleAsphaltTypesForNewTile;
    }

    public List<GameEnums.RoadPropsType> PossiblePropsType(RoadTileConfiguration newRoadTilesConfig)
    {
        List<GameEnums.RoadPropsType> allPossiblePropsTypesForNewTile = new List<GameEnums.RoadPropsType>(System.Enum.GetNames(typeof(GameEnums.RoadPropsType)).Length);

        switch (newRoadTilesConfig.propsType)
        {
            case GameEnums.RoadPropsType.Town:
                allPossiblePropsTypesForNewTile.Add(GameEnums.RoadPropsType.Town);
                allPossiblePropsTypesForNewTile.Add(GameEnums.RoadPropsType.FromTownToDesert);
                break;
            case GameEnums.RoadPropsType.Desert:
                allPossiblePropsTypesForNewTile.Add(GameEnums.RoadPropsType.Desert);
                allPossiblePropsTypesForNewTile.Add(GameEnums.RoadPropsType.FromDesertToTown);
                break;
            case GameEnums.RoadPropsType.FromTownToDesert:
                allPossiblePropsTypesForNewTile.Add(GameEnums.RoadPropsType.Desert);
                break;
            case GameEnums.RoadPropsType.FromDesertToTown:
                allPossiblePropsTypesForNewTile.Add(GameEnums.RoadPropsType.Town);
                break;
        }
        if(allPossiblePropsTypesForNewTile.Count == 0)
        {
            throw new System.Exception("Cannot return PropsType");
        }
        Debug.Log($"<color=blue> {allPossiblePropsTypesForNewTile} was returned with count {allPossiblePropsTypesForNewTile.Count} </color>");

        return allPossiblePropsTypesForNewTile;
    }

    public List<GameEnums.RoadShapeType> PossibleShapeType(RoadTileConfiguration newRoadTilesConfig)
    {
        List<GameEnums.RoadShapeType> allPossibleShapeTypesForNewTile = new List<GameEnums.RoadShapeType>(System.Enum.GetNames(typeof(GameEnums.RoadShapeType)).Length);

        if (newRoadTilesConfig.propsType == GameEnums.RoadPropsType.FromDesertToTown || newRoadTilesConfig.propsType == GameEnums.RoadPropsType.FromTownToDesert)
        {
            allPossibleShapeTypesForNewTile.Add(GameEnums.RoadShapeType.Start);
        }
        else
        {
            switch (newRoadTilesConfig.shapeType)
            {
                case GameEnums.RoadShapeType.Start:
                    allPossibleShapeTypesForNewTile.Add(GameEnums.RoadShapeType.Middle);
                    break;
                case GameEnums.RoadShapeType.Middle:
                    allPossibleShapeTypesForNewTile.Add(GameEnums.RoadShapeType.Middle);
                    allPossibleShapeTypesForNewTile.Add(GameEnums.RoadShapeType.End);
                    break;
                case GameEnums.RoadShapeType.End:
                    allPossibleShapeTypesForNewTile.Add(GameEnums.RoadShapeType.Start);
                    break;
            }
        }
            
        if(allPossibleShapeTypesForNewTile.Count == 0)
        {
            throw new System.Exception("Cannot return ShapeType");
        }
        Debug.Log($"<color=blue> {allPossibleShapeTypesForNewTile} was returned with count {allPossibleShapeTypesForNewTile.Count} </color>");

        return allPossibleShapeTypesForNewTile;
    }
}

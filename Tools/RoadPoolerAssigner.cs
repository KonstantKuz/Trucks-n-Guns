using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPoolerAssigner : MonoBehaviour
{
    public RoadTile[] tilesToSetUP;
    public ObjectPoolerBase roadPooler;

    public int startTilesSize;
    public int endTilesSize;
    public int middleTilesSize;

    [ContextMenu("Assign Road Prefabs to RoadPooler")]
    public void SetUpRoadPooler()
    {
        roadPooler.pools.Clear();

        for (int i = 0; i < tilesToSetUP.Length; i++)
        {
            ObjectPoolerBase.Pool newRoadTilePool = new ObjectPoolerBase.Pool();
            newRoadTilePool.prefab = tilesToSetUP[i].gameObject;
            newRoadTilePool.tag = tilesToSetUP[i].name;
            switch (tilesToSetUP[i].roadTileConfiguration.shapeType)
            {
                case GameEnums.RoadShapeType.Start:
                    newRoadTilePool.size = startTilesSize;
                    break;
                case GameEnums.RoadShapeType.Middle:
                    newRoadTilePool.size = middleTilesSize;
                    break;
                case GameEnums.RoadShapeType.End:
                    newRoadTilePool.size = endTilesSize;
                    break;
            }
            roadPooler.pools.Add(newRoadTilePool);
            //UnityEditor.AssetDatabase.SaveAssets();
        }

    }
}

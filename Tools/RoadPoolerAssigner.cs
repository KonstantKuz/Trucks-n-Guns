using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPoolerAssigner : MonoBehaviour
{
    public RoadTile[] tilesToSetUP;
    public ObjectPoolerBase roadPooler;

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
                    newRoadTilePool.size = 1;
                    break;
                case GameEnums.RoadShapeType.Middle:
                    newRoadTilePool.size = 3;
                    break;
                case GameEnums.RoadShapeType.End:
                    newRoadTilePool.size = 1;
                    break;
            }
            roadPooler.pools.Add(newRoadTilePool);
        }
    }
}

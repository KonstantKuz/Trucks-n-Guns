using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

public class RoadHandler : MonoCached
{
    [SerializeField]
    private float roadLength;

    public float playerPosCheckPeriod;
    public float distanceFromPlayerToMakeNewRoadComplex;

    private float allSpawnedRoadLength;

    private ObjectPoolerBase roadPooler;

    private PathHandler pathHandler;

    private void Awake()
    {
        roadPooler = ObjectPoolersHolder.Instance.RoadPooler;
    }
    public void InjectPathHandler(PathHandler pathHandler)
    {
        this.pathHandler = pathHandler;
    }
   
    public void StartRoadHandle(Rigidbody player_rigidbody)
    {
        StartCoroutine(RoadHandle(player_rigidbody, playerPosCheckPeriod));
    }
    private IEnumerator RoadHandle(Rigidbody player_rigidbody, float period = 2f)
    {
        yield return new WaitForSecondsRealtime(period);
        if (player_rigidbody.position.z > (allSpawnedRoadLength - distanceFromPlayerToMakeNewRoadComplex))
        {
            MakeNewRoadComplex();
        }


        yield return StartCoroutine(RoadHandle(player_rigidbody, period));
    }

    private void MakeNewRoadComplex()
    {
        GameObject generalRoad = TranslateGeneralRoad();
        TranslatePathGrid(generalRoad.transform.position);
    }
    private RoadTile previousRoadTile;

    private GameObject TranslateGeneralRoad()
    {
        GameObject generalRoad;
        if (previousRoadTile == null)
        {
            generalRoad = roadPooler.Spawn("TDR_Begin_1",Vector3.zero, Quaternion.identity);
        }
        else
        {
            generalRoad = roadPooler.Spawn(previousRoadTile.NextRoadTileCapabilitiesHolder.GetNextRandomWeightedRoadTileName());
        }
        previousRoadTile = generalRoad.GetComponent<RoadTile>();
        previousRoadTile.ResetEnv(allSpawnedRoadLength);
        generalRoad.transform.localPosition = Vector3.forward * allSpawnedRoadLength;
        allSpawnedRoadLength += roadLength;

        return generalRoad;
    }
    
    private void TranslatePathGrid(Vector3 newRoadPos)
    {
        pathHandler.StartTranslateGrid(newRoadPos);
    }
   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadHandler : MonoBehaviour
{
    [SerializeField]
    private float roadLength;

    public float playerPosCheckPeriod;
    public float distanceFromPlayerToMakeNewRoadComplex;

    private float allSpawnedRoadLength;

    private ObjectPoolerBase roadPooler;

    private PathHandler pathHandler;

    private RoadTile previousRoadTile;

    private Bounds nextWorldBounds;

    private Rigidbody player_rigidbody;

    private void Awake()
    {
        roadPooler = ObjectPoolersHolder.Instance.RoadPooler;

        nextWorldBounds = new Bounds
        {
            center = transform.position,
            extents = new Vector3(100, 10, 800)
        };
    }
    public void InjectPathHandler(PathHandler pathHandler)
    {
        this.pathHandler = pathHandler;
    }
   
    public void StartRoadHandle(Rigidbody player_rigidbody)
    {
        this.player_rigidbody = player_rigidbody;
        StartCoroutine(RoadHandle());
    }
    private IEnumerator RoadHandle()
    {
        yield return new WaitForSecondsRealtime(playerPosCheckPeriod);
        if (player_rigidbody.position.z > (allSpawnedRoadLength - distanceFromPlayerToMakeNewRoadComplex))
        {
            MakeNewRoadComplex();
        }
        yield return StartCoroutine(RoadHandle());
    }

    private void MakeNewRoadComplex()
    {
        GameObject generalRoad = TranslateGeneralRoad();
        TranslatePathGrid(generalRoad.transform.position);
    }

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

            ///for cinema
            
        }
        previousRoadTile = generalRoad.GetComponent<RoadTile>();
        previousRoadTile.ResetPhysicalEnvironment(allSpawnedRoadLength);
        generalRoad.transform.localPosition = Vector3.forward * allSpawnedRoadLength;

        //roadPooler.Spawn(previousRoadTile.NextRoadTileCapabilitiesHolder.GetNextRandomWeightedRoadTileName()).transform.localPosition = generalRoad.transform.localPosition + Vector3.right * 81.45f;
        //roadPooler.Spawn(previousRoadTile.NextRoadTileCapabilitiesHolder.GetNextRandomWeightedRoadTileName()).transform.localPosition = generalRoad.transform.localPosition - Vector3.right * 81.45f;

        //roadPooler.Spawn(previousRoadTile.NextRoadTileCapabilitiesHolder.GetNextRandomWeightedRoadTileName()).transform.localPosition = generalRoad.transform.localPosition + Vector3.right * 81.45f * 2;
        //roadPooler.Spawn(previousRoadTile.NextRoadTileCapabilitiesHolder.GetNextRandomWeightedRoadTileName()).transform.localPosition = generalRoad.transform.localPosition - Vector3.right * 81.45f * 2;

        allSpawnedRoadLength += roadLength;

        StartCoroutine(PhysicsBoundariesRebuild());

        return generalRoad;
    }

    private IEnumerator PhysicsBoundariesRebuild()
    {
        yield return new WaitForFixedUpdate();

        nextWorldBounds.center = player_rigidbody.position;
        Physics.RebuildBroadphaseRegions(nextWorldBounds, 2);
    }
    
    private void TranslatePathGrid(Vector3 newRoadPos)
    {
        pathHandler.StartTranslateGrid(newRoadPos);
    }
   
}

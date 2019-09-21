using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadHandler : MonoCached, INeedObjectPooler
{

    public string generalRoadNameToSpawn;
    public float roadLength;
    private float allSpawnedRoadLength;

    public int roadGridSize;
    public int roadTroubleCount;
    public int maxRoadTroubleCount;
    public float countIncrementPeriod;

    public float playerPosCheckPeriod;
    public float distanceFromPlayerToMakeNewRoadComplex;
    public float xOffsetFromRoad;

    public Vector3[,] roadMapPoints { get; private set; }

    ObjectPoolerBase roadPooler;

    PathHandler pathHandler;

    public void InjectPathHandler(PathHandler pathHandler)
    {
        this.pathHandler = pathHandler;
    }
    public void InjectNeededPooler(ObjectPoolerBase objectPooler)
    {
        roadPooler = objectPooler;
    }

    public void CreateRoadGrid()
    {
        roadMapPoints = new Vector3[roadGridSize, roadGridSize];
    }

    public void SetRoadTroubleCount(int count)
    {
        roadTroubleCount = count;
    }

    public void StartIncrementRoadTroubleCount()
    {
        StartCoroutine(IncrementRoadTroubleCount(countIncrementPeriod));
    }
    private IEnumerator IncrementRoadTroubleCount(float countIncrementPeriod)
    {
        yield return new WaitForSeconds(countIncrementPeriod*2.5f);
        if(roadTroubleCount < maxRoadTroubleCount)
        {
            roadTroubleCount++;
        }
        yield return StartCoroutine(IncrementRoadTroubleCount(countIncrementPeriod));
    }

    public void StartPlayerPositionCheckForMakeRoadComplex(Rigidbody player_rigidbody)
    {
        StartCoroutine(PlayerPositionCheckForMakeRoadComplex(player_rigidbody, playerPosCheckPeriod));
    }
    private IEnumerator PlayerPositionCheckForMakeRoadComplex(Rigidbody player_rigidbody, float period = 2f)
    {
        yield return new WaitForSeconds(period);
        if (player_rigidbody.position.z > (allSpawnedRoadLength - distanceFromPlayerToMakeNewRoadComplex))
        {
            MakeNewRoadComplex();
        }
        yield return StartCoroutine(PlayerPositionCheckForMakeRoadComplex(player_rigidbody, period));
    }

    private void MakeNewRoadComplex()
    {
        GameObject generalRoad = roadPooler.SpawnFromPool(generalRoadNameToSpawn);
        generalRoad.transform.localPosition = Vector3.forward * allSpawnedRoadLength;
        allSpawnedRoadLength += roadLength;
        TranslateRoadGrid(generalRoad.transform);
        TranslateRoadTroubles();
        TranslatePathGrid(generalRoad.transform.position);
        generalRoad.GetComponent<CanyonElementsRotator>().RotateCanyonElements();
    }

    private void TranslateRoadGrid(Transform generalRoadTransform)
    {
        Vector3 roadCenter = generalRoadTransform.position;

        for (int i = 1; i < roadGridSize + 1; i++)
        {
            for (int j = 1; j < roadGridSize + 1; j++)
            {
                roadMapPoints[i - 1, j - 1].x = (roadCenter.x - (roadLength / roadGridSize) / 2) + i * (roadLength / roadGridSize) + xOffsetFromRoad;
                roadMapPoints[i - 1, j - 1].z = (roadCenter.z - (roadLength / roadGridSize) / 2) + j * (roadLength / roadGridSize);
            }
        }
    }

    private void TranslateRoadTroubles()
    {
        for (int i = 0; i < roadTroubleCount; i++)
        {
            Vector3 randomPosition = roadMapPoints[GetRandomIndex(roadGridSize), GetRandomIndex(roadGridSize)];
            GameObject roadTrouble = roadPooler.SpawnRandomItemFromPool(randomPosition, Quaternion.identity, 0);
            roadTrouble.transform.rotation = Quaternion.AngleAxis(Random.Range(50, 280), transform.up);
        }
    }
    private int GetRandomIndex(int length)
    {
        return Random.Range(0, length);
    }
    
    private void TranslatePathGrid(Vector3 newRoadPos)
    {
        pathHandler.StartTranslateGrid(newRoadPos);
    }
   

    private void OnDrawGizmos()
    {
        if (roadMapPoints != null)
        {
            for (int i = 0; i < roadGridSize; i++)
            {
                for (int j = 0; j < roadGridSize; j++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(roadMapPoints[i, j], 0.5f);
                }
            }
        }
    }

}

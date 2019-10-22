using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

public class RoadHandler : MonoCached
{
    [SerializeField]
    private string generalRoadNameToSpawn;
    [SerializeField]
    private float roadLength;
    [SerializeField]
    private int roadGridSize;
    [SerializeField]
    private Vector3 gridOffset;
    
    public int roadTroubleCount;
    public int maxRoadTroubleCount;
    public float countIncrementPeriod;

    public float playerPosCheckPeriod;
    public float distanceFromPlayerToMakeNewRoadComplex;

    private float allSpawnedRoadLength;

    public Vector3[,] roadMapPoints { get; private set; }

    private List<Collider> obstacleColliders = new List<Collider>(100);

    private ObjectPoolerBase roadPooler;

    private PathHandler pathHandler;

    private void Awake()
    {
        roadPooler = ObjectPoolersHolder.Instance.RoadPooler;
        SetUpObstacleHandle();
    }
    public void InjectPathHandler(PathHandler pathHandler)
    {
        this.pathHandler = pathHandler;
        SetUpPathHandler();
    }
    public void SetUpPathHandler()
    {
        pathHandler.pathGridWidth = roadGridSize;
        pathHandler.roadLength = roadLength;
    }

    public void CreateRoadGrid()
    {
        roadMapPoints = new Vector3[roadGridSize, roadGridSize];
    }

    public void SetUpObstacleHandle()
    {
        for (int i = 0; i < roadPooler.pools.Count; i++)
        {
            if(roadPooler.pools[i].tag == generalRoadNameToSpawn)
            {
                continue;
            }

            GameObject[] obstacles = roadPooler.poolDictionary[roadPooler.pools[i].tag].ToArray();
            for (int j = 0; j < obstacles.Length; j++)
            {
                for (int n = 0; n < obstacles[j].GetComponents<Collider>().Length; n++)
                {
                    obstacleColliders.Add(obstacles[j].GetComponents<Collider>()[n]);
                }
            }
        }
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

    public void StartRoadHandle(Rigidbody player_rigidbody)
    {
        StartCoroutine(RoadHandle(player_rigidbody, playerPosCheckPeriod));
    }
    private IEnumerator RoadHandle(Rigidbody player_rigidbody, float period = 2f)
    {
        yield return new WaitForSeconds(period);
        if (player_rigidbody.position.z > (allSpawnedRoadLength - distanceFromPlayerToMakeNewRoadComplex))
        {
            MakeNewRoadComplex();
            ObstacleHandle(player_rigidbody);
        }


        yield return StartCoroutine(RoadHandle(player_rigidbody, period));
    }

    public void ObstacleHandle(Rigidbody player_rigidbody)
    {
        for (int i = 0; i < obstacleColliders.Count; i++)
        {
            float distance = player_rigidbody.position.z - obstacleColliders[i].transform.position.z;
            if (distance > 50)
            {
                obstacleColliders[i].enabled = false;
            }
            else
            {
                obstacleColliders[i].enabled = true;
            }
        }

    }

    private void MakeNewRoadComplex()
    {
        GameObject generalRoad = TranslateGeeneralRoad();
        
        TranslateRoadGrid(generalRoad.transform);
        TranslateRoadTroubles();
        TranslatePathGrid(generalRoad.transform.position);
    }

    private GameObject TranslateGeeneralRoad()
    {
        GameObject generalRoad = roadPooler.SpawnFromPool(generalRoadNameToSpawn);
        generalRoad.transform.localPosition = Vector3.forward * allSpawnedRoadLength;
        allSpawnedRoadLength += roadLength;
        generalRoad.GetComponent<CanyonElementsRotator>().RotateCanyonElements();

        return generalRoad;
    }

    private void TranslateRoadGrid(Transform generalRoadTransform)
    {
        for (int i = 0; i < roadGridSize; i++)
        {
            for (int j = 0; j < roadGridSize; j++)
            {
                roadMapPoints[i, j] = generalRoadTransform.position 
                    - Vector3.right * ((roadLength / roadGridSize) / 2 + i * (roadLength / roadGridSize) + gridOffset.x) 
                    - Vector3.forward * ((roadLength / roadGridSize) / 2 + j * (roadLength / roadGridSize) + gridOffset.z);
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

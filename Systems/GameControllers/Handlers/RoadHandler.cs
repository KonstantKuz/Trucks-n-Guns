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
    
    [Range(0,4)]
    public int obstaclesCount;
    [Range(0, 4)]
    public int maxObstaclesCount;
    [Range(10, 30)]
    public float countIncrementPeriod;

    public float playerPosCheckPeriod;
    public float distanceFromPlayerToMakeNewRoadComplex;

    private float allSpawnedRoadLength;

    public Vector3[,] roadMapPoints { get; private set; }

    private List<Collider> obstacleColliders = new List<Collider>(200);

    private ObjectPoolerBase obstaclePooler;
    private ObjectPoolerBase roadPooler;

    private PathHandler pathHandler;

    private void Awake()
    {
        obstaclePooler = ObjectPoolersHolder.Instance.ObstaclePooler;
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
        for (int i = 0; i < obstaclePooler.pools.Count; i++)
        {
            if(obstaclePooler.pools[i].tag == generalRoadNameToSpawn)
            {
                continue;
            }

            GameObject[] obstacles = obstaclePooler.poolDictionary[obstaclePooler.pools[i].tag].ToArray();
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
        obstaclesCount = count;
    }

    public void StartIncrementRoadTroubleCount()
    {
        StartCoroutine(IncrementRoadTroubleCount(countIncrementPeriod));
    }
    private IEnumerator IncrementRoadTroubleCount(float countIncrementPeriod)
    {
        yield return new WaitForSeconds(countIncrementPeriod*2.5f);
        if(obstaclesCount < maxObstaclesCount)
        {
            obstaclesCount++;
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
            ObstaclesColliderHandle(player_rigidbody);
        }


        yield return StartCoroutine(RoadHandle(player_rigidbody, period));
    }

    public void ObstaclesColliderHandle(Rigidbody player_rigidbody)
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
        GameObject generalRoad = TranslateGeneralRoad();
        
        TranslateRoadGrid(generalRoad.transform);
        TranslateObstacles();
        TranslatePathGrid(generalRoad.transform.position);
    }

    private GameObject TranslateGeneralRoad()
    {
        GameObject generalRoad = roadPooler.Spawn(generalRoadNameToSpawn);
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

    private void TranslateObstacles()
    {
        for (int i = 0; i < obstaclesCount; i++)
        {
            Vector3 randomPosition = roadMapPoints[GetRandomIndex(roadGridSize), GetRandomIndex(roadGridSize)];
            GameObject obstacle = obstaclePooler.SpawnWeightedRandom(randomPosition, Quaternion.identity);

            obstacle.transform.rotation = Quaternion.AngleAxis(Random.Range(50, 280), transform.up);
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

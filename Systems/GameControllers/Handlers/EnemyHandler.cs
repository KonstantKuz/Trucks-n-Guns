using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;
using Pathfinding;
public class EnemyHandler : MonoBehaviour
{
    List<Enemy> currentSessionEnemies = new List<Enemy>(15);

    public List<Enemy> CurrentSessionEnemies { get { return currentSessionEnemies; } }


    public float distanceToDestroyEnemy = 60f;
    [Range(0.5f,2)]
    public float enemiesPositionsCheckPeriod = 5f;
    [Range(1, 5)]
    public float enemySpawnEveryPeriodTime = 15f;
    [Range(0, 3)]
    public int maxCurrentSessionEnemiesCount = 3;
    private int enemiesPoolCount;
    private ObjectPoolerBase enemyPooler;
    public int CurrentSessionEnemiesCount
    {
        get { return currentSessionEnemies.Count; }
    }
    private PathHandler pathHandler;
    private Rigidbody player_rigidbody;

    private void Awake()
    {
        Enemy.CreateStateDictionary();
        enemyPooler = ObjectPoolersHolder.Instance.EnemyPooler;
    }
   
    public void InjectPathHandler(PathHandler pathHandler)
    {
        this.pathHandler = pathHandler;
    }


    public void StartCheckingEnemiesPositions(Rigidbody player_rigidbody)
    {
        this.player_rigidbody = player_rigidbody;
        StartCoroutine(CheckingEnemiesPosition());
    }

    private IEnumerator CheckingEnemiesPosition()
    {
        yield return new WaitForSecondsRealtime(enemiesPositionsCheckPeriod);

        for (int i = 0; i < currentSessionEnemies.Count; i++)
        {
            var enemy = currentSessionEnemies[i];
            var distance = (player_rigidbody.position - enemy.transform.position).magnitude;
            if(enemy.truck.trucksCondition.currentCondition<0.95f)
            {
                currentSessionEnemies.Remove(enemy);
            }
            if (distance > distanceToDestroyEnemy)
            {
                enemy.ReturnObjectsToPool();
                enemy.gameObject.SetActive(false);
                currentSessionEnemies.Remove(enemy);
            }

            //new delayed 171719
            yield return new WaitForEndOfFrame();
        }
        yield return StartCoroutine(CheckingEnemiesPosition());
    }

    public void StartSpawnRandomEnemyEveryPeriod(Rigidbody player_rigidbody)
    {
        this.player_rigidbody = player_rigidbody;
        StartCoroutine(SpawnRandomEnemyEveryPeriod());
        StartCoroutine(IncreaseMaxCount());
    }
    private IEnumerator SpawnRandomEnemyEveryPeriod()
    {
        yield return new WaitForSecondsRealtime(enemySpawnEveryPeriodTime);
        if (currentSessionEnemies.Count < maxCurrentSessionEnemiesCount)
        {
            SpawnRandomEnemy();
        }
        yield return StartCoroutine(SpawnRandomEnemyEveryPeriod());
    }
    private IEnumerator IncreaseMaxCount()
    {
        yield return new WaitForSecondsRealtime(10f);
        if(maxCurrentSessionEnemiesCount<3)
        {
            maxCurrentSessionEnemiesCount++;
            yield return StartCoroutine(IncreaseMaxCount());
        }
        else
        {
            StopCoroutine(IncreaseMaxCount());
        }
    }
    public void SpawnRandomEnemy()
    {
        GameObject enemy = enemyPooler.SpawnRandom(RandomPositionNearPlayer(), Quaternion.identity);
        SetUpEnemyAndLaunch(enemy);
    }
    
    private void SetUpEnemyAndLaunch(GameObject enemyToLaunch)
    {
        var enemy = enemyToLaunch.GetComponent<Enemy>();
        enemy.AwakeEnemy();
        enemy.RandomizeData();
        var enemyTransform = enemy.truck._transform;
        currentSessionEnemies.Add(enemy);
        enemy.InjectNewTargetData(player_rigidbody);
        UpdateEnemyPath(enemyToLaunch.GetComponent<Enemy>());
    }

    private Vector3 RandomPositionNearPlayer()
    {
        int randomX = Random.Range(-pathHandler.pathGridWidth / 2, pathHandler.pathGridWidth / 2);
        Vector3 newPos = player_rigidbody.position;
        newPos.z -= 70;
        newPos.x = randomX;
        return newPos;
    }

    public static int EnemyConditionCalculatedFromPlayerLevelAndComplexity()
    {
        int playerFPType = (int)PlayerHandler.PlayerInstance.truck.TruckData.firePointType;
        int complexityLvl = (int)PlayerSessionHandler.SessionComplexity;

        if (playerFPType == 0)
        {
            return Random.Range(5000 + complexityLvl * 1000, 20000 + complexityLvl * 5000);
        }
        if (playerFPType == 1)
        {
            return Random.Range(10000 + complexityLvl*5000, 30000 + complexityLvl*10000);
        }
        if (playerFPType == 3)
        {
            return Random.Range(30000 + complexityLvl * 10000, 50000 + complexityLvl * 20000);
        }
        if (playerFPType == 7)
        {
            return Random.Range(50000 + complexityLvl * complexityLvl * 25000, 70000 + complexityLvl *complexityLvl* 25000);
        }

        return 0;
    }

    public static int[] PossibleGunDataTypesFromComplexity()
    {
        int[] gundataTypes = { 111, 121, 131, 211, 221, 231, 311, 321, 331, 112, 122, 132, 212, 222, 232, 312, 322, 332, 113, 123, 133, 213, 223, 233, 313, 323, 333 };
        List<int> typesToReturn = new List<int>(20);

        for (int i = 0; i < gundataTypes.Length; i++)
        {
            char[] typesTiles = gundataTypes[i].ToString().ToCharArray();
            int summOfTiles = 0;

            for (int j = 0; j < typesTiles.Length; j++)
            {
                summOfTiles += typesTiles[j] - 48;
            }
            switch (PlayerSessionHandler.SessionComplexity)
            {
                case GameEnums.SessionComplexity.Low:
                    if (summOfTiles <= 5)
                        typesToReturn.Add(gundataTypes[i]);
                    break;
                case GameEnums.SessionComplexity.Medium:
                    if (summOfTiles > 5 && summOfTiles < 7)
                        typesToReturn.Add(gundataTypes[i]);
                    break;
                case GameEnums.SessionComplexity.High:
                    if (summOfTiles >= 7)
                        typesToReturn.Add(gundataTypes[i]);
                    break;
            }
        }

        return typesToReturn.ToArray();
    }

    public static int RandomFirePointTypeFromComplexity()
    {
        int playersFirePointType = (int)PlayerHandler.PlayerInstance.truck.TruckData.firePointType;

        switch (PlayerSessionHandler.SessionComplexity)
        {
            case GameEnums.SessionComplexity.Low:
                return Random.Range(playersFirePointType - 2, playersFirePointType);
            case GameEnums.SessionComplexity.Medium:
                return Random.Range(playersFirePointType, playersFirePointType + 2);
            case GameEnums.SessionComplexity.High:
                return Random.Range(playersFirePointType + 2, 10);
        }

        return 3;
    }

    public void StartUpdateEnemiesPaths(PathHandler pathHandler)
    {
        StartCoroutine(SlowUpdateEnemiesPaths(pathHandler));
    }

    private IEnumerator SlowUpdateEnemiesPaths(PathHandler pathHandler)
    {
        if (currentSessionEnemies != null && currentSessionEnemies.Count > 0)
        {
            for (int i = 0; i < currentSessionEnemies.Count; i++)
            {
                Transform enemyTruck_transform = currentSessionEnemies[i].truck._transform;
                Transform pathsEndPoints = pathHandler.pathsEndPoints[i];
                List<Node> newPathForEnemy = pathHandler.FindPath(enemyTruck_transform.position + enemyTruck_transform.forward, pathsEndPoints.position);

                currentSessionEnemies[i].ReTracePath(newPathForEnemy);

                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void UpdateEnemyPath(Enemy enemy)
    {
        Transform enemyTruck_transform = enemy.truck._transform;
        Transform pathsEndPoints = pathHandler.pathsEndPoints[Random.Range(0,pathHandler.pathsEndPoints.Length)];
        List<Node> newPathForEnemy = pathHandler.FindPath(enemyTruck_transform.position + enemyTruck_transform.forward, pathsEndPoints.position);

        enemy.ReTracePath(newPathForEnemy);
    }

    public void GameOver()
    {
        for (int i = 0; i < currentSessionEnemies.Count; i++)
        {
            currentSessionEnemies[i].GetComponent<EntityCondition>().AddDamage(Mathf.Infinity);
        }
    }

    public void AddEnemyPlayerToCurrentSession(Enemy playerEnemy)
    {
        currentSessionEnemies.Add(playerEnemy);
    }
    
}

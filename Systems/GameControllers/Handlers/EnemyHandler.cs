using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;
using Pathfinding;
public class EnemyHandler : MonoCached
{
    List<Enemy> currentSessionEnemies = new List<Enemy>(15);

    public List<Enemy> CurrentSessionEnemies { get { return currentSessionEnemies; } }


    public float distanceToDestroyEnemy = 60f;
    [Range(2,6)]
    public float enemiesPositionsCheckPeriod = 5f;
    [Range(5, 15)]
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
        }
        yield return StartCoroutine(CheckingEnemiesPosition());
    }

    public void StartSpawnRandomEnemyEveryPeriod(Rigidbody player_rigidbody)
    {
        this.player_rigidbody = player_rigidbody;
        StartCoroutine(SpawnRandomEnemyEveryPeriod());
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
        newPos.z -= 60;
        newPos.x = randomX;
        return newPos;
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

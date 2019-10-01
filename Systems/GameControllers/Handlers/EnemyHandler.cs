using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;
using Pathfinding;
public class EnemyHandler : MonoCached, INeedObjectPooler
{
    List<Enemy> currentSessionEnemies = new List<Enemy>(15);

    public List<Enemy> CurrentSessionEnemies { get { return currentSessionEnemies; } }


    public float distanceToDestroyEnemy = 60f;
    public float enemiesPositionsCheckPeriod = 5f;
    public float enemySpawnEveryPeriodTime = 15f;
    public int maxCurrentSessionEnemiesCount = 4;
    private int enemiesPoolCount;
    private ObjectPoolerBase enemyPooler;
    public int CurrentSessionEnemiesCount
    {
        get { return currentSessionEnemies.Count; }
    }
    private PathHandler pathHandler;

    public void InjectNeededPooler(ObjectPoolerBase objectPooler)
    {
        enemyPooler = objectPooler;
        enemiesPoolCount = enemyPooler.pools.Count;
    }
   
    public void InjectPathHandler(PathHandler pathHandler)
    {
        this.pathHandler = pathHandler;
    }
    public void StartCheckingEnemiesPositions(Rigidbody player_rigidbody)
    {
        StartCoroutine(CheckingEnemiesPosition(player_rigidbody));
    }

    private IEnumerator CheckingEnemiesPosition(Rigidbody player_rigidbody)
    {
        yield return new WaitForSeconds(enemiesPositionsCheckPeriod);

        for (int i = 0; i < currentSessionEnemies.Count; i++)
        {
            var distance = (player_rigidbody.position - currentSessionEnemies[i].transform.position).magnitude;
            if (distance > distanceToDestroyEnemy)
            {
                currentSessionEnemies[i].gameObject.SetActive(false);
                currentSessionEnemies.Remove(currentSessionEnemies[i]);
            }
        }
        yield return StartCoroutine(CheckingEnemiesPosition(player_rigidbody));
    }

    public void StartIncrementMaxEnemiesCount(float period)
    {
        StartCoroutine(IncrementMaxEnemiesCount(period));
    }

    private IEnumerator IncrementMaxEnemiesCount(float period)
    {
        yield return new WaitForSeconds(period);
        maxCurrentSessionEnemiesCount++;
        StartCoroutine(IncrementMaxEnemiesCount(period * 2f));

    }

    public void StartSpawnAllEnemiesEveryPeriod(Rigidbody player_rigidbody)
    {
        StartCoroutine(SpawnAllEnemiesEveryPeriod(player_rigidbody));
    }

    private IEnumerator SpawnAllEnemiesEveryPeriod(Rigidbody player_rigidbody)
    {
        yield return new WaitForSeconds(enemySpawnEveryPeriodTime);
        if (currentSessionEnemies.Count < maxCurrentSessionEnemiesCount)
        {
            SpawnAllEnemies(player_rigidbody);
        }
        yield return StartCoroutine(SpawnRandomEnemyEveryPeriod(player_rigidbody));
    }

    public void StartSpawnRandomEnemyEveryPeriod(Rigidbody player_rigidbody)
    {
        StartCoroutine(SpawnRandomEnemyEveryPeriod(player_rigidbody));
    }
    private IEnumerator SpawnRandomEnemyEveryPeriod(Rigidbody player_rigidbody)
    {
        yield return new WaitForSeconds(enemySpawnEveryPeriodTime);
        if (currentSessionEnemies.Count < maxCurrentSessionEnemiesCount)
        {
            SpawnRandomEnemy(player_rigidbody);
        }
        yield return StartCoroutine(SpawnRandomEnemyEveryPeriod(player_rigidbody));
    }
    public void SpawnRandomEnemy(Rigidbody player_rigidbody)
    {
        GameObject enemy = enemyPooler.SpawnRandomItemFromPool(RandomPositionNearPlayer(player_rigidbody), Quaternion.identity, 100);
        SetUpEnemyAndLaunch(enemy, player_rigidbody);
    }

    public void SpawnAllEnemies(Rigidbody player_rigidbody)
    {
        for (int i = 0; i < enemiesPoolCount; i++)
        {
            GameObject enemy = enemyPooler.SpawnFromPool(enemyPooler.tags[i], RandomPositionNearPlayer(player_rigidbody), Quaternion.identity);
            SetUpEnemyAndLaunch(enemy, player_rigidbody);
        }
    }

    private void SetUpEnemyAndLaunch(GameObject enemyToLaunch, Rigidbody player_rigidbody)
    {
        var enemy = enemyToLaunch.GetComponent<Enemy>();
        enemy.AwakeEnemy();
        var enemyTransform = enemy.truck._transform;
        currentSessionEnemies.Add(enemy);
        enemy.InjectNewTargetData(player_rigidbody);
        UpdateEnemyPath(enemyToLaunch.GetComponent<Enemy>());
    }
    
    private Vector3 RandomPositionNearPlayer(Rigidbody player_rigidbody)
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

                yield return new WaitForSeconds(0.5f);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;
public class ObjectPoolersHolder : Singleton<ObjectPoolersHolder>
{
    
    [SerializeField]
    ObjectPoolerBase battleUnitPooler, roadPooler, enemyPooler, effectPooler, eventPooler, trucksFirePointPooler, roadBlocksFirePointPooler, gunsPooler, trucksPooler;

    public ObjectPoolerBase BattleUnitPooler { get { return battleUnitPooler; } }

    public ObjectPoolerBase RoadPooler { get { return roadPooler; } }

    public ObjectPoolerBase EnemyPooler{ get { return enemyPooler; } }

    public ObjectPoolerBase EffectPooler { get { return effectPooler; } }

    public ObjectPoolerBase EventPooler { get { return eventPooler; } }

    public ObjectPoolerBase TrucksFirePointPooler { get { return trucksFirePointPooler; } }

    public ObjectPoolerBase GunsPooler { get { return gunsPooler; } }

    public ObjectPoolerBase TrucksPooler { get { return trucksPooler; } }

    //inactive
    public ObjectPoolerBase RoadBlocksFirePointPooler { get { return roadBlocksFirePointPooler; } }


    private void OnEnable()
    {
        AwakeGeneralGameStatePoolers();
    }

    public void AwakeGeneralGameStatePoolers()
    {
        Data.objectPoolersHolder = this;
        
        battleUnitPooler.AwakePooler();
        roadPooler.AwakePooler();
        effectPooler.AwakePooler();
        eventPooler.AwakePooler();
        trucksFirePointPooler.AwakePooler();
        roadBlocksFirePointPooler.AwakePooler();
        gunsPooler.AwakePooler();
        trucksPooler.AwakePooler();
        enemyPooler.AwakePooler();
    }

    public void AwakeCustomizationGameStatePooler()
    {
        Data.objectPoolersHolder = this;

        trucksFirePointPooler.AwakePooler();
        gunsPooler.AwakePooler();
        trucksPooler.AwakePooler();
    }

    // [System.Serializable]
    // public class Pool
    // {
    //     public string tag;
    //     public GameObject prefab;
    //     public int size;
    // }

    // public List<Pool> pools;
    // public Dictionary<string, Queue<GameObject>> poolDictionary;
    // void Start()
    // {
    //     poolDictionary = new Dictionary<string, Queue<GameObject>>();
    //     foreach (Pool pool in pools)
    //     {
    //         Queue<GameObject> objectPool = new Queue<GameObject>();

    //         for (int i = 0; i < pool.size; i++)
    //         {
    //              GameObject instatObj = Instantiate(pool.prefab);
    //                 instatObj.SetActive(false);
    //                 objectPool.Enqueue(instatObj);

    //         }
    //         poolDictionary.Add(pool.tag, objectPool);
    //     }
    // }
    //public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    //{
    //     if(!poolDictionary.ContainsKey(tag))
    //     {
    //         return null;
    //     }

    //     GameObject objToSpawn = poolDictionary[tag].Dequeue();

    //     objToSpawn.SetActive(true);
    //     objToSpawn.transform.position = position;
    //     objToSpawn.transform.rotation = rotation;

    //     poolDictionary[tag].Enqueue(objToSpawn);

    //     return objToSpawn;
    // }

    // public GameObject SpawnFromPool(string tag)
    // {
    //     if (!poolDictionary.ContainsKey(tag))
    //     {
    //         return null;
    //     }

    //     GameObject objToSpawn = poolDictionary[tag].Dequeue();

    //     objToSpawn.SetActive(true);

    //     poolDictionary[tag].Enqueue(objToSpawn);

    //     return objToSpawn;
    // }



    // //???
    // public void AddNewPool(GameObject prefab, int size, string tag)
    // {
    //     Pool newPool = new Pool
    //     {
    //         prefab = prefab,
    //         size = size,
    //         tag = tag
    //     };

    //     pools.Add(newPool);

    //     Queue<GameObject> newObjectPool = new Queue<GameObject>();
    //     for (int i = 0; i < newPool.size; i++)
    //     {
    //         GameObject instatntObj = Instantiate(newPool.prefab);
    //         instatntObj.SetActive(false);
    //         newObjectPool.Enqueue(instatntObj);
    //     }
    //     poolDictionary.Add(newPool.tag, newObjectPool);
    // }
    // public void RemovePool(string tag)
    // {
    //     for (int i = 0; i < pools.Count; i++)
    //     {
    //         if (pools[i].tag == tag)
    //         {
    //             for (int j = 0; j < pools[i].size; j++)
    //             {
    //                 Destroy(SpawnFromPool(tag));
    //             }
    //             poolDictionary[tag].Clear();
    //         }
    //     }
    // }
}

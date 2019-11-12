using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;
public class ObjectPoolersHolder : Singleton<ObjectPoolersHolder>
{
        public ObjectPoolerBase 
        BattleUnitPooler, 
        ObstaclePooler, 
        RoadPooler,
        EnemyPooler, 
        EffectPooler, 
        EventPooler, 
        TrucksFirePointPooler, 
        RoadBlocksFirePointPooler, 
        HelicoptersFirePointPooler, 
        GunsPooler, 
        TrucksPooler,
        StoneBlockagePooler;
    
    public void AwakeGeneralGameStatePoolers()
    {
        Data.ObjectPoolersHolder = this;
        
        BattleUnitPooler.AwakePooler();
        RoadPooler.AwakePooler();
        ObstaclePooler.AwakePooler();
        StoneBlockagePooler.AwakePooler();
        EffectPooler.AwakePooler();
        EventPooler.AwakePooler();
        TrucksFirePointPooler.AwakePooler();
        RoadBlocksFirePointPooler.AwakePooler();
        HelicoptersFirePointPooler.AwakePooler();
        GunsPooler.AwakePooler();
        TrucksPooler.AwakePooler();
        EnemyPooler.AwakePooler();
    }

    public void AwakeCustomizationGameStatePooler()
    {
        Data.ObjectPoolersHolder = this;

        TrucksFirePointPooler.AwakePooler();
        GunsPooler.AwakePooler();
        TrucksPooler.AwakePooler();
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

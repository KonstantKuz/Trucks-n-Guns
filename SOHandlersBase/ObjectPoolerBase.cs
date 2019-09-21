using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPooler", menuName = "Handlers/ObjectPooler")]
public class ObjectPoolerBase : ScriptableObject
{
    [SerializeField]
    private GameObject parentInScenePrefab;
    private Transform parentInScene;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public List<string> tags { get; private set; }

    public void AwakePooler()
    {
        GameObject parent = Instantiate(parentInScenePrefab);
        parentInScene = parent.transform;

        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        tags = new List<string>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject instatObj = Instantiate(pool.prefab, parentInScene);
                instatObj.SetActive(false);
                objectPool.Enqueue(instatObj);
            }
            poolDictionary.Add(pool.tag, objectPool);
            tags.Add(pool.tag);
        }
    }
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        GameObject objToSpawn = poolDictionary[tag].Dequeue();

        objToSpawn.SetActive(true);
        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objToSpawn);

        return objToSpawn;
    }

    public GameObject SpawnFromPool(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        GameObject objToSpawn = poolDictionary[tag].Dequeue();

        objToSpawn.SetActive(true);

        poolDictionary[tag].Enqueue(objToSpawn);

        return objToSpawn;
    }

    public GameObject PermanentSpawnFromPool(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        GameObject objToSpawn = poolDictionary[tag].Dequeue();

        objToSpawn.SetActive(true);

        return objToSpawn;
    }

    public void ReturnGameObjectToPool(GameObject objToReturn, string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return;
        }
        Debug.Log(objToReturn.name + "has been returned to" + tag + "pool");
        poolDictionary[tag].Enqueue(objToReturn);

        objToReturn.SetActive(false);
    }
    /// <summary>
    /// witout - index of item that cant be spawned
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="withOut"></param>
    /// <returns></returns>
    public GameObject SpawnRandomItemFromPool(Vector3 position, Quaternion rotation, int withOut)
    {
        int randomIndex = RandomIndexWithOut(withOut);
        string randomTag = tags[randomIndex];

        GameObject objToSpawn = poolDictionary[randomTag].Dequeue();

        objToSpawn.SetActive(true);

        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;


        poolDictionary[randomTag].Enqueue(objToSpawn);

        return objToSpawn;
    }

    int RandomIndexWithOut(int withOut)
    {
        int randomIndex = Random.Range(0, tags.Count);
        if (randomIndex == withOut)
            return RandomIndexWithOut(withOut);
        else
            return randomIndex;
    }
    //???
    //public void AddNewPool(GameObject prefab, int size, string tag)
    //{
    //    Pool newPool = new Pool
    //    {
    //        prefab = prefab,
    //        size = size,
    //        tag = tag
    //    };

    //    pools.Add(newPool);

    //    Queue<GameObject> newObjectPool = new Queue<GameObject>();
    //    for (int i = 0; i < newPool.size; i++)
    //    {
    //        GameObject instatntObj = Instantiate(newPool.prefab);
    //        instatntObj.SetActive(false);
    //        newObjectPool.Enqueue(instatntObj);
    //    }
    //    poolDictionary.Add(newPool.tag, newObjectPool);
    //}
    //public void RemovePool(string tag)
    //{
    //    for (int i = 0; i < pools.Count; i++)
    //    {
    //        if (pools[i].tag == tag)
    //        {
    //            for (int j = 0; j < pools[i].size; j++)
    //            {
    //                Destroy(SpawnFromPool(tag));
    //            }
    //            poolDictionary[tag].Clear();
    //        }
    //    }
    //}
}

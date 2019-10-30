using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
[CreateAssetMenu(fileName = "NewPooler", menuName = "Handlers/ObjectPooler")]
public class ObjectPoolerBase : ScriptableObject
{
    [SerializeField]
    private GameObject parentInScenePrefab;
    private Transform parentInScene;
    private int totalWeight;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;

        [Range(10, 100)]
        public int weight;
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


                instatObj.name = instatObj.name.Replace("(Clone)", "");


                instatObj.SetActive(false);
                objectPool.Enqueue(instatObj);
            }
            poolDictionary.Add(pool.tag, objectPool);
            tags.Add(pool.tag);
        }

        totalWeight = 0;
        for (int i = 0; i < pools.Count; i++)
        {
            totalWeight += pools[i].weight;
        }
    }

    public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
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

    public GameObject Spawn(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log($"PoolDictionaty does not contain tag {tag}");
            return null;
        }

        GameObject objToSpawn = poolDictionary[tag].Dequeue();

        objToSpawn.SetActive(true);

        poolDictionary[tag].Enqueue(objToSpawn);

        return objToSpawn;
    }

    public GameObject PermanentSpawn(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log($"PoolDictionaty does not contain tag {tag}");
            return null;
        }

        GameObject objToSpawn = poolDictionary[tag].Dequeue();

        objToSpawn.SetActive(true);

        return objToSpawn;
    }

    public void ReturnToPool(GameObject objToReturn, string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log($"<color=red> PoolDictionary does not contain tag {tag} </color>");
            return;
        }
        //else
        //{
        //    Debug.Log($"{objToReturn.name} was returned to {tag} pool");
        //}
        poolDictionary[tag].Enqueue(objToReturn);

        objToReturn.transform.parent = parentInScene;

        objToReturn.SetActive(false);
    }

    public GameObject SpawnRandom(Vector3 position, Quaternion rotation)
    {
        int randomIndex = Random.Range(0, tags.Count);
        string randomTag = tags[randomIndex];

        GameObject objToSpawn = poolDictionary[randomTag].Dequeue();

        objToSpawn.SetActive(true);

        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;


        poolDictionary[randomTag].Enqueue(objToSpawn);

        return objToSpawn;
    }

    public GameObject SpawnWeightedRandom(Vector3 position, Quaternion rotation)
    {
        int randomValue = Random.Range(0, totalWeight + 1);

        for (int i = 0; i < pools.Count; i++)
        {
            if (randomValue <= pools[i].weight)
            {
                GameObject objToSpawn = poolDictionary[pools[i].tag].Dequeue();

                objToSpawn.SetActive(true);

                objToSpawn.transform.position = position;
                objToSpawn.transform.rotation = rotation;


                poolDictionary[pools[i].tag].Enqueue(objToSpawn);

                //Debug.Log($"<color=green> {objToSpawn.name} was spawned & its weight is equals to {pools[i].weight} </color>");

                return objToSpawn;
            }

            randomValue -= pools[i].weight;
        }

        //Debug.Log("<color=red> Weigted Random was return null </color>");
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDataReturner", menuName = "Handlers/DataReturner")]
public class DataReturnerBase : ScriptableObject
{
    [System.Serializable]
    public class DataToHold
    {
        public Data data;
        public string tag;
    }

    public List<DataToHold> dataToHold;

    public Dictionary<string, Data> DataDictionary;

    public void AwakeDataHolder()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif

        DataDictionary = new Dictionary<string, Data>();
        DataDictionary.Clear();
        for (int i = 0; i < dataToHold.Count; i++)
        {
            DataDictionary.Add(dataToHold[i].tag, Instantiate(dataToHold[i].data));
        }
    }

    public Data GetData(string tag)
    {
        if(DataDictionary.ContainsKey(tag))
        {
            //Debug.Log(DataDictionary[tag].data.name);
            return DataDictionary[tag];
        }
        else
        {
            Debug.LogError($"DataDictionary does not contain data with {tag} tag.");
            return null;
        }
    }

    public Data GetRandomData()
    {
        return DataDictionary[dataToHold[Random.Range(0, dataToHold.Count)].tag];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RandomWeight : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string name;
        public int weight;
    }

    public Button dropButton;
    public Item[] items;

    private void Start()
    {
        dropButton.onClick.AddListener(() => RandomDrop());
    }

    public void RandomDrop()
    {
        int totalWeight = 0;
        for (int i = 0; i < items.Length; i++)
        {
            totalWeight += items[i].weight;
        }

        int randomValue = Random.Range(0, totalWeight + 1);

        for (int i = 0; i < items.Length; i++)
        {
            if(randomValue<=items[i].weight)
            {
                Debug.Log($"{items[i].name} was dropped");
                return;
            }

            randomValue -= items[i].weight;
        }
    }
}

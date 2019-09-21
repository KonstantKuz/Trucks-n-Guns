using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanyonElementsRotator : MonoBehaviour
{
    public Transform[] canyonElements;
    public Transform[] smallPlants;

    private  Vector3[] plantsPositions;
    private bool[] isPlanted;


    private void Awake()
    {
        plantsPositions = new Vector3[smallPlants.Length];
        isPlanted = new bool[smallPlants.Length];
    }
    
    public void RotateCanyonElements()
    {
        for (int i = 0; i < canyonElements.Length; i++)
        {
            canyonElements[i].transform.localRotation = Quaternion.AngleAxis(Random.Range(-360, 360), Vector3.up);
        }
        for (int i = 0; i < smallPlants.Length; i++)
        {
            plantsPositions[i] = smallPlants[i].position;
            isPlanted[i] = false;
        }
        ReplacePlants();
    }

    private void ReplacePlants()
    {
        for (int i = 0; i < smallPlants.Length; i++)
        {
            ReplacePlant(smallPlants[i]);
        }
    }


    private void ReplacePlant(Transform plant)
    {
        int randomPositionIndex = Random.Range(0, plantsPositions.Length);
        if (isPlanted[randomPositionIndex] == false)
        {
            plant.position = plantsPositions[randomPositionIndex];
            isPlanted[randomPositionIndex] = true;
        }
        else
        {
            ReplacePlant(plant);
        }
    }
}

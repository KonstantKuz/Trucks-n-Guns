using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanyonElementsRotator : MonoCached
{
    public Transform[] canyonElements;
    public Transform[] smallPlants;

    
    public void RotateCanyonElements()
    {
        for (int i = 0; i < canyonElements.Length; i++)
        {
            canyonElements[i].transform.localRotation = Quaternion.AngleAxis(Random.Range(-360, 360), Vector3.up);
        }
        ReplacePlants();
    }

    private void ReplacePlants()
    {
        for (int i = 0; i < smallPlants.Length; i++)
        {
            bool randomBool = Random.value > 0.85f ? true : false;
            smallPlants[i].gameObject.SetActive(randomBool);
            smallPlants[i].rotation = Quaternion.AngleAxis(Random.Range(-200, 200), smallPlants[i].up);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlantsActivator : MonoCached
{
    public Transform[] smallPlants;

    public void ReplacePlants()
    {
        for (int i = 0; i < smallPlants.Length; i++)
        {
            bool randomBool = Random.value > 0.85f ? true : false;
            smallPlants[i].gameObject.SetActive(randomBool);
            smallPlants[i].rotation = Quaternion.AngleAxis(Random.Range(-200, 200), smallPlants[i].up);
        }
    }
}

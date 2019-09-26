using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckTest : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(latecall());
    }

   
    private IEnumerator latecall()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<Truck>().TruckData.SetUpTruck(GetComponent<Truck>());

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            GetComponent<Truck>().SteeringWheels(1f);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            GetComponent<Truck>().SteeringWheels(-1f);
        }
    }
}

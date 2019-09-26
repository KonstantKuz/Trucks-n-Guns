﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CustomizationHandler : MonoBehaviour
{
    public Truck playerTruck;

    public Button gunsChangeButton;
    public Button firePointChangeButton;
    public Button truckChangeButton;

    private void Start()
    {
        gunsChangeButton.onClick.AddListener(() => ChangeGuns());
        firePointChangeButton.onClick.AddListener(() => ChangeFirePoint());
        truckChangeButton.onClick.AddListener(() => ChangeTruck());
    }

    public void ChangeGuns()
    {
        playerTruck.TruckData.ReturnObjectsToPool(playerTruck);
        for (int i = 0; i < playerTruck.TruckData.firePointDataToCopy.gunsConfigurations.Length; i++)
        {
            playerTruck.TruckData.firePointDataToCopy.gunsConfigurations[i].gunType = (GameEnums.Gun)Random.Range(1, System.Enum.GetNames(typeof(GameEnums.Gun)).Length);
        }

        playerTruck.TruckData.SetUpTruck(playerTruck);
    }

    public void ChangeFirePoint()
    {
        playerTruck.TruckData.ReturnObjectsToPool(playerTruck);

        playerTruck.TruckData.firePointDataToCopy.firePointType++;

        playerTruck.TruckData.SetUpTruck(playerTruck);
    }

    public void ChangeTruck()
    {

    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationGameStateHandler : MonoBehaviour
{
    [SerializeField]
    private CustomizationHandler customizationHandler;
    [SerializeField]
    private InputHandler inputHandler;
    [SerializeField]
    private PlayerHandler playerHandler;
    [SerializeField]
    private ObjectPoolersHolder objectPoolersHolder;
    [SerializeField]
    private DataReturnersHolder dataReturnersHolder;

    private void OnEnable()
    {
        StartCustomization();
    }

    public void StartCustomization()
    {
        PlayerStaticRunTimeData.customizationTruckData.RewriteData(PlayerStaticRunTimeData.playerTruckData);
        PlayerStaticRunTimeData.customizationFirePointData.RewriteData(PlayerStaticRunTimeData.playerFirePointData);

        objectPoolersHolder.AwakeCustomizationGameStatePooler();
        dataReturnersHolder.AwakeDataReturners();
        playerHandler.CreateCamera();
        playerHandler.CreatePlayer(inputHandler);
        customizationHandler.InjectPlayerTruck(playerHandler.player.GetComponent<Truck>());
    }
}

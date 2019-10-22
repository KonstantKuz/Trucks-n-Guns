using System.Collections;
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

    private void OnEnable()
    {
        StartCustomization();
    }

    public void StartCustomization()
    {
        objectPoolersHolder.AwakeCustomizationGameStatePooler();
        playerHandler.CreateCamera();
        playerHandler.CreatePlayer(inputHandler);
        customizationHandler.InjectPlayerTruck(playerHandler.player.GetComponent<Truck>());
    }
}

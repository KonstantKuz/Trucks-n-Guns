using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CustomizationHandler : MonoCached
{
    public static FirePointData playerFPdata;

    private Truck playerTruck;
    public GameObject gunButtonPrefab;
    private Button[] gunButtons;
    private GameObject[] gunButtObj;
    private Button upgradeButton;

    public void InjectPlayerTruck(Truck truck)
    {
        if(gunButtons != null)
        {
            for (int i = 0; i < gunButtons.Length; i++)
            {
                Destroy(gunButtObj[i]);
            }
        }

        playerTruck = truck;
        playerFPdata = truck.TruckData.firePointData;
        gunButtons = new Button[playerTruck.firePoint.gunsPoints.Count];
        gunButtObj = new GameObject[playerTruck.firePoint.gunsPoints.Count];

        for (int i = 0; i < playerTruck.firePoint.gunsPoints.Count; i++)
        {
            gunButtObj[i] = Instantiate(gunButtonPrefab);
            gunButtons[i] = gunButtObj[i].GetComponentInChildren<Button>();
            gunButtObj[i].transform.parent = playerTruck.firePoint.gunsPoints[i].gunsLocation;
            gunButtObj[i].transform.localPosition = Vector3.zero;
            gunButtObj[i].transform.LookAt(Camera.main.transform);
            gunButtObj[i].GetComponentInChildren<GunChangeButton>().StartListeningGunPoint(playerTruck.firePoint.gunsPoints[i]);
            gunButtObj[i].transform.parent = null;
        }

        upgradeButton = GameObject.Find("UpgradeButton").GetComponent<Button>();
        upgradeButton.onClick.AddListener(() => UpgradeTruck());
    }

    int typeCount = 0;

    public void UpgradeTruck()
    {
        typeCount++;

        if(typeCount> System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length - 1)
        {
            typeCount = 0;
        }

        playerTruck.TruckData.firePointType = (GameEnums.FirePointType)typeCount;
        playerTruck.TruckData.ReturnObjectsToPool(playerTruck);
        playerTruck.SetUpTruck();

        upgradeButton.onClick.RemoveAllListeners();

        InjectPlayerTruck(playerTruck);
    }
}

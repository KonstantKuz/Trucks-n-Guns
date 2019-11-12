using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationHandler : MonoCached
{
    public ShopCosts shopCosts;

    public static ShopCosts ShopCosts { get; set; }

    public static Truck playerTruck;

    public GameObject gunSelectButtonPrefab;
    private Button[] gunButtons;
    private GameObject[] gunButtObj;
    
    private Button upgradeFirePointButton;
    private Button changeTruckButton;
    private Button buyButton;

    private int firePointTypeCount = 0;
    private int truckTypeCount = 0;
    

    private void Awake()
    {
        ShopCosts = shopCosts;
    }

    public void InjectPlayerTruck(Truck truck)
    {

        if(gunButtObj != null || gunButtons != null)
        {
            foreach (var item in gunButtObj)
            {
                item.SetActive(false);
                Destroy(item.gameObject);
            }

            foreach (var item in gunButtons)
            {
                item.onClick.RemoveAllListeners();
                item.gameObject.SetActive(false);
                Destroy(item.gameObject);
            }
        }

        playerTruck = truck;
        playerTruck._rigidbody.useGravity = false;

        gunButtons = new Button[playerTruck.firePoint.gunsPoints.Count];
        gunButtObj = new GameObject[playerTruck.firePoint.gunsPoints.Count];

        for (int i = 0; i < playerTruck.firePoint.gunsPoints.Count; i++)
        {
            gunButtObj[i] = Instantiate(gunSelectButtonPrefab);
            gunButtons[i] = gunButtObj[i].GetComponentInChildren<Button>();
            gunButtObj[i].transform.parent = playerTruck.firePoint.gunsPoints[i].gunsLocation;
            gunButtObj[i].transform.localPosition = Vector3.zero;
            gunButtObj[i].transform.LookAt(Camera.main.transform);
            gunButtObj[i].GetComponentInChildren<GunChangeButton>().StartListeningGunPoint(playerTruck.firePoint.gunsPoints[i]);
            gunButtObj[i].transform.parent = null;
        }
        changeTruckButton = GameObject.Find("ChangeTruck").GetComponent<Button>();
        changeTruckButton.onClick.AddListener(() => ChangeTruck());

        upgradeFirePointButton = GameObject.Find("UpgradeFirePoint").GetComponent<Button>();
        upgradeFirePointButton.onClick.AddListener(() => UpgradeFirePoint());

        buyButton = GameObject.Find("Buy").GetComponent<Button>();

    }
    
    public void UpgradeFirePoint()
    {
        playerTruck.TruckData = PlayerStaticRunTimeData.customizationTruckData;

        firePointTypeCount++;

        if(firePointTypeCount> System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length - 1)
        {
            firePointTypeCount = 0;
        }

        GameEnums.FirePointType firePointToBuy = (GameEnums.FirePointType)firePointTypeCount;

        playerTruck.TruckData.firePointType = firePointToBuy;
        playerTruck.TruckData.ReturnObjectsToPool(playerTruck);
        playerTruck.SetUpTruck();

        upgradeFirePointButton.onClick.RemoveAllListeners();

        buyButton.gameObject.GetComponentInChildren<Text>().text = "Buy FirePoint " + ShopCosts.ItemsCost(firePointToBuy.ToString()).ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => BuyFirePoint(firePointToBuy));

        InjectPlayerTruck(playerTruck);

    }

    public void BuyFirePoint(GameEnums.FirePointType fptype)
    {
        int cost = shopCosts.ItemsCost(fptype.ToString());
        if (PlayerStaticRunTimeData.coins >= cost)
        {
            PlayerStaticRunTimeData.playerTruckData.RewriteData(PlayerStaticRunTimeData.customizationTruckData);
            PlayerStaticRunTimeData.coins -= cost;
        }
        buyButton.onClick.RemoveAllListeners();
        PlayerDataDebug.RefreshStatistics();
    }

    public void ChangeTruck()
    {
        playerTruck.TruckData = PlayerStaticRunTimeData.customizationTruckData;

        truckTypeCount++;

        if (truckTypeCount > System.Enum.GetNames(typeof(GameEnums.Truck)).Length - 1)
        {
            truckTypeCount = 0;
        }
        GameEnums.Truck truckToBuy = (GameEnums.Truck)truckTypeCount;
        playerTruck.TruckData.truckType = truckToBuy;
        playerTruck.TruckData.ReturnObjectsToPool(playerTruck);
        playerTruck.SetUpTruck();

        changeTruckButton.onClick.RemoveAllListeners();

        buyButton.gameObject.GetComponentInChildren<Text>().text = "Buy Truck " + ShopCosts.ItemsCost(truckToBuy.ToString()).ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => BuyTruck(truckToBuy));

        InjectPlayerTruck(playerTruck);

    }

    public void BuyTruck(GameEnums.Truck trucktype)
    {
        int cost = shopCosts.ItemsCost(trucktype.ToString());
        if (PlayerStaticRunTimeData.coins >= cost)
        {
            PlayerStaticRunTimeData.playerTruckData.RewriteData(PlayerStaticRunTimeData.customizationTruckData);
            PlayerStaticRunTimeData.coins -= cost;
        }
        buyButton.onClick.RemoveAllListeners();
        PlayerDataDebug.RefreshStatistics();
    }
}

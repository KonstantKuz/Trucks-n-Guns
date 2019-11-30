using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationHandler : MonoCached
{
    public ShopCosts shopCosts;

    public static ShopCosts ShopCosts { get; set; }

    public static Truck playerTruck;

    public GameObject shopGunButtonGameObject;
    private Button[] shopGunButtons;
    private GameObject[] shopGunButtonGameObjects;
    
    private Button upgradeFirePointButton;
    private Button changeTruckButton;
    private Button buyButton;

    private Slider trucksConditionSlider;

    private int firePointTypeCount = 1;
    private int truckTypeCount = 0;
    

    private void Awake()
    {
        ShopCosts = shopCosts;
    }

    public void InjectPlayerTruck(Truck truck)
    {
        if (shopGunButtonGameObjects != null)
        {
            for (int i = 0; i < shopGunButtonGameObjects.Length; i++)
            {
                Destroy(shopGunButtonGameObjects[i]);
            }
        }

        playerTruck = truck;
        playerTruck._rigidbody.useGravity = false;

        playerTruck.TruckData = PlayerStaticRunTimeData.customizationTruckData;
        playerTruck.TruckData.ReturnObjectsToPool(playerTruck);
        playerTruck.TruckData.SetUpTruck(playerTruck);

        shopGunButtons = new Button[playerTruck.firePoint.gunsPoints.Count];
        shopGunButtonGameObjects = new GameObject[playerTruck.firePoint.gunsPoints.Count];

        for (int i = 0; i < playerTruck.firePoint.gunsPoints.Count; i++)
        {
            shopGunButtonGameObjects[i] = Instantiate(shopGunButtonGameObject);
            shopGunButtons[i] = shopGunButtonGameObjects[i].GetComponentInChildren<Button>();
            shopGunButtonGameObjects[i].transform.parent = playerTruck.firePoint.gunsPoints[i].gunsLocation;
            shopGunButtonGameObjects[i].transform.localPosition = Vector3.zero;
            shopGunButtonGameObjects[i].transform.LookAt(Camera.main.transform);
            shopGunButtonGameObjects[i].GetComponentInChildren<ShopGunHandler>().StartListeningGunPoint(playerTruck.firePoint.gunsPoints[i]);
            shopGunButtonGameObjects[i].transform.parent = null;
        }

        changeTruckButton = GameObject.Find("ChangeTruck").GetComponent<Button>();
        changeTruckButton.onClick.AddListener(() => ChangeTruck());

        upgradeFirePointButton = GameObject.Find("UpgradeFirePoint").GetComponent<Button>();
        upgradeFirePointButton.onClick.AddListener(() => UpgradeFirePoint());

        trucksConditionSlider = GameObject.Find("TrucksConditionSlider").GetComponent<Slider>();
        trucksConditionSlider.onValueChanged.AddListener(delegate { ChangeCondition(trucksConditionSlider.value); });

        buyButton = GameObject.Find("Buy").GetComponent<Button>();
    }
    
    public void UpgradeFirePoint()
    {

        firePointTypeCount++;

        if(firePointTypeCount> System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length - 1)
        {
            firePointTypeCount = 1;
        }

        GameEnums.FirePointType firePointToBuy = (GameEnums.FirePointType)firePointTypeCount;

        PlayerStaticRunTimeData.customizationTruckData.firePointType = firePointToBuy;

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
            PlayerDataDebug.RefreshStatistics();
            PersistentPlayerDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, PlayerStaticRunTimeData.playerFirePointData, new PlayerSessionData(0, 0));
            buyButton.gameObject.GetComponentInChildren<Text>().text = "";

        }

        InjectPlayerTruck(playerTruck);

        buyButton.onClick.RemoveAllListeners();
    }

    public void ChangeTruck()
    {

        truckTypeCount++;

        if (truckTypeCount > System.Enum.GetNames(typeof(GameEnums.Truck)).Length - 1)
        {
            truckTypeCount = 0;
        }
        GameEnums.Truck truckToBuy = (GameEnums.Truck)truckTypeCount;
        PlayerStaticRunTimeData.customizationTruckData.truckType = truckToBuy;

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
            PlayerDataDebug.RefreshStatistics();
            PersistentPlayerDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, PlayerStaticRunTimeData.playerFirePointData, new PlayerSessionData(0, 0));
            buyButton.gameObject.GetComponentInChildren<Text>().text = "";

        }
        InjectPlayerTruck(playerTruck);

        buyButton.onClick.RemoveAllListeners();
    }

    public void ChangeCondition(float value)
    {
        PlayerStaticRunTimeData.playerTruckData.maxTrucksCondition = value;
    }
}

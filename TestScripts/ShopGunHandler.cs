using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopGunHandler : MonoCached
{
    public FirePoint.GunPoint gunPoint { get; set; }
    
    private Vector3 camStartPos;
    private Quaternion camStartRot;
    private GameEnums.Gun gunTypeToBuy;
    public GameEnums.GunDataType currentGunDataType;
    private int gunTypeCount;

    private Button buyButton;
    private Button backButton;
    private Button changeGunButton;

    private GameObject rateOfFireStat;
    private GameObject damageStat;

    private void Awake()
    {
        backButton = GameObject.Find("Back").GetComponent<Button>();
        buyButton = GameObject.Find("Buy").GetComponent<Button>();
        changeGunButton = GameObject.Find("ChangeGun").GetComponent<Button>();

        rateOfFireStat = GameObject.Find("RateOfFireStat");
        damageStat = GameObject.Find("DamageStat");

        camStartPos = Camera.main.transform.position;
        camStartRot = Camera.main.transform.rotation;
    }

    public void StartListeningGunPoint(FirePoint.GunPoint gunPoint)
    {
        this.gunPoint = gunPoint;
        GetComponent<Button>().onClick.AddListener(() => SelectGun());
    }

    private void SelectGun()
    {
        Camera.main.transform.position = transform.position + new Vector3(2, 2, 2);
        Camera.main.transform.LookAt(gunPoint.gunsLocation);

        if(gunPoint.gunsLocation.childCount>0)
        {
            rateOfFireStat.SetActive(true);
            damageStat.SetActive(true);

            for (int i = 0; i < CustomizationHandler.playerTruck.TruckData.firePointData.gunsConfigurations.Length; i++)
            {
                if(CustomizationHandler.playerTruck.TruckData.firePointData.gunsConfigurations[i].locationPath.ToString() == gunPoint.locationPath)
                {
                    currentGunDataType = CustomizationHandler.playerTruck.TruckData.firePointData.gunsConfigurations[i].gunDataType;
                }
            }

            UpdateGunStats();

            rateOfFireStat.GetComponentInChildren<Button>().onClick.AddListener(() => UpgradeRateOfFire());
            damageStat.GetComponentInChildren<Button>().onClick.AddListener(() => UpgradeDamage());
        }

        changeGunButton.gameObject.SetActive(true);
        changeGunButton.onClick.AddListener(() => ChangeGun());

        gameObject.GetComponent<Image>().enabled = false;
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => BackToMain());
    }

    private void UpgradeRateOfFire()
    {
        UpdateGunStats();

        int upgradedRateOfFire = (int)currentGunDataType + 10;
        char[] typeNumbersChars = upgradedRateOfFire.ToString().ToCharArray();
        if (typeNumbersChars[0] - 48 > 3)
        {
            return;
        }

        GameEnums.GunDataType upgradedGunData = (GameEnums.GunDataType)upgradedRateOfFire;
        //int cost = CustomizationHandler.ShopCosts.ItemsCost(upgradedGunData.ToString());

        UpgradeGunData(upgradedGunData, 500);

       
        //rateOfFireStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();

    }

    private void UpgradeDamage()
    {
        UpdateGunStats();

        int upgradedDamage = (int)currentGunDataType + 1;
        char[] typeNumbersChars = upgradedDamage.ToString().ToCharArray();
        if (typeNumbersChars[1] - 48 > 3)
        {
            return;
        }

        GameEnums.GunDataType upgradedGunData = (GameEnums.GunDataType)upgradedDamage;
        //int cost = CustomizationHandler.ShopCosts.ItemsCost(upgradedGunData.ToString());

        UpgradeGunData(upgradedGunData, 300);

        //damageStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
    }

    public void UpgradeGunData(GameEnums.GunDataType gunDataTypeToBuy, int cost)
    {
        if (PlayerStaticRunTimeData.coins >= cost)
        {
            for (int i = 0; i < PlayerStaticRunTimeData.playerFirePointData.gunsConfigurations.Length; i++)
            {
                if (PlayerStaticRunTimeData.playerFirePointData.gunsConfigurations[i].locationPath.ToString() == gunPoint.locationPath)
                {
                    PlayerStaticRunTimeData.playerFirePointData.gunsConfigurations[i].gunDataType = gunDataTypeToBuy;
                    PlayerStaticRunTimeData.customizationFirePointData.gunsConfigurations[i].gunDataType = gunDataTypeToBuy;
                }
            }
            PlayerStaticRunTimeData.coins -= cost;
            currentGunDataType = gunDataTypeToBuy;
            PersistentPlayerDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, PlayerStaticRunTimeData.playerFirePointData, new PlayerSessionData(0, 0));
        }
        PlayerDataDebug.RefreshStatistics();
        UpdateGunStats();
    }

    public void UpdateGunStats()
    {
        int typeNumber = (int)currentGunDataType;
        char[] typeNumbersChars = typeNumber.ToString().ToCharArray();
        Debug.Log($"<color=green> {typeNumber} typeNumber. first in char array = {typeNumbersChars[0]}, second = {typeNumbersChars[1]}" +
            $" RateOfFireStat = {rateOfFireStat.GetComponent<Slider>().value}," +
            $"DamageStat = {damageStat.GetComponent<Slider>().value} </color>");
        rateOfFireStat.GetComponent<Slider>().value = typeNumbersChars[0] - 48;
        damageStat.GetComponent<Slider>().value = typeNumbersChars[1] - 48;
        Debug.Log($"<color=green> {typeNumber} typeNumber. first in char array = {typeNumbersChars[0]}, second = {typeNumbersChars[1]}" +
            $" RateOfFireStat = {rateOfFireStat.GetComponent<Slider>().value}," +
            $"DamageStat = {damageStat.GetComponent<Slider>().value} </color>");
    }

    private void ChangeGun()
    {
        if (gunPoint.gunsLocation.childCount > 0)
        {
            ObjectPoolersHolder.Instance.GunsPooler.ReturnToPool(gunPoint.gunsLocation.GetChild(0).gameObject, gunPoint.gunsLocation.GetChild(0).gameObject.name);
        }

        gunTypeCount++;

        if (gunTypeCount > System.Enum.GetNames(typeof(GameEnums.Gun)).Length - 1)
        {
            gunTypeCount = 0;
        }

        gunTypeToBuy = (GameEnums.Gun)gunTypeCount;

        GameObject gun = ObjectPoolersHolder.Instance.GunsPooler.Spawn(gunTypeToBuy.ToString(), gunPoint.gunsLocation.position, Quaternion.identity);

        gun.transform.parent = gunPoint.gunsLocation;
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localEulerAngles = new Vector3(0, gunPoint.allowableAnglesOnPoint.StartDirectionAngle, 0);
        rateOfFireStat.SetActive(false);
        damageStat.SetActive(false);
        buyButton.gameObject.GetComponentInChildren<Text>().text = "Buy Gun " + CustomizationHandler.ShopCosts.ItemsCost(gunTypeToBuy.ToString()).ToString();
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => BuyGun(gunTypeToBuy));
    }

    public void BuyGun(GameEnums.Gun guntype)
    {
        int cost = CustomizationHandler.ShopCosts.ItemsCost(guntype.ToString());
        if (PlayerStaticRunTimeData.coins >= cost)
        {
            for (int i = 0; i < PlayerStaticRunTimeData.playerFirePointData.gunsConfigurations.Length; i++)
            {
                if (PlayerStaticRunTimeData.playerFirePointData.gunsConfigurations[i].locationPath.ToString() == gunPoint.locationPath)
                {
                    PlayerStaticRunTimeData.playerFirePointData.gunsConfigurations[i].gunType = gunTypeToBuy;
                    PlayerStaticRunTimeData.playerFirePointData.gunsConfigurations[i].gunDataType = GameEnums.GunDataType.LrLd;
                    PlayerStaticRunTimeData.customizationFirePointData.gunsConfigurations[i].gunType = gunTypeToBuy;
                    PlayerStaticRunTimeData.customizationFirePointData.gunsConfigurations[i].gunDataType = GameEnums.GunDataType.LrLd;

                    currentGunDataType = GameEnums.GunDataType.LrLd;
                }
            }

            PlayerStaticRunTimeData.coins -= cost;
            PlayerDataDebug.RefreshStatistics();
            PersistentPlayerDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, PlayerStaticRunTimeData.playerFirePointData, new PlayerSessionData(0, 0));

            buyButton.gameObject.GetComponentInChildren<Text>().text = "";

            changeGunButton.onClick.RemoveAllListeners();
            buyButton.onClick.RemoveAllListeners();
            damageStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            rateOfFireStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();


            SelectGun();

        }

    }

    private void BackToMain()
    {
        Camera.main.transform.position = camStartPos;
        Camera.main.transform.rotation = camStartRot;

        gameObject.GetComponent<Image>().enabled = true;

        changeGunButton.onClick.RemoveAllListeners();

        rateOfFireStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        damageStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        rateOfFireStat.GetComponent<Slider>().value = 0;
        damageStat.GetComponent<Slider>().value = 0;
        rateOfFireStat.SetActive(false);
        damageStat.SetActive(false);
        buyButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        //backButton.onClick.AddListener(() => CustomizationHandler.BackToMenu());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunChangeButton : MonoCached
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

    public void StartListeningGunPoint(FirePoint.GunPoint gunPoint)
    {
        this.gunPoint = gunPoint;
        GetComponent<Button>().onClick.AddListener(() => SelectGun());
        
        backButton = GameObject.Find("Back").GetComponent<Button>();
        buyButton = GameObject.Find("Buy").GetComponent<Button>();
        changeGunButton = GameObject.Find("ChangeGun").GetComponent<Button>();

        rateOfFireStat = GameObject.Find("RateOfFireStat");
        damageStat = GameObject.Find("DamageStat");

        camStartPos = Camera.main.transform.position;
        camStartRot = Camera.main.transform.rotation;
    }

    private void SelectGun()
    {
        Camera.main.transform.position = transform.position + new Vector3(2, 2, 2);
        Camera.main.transform.LookAt(gunPoint.gunsLocation);

        if(gunPoint.gunsLocation.childCount>0)
        {
            rateOfFireStat.SetActive(true);
            damageStat.SetActive(true);
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
        int upgradedRateOfFire = (int)currentGunDataType + 10;
        GameEnums.GunDataType upgradedGunData = (GameEnums.GunDataType)upgradedRateOfFire;
        //int cost = CustomizationHandler.ShopCosts.ItemsCost(upgradedGunData.ToString());
        if (PlayerStaticRunTimeData.coins >= 500)
        {
            for (int i = 0; i < PlayerStaticRunTimeData.playerFirePointData.gunsConfigurations.Length; i++)
            {
                if (PlayerStaticRunTimeData.playerFirePointData.gunsConfigurations[i].locationPath.ToString() == gunPoint.locationPath)
                {
                    PlayerStaticRunTimeData.playerFirePointData.gunsConfigurations[i].gunDataType = upgradedGunData;
                }
            }
            rateOfFireStat.GetComponent<Slider>().value += 0.35f;
            PlayerStaticRunTimeData.coins -= 500;
        }

        PlayerDataDebug.RefreshStatistics();
    }

    private void UpgradeDamage()
    {
        int upgradedDamage = (int)currentGunDataType + 1;
        GameEnums.GunDataType upgradedGunData = (GameEnums.GunDataType)upgradedDamage;
        //int cost = CustomizationHandler.ShopCosts.ItemsCost(upgradedGunData.ToString());
        if (PlayerStaticRunTimeData.coins >= 300)
        {
            for (int i = 0; i < PlayerStaticRunTimeData.playerFirePointData.gunsConfigurations.Length; i++)
            {
                if (PlayerStaticRunTimeData.playerFirePointData.gunsConfigurations[i].locationPath.ToString() == gunPoint.locationPath)
                {
                    PlayerStaticRunTimeData.playerFirePointData.gunsConfigurations[i].gunDataType = upgradedGunData;
                }
            }
            damageStat.GetComponent<Slider>().value += 0.35f;
            PlayerStaticRunTimeData.coins -= 300;
        }

        PlayerDataDebug.RefreshStatistics();
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
                }
            }

            PlayerStaticRunTimeData.coins -= cost;
        }
       
        changeGunButton.onClick.RemoveAllListeners();
        buyButton.onClick.RemoveAllListeners();

        PlayerDataDebug.RefreshStatistics();
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
        //rateOfFireStat.SetActive(false);
        //damageStat.SetActive(false);
        buyButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        //backButton.onClick.AddListener(() => CustomizationHandler.BackToMenu());
    }
}

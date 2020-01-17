using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopGunHandler : MonoBehaviour
{
    public FirePoint.GunPoint gunPoint { get; set; }
    private GameEnums.Gun gunTypeToBuy;
    public GameEnums.GunDataType currentGunDataType;
    private int gunTypeCount;

    private MenuHandler menuHandler;

    private void Awake()
    {
        menuHandler = MenuHandler.Instance;
    }

    public void StartListeningGunPoint(FirePoint.GunPoint gunPoint)
    {
        this.gunPoint = gunPoint;
        gameObject.GetComponent<Button>().onClick.AddListener(() => SelectGun());
    }

    private void SelectGun()
    {
        CustomizationState.Instance.RefreshPlayerStats();

        CustomizationHandler.Instance.StartCoroutine(CustomizationHandler.Instance.CameraTracking(Camera.main.transform.position, transform.position + gunPoint.gunsLocation.up * 2 + gunPoint.gunsLocation.forward * 2 , gunPoint.gunsLocation.position));

        FirePointData.GunConfiguration ConfigFromPath = PlayerStaticRunTimeData.playerTruckData.firePointData.GetConfigOnLocation(gunPoint.locationPath);

        if (ConfigFromPath.gunType != GameEnums.Gun.None)
        {
            menuHandler.customization.RateOfFireStat.SetActive(true);
            menuHandler.customization.DamageStat.SetActive(true);
            menuHandler.customization.TargetingSpeedStat.SetActive(true);

            currentGunDataType = ConfigFromPath.gunDataType;
            int gunType = (int)currentGunDataType;
            char[] typeNumbersChars = gunType.ToString().ToCharArray();


            menuHandler.customization.RateOfFireStat.GetComponent<Slider>().value = typeNumbersChars[0] - 48;
            menuHandler.customization.DamageStat.GetComponent<Slider>().value = typeNumbersChars[1] - 48;
            menuHandler.customization.TargetingSpeedStat.GetComponent<Slider>().value = typeNumbersChars[2] - 48;

            menuHandler.customization.RateOfFireStat.GetComponentInChildren<Button>().onClick.AddListener(() => UpgradeRateOfFire());
            menuHandler.customization.DamageStat.GetComponentInChildren<Button>().onClick.AddListener(() => UpgradeDamage());
            menuHandler.customization.TargetingSpeedStat.GetComponentInChildren<Button>().onClick.AddListener(() => UpgradeTargetingSpeed());

            menuHandler.customization.RateOfFireStat.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = UpgradeRateOfFireCost();
            menuHandler.customization.DamageStat.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = UpgradeDamageCost();
            menuHandler.customization.TargetingSpeedStat.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = UpgradeTargetingSpeedCost();
        }

        menuHandler.customization.ChangeGunButton.SetActive(true);
        menuHandler.customization.ChangeGunButton.GetComponent<Button>().onClick.AddListener(() => ChangeGun());
        
        menuHandler.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        menuHandler.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToGunsOverView());

        CustomizationHandler.Instance.ShopGunButtonsRenderSetActive(false);
    }

    private void ChangeGun()
    {
        gunTypeCount++;

        if (gunTypeCount > System.Enum.GetNames(typeof(GameEnums.Gun)).Length - 1)
        {
            gunTypeCount = 1;
        }

        gunTypeToBuy = (GameEnums.Gun)gunTypeCount;
        
        PlayerStaticRunTimeData.customizationTruckData.firePointData.GetConfigOnLocation(gunPoint.locationPath).gunType = gunTypeToBuy;
        CustomizationState.Instance.RefreshPlayerTruckVisualBy(PlayerStaticRunTimeData.customizationTruckData);

        menuHandler.customization.RateOfFireStat.SetActive(false);
        menuHandler.customization.DamageStat.SetActive(false);
        menuHandler.customization.TargetingSpeedStat.SetActive(false);

        menuHandler.customization.BuyButton.SetActive(true);
        menuHandler.customization.BuyButton.GetComponentInChildren<Text>().text = CustomizationHandler.Instance.shopCosts.ItemsCost(gunTypeToBuy.ToString()).ToString() + "$";
        menuHandler.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();
        menuHandler.customization.BuyButton.GetComponent<Button>().onClick.AddListener(() => BuyGun(gunTypeToBuy));

        if(((GameEnums.Gun)gunTypeCount).ToString().Contains("Gun2"))
        {
            menuHandler.customization.GunInfoWindow.SetActive(true);
            menuHandler.customization.GunInfoWindow.GetComponentInChildren<LocalizedText>().ResetText();
            menuHandler.customization.GunInfoWindow.GetComponentInChildren<Text>().text = menuHandler.customization.TargetingSpeedStat.GetComponentInChildren<Text>().text 
                + "\n" + menuHandler.customization.GunInfoWindow.GetComponentInChildren<Text>().text;
        }
        else if(((GameEnums.Gun)gunTypeCount).ToString().Contains("Gun3"))
        {
            menuHandler.customization.GunInfoWindow.SetActive(true);
            menuHandler.customization.GunInfoWindow.GetComponentInChildren<LocalizedText>().ResetText();
            menuHandler.customization.GunInfoWindow.GetComponentInChildren<Text>().text = menuHandler.customization.RateOfFireStat.GetComponentInChildren<Text>().text 
                + "\n" + menuHandler.customization.GunInfoWindow.GetComponentInChildren<Text>().text;
        }
        else
        {
            menuHandler.customization.GunInfoWindow.SetActive(false);
        }

        menuHandler.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        menuHandler.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToGunsOverView());
    }

    public void BuyGun(GameEnums.Gun guntype)
    {
        int cost = CustomizationHandler.Instance.shopCosts.ItemsCost(guntype.ToString());
        if (PlayerStaticRunTimeData.coins >= cost)
        {
            FirePointData.GunConfiguration PlayerConfigFromPath = PlayerStaticRunTimeData.playerTruckData.firePointData.GetConfigOnLocation(gunPoint.locationPath);
            FirePointData.GunConfiguration CustomizationConfigFromPath = PlayerStaticRunTimeData.customizationTruckData.firePointData.GetConfigOnLocation(gunPoint.locationPath);

            PlayerConfigFromPath.gunType = gunTypeToBuy;
            PlayerConfigFromPath.gunDataType = GameEnums.GunDataType.LrLdLs;

            CustomizationConfigFromPath.gunType = gunTypeToBuy;
            CustomizationConfigFromPath.gunDataType = GameEnums.GunDataType.LrLdLs;

            currentGunDataType = GameEnums.GunDataType.LrLdLs;

            PlayerStaticRunTimeData.coins -= cost;
            PlayerStaticDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, new PlayerSessionData(0, 0, 0));

            menuHandler.customization.BuyButton.GetComponentInChildren<Text>().text = "";

            menuHandler.customization.ChangeGunButton.GetComponent<Button>().onClick.RemoveAllListeners();

            menuHandler.customization.BuyButton.SetActive(false);
            menuHandler.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();
            menuHandler.customization.RateOfFireStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            menuHandler.customization.DamageStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            menuHandler.customization.TargetingSpeedStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();

            menuHandler.customization.GunInfoWindow.SetActive(false);

            SelectGun();
        }
        else
        {
            WarningWindow.Instance.ShowWarning(WarningStrings.DontHaveMoney());
        }
    }

    private void UpgradeRateOfFire()
    {
        int upgradedRateOfFire = (int)currentGunDataType + 100;
        char[] typeNumbersChars = upgradedRateOfFire.ToString().ToCharArray();
        if (typeNumbersChars[0] - 48 > 3)
        {
            return;
        }

        GameEnums.GunDataType upgradedGunData = (GameEnums.GunDataType)upgradedRateOfFire;
        UpgradeGunData(upgradedGunData, 500 * (typeNumbersChars[0] - 48));

        menuHandler.customization.RateOfFireStat.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = UpgradeRateOfFireCost();
    }

    private void UpgradeDamage()
    {
        int upgradedDamage = (int)currentGunDataType + 10;
        char[] typeNumbersChars = upgradedDamage.ToString().ToCharArray();
        if (typeNumbersChars[1] - 48 > 3)
        {
            return;
        }

        GameEnums.GunDataType upgradedGunData = (GameEnums.GunDataType)upgradedDamage;
        UpgradeGunData(upgradedGunData, 500 * (typeNumbersChars[1] - 48));
        
        menuHandler.customization.DamageStat.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = UpgradeDamageCost();
    }

    public void UpgradeTargetingSpeed()
    {
        int upgradedTargetingSpeed = (int)currentGunDataType + 1;
        char[] typeNumbersChars = upgradedTargetingSpeed.ToString().ToCharArray();
        if (typeNumbersChars[2] - 48 > 3)
        {
            return;
        }

        GameEnums.GunDataType upgradedGunData = (GameEnums.GunDataType)upgradedTargetingSpeed;
        UpgradeGunData(upgradedGunData, 500 * (typeNumbersChars[2] - 48));
        
        menuHandler.customization.TargetingSpeedStat.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = UpgradeTargetingSpeedCost();
    }

    private string UpgradeRateOfFireCost()
    {
        int upgradedRateOfFire = (int)currentGunDataType + 100;
        char[] typeNumbersChars = upgradedRateOfFire.ToString().ToCharArray();
        if (typeNumbersChars[0] - 48 > 3)
        {
            return "max";
        }
        else
        {
            return $"+ ({500 * (typeNumbersChars[0] - 48)} $)";
        }
    }

    private string UpgradeDamageCost()
    {
        int upgradedDamage = (int)currentGunDataType + 10;
        char[] typeNumbersChars = upgradedDamage.ToString().ToCharArray();
        if (typeNumbersChars[1] - 48 > 3)
        {
            return "max";
        }
        else
        {
            return $"+ ({500 * (typeNumbersChars[1] - 48)} $)";
        }
    }

    public string UpgradeTargetingSpeedCost()
    {
        int upgradedTargetingSpeed = (int)currentGunDataType + 1;
        char[] typeNumbersChars = upgradedTargetingSpeed.ToString().ToCharArray();
        if (typeNumbersChars[2] - 48 > 3)
        {
            return "max";
        }
        else
        {
            return $"+ ({500 * (typeNumbersChars[2] - 48)} $)";
        }
    }

    public void UpgradeGunData(GameEnums.GunDataType gunDataTypeToBuy, int cost)
    {
        if (PlayerStaticRunTimeData.coins >= cost)
        {

            FirePointData.GunConfiguration PlayerConfigFromPath = PlayerStaticRunTimeData.playerTruckData.firePointData.GetConfigOnLocation(gunPoint.locationPath);
            FirePointData.GunConfiguration CustomizationConfigFromPath = PlayerStaticRunTimeData.customizationTruckData.firePointData.GetConfigOnLocation(gunPoint.locationPath);

            PlayerConfigFromPath.gunDataType = gunDataTypeToBuy;
            CustomizationConfigFromPath.gunDataType = gunDataTypeToBuy;

            currentGunDataType = gunDataTypeToBuy;

            int gunType = (int)PlayerConfigFromPath.gunDataType;
            char[] typeNumbersChars = gunType.ToString().ToCharArray();


            menuHandler.customization.RateOfFireStat.GetComponent<Slider>().value = typeNumbersChars[0] - 48;
            menuHandler.customization.DamageStat.GetComponent<Slider>().value = typeNumbersChars[1] - 48;
            menuHandler.customization.TargetingSpeedStat.GetComponent<Slider>().value = typeNumbersChars[2] - 48;


            PlayerStaticRunTimeData.coins -= cost;
            currentGunDataType = gunDataTypeToBuy;
            PlayerStaticDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, new PlayerSessionData(0,0, 0));

            CustomizationState.Instance.RefreshPlayerStats();
        }
        else
        {
            WarningWindow.Instance.ShowWarning(WarningStrings.DontHaveMoney());
        }
    }
   
    private void BackToGunsOverView()
    {

        CustomizationHandler.Instance.StartCoroutine(CustomizationHandler.Instance.CameraTracking(Camera.main.transform.position, menuHandler.cameraStartPosition, -3.5f*CustomizationState.Instance.PlayerTruck.transform.forward));

        gameObject.GetComponent<Image>().enabled = true;

        menuHandler.customization.ChangeGunButton.GetComponent<Button>().onClick.RemoveAllListeners();
        menuHandler.customization.ChangeGunButton.SetActive(false);

        menuHandler.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();
        menuHandler.customization.BuyButton.SetActive(false);
        menuHandler.customization.RateOfFireStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        menuHandler.customization.RateOfFireStat.SetActive(false);
        menuHandler.customization.DamageStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        menuHandler.customization.DamageStat.SetActive(false);
        menuHandler.customization.TargetingSpeedStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        menuHandler.customization.TargetingSpeedStat.SetActive(false);
        

        menuHandler.customization.GunInfoWindow.SetActive(false);

        menuHandler.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        menuHandler.BackButton.GetComponent<Button>().onClick.AddListener(() => CustomizationState.Instance.BackToSectionsWindow(MenuHandler.Instance));
        CustomizationState.Instance.RefreshPlayerTruckVisualBy(PlayerStaticRunTimeData.playerTruckData);
        CustomizationState.Instance.RewriteCustomizationData();

        CustomizationHandler.Instance.ShopGunButtonsRenderSetActive(true);
    }
}

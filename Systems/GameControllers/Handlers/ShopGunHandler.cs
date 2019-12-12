using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopGunHandler : MonoCached
{
    public FirePoint.GunPoint gunPoint { get; set; }
    private GameEnums.Gun gunTypeToBuy;
    public GameEnums.GunDataType currentGunDataType;
    private int gunTypeCount;
    
    public void StartListeningGunPoint(FirePoint.GunPoint gunPoint)
    {
        this.gunPoint = gunPoint;
        gameObject.GetComponent<Button>().onClick.AddListener(() => SelectGun());
    }

    private void SelectGun()
    {
        //Camera.main.transform.position = transform.position + new Vector3(2, 2, 2);
        //Camera.main.transform.LookAt(gunPoint.gunsLocation);

        StartCoroutine(CameraTracking(Camera.main.transform.position, transform.position + new Vector3(3, 3, 3), gunPoint.gunsLocation.position/*,*/
                                                                                                                                      /*Camera.main.transform.eulerAngles, Quaternion.LookRotation(transform.position  - Camera.main.transform.position).eulerAngles*/));

        FirePointData.GunConfiguration ConfigFromPath = PlayerStaticRunTimeData.playerTruckData.firePointData.GetConfigOnLocation(gunPoint.locationPath);

        if (ConfigFromPath.gunType != GameEnums.Gun.None)
        {
            MenuHandler.Instance.customization.RateOfFireStat.SetActive(true);
            MenuHandler.Instance.customization.DamageStat.SetActive(true);

            currentGunDataType = ConfigFromPath.gunDataType;
            int gunType = (int)currentGunDataType;
            char[] typeNumbersChars = gunType.ToString().ToCharArray();


            MenuHandler.Instance.customization.RateOfFireStat.GetComponent<Slider>().value = typeNumbersChars[0] - 48;
            MenuHandler.Instance.customization.DamageStat.GetComponent<Slider>().value = typeNumbersChars[1] - 48;
           
            MenuHandler.Instance.customization.RateOfFireStat.GetComponentInChildren<Button>().onClick.AddListener(() => UpgradeRateOfFire());
            MenuHandler.Instance.customization.DamageStat.GetComponentInChildren<Button>().onClick.AddListener(() => UpgradeDamage());
        }

        MenuHandler.Instance.customization.ChangeGunButton.SetActive(true);
        MenuHandler.Instance.customization.ChangeGunButton.GetComponent<Button>().onClick.AddListener(() => ChangeGun());

        //gameObject.GetComponent<Image>().enabled = false;
        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToGunsOverView());

        CustomizationHandler.Instance.ShopGunButtonsRenderSetActive(false);
    }

    private IEnumerator CameraTracking(Vector3 from, Vector3 to, Vector3 lookAt/*, Vector3 fromeuler, Vector3 toeuler*/)
    {
        float waitTime = 1;
        float elapsedTime = 0;
        while (elapsedTime < waitTime)
        {

            Camera.main.transform.position = Vector3.Lerp(from, to, (elapsedTime / waitTime));
            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, Quaternion.LookRotation(lookAt - Camera.main.transform.position), (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;

            // Yield here
            yield return null;
        }
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

        MenuHandler.Instance.customization.RateOfFireStat.SetActive(false);
        MenuHandler.Instance.customization.DamageStat.SetActive(false);

        MenuHandler.Instance.customization.BuyButton.SetActive(true);
        MenuHandler.Instance.customization.BuyButton.GetComponentInChildren<Text>().text = "Buy Gun " + CustomizationHandler.Instance.shopCosts.ItemsCost(gunTypeToBuy.ToString()).ToString();
        MenuHandler.Instance.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.customization.BuyButton.GetComponent<Button>().onClick.AddListener(() => BuyGun(gunTypeToBuy));

        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToGunsOverView());
    }

    public void BuyGun(GameEnums.Gun guntype)
    {
        int cost = CustomizationHandler.Instance.shopCosts.ItemsCost(guntype.ToString());
        if (PlayerStaticRunTimeData.coins >= cost)
        {
            FirePointData.GunConfiguration PlayerConfigFromPath = PlayerStaticRunTimeData.playerTruckData.firePointData.GetConfigOnLocation(gunPoint.locationPath);
            FirePointData.GunConfiguration CustomizationConfigFromPath = PlayerStaticRunTimeData.customizationTruckData.firePointData.GetConfigOnLocation(gunPoint.locationPath);

            PlayerConfigFromPath.gunType = gunTypeToBuy;
            PlayerConfigFromPath.gunDataType = GameEnums.GunDataType.LrLd;

            CustomizationConfigFromPath.gunType = gunTypeToBuy;
            CustomizationConfigFromPath.gunDataType = GameEnums.GunDataType.LrLd;

            currentGunDataType = GameEnums.GunDataType.LrLd;

            PlayerStaticRunTimeData.coins -= cost;
            PlayerStaticDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, new PlayerSessionData(0, 0, 0));

            MenuHandler.Instance.customization.BuyButton.GetComponentInChildren<Text>().text = "";

            MenuHandler.Instance.customization.ChangeGunButton.GetComponent<Button>().onClick.RemoveAllListeners();

            MenuHandler.Instance.customization.BuyButton.SetActive(false);
            MenuHandler.Instance.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();
            MenuHandler.Instance.customization.RateOfFireStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            MenuHandler.Instance.customization.DamageStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            SelectGun();
        }
    }

    private void UpgradeRateOfFire()
    {
        int upgradedRateOfFire = (int)currentGunDataType + 10;
        char[] typeNumbersChars = upgradedRateOfFire.ToString().ToCharArray();
        if (typeNumbersChars[0] - 48 > 3)
        {
            return;
        }

        GameEnums.GunDataType upgradedGunData = (GameEnums.GunDataType)upgradedRateOfFire;
        UpgradeGunData(upgradedGunData, 500);
    }

    private void UpgradeDamage()
    {
        int upgradedDamage = (int)currentGunDataType + 1;
        char[] typeNumbersChars = upgradedDamage.ToString().ToCharArray();
        if (typeNumbersChars[1] - 48 > 3)
        {
            return;
        }

        GameEnums.GunDataType upgradedGunData = (GameEnums.GunDataType)upgradedDamage;
        UpgradeGunData(upgradedGunData, 300);
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


            MenuHandler.Instance.customization.RateOfFireStat.GetComponent<Slider>().value = typeNumbersChars[0] - 48;
            MenuHandler.Instance.customization.DamageStat.GetComponent<Slider>().value = typeNumbersChars[1] - 48;


            PlayerStaticRunTimeData.coins -= cost;
            currentGunDataType = gunDataTypeToBuy;
            PlayerStaticDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, new PlayerSessionData(0,0, 0));
        }
    }
   
    private void BackToGunsOverView()
    {
        //CustomizationHandler.Instance.RewriteCustomizationDataToPlayerData();
        //Camera.main.transform.position = MenuHandler.Instance.cameraStartPosition;

        StartCoroutine(CameraTracking(Camera.main.transform.position, MenuHandler.Instance.cameraStartPosition, 
            CustomizationState.Instance.PlayerTruck.transform.position - 2* CustomizationState.Instance.PlayerTruck.transform.forward/*,*/
            /*Camera.main.transform.eulerAngles, MenuHandler.Instance.cameraStartRotation*/));

        gameObject.GetComponent<Image>().enabled = true;

        MenuHandler.Instance.customization.ChangeGunButton.GetComponent<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.customization.ChangeGunButton.SetActive(false);

        MenuHandler.Instance.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.customization.BuyButton.SetActive(false);
        MenuHandler.Instance.customization.RateOfFireStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.customization.RateOfFireStat.SetActive(false);
        MenuHandler.Instance.customization.DamageStat.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.customization.DamageStat.SetActive(false);

        MenuHandler.Instance.customization.RateOfFireStat.GetComponent<Slider>().value = 0;
        MenuHandler.Instance.customization.DamageStat.GetComponent<Slider>().value = 0;


        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.AddListener(() => CustomizationState.Instance.BackToSectionsWindow(MenuHandler.Instance));
        CustomizationState.Instance.RefreshPlayerTruckVisualBy(PlayerStaticRunTimeData.playerTruckData);
        CustomizationState.Instance.RewriteCustomizationData();
        //CustomizationState.Instance.RefreshPlayerTruckVisualBy(PlayerStaticRunTimeData.playerTruckData);

        CustomizationHandler.Instance.ShopGunButtonsRenderSetActive(true);
    }
}

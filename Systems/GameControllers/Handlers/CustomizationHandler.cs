using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Singleton;

public class CustomizationHandler : Singleton<CustomizationHandler>
{
    public ShopCosts shopCosts;

    private int firePointTypeCount = 1;
    private int truckTypeCount = 0;

    private MenuHandler menuHandler;

    private void Awake()
    {
        menuHandler = MenuHandler.Instance;

        GameObject.Find("PlayerTruckPreset(Clone)").GetComponent<AudioSource>().volume = 0.1f;
    }

    public void StartCustomizeTruck()
    {
        menuHandler.customization.ChangeTruckButton.GetComponent<Button>().onClick.AddListener(() => ChangeTruck());
        menuHandler.customization.UpgradeFirePointButton.GetComponent<Button>().onClick.AddListener(() => UpgradeFirePoint());

        CustomizationState.Instance.RefreshPlayerStats();
        CustomizationState.Instance.RefreshPlayerTruckVisualBy(PlayerStaticRunTimeData.customizationTruckData);
    }
    public void StartCustomizeGuns()
    {
        CustomizationState.Instance.RefreshPlayerStats();
        RemoveGunButtons();
        SetUpShopGunButtons();
    }

    public void StopCustomizeTruck()
    {
        CustomizationState.Instance.RefreshPlayerTruckVisualBy(PlayerStaticRunTimeData.playerTruckData);
    }

    public void StopCustomizeGuns()
    {
        RemoveGunButtons();
        CustomizationState.Instance.RefreshPlayerTruckVisualBy(PlayerStaticRunTimeData.playerTruckData);
    }

    public void SetUpShopGunButtons()
    {
        menuHandler.customization.selectGunButtons = new GameObject[CustomizationState.Instance.PlayerTruck.GetComponent<Truck>().firePoint.gunsPoints.Count];

        for (int i = 0; i < CustomizationState.Instance.PlayerTruck.GetComponent<Truck>().firePoint.gunsPoints.Count; i++)
        {
            menuHandler.customization.selectGunButtons[i] = Instantiate(menuHandler.customization.SelectGunButtonPrefab);
            menuHandler.customization.selectGunButtons[i].transform.parent = CustomizationState.Instance.PlayerTruck.GetComponent<Truck>().firePoint.gunsPoints[i].gunsLocation;
            menuHandler.customization.selectGunButtons[i].transform.localPosition = /*Vector3.zero + */Vector3.up * 1.5f;
            menuHandler.customization.selectGunButtons[i].transform.transform.LookAt(Camera.main.transform);
            menuHandler.customization.selectGunButtons[i].transform.GetComponentInChildren<ShopGunHandler>().
                StartListeningGunPoint(CustomizationState.Instance.PlayerTruck.GetComponent<Truck>().firePoint.gunsPoints[i]);
            menuHandler.customization.selectGunButtons[i].transform.parent = null;
        }

        menuHandler.BackButton.GetComponent<Button>().onClick.AddListener(() => RemoveGunButtons());
    }

    public void RemoveGunButtons()
    {
        if(!ReferenceEquals(menuHandler.customization.selectGunButtons, null))
        {
            for (int i = 0; i < menuHandler.customization.selectGunButtons.Length; i++)
            {
                Destroy(menuHandler.customization.selectGunButtons[i].gameObject);
            }
        }
    }
    
    public void ShopGunButtonsRenderSetActive(bool enabled)
    {
        if (!ReferenceEquals(menuHandler.customization.selectGunButtons, null))
        {
            for (int i = 0; i < menuHandler.customization.selectGunButtons.Length; i++)
            {
                for (int j = 0; j < menuHandler.customization.selectGunButtons[i].GetComponentsInChildren<Image>().Length; j++)
                {
                    menuHandler.customization.selectGunButtons[i].GetComponentsInChildren<Image>()[j].enabled = enabled;
                }
                for (int j = 0; j < menuHandler.customization.selectGunButtons[j].GetComponentsInChildren<Button>().Length; j++)
                {
                    menuHandler.customization.selectGunButtons[i].GetComponentsInChildren<Button>()[j].enabled = enabled;
                }
            }
        }
    }


    public void UpgradeFirePoint()
    {
        menuHandler.customization.ChangeTruckButton.SetActive(false);

        firePointTypeCount++;

        if(firePointTypeCount> System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length - 1)
        {
            firePointTypeCount = 1;
        }

        GameEnums.FirePointType firePointToBuy = (GameEnums.FirePointType)firePointTypeCount;

        PlayerStaticRunTimeData.customizationTruckData.firePointType = firePointToBuy;

        menuHandler.customization.UpgradeFirePointButton.GetComponent<Button>().onClick.RemoveAllListeners();


        menuHandler.customization.BuyButton.SetActive(true);
        menuHandler.customization.BuyButton.GetComponentInChildren<Text>().text = shopCosts.ItemsCost(firePointToBuy.ToString()).ToString() + "$";

        menuHandler.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();
        menuHandler.customization.BuyButton.GetComponent<Button>().onClick.AddListener(() => BuyFirePoint(firePointToBuy));

        menuHandler.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        menuHandler.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToTruckSectionWindow());


        StartCustomizeTruck();
    }

    public void BuyFirePoint(GameEnums.FirePointType fptype)
    {


        int cost = shopCosts.ItemsCost(fptype.ToString());
        if (PlayerStaticRunTimeData.coins >= cost)
        {
            PlayerStaticRunTimeData.playerTruckData.RewriteData(PlayerStaticRunTimeData.customizationTruckData);
            PlayerStaticRunTimeData.coins -= cost;
            PlayerStaticDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, new PlayerSessionData(0, 0, 0));
            menuHandler.customization.BuyButton.GetComponentInChildren<Text>().text = "";

            menuHandler.customization.ChangeTruckButton.SetActive(true);
            menuHandler.customization.BuyButton.SetActive(false);
            menuHandler.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();

            menuHandler.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
            menuHandler.BackButton.GetComponent<Button>().onClick.AddListener(() => CustomizationState.Instance.BackToSectionsWindow(MenuHandler.Instance));
        }

        

        StartCustomizeTruck();
    }

    public void ChangeTruck()
    {
        menuHandler.customization.UpgradeFirePointButton.SetActive(false);

        truckTypeCount++;

        if (truckTypeCount > System.Enum.GetNames(typeof(GameEnums.Truck)).Length - 1)
        {
            truckTypeCount = 0;
        }
        GameEnums.Truck truckToBuy = (GameEnums.Truck)truckTypeCount;
        PlayerStaticRunTimeData.customizationTruckData.truckType = truckToBuy;

        menuHandler.customization.ChangeTruckButton.GetComponent<Button>().onClick.RemoveAllListeners();


        menuHandler.customization.BuyButton.SetActive(true);
        menuHandler.customization.BuyButton.GetComponentInChildren<Text>().text = shopCosts.ItemsCost(truckToBuy.ToString()).ToString() + "$";

        menuHandler.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();
        menuHandler.customization.BuyButton.GetComponent<Button>().onClick.AddListener(() => BuyTruck(truckToBuy));

        menuHandler.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        menuHandler.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToTruckSectionWindow());

        StartCustomizeTruck();
    }

    public void BuyTruck(GameEnums.Truck trucktype)
    {


        int cost = shopCosts.ItemsCost(trucktype.ToString());
        if (PlayerStaticRunTimeData.coins >= cost)
        {
            PlayerStaticRunTimeData.playerTruckData.RewriteData(PlayerStaticRunTimeData.customizationTruckData);
            PlayerStaticRunTimeData.coins -= cost;
            PlayerStaticDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, new PlayerSessionData(0,0, 0));
            MenuHandler.Instance.customization.BuyButton.GetComponentInChildren<Text>().text = "";

            menuHandler.customization.UpgradeFirePointButton.SetActive(true);
            menuHandler.customization.BuyButton.gameObject.SetActive(false);
            menuHandler.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();


            menuHandler.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
            menuHandler.BackButton.GetComponent<Button>().onClick.AddListener(() => CustomizationState.Instance.BackToSectionsWindow(MenuHandler.Instance));
        }
        StartCustomizeTruck();
    }

    public void BackToTruckSectionWindow()
    {
        menuHandler.customization.ChangeTruckButton.SetActive(true);
        menuHandler.customization.UpgradeFirePointButton.SetActive(true);
        menuHandler.customization.BuyButton.SetActive(false);

        CustomizationState.Instance.RefreshPlayerTruckVisualBy(PlayerStaticRunTimeData.playerTruckData);
        CustomizationState.Instance.RewriteCustomizationData();

        menuHandler.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        menuHandler.BackButton.GetComponent<Button>().onClick.AddListener(() => CustomizationState.Instance.BackToSectionsWindow(MenuHandler.Instance));
    }
}

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


    public void StartCustomizeTruck()
    {
        MenuHandler.Instance.customization.ChangeTruckButton.GetComponent<Button>().onClick.AddListener(() => ChangeTruck());
        MenuHandler.Instance.customization.UpgradeFirePointButton.GetComponent<Button>().onClick.AddListener(() => UpgradeFirePoint());

        CustomizationState.Instance.RefreshPlayerTruckVisualBy(PlayerStaticRunTimeData.customizationTruckData);
    }
    public void StartCustomizeGuns()
    {
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
        MenuHandler.Instance.customization.selectGunButtons = new GameObject[CustomizationState.Instance.PlayerTruck.GetComponent<Truck>().firePoint.gunsPoints.Count];

        for (int i = 0; i < CustomizationState.Instance.PlayerTruck.GetComponent<Truck>().firePoint.gunsPoints.Count; i++)
        {
            MenuHandler.Instance.customization.selectGunButtons[i] = Instantiate(MenuHandler.Instance.customization.SelectGunButtonPrefab);
            MenuHandler.Instance.customization.selectGunButtons[i].transform.parent = CustomizationState.Instance.PlayerTruck.GetComponent<Truck>().firePoint.gunsPoints[i].gunsLocation;
            MenuHandler.Instance.customization.selectGunButtons[i].transform.localPosition = /*Vector3.zero + */Vector3.up * 1.5f;
            MenuHandler.Instance.customization.selectGunButtons[i].transform.transform.LookAt(Camera.main.transform);
            MenuHandler.Instance.customization.selectGunButtons[i].transform.GetComponentInChildren<ShopGunHandler>().
                StartListeningGunPoint(CustomizationState.Instance.PlayerTruck.GetComponent<Truck>().firePoint.gunsPoints[i]);
            MenuHandler.Instance.customization.selectGunButtons[i].transform.parent = null;
        }

        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.AddListener(() => RemoveGunButtons());
    }

    public void RemoveGunButtons()
    {
        if(!ReferenceEquals(MenuHandler.Instance.customization.selectGunButtons, null))
        {
            for (int i = 0; i < MenuHandler.Instance.customization.selectGunButtons.Length; i++)
            {
                Destroy(MenuHandler.Instance.customization.selectGunButtons[i].gameObject);
            }
        }
    }
    
    public void ShopGunButtonsRenderSetActive(bool enabled)
    {
        if (!ReferenceEquals(MenuHandler.Instance.customization.selectGunButtons, null))
        {
            for (int i = 0; i < MenuHandler.Instance.customization.selectGunButtons.Length; i++)
            {
                for (int j = 0; j < MenuHandler.Instance.customization.selectGunButtons[i].GetComponentsInChildren<Image>().Length; j++)
                {
                    MenuHandler.Instance.customization.selectGunButtons[i].GetComponentsInChildren<Image>()[j].enabled = enabled;
                }
                for (int j = 0; j < MenuHandler.Instance.customization.selectGunButtons[j].GetComponentsInChildren<Button>().Length; j++)
                {
                    MenuHandler.Instance.customization.selectGunButtons[i].GetComponentsInChildren<Button>()[j].enabled = enabled;
                }
            }
        }
    }


    public void UpgradeFirePoint()
    {
        MenuHandler.Instance.customization.ChangeTruckButton.SetActive(false);

        firePointTypeCount++;

        if(firePointTypeCount> System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length - 1)
        {
            firePointTypeCount = 1;
        }

        GameEnums.FirePointType firePointToBuy = (GameEnums.FirePointType)firePointTypeCount;

        PlayerStaticRunTimeData.customizationTruckData.firePointType = firePointToBuy;

        MenuHandler.Instance.customization.UpgradeFirePointButton.GetComponent<Button>().onClick.RemoveAllListeners();


        MenuHandler.Instance.customization.BuyButton.SetActive(true);
        MenuHandler.Instance.customization.BuyButton.GetComponentInChildren<Text>().text = "Buy FirePoint " + shopCosts.ItemsCost(firePointToBuy.ToString()).ToString();

        MenuHandler.Instance.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.customization.BuyButton.GetComponent<Button>().onClick.AddListener(() => BuyFirePoint(firePointToBuy));

        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToTruckSectionWindow());


        StartCustomizeTruck();
    }

    public void BuyFirePoint(GameEnums.FirePointType fptype)
    {
        MenuHandler.Instance.customization.ChangeTruckButton.SetActive(true);


        int cost = shopCosts.ItemsCost(fptype.ToString());
        if (PlayerStaticRunTimeData.coins >= cost)
        {
            PlayerStaticRunTimeData.playerTruckData.RewriteData(PlayerStaticRunTimeData.customizationTruckData);
            PlayerStaticRunTimeData.coins -= cost;
            PlayerStaticDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, new PlayerSessionData(0, 0, 0));
            MenuHandler.Instance.customization.BuyButton.GetComponentInChildren<Text>().text = "";

        }

        MenuHandler.Instance.customization.BuyButton.SetActive(false);
        MenuHandler.Instance.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();

        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.AddListener(() => CustomizationState.Instance.BackToSectionsWindow(MenuHandler.Instance));

        StartCustomizeTruck();
    }

    public void ChangeTruck()
    {
        MenuHandler.Instance.customization.UpgradeFirePointButton.SetActive(false);

        truckTypeCount++;

        if (truckTypeCount > System.Enum.GetNames(typeof(GameEnums.Truck)).Length - 1)
        {
            truckTypeCount = 0;
        }
        GameEnums.Truck truckToBuy = (GameEnums.Truck)truckTypeCount;
        PlayerStaticRunTimeData.customizationTruckData.truckType = truckToBuy;

        MenuHandler.Instance.customization.ChangeTruckButton.GetComponent<Button>().onClick.RemoveAllListeners();


        MenuHandler.Instance.customization.BuyButton.SetActive(true);
        MenuHandler.Instance.customization.BuyButton.GetComponentInChildren<Text>().text = "Buy Truck " + shopCosts.ItemsCost(truckToBuy.ToString()).ToString();

        MenuHandler.Instance.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.customization.BuyButton.GetComponent<Button>().onClick.AddListener(() => BuyTruck(truckToBuy));

        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToTruckSectionWindow());

        StartCustomizeTruck();
    }

    public void BuyTruck(GameEnums.Truck trucktype)
    {
        MenuHandler.Instance.customization.UpgradeFirePointButton.SetActive(true);


        int cost = shopCosts.ItemsCost(trucktype.ToString());
        if (PlayerStaticRunTimeData.coins >= cost)
        {
            PlayerStaticRunTimeData.playerTruckData.RewriteData(PlayerStaticRunTimeData.customizationTruckData);
            PlayerStaticRunTimeData.coins -= cost;
            PlayerStaticDataHandler.SaveData(PlayerStaticRunTimeData.playerTruckData, new PlayerSessionData(0,0, 0));
            MenuHandler.Instance.customization.BuyButton.GetComponentInChildren<Text>().text = "";

        }

        MenuHandler.Instance.customization.BuyButton.gameObject.SetActive(false);
        MenuHandler.Instance.customization.BuyButton.GetComponent<Button>().onClick.RemoveAllListeners();


        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.AddListener(() => CustomizationState.Instance.BackToSectionsWindow(MenuHandler.Instance));

        StartCustomizeTruck();
    }

    public void BackToTruckSectionWindow()
    {
        MenuHandler.Instance.customization.ChangeTruckButton.SetActive(true);
        MenuHandler.Instance.customization.UpgradeFirePointButton.SetActive(true);
        MenuHandler.Instance.customization.BuyButton.SetActive(false);

        CustomizationState.Instance.RefreshPlayerTruckVisualBy(PlayerStaticRunTimeData.playerTruckData);
        CustomizationState.Instance.RewriteCustomizationData();

        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        MenuHandler.Instance.BackButton.GetComponent<Button>().onClick.AddListener(() => CustomizationState.Instance.BackToSectionsWindow(MenuHandler.Instance));
    }
}

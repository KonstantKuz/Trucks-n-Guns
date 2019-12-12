using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using UnityEngine.UI;
public class CustomizationState : State<MenuHandler>
{
    public static CustomizationState _instance;

    private CustomizationState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static CustomizationState Instance
    {
        get
        {
            if (_instance == null)
                new CustomizationState();

            return _instance;
        }
    }

    public GameObject PlayerTruck;

    public override void EnterState(MenuHandler _owner)
    {

        PlayerTruck = GameObject.Find("PlayerTruckPreset(Clone)");

        _owner.customization.CustomizationWindow.SetActive(true);

        _owner.customization.TrucksSectionButton.SetActive(true);
        _owner.customization.GunSectionButton.SetActive(true);

        _owner.BackButton.SetActive(true);
        _owner.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _owner.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToMainMenu(_owner));

        _owner.customization.TrucksSectionButton.GetComponent<Button>().onClick.AddListener(() => EnterTruckSection(_owner));
        _owner.customization.GunSectionButton.GetComponent<Button>().onClick.AddListener(() => EnterGunSection(_owner));

        _owner.customization.coins.GetComponentInChildren<Button>().onClick.AddListener(() => GoogleAdmobHandler.Instance.ShowRewardBasedVideo());
    }

    public override void ExitState(MenuHandler _owner)
    {
        _owner.customization.CustomizationWindow.SetActive(false);

        _owner.BackButton.SetActive(false);
        _owner.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();

        _owner.customization.coins.GetComponentInChildren<Button>().onClick.RemoveAllListeners();

    }

    public override void UpdateState(MenuHandler _owner)
    {
        throw new System.NotImplementedException();
    }

    
    public void EnterTruckSection(MenuHandler _owner)
    {
        _owner.customization.TruckSectionWindow.SetActive(true);

        _owner.customization.TrucksSectionButton.SetActive(false);
        _owner.customization.GunSectionButton.SetActive(false);

        _owner.BackButton.SetActive(true);
        _owner.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _owner.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToSectionsWindow(_owner));
        CustomizationHandler.Instance.StartCustomizeTruck();
    }

    public void EnterGunSection(MenuHandler _owner)
    {
        _owner.customization.GunSectionWindow.SetActive(true);

        _owner.customization.TrucksSectionButton.SetActive(false);
        _owner.customization.GunSectionButton.SetActive(false);
        _owner.BackButton.SetActive(true);
        _owner.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _owner.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToSectionsWindow(_owner));
        CustomizationHandler.Instance.StartCustomizeGuns();
    }


    public void BackToSectionsWindow(MenuHandler _owner)
    {
        CustomizationHandler.Instance.StopCustomizeTruck();
        CustomizationHandler.Instance.StopCustomizeGuns();
        RewriteCustomizationData();

        _owner.customization.TrucksSectionButton.SetActive(true);
        _owner.customization.GunSectionButton.SetActive(true);

        _owner.customization.TruckSectionWindow.SetActive(false);
        _owner.customization.GunSectionWindow.SetActive(false);

        
        _owner.customization.BuyButton.SetActive(false);
        _owner.BackButton.SetActive(true);
        _owner.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _owner.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToMainMenu(_owner));

    }

    public void BackToMainMenu(MenuHandler _owner)
    {
        RewriteCustomizationData();
        RefreshPlayerTruckVisualBy(PlayerStaticRunTimeData.playerTruckData);

        _owner.menu.ChangeState(MainMenuState.Instance);
    }

    public void RefreshPlayerTruckVisualBy(TruckData truckdata)
    {
        RefreshPlayerStats();

        PlayerTruck.GetComponent<Truck>().TruckData.ReturnObjectsToPool(PlayerTruck.GetComponent<Truck>());

        PlayerTruck.GetComponent<Truck>().TruckData = truckdata;

        PlayerTruck.GetComponent<Truck>().TruckData.SetUpTruck(PlayerTruck.GetComponent<Truck>());
    }

    public void RewriteCustomizationData()
    {
        PlayerStaticRunTimeData.customizationTruckData.RewriteData(PlayerStaticRunTimeData.playerTruckData);
        PlayerStaticRunTimeData.customizationTruckData.firePointData.RewriteData(PlayerStaticRunTimeData.playerTruckData.firePointData);
    }

    public void RefreshPlayerStats()
    {
        MenuHandler.Instance.customization.coins.GetComponentInChildren<Text>().text = $"{PlayerStaticRunTimeData.coins} $";
        MenuHandler.Instance.customization.experience.GetComponentInChildren<Text>().text = $"{PlayerStaticRunTimeData.experience}";
    }
}

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

        RefreshPlayerStats();

        if(PlayerStaticRunTimeData.experience > GetNextLevelExperienceCost(GetNextLevel()))
        {
            WarningWindow.Instance.ShowWarning(WarningStrings.CanImproveTruck());
        }
    }

    public override void ExitState(MenuHandler _owner)
    {
        _owner.customization.CustomizationWindow.SetActive(false);

        _owner.BackButton.SetActive(false);
        _owner.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();

        _owner.customization.coins.GetComponentInChildren<Button>().onClick.RemoveAllListeners();

        RefreshPlayerStats();

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
        MenuHandler menuHandler = MenuHandler.Instance;

        menuHandler.customization.coins.GetComponentInChildren<Text>().text = $"{PlayerStaticRunTimeData.coins} $";

        Slider experience = menuHandler.customization.experience.GetComponent<Slider>();
        experience.minValue = 0;

        experience.maxValue = GetNextLevelExperienceCost(GetNextLevel());
        experience.value = PlayerStaticRunTimeData.experience;
        string levelToShow = "";
        switch (PlayerStaticRunTimeData.playerTruckData.firePointType)
        {
            case GameEnums.FirePointType.D_FPType:
                levelToShow = "1";
                break;
            case GameEnums.FirePointType.DM_FPType:
                levelToShow = "2";
                break;
            case GameEnums.FirePointType.DMP_FPType:
                levelToShow = "3";
                break;
            case GameEnums.FirePointType.DCMP_FPType:
                levelToShow = "Max";
                experience.maxValue = 1;
                experience.value = 1;
                break;
        }

        switch (Localization.currentLanguage)
        {
            case GameEnums.Language.RU:
                menuHandler.customization.experience.GetComponentInChildren<Text>().text = $"{levelToShow} уровень";
                break;
            case GameEnums.Language.ENG:
                menuHandler.customization.experience.GetComponentInChildren<Text>().text = $"{levelToShow} level";
                break;
            default:
                break;
        }
    }

    public GameEnums.FirePointType GetNextLevel()
    {
        GameEnums.FirePointType currentLevel = PlayerStaticRunTimeData.playerTruckData.firePointType;

        if ((int)currentLevel == 0)
        {
            return GameEnums.FirePointType.DM_FPType;
        }
        else if ((int)currentLevel == 1)
        {
            return GameEnums.FirePointType.DMP_FPType;
        }
        else
        {
            return GameEnums.FirePointType.DCMP_FPType;
        }
    }

    public int GetNextLevelExperienceCost(GameEnums.FirePointType nextLevel)
    {
        ShopCosts experienceCosts = CustomizationHandler.Instance.experienceCosts;
        return experienceCosts.ItemsCost(nextLevel.ToString());
    }
}

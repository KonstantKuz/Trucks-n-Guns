using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationHandler : MonoCached
{
    private Truck playerTruck;
    public GameObject gunButtonPrefab;
    private Button[] gunButtons;
    private GameObject[] gunButtObj;
    private Button upgradeButton;
    private Button changeTruckButton;
    private Button applyAllChanges;

    int firePointTypeCount = 0;
    int truckTypeCount = 0;

    //private Button startGame;
    AsyncOperation generalGameScene;

    private void Start()
    {
        StartCoroutine(AsyncLoadGeneralScene());
    }

    private IEnumerator AsyncLoadGeneralScene()
    {
        yield return new WaitForSeconds(1f);
        generalGameScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GeneralGameState", UnityEngine.SceneManagement.LoadSceneMode.Single);
        generalGameScene.allowSceneActivation = false;
    }
    //public void StartGame()
    //{
    //    GunChangeButton[] oldButtons = FindObjectsOfType<GunChangeButton>();
    //    foreach (var item in oldButtons)
    //    {
    //        Destroy(item.gameObject);
    //    }
    //    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GeneralGameState");
    //}

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
            gunButtObj[i] = Instantiate(gunButtonPrefab);
            gunButtons[i] = gunButtObj[i].GetComponentInChildren<Button>();
            gunButtObj[i].transform.parent = playerTruck.firePoint.gunsPoints[i].gunsLocation;
            gunButtObj[i].transform.localPosition = Vector3.zero;
            gunButtObj[i].transform.LookAt(Camera.main.transform);
            gunButtObj[i].GetComponentInChildren<GunChangeButton>().StartListeningGunPoint(playerTruck.firePoint.gunsPoints[i]);
            gunButtObj[i].transform.parent = null;
        }
        changeTruckButton = GameObject.Find("ChangeTruckButton").GetComponent<Button>();
        changeTruckButton.onClick.AddListener(() => ChangeTruck());

        upgradeButton = GameObject.Find("UpgradeButton").GetComponent<Button>();
        upgradeButton.onClick.AddListener(() => UpgradeTruck());

        applyAllChanges = GameObject.Find("ApplyAll").GetComponent<Button>();
        applyAllChanges.onClick.AddListener(() => ApplyAllChanges());
    }


    public void UpgradeTruck()
    {
        firePointTypeCount++;

        if(firePointTypeCount> System.Enum.GetNames(typeof(GameEnums.FirePointType)).Length - 1)
        {
            firePointTypeCount = 0;
        }

        playerTruck.TruckData.firePointType = (GameEnums.FirePointType)firePointTypeCount;
        playerTruck.TruckData.ReturnObjectsToPool(playerTruck);
        playerTruck.SetUpTruck();

        upgradeButton.onClick.RemoveAllListeners();

        InjectPlayerTruck(playerTruck);
    }

    public void ChangeTruck()
    {
        truckTypeCount++;

        if (truckTypeCount > System.Enum.GetNames(typeof(GameEnums.Truck)).Length - 1)
        {
            truckTypeCount = 0;
        }

        playerTruck.TruckData.truckType = (GameEnums.Truck)truckTypeCount;
        playerTruck.TruckData.ReturnObjectsToPool(playerTruck);
        playerTruck.SetUpTruck();

        changeTruckButton.onClick.RemoveAllListeners();

        InjectPlayerTruck(playerTruck);
    }

    public void ApplyAllChanges()
    {
        for (int i = 0; i < playerTruck.firePoint.gunsPoints.Count; i++)
        {
            for (int j = 0; j < playerTruck.TruckData.firePointData.gunsConfigurations.Length; j++)
            {
                if(playerTruck.firePoint.gunsPoints[i].locationPath == playerTruck.TruckData.firePointData.gunsConfigurations[j].locationPath.ToString() && playerTruck.firePoint.gunsPoints[i].gunsLocation.childCount>0)
                {
                    playerTruck.TruckData.firePointData.gunsConfigurations[j].gun = (GameEnums.Gun)System.Enum.Parse(typeof(GameEnums.Gun), playerTruck.firePoint.gunsPoints[i].gunsLocation.GetChild(0).name);
                }
            }
        }

        GunChangeButton[] oldButtons = FindObjectsOfType<GunChangeButton>();
        foreach (var item in oldButtons)
        {
            Destroy(item.gameObject);
        }

        applyAllChanges.onClick.RemoveAllListeners();

        StartCoroutine(UnloadCurrScene());
    }

    private IEnumerator UnloadCurrScene()
    {
        UnityEngine.SceneManagement.SceneManager.UnloadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

        yield return new WaitForSecondsRealtime(2f);

        StartCoroutine(ActivateGeneralScene());
    }

    private IEnumerator ActivateGeneralScene()
    {
        yield return new WaitForSeconds(1f);
        if (!ReferenceEquals(generalGameScene, null))
        {
            generalGameScene.allowSceneActivation = true;
        }
        else
        {
            yield return StartCoroutine(ActivateGeneralScene());
        }
    }
}

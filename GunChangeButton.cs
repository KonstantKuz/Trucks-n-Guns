using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunChangeButton : MonoCached
{
    
    public FirePoint.GunPoint gunPoint { get; set; }

    private Button setUpGunButton;
    private Button gunChangeButton;
    private Button backButton;
    private Vector3 camStartPos;
    private Quaternion camStartRot;

    public void StartListeningGunPoint(FirePoint.GunPoint gunPoint)
    {
        this.gunPoint = gunPoint;
        GetComponent<Button>().onClick.AddListener(() => SelectGun());

        gunChangeButton = GameObject.Find("RealChangeButton").GetComponent<Button>();
        backButton = GameObject.Find("BackButton").GetComponent<Button>();
        setUpGunButton = GameObject.Find("SetUpGun").GetComponent<Button>();

        camStartPos = Camera.main.transform.position;
        camStartRot = Camera.main.transform.rotation;
    }

    private void SelectGun()
    {
        Camera.main.transform.position = transform.position + new Vector3(2, 2, 2);
        Camera.main.transform.LookAt(gunPoint.gunsLocation);

        gunChangeButton.GetComponent<Image>().enabled = true;
        backButton.GetComponent<Image>().enabled = true;
        gameObject.GetComponent<Image>().enabled = false;
        gunChangeButton.onClick.AddListener(() => ChangeGun());
        backButton.onClick.AddListener(() => BackToMain());
    }

    private void ChangeGun()
    {
        if (gunPoint.gunsLocation.childCount > 0)
        {
            ObjectPoolersHolder.Instance.GunsPooler.ReturnGameObjectToPool(gunPoint.gunsLocation.GetChild(0).gameObject, gunPoint.gunsLocation.GetChild(0).gameObject.name);
        }

        GameObject gun = ObjectPoolersHolder.Instance.GunsPooler.SpawnRandomItemFromPool(gunPoint.gunsLocation.position, Quaternion.identity, 100);

        gun.transform.parent = gunPoint.gunsLocation;
        gun.transform.localPosition = Vector3.zero;

        if (gun.name != "None")
        {
            GunParent gunComponent = gun.GetComponent<GunParent>();
            gunComponent.SetUpAngles(gunPoint.allowableAnglesOnPoint);
        }

        setUpGunButton.GetComponent<Image>().enabled = true;

        setUpGunButton.onClick.AddListener(() => SetUpGun(gun.name));
    }

    private void SetUpGun(string newGunName)
    {
        for (int i = 0; i < CustomizationHandler.playerFPdata.gunsConfigurations.Length; i++)
        {
            if(CustomizationHandler.playerFPdata.gunsConfigurations[i].locationPath.ToString() == gunPoint.locationPath)
            {
                CustomizationHandler.playerFPdata.gunsConfigurations[i].gun = (GameEnums.Gun)System.Enum.Parse(typeof(GameEnums.Gun), newGunName);
            }
        }
    }

    private void BackToMain()
    {
        Camera.main.transform.position = camStartPos;
        Camera.main.transform.rotation = camStartRot;

        gameObject.GetComponent<Image>().enabled = true;

        gunChangeButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        setUpGunButton.onClick.RemoveAllListeners();

        setUpGunButton.GetComponent<Image>().enabled = false;
        gunChangeButton.GetComponent<Image>().enabled = false;
        backButton.GetComponent<Image>().enabled = false;

    }
}

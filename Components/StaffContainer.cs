using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffContainer : MonoBehaviour
{
    private StaffContainerData containerData;

    private void OnEnable()
    {
        containerData = ScriptableObject.CreateInstance<StaffContainerData>();

        containerData.additionAmountPercent = Random.Range(0.005f, 0.1f);
        int containerTypeInt = Random.Range(0, System.Enum.GetNames(typeof(Data.ImproveType)).Length);
        containerData.improveType = (Data.ImproveType)containerTypeInt;

        var smoke = gameObject.GetComponentInChildren<ParticleSystem>();
        containerData.improveType = Data.ImproveType.TrucksCondition;

        switch (containerData.improveType)
        {
            case Data.ImproveType.TrucksCondition:
                smoke.startColor = Color.green;
                break;
            case Data.ImproveType.TrucksSteeringSpeed:
                smoke.startColor = Color.cyan;
                break;
            case Data.ImproveType.GunsRateOfFire:
                smoke.startColor = Color.red;
                break;
            case Data.ImproveType.GunsBattleUnitsSpeed:
                smoke.startColor = Color.yellow;
                break;
            case Data.ImproveType.GunsSpreadForce:
                smoke.startColor = Color.black;
                break;
            case Data.ImproveType.GunsBattleUnitsDamage:
                smoke.startColor = Color.white;
                break;
            default:
                break;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //if(other.GetComponent<Truck>() != null)
        //{
        //    containerData.ActivateContainer(other.GetComponent<Truck>());
        //}
        //gameObject.SetActive(false);
    }
}

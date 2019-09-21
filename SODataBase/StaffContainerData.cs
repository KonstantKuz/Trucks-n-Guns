using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffContainerData : Data
{
    public ImproveType improveType;

    public float effectTime;
    public float additionAmountPercent;

    public void ActivateContainer(Truck truck)
    {
        switch (improveType)
        {
            case ImproveType.TrucksCondition:
                truck.trucksCondition.AddHealth(truck.trucksCondition.currentCondition * 10f * additionAmountPercent);
                break;
        }
    }
}

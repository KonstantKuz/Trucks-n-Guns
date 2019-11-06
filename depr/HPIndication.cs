using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HPIndication : MonoCached
{
    [SerializeField]
    private EntityCondition condition;
    [SerializeField]
    private Slider conditionBar;

    private void OnEnable()
    {
        allTicks.Add(this);
        conditionBar.minValue = 0;
        conditionBar.maxValue = condition.maxCondition;
        conditionBar.value = condition.currentCondition;
    }
    public override void OnTick()
    {
        conditionBar.value = condition.currentCondition;
    }

}

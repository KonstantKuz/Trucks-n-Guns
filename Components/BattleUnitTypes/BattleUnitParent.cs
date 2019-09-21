using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleUnitParent : MonoCached
{
    public abstract BattleUnitData myData { get; set; }
    
    public abstract void Fly();

    public abstract void SearchTargets();

    public virtual void SetDamage(EntityCondition targetToHit)
    {
        if(targetToHit!=null)
        {
            targetToHit.AddDamage(myData.damage);
        }
            Deactivate();
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }
}

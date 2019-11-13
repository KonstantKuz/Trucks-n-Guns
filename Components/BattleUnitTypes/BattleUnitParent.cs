using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BattleUnit
{
    BattleUnitData battleUnitData { get; set; }
    
    void Fly();

    void SearchTargets();

    void SetDamage(EntityCondition targetToHit);

    void Deactivate();
}

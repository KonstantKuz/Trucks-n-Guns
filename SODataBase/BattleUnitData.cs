using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewBattleUnit", menuName = ("Data/BattleUnitData"))]
[System.Serializable]
public class BattleUnitData : Data
{
    public LayerMask interactibleWith;
    public float speed;
    public float damage;
    [Header("RocketOnly")]
    public float damageRadius;
    public GameEnums.BattleType battleType;
}

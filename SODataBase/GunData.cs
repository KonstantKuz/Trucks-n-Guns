using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewGun", menuName = ("Data/GunData"))]
public class GunData : Data
{
    
    public GameEnums.BattleType battleType;

    public BattleUnitData battleUnitToCopy;
    public BattleUnitData myBattleUnit { get; private set; }

    public float rateofFire;

    public AnimationCurve spreadCurve;
    public float spreadForce;

    public float timeElapsed { get; set; }
    public float timeSinceLastShot { get; set; }


    public void CreateBattleUnitInstance()
    {
        myBattleUnit = Instantiate(battleUnitToCopy);
    }

}

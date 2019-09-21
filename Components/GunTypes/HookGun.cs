using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookGun : GunParent
{
    [SerializeField]
    private GunData gunDataToCopy;

    public override GunData myData { get; set; }
    public override TargetData targetData { get; set; }

    public Transform rotationPoint;
    public Transform gunsMouth;

    private void Awake()
    {
        myData = Instantiate(gunDataToCopy);
        myData.CreateBattleUnitInstance();
    }
    public override void SetTargetData(TargetData targetData)
    {
        this.targetData = targetData;
    }
    public override void Fire()
    {
        //rotationPoint.LookAt(target);

        //if (myData.timeSinceLastShot <= 0)
        //{
        //    GameObject bullet = ObjectPooler.Instance.SpawnFromPool(myData.nameOfBattleUnit, gunsMouth.position, gunsMouth.rotation);
        //    Debug.Log(myData.nameOfThisGun + string.Empty + gameObject.name);
        //}
    }
}

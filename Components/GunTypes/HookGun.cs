using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookGun : MonoCached, Gun
{
    [SerializeField]
    private GunData gunDataToCopy;

    public GunData myData { get; set; }
    public TargetData targetData { get; set; }

    public int HeadHolderMaxAngle { get; set; }
    public int HeadMaxAngle { get; set; }
    public int HeadMinAngle { get; set; }

    public Transform rotationPoint;
    public Transform gunsMouth;

    private void Awake()
    {
        myData = Instantiate(gunDataToCopy);
        myData.CreateBattleUnitInstance();
    }
    public void SetTargetData(TargetData targetData)
    {
        this.targetData = targetData;
    }
    public void Fire()
    {
        //rotationPoint.LookAt(target);

        //if (myData.timeSinceLastShot <= 0)
        //{
        //    GameObject bullet = ObjectPooler.Instance.SpawnFromPool(myData.nameOfBattleUnit, gunsMouth.position, gunsMouth.rotation);
        //    Debug.Log(myData.nameOfThisGun + string.Empty + gameObject.name);
        //}
    }

    public void SetAllowableAngles(int headHolderMaxAngle, int headMaxAngle, int headMinAngle)
    {
    }

    public void SetStandardAllowableAngles()
    {
    }
}

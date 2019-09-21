using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketGun : GunParent
{
    [SerializeField]
    private GunData gunDataToCopy;

    public override GunData myData { get; set; }
    public override TargetData targetData { get; set; }

    public Transform head, platform;
    public Transform[] gunsBarrels;

    private Vector3 tempTargetForPlatform = Vector3.zero, tempTargetForHead = Vector3.zero;

    private ObjectPoolerBase battleUnitPooler;

    public override void SetTargetData(TargetData targetData)
    {
        switch (myData.battleType)
        {
            case GameEnums.BattleType.Tracking:
                this.targetData = targetData;
                break;
            case GameEnums.BattleType.Static:
                this.targetData = null;
                break;
        }
    }

    private void Awake()
    {
        myData = Instantiate(gunDataToCopy);
        myData.CreateBattleUnitInstance();
        battleUnitPooler = ObjectPoolersHolder.Instance.BattleUnitPooler;
    }

    public override void Fire()
    {
        if (myData.battleType == GameEnums.BattleType.Tracking)
        {
            LookAtTarget();
        }

        UpdateTimers();

        if (myData.timeSinceLastShot <= 0)
        {
            myData.timeSinceLastShot = myData.rateofFire;

            for (int i = 0; i < gunsBarrels.Length; i++)
            {
                battleUnitPooler.SpawnFromPool(myData.battleUnitToCopy.name, gunsBarrels[i].position, gunsBarrels[i].rotation).GetComponent<Rocket>().Launch(targetData);
            }
        }
    }

    void LookAtTarget()
    {
        tempTargetForHead = targetData.target_rigidbody.position - head.position;

        tempTargetForPlatform = Vector3.ProjectOnPlane(tempTargetForHead, platform.up);

        head.rotation = Quaternion.LookRotation(tempTargetForHead, platform.up);
        platform.rotation = Quaternion.LookRotation(tempTargetForPlatform, platform.up);
    }

    void UpdateTimers()
    {
        myData.timeElapsed += 0.01f;
        myData.timeSinceLastShot -= Time.fixedDeltaTime + Random.Range(0.0001f, 0.0005f);
    }

}

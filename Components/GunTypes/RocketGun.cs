using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketGun : GunParent
{
    [SerializeField]
    private GunData gunDataToCopy;

    public override GunData myData { get; set; }
    public override TargetData targetData { get; set; }

    public Transform head, headHolder;
    public Transform[] gunsBarrels;

    private Vector3 targetDirection, targetDirection_XZprojection, targetDirection_ZYprojection;
    private Transform headHolderRoot;

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
        headHolderRoot = headHolder.root;
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
        targetDirection = (targetData.target_rigidbody.position + targetData.target_rigidbody.velocity * 0.1f) - headHolder.position;

        targetDirection_XZprojection = Vector3.ProjectOnPlane(targetDirection, headHolder.up);
        targetDirection_ZYprojection = Vector3.ProjectOnPlane(targetDirection, headHolder.right);

        float headForwardAngle = Vector3.SignedAngle(targetDirection_ZYprojection.normalized, headHolder.forward, headHolder.right);
        float headRightAngle = Vector3.Angle(targetDirection_XZprojection.normalized, headHolderRoot.forward);

        if (headRightAngle < 120)
        {
            headHolder.rotation = Quaternion.LookRotation(targetDirection_XZprojection, headHolder.up);

            if (headForwardAngle < 60 && headForwardAngle > -20 && headRightAngle < 120)
            {
                head.rotation = Quaternion.LookRotation(targetDirection_ZYprojection, headHolder.up);
            }
        }
    }

    void UpdateTimers()
    {
        myData.timeElapsed += 0.01f;
        myData.timeSinceLastShot -= Time.fixedDeltaTime + Random.Range(0.0001f, 0.0005f);
    }

}

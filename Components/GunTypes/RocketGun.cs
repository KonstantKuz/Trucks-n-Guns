using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketGun : MonoCached, Gun
{
    public GunData gunData { get; set; }
    public TargetData targetData { get; set; }
    public GunAnglesData allowableAngles { get; set; }
    
    public Transform head, headHolder;
    public Transform[] gunsBarrels;
    private Transform headHolderRoot;

    private Vector3 targetDirection, targetPosSpeedBased, targetPosHeightDiffBased, targetDirection_XZprojection, targetDirection_ZYprojection;
    private bool targetIsVisible = true;
    private float fireTimer;

    private ObjectPoolerBase battleUnitPooler;

    private AudioSource rocketLaunchSource;

    private string staticRocketName = "RocketStatic";


    private void Awake()
    {
        headHolderRoot = headHolder.parent;

        rocketLaunchSource = GetComponent<AudioSource>();

        SetAnglesData(null);

        battleUnitPooler = ObjectPoolersHolder.Instance.BattleUnitPooler;
    }

    public void SetGunData(GameEnums.GunDataType gunData)
    {
        this.gunData = DataReturnersHolder.Instance.RocketGunDataReturner.GetData(gunData.ToString()) as GunData;
    }

    public void SetTargetData(TargetData targetData)
    {
        switch (gunData.battleType)
        {
            case GameEnums.BattleType.Tracking:
                this.targetData = targetData;
                break;
            case GameEnums.BattleType.Static:
                this.targetData = null;
                break;
        }
        targetIsVisible = true;
    }

    public void SetAnglesData(GunAnglesData anglesData)
    {
        if (anglesData == null)
        {
            allowableAngles = ScriptableObject.CreateInstance<GunAnglesData>();

            allowableAngles.HeadHolderMaxAngle = GunAnglesData.headHolderMaxAngle;
            allowableAngles.HeadMaxAngle = GunAnglesData.headMaxAngle;
            allowableAngles.HeadMinAngle = GunAnglesData.headMinAngle;
            allowableAngles.StartDirectionAngle = 0;
        }
        else
        {
            allowableAngles = anglesData;
        }

        head.localEulerAngles = Vector3.zero;
        headHolder.localEulerAngles = Vector3.zero;

        transform.localEulerAngles = new Vector3(0, allowableAngles.StartDirectionAngle, 0);
    }

    public void Fire()
    {
        if (Time.time >= fireTimer && targetIsVisible)
        {
            fireTimer = Time.time + gunData.rateofFire;

            for (int i = 0; i < gunsBarrels.Length; i++)
            {
                battleUnitPooler.Spawn(gunData.battleUnit, gunsBarrels[i].position, gunsBarrels[i].rotation).GetComponent<Rocket>().Launch(targetData);
            }
            if (!rocketLaunchSource.isPlaying)
            {
                rocketLaunchSource.Play();
            }
        }

        if (gunData.battleType == GameEnums.BattleType.Tracking)
        {
            LookAtTarget();
        }
    }

    void LookAtTarget()
    {
        if(gunData.battleUnit == staticRocketName)
        {
            targetPosSpeedBased = targetData.target_rigidbody.position + targetData.target_rigidbody.velocity * 0.25f;
            targetPosHeightDiffBased = headHolderRoot.up * (targetData.target_rigidbody.position.y - headHolderRoot.position.y) * 0.1f;
            targetDirection = (targetPosSpeedBased + targetPosHeightDiffBased) - headHolder.position;
        }
        else
        {
            targetDirection = targetData.target_rigidbody.position- headHolder.position;
        }

        targetDirection_XZprojection = Vector3.ProjectOnPlane(targetDirection, headHolder.up);
        targetDirection_ZYprojection = Vector3.ProjectOnPlane(targetDirection, headHolder.right);

        float angleBtwn_targetDirZY_hhFWD = Vector3.SignedAngle(targetDirection_ZYprojection.normalized, headHolder.forward, headHolder.right);
        float angleBtwn_targetDirXZ_hhrootFWD = Vector3.Angle(targetDirection_XZprojection.normalized, headHolderRoot.forward);

        if (angleBtwn_targetDirXZ_hhrootFWD < allowableAngles.HeadHolderMaxAngle)
        {
            headHolder.rotation = Quaternion.LookRotation(targetDirection_XZprojection, headHolder.up);

            if (angleBtwn_targetDirZY_hhFWD < allowableAngles.HeadMaxAngle && angleBtwn_targetDirZY_hhFWD > allowableAngles.HeadMinAngle)
            {
                head.rotation = Quaternion.LookRotation(targetDirection_ZYprojection, headHolder.up);
                targetIsVisible = true;
            }
            else
            {
                targetIsVisible = false;
            }
        }
        else
        {
            targetIsVisible = false;
        }
    }
}
